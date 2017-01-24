using System;

namespace CryptoKnight.Library.Network.ProtocolMessages.Server
{
    [Serializable]
    public class LoginResponseMessage : IMessage
    {
        public MessageType Type => MessageType.LoginResponse;
        public bool LoggedIn { get; set; }
    }
}
