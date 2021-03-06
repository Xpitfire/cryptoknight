﻿using CryptoKnight.Library.Network.ProtocolMessages.Common;
using System;

namespace CryptoKnight.Library.Network.ProtocolMessages.Client
{
    [Serializable]
    public class LoginMessage : IMessage
    {
        public MessageType Type => MessageType.Login;
        public User User { get; set; }
        public Key Key { get; set; }
    }
}
