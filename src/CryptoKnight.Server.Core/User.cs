using System;

namespace CryptoKnight.Server.Core
{
    [Serializable]
    public class User
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this == obj) return true;
            var user = obj as User;
            if (user == null) return false;

            return user.Email == Email;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Email?.GetHashCode() ?? 0) * 397;
            }
        }
    }
}