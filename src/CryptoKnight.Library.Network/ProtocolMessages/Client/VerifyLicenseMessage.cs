using System;

namespace CryptoKnight.Library.Network.ProtocolMessages.Client
{
    [Serializable]
    public class VerifyLicenseMessage : IMessage
    {
        public MessageType Type => MessageType.VerifyLicense;
        public string Code { get; set; }
    }
}
