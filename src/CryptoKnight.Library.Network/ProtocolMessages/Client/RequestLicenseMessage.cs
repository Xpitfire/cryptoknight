using CryptoKnight.Library.Network.ProtocolMessages.Common;
using System;

namespace CryptoKnight.Library.Network.ProtocolMessages.Client
{
    [Serializable]
    public class RequestLicenseMessage : IMessage
    {
        public MessageType Type => MessageType.RequestLicense;
        public User User { get; set; }
    }
}
