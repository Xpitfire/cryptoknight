using CryptoKnight.Library.Network;
using CryptoKnight.Server.KeyGenerator;
using System;
using System.Configuration;

namespace CryptoKnight.Server.Core
{
    public class DefaultAuthService : IAuthService
    {
        private readonly int _maxLicenseActivations = 0;

        public DefaultAuthService()
        {
            var data = ConfigurationManager.AppSettings["MaxLicenseActivations"];
            var protectedData = DataProtectionApi.Unprotect(Convert.FromBase64String(data));
            _maxLicenseActivations = protectedData.ToType<int>();
        }

        public bool Login(User user, Key key)
        {
            if (user?.Email == null) return false;
            if (key?.Code == null) return false;
            if (!KeyStore.AvailableLicenseActivations.ContainsKey(user)) return false;
            if (!KeyStore.AvailableLicenseActivations[user].Keys.Contains(key)) return false;
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
            if (!KeyStore.AvailableLicenseActivations.ContainsKey(user)) return;
            if (!KeyStore.AvailableLicenseActivations[user].Keys.Contains(key)) return;
            if (!LicenseRuntime.ActiveInstance.ContainsKey(key)) return;
            LicenseRuntime.ActiveInstance[key] -= 1;
        }
    }
}
