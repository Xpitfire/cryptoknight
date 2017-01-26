using System;

namespace CryptoKnight.Library.Network.ProtocolMessages.Common
{
    [Serializable]
    public class FileKey
    {
        public string Password { get; set; }
        public DateTime Expire { get; set; }
    }
}
