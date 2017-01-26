using CryptoKnight.Server.KeyGenerator;

namespace CryptoKnight.Server.Core
{
    public interface ILicenseService
    {
        Key RequestLicenseKey(User user);
        void RegisterUser(User user);
        void ShowInfo();
    }
}
