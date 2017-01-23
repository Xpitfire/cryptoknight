using System;

namespace CryptoKnight.Library.Network.ProtocolMessages
{
    [Serializable]
    public enum MessageType
    {
        // client to server
        Login,
        VerifyLicense,
        // server to client
        LoginResponse,
        VerifyLicenseResponse
    }
}
