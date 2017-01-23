using System;

namespace CryptoKnight.Library.Network.ProtocolMessages.Client
{
    [Serializable]
    public class LoginMessage : IMessage
    {
        public MessageType Type => MessageType.Login;
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
