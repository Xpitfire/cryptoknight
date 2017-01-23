using System;

namespace CryptoKnight.Library.Network.ProtocolMessages.Server.Enums
{
    [Serializable]
    public enum LoginStatus
    {
        LoggedIn,
        WrongEmailOrPassword
    }
}
