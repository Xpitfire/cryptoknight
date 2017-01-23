using CryptoKnight.Library.Network.ProtocolMessages.Server.Enums;
using System;

namespace CryptoKnight.Library.Network.ProtocolMessages.Server
{
    [Serializable]
    public class VerifyLicenseResponseMessage : IMessage
    {
        public MessageType Type => MessageType.VerifyLicenseResponse;
        public LicenseStatus Status { get; set; }
        // the code is for testing purposes and will be removed later on
        public string Code { get; set; }
    }
}
