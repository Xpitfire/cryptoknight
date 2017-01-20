using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoKnight.Server.KeyGenerator;

namespace CryptoKnight.Server.Core
{
    [Serializable]
    public class LicenseGroup
    {
        public const int MaxLicenseKeys = 3;

        public IList<Key> Keys { get; } = new List<Key>();
        public int Available { get; set; } = MaxLicenseKeys;
    }
}
