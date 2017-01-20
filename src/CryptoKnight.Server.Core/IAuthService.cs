using CryptoKnight.Server.KeyGenerator;

namespace CryptoKnight.Server.Core
{
    public interface IAuthService
    {
        bool Login(User user, Key key);
        void Logout(User user, Key key);
    }
}
