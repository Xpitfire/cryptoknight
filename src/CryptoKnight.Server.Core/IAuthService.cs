using CryptoKnight.Server.KeyGenerator;

namespace CryptoKnight.Server.Core
{
    public interface IAuthService
    {
        int MaxActivations { get; }
        bool Login(User user, Key key);
        void Logout(User user, Key key);
    }
}
