using CryptoKnight.Library.Network;
using CryptoKnight.Server.KeyGenerator;
using System;
using System.Configuration;

namespace CryptoKnight.Server.Core
{
    public class DefaultAuthService : IAuthService
    {
        private readonly int _maxLicenseActivations = 0;
        public int MaxActivations => 10;

        public DefaultAuthService()
        {
            string data = null;
            byte[] protectedData = null;
            int retries = 0;
            do
            {
                try
                {
                    data = ConfigurationManager.AppSettings["MaxLicenseActivations"];
                    if (!string.IsNullOrEmpty(data))
                        protectedData = DataProtectionApi.Unprotect(Convert.FromBase64String(data));
                    _maxLicenseActivations = protectedData.ToType<int>();
                }
                catch
                {
                    ApplySettings("MaxLicenseActivations", Convert.ToBase64String(
                        DataProtectionApi.Protect(MaxActivations.ToBytes())));
                }
            } while (_maxLicenseActivations != MaxActivations && ++retries < 2);
        }

        private static void ApplySettings(string key, string value)
        {

            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            if (settings[key] == null)
            {
                settings.Add(key, value);
            }
            else
            {
                settings[key].Value = value;
            }
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }

        public bool Login(User user, Key key)
        {
            if (user?.Email == null) return false;
            if (key?.Code == null) return false;
            if (!KeyStore.Contains(user, key)) return false;
            if (!LicenseRuntime.ActiveInstance.ContainsKey(key))
            {
                LicenseRuntime.ActiveInstance[key] = 0;
            }
            if (LicenseRuntime.ActiveInstance[key] >= _maxLicenseActivations) return false;
            LicenseRuntime.ActiveInstance[key] += 1;
            return true;
        }

        public void Logout(User user, Key key)
        {
            if (user?.Email == null) return;
            if (key?.Code == null) return;
            if (!KeyStore.Contains(user, key)) return;
            if (!LicenseRuntime.ActiveInstance.ContainsKey(key)) return;
            LicenseRuntime.ActiveInstance[key] -= 1;
        }
    }
}
