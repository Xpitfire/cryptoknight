using System;

namespace CryptoKnight.Library.Network.ProtocolMessages.Server
{
    [Serializable]
    public class PluginResponseMessage : IMessage
    {
        public MessageType Type => MessageType.PluginResponse;

        public byte[] Plugin { get; set; }
    }
}
