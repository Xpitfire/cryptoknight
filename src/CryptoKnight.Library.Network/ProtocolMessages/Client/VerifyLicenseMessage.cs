using System;

namespace CryptoKnight.Library.Network.ProtocolMessages.Client
{
    [Serializable]
    public class VerifyLicenseMessage
    {
        public MessageType Type => MessageType.VerifyLicense;
        public string Code { get; set; }
    }
}
