using System;

namespace CryptoKnight.Server.KeyGenerator
{
    [Serializable]
    public class Key
    {
        public string Code { get; set; }
        public byte[] Checksum { get; set; }
        public bool Used { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this == obj) return true;
            var user = obj as Key;
            if (user == null) return false;

            return user.Code == Code;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Code?.GetHashCode() ?? 0) * 397;
            }
        }

        public override string ToString()
        {
            return Code;
        }
    }
}
