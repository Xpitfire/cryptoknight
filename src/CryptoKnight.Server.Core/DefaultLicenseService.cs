using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CryptoKnight.Server.KeyGenerator;

namespace CryptoKnight.Server.Core
{
    public class DefaultLicenseService : ILicenseService
    {
        public bool RegisterUser(User user)
        {
            return KeyStore.RegisterUser(user);
        }

        public Key RequestLicenseKey(User user)
        {
            return KeyStore.RequestLicenseKey(user);
        }

        public void ShowInfo()
        {
            KeyStore.ShowInfo();
        }
    }
}
