using CryptoKnight.Server.KeyGenerator;

namespace CryptoKnight.Server.Core
{
    public class DefaultAuthService : IAuthService
    {
        public const int MaxLicenseActivations = 1;

        public bool Login(User user, Key key)
        {
            if (user?.Email == null) return false;
            if (key?.Code == null) return false;
            if (!KeyStore.Contains(user, key)) return false;
            if (!LicenseRuntime.ActiveInstance.ContainsKey(key))
            {
                LicenseRuntime.ActiveInstance[key] = 0;
            }
            if (LicenseRuntime.ActiveInstance[key] >= MaxLicenseActivations) return false;
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
