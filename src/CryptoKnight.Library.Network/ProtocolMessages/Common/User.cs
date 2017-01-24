using System;

namespace CryptoKnight.Library.Network.ProtocolMessages.Common
{
    [Serializable]
    public class User
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
