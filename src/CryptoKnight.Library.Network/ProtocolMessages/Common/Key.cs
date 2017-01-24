using System;

namespace CryptoKnight.Library.Network.ProtocolMessages.Common
{
    // this class could be removed and replaced by "string key/keyCode"
    [Serializable]
    public class Key
    {
        public string Code { get; set; }
    }
}
