using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CryptoKnight.Server.KeyGenerator;

namespace CryptoKnight.Server.Core
{
    public class DefaultLicenseServiceImpl : ILicenseService
    {
        public void RegisterUser(User user)
        {
            KeyStore.RegisterUser(user);
        }

        public Key RequestLicenseKey(User user)
        {
            return KeyStore.RequestLicenseKey(user);
        }
    }
}
