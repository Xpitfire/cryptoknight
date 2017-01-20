using System.Collections.Concurrent;
using System.Collections.Generic;
using CryptoKnight.Server.KeyGenerator;

namespace CryptoKnight.Server.Core
{
    public class DefaultLicenseServiceImpl : ILicenseService
    {
        public Key RequestLicenseKey(User user)
        {
            return KeyStore.RequestLicenseKey(user);
        }
    }
}
