using System;

namespace CryptoKnight.Library.Network.ProtocolMessages
{
    [Serializable]
    public enum MessageType
    {
        // client to server
        Login,
        RequestLicense,
        // server to client
        LoginResponse,
        RequestLicenseResponse,
        PluginResponse
    }
}
