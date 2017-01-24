using CryptoKnight.Library.Network.ProtocolMessages.Common;
using System;

namespace CryptoKnight.Library.Network.ProtocolMessages.Server
{
    [Serializable]
    public class RequestLicenseResponseMessage : IMessage
    {
        public MessageType Type => MessageType.RequestLicenseResponse;
        public Key Key { get; set; }
    }
}
