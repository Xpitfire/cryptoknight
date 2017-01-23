using CryptoKnight.Library.Network.ProtocolMessages.Server.Enums;
using System;

namespace CryptoKnight.Library.Network.ProtocolMessages.Server
{
    [Serializable]
    public class LoginResponseMessage : IMessage
    {
        public MessageType Type => MessageType.LoginResponse;
        public LoginStatus Status { get; set; }
    }
}
