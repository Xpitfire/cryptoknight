using System.Collections.Concurrent;
using System.Collections.Generic;
using CryptoKnight.Server.KeyGenerator;

namespace CryptoKnight.Server.Core
{
    internal static class LicenseRuntime
    {
        public static readonly IDictionary<Key, int> ActiveInstance = new ConcurrentDictionary<Key, int>();
    }
}
