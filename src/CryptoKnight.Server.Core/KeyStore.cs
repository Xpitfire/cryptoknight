using System.Collections.Concurrent;
using System.Collections.Generic;
using CryptoKnight.Server.KeyGenerator;

namespace CryptoKnight.Server.Core
{
    internal static class KeyStore
    {
        public static readonly IDictionary<User, LicenseGroup> AvailableLicenseActivations =
            new ConcurrentDictionary<User, LicenseGroup>();

        public const string KeyTemplate = "kkkk-kkkkkkk-kk-kkkkkkk";

        static KeyStore()
        {
            var licenseGroup = new LicenseGroup();
            var generator = new Generator
            {
                Template = KeyTemplate
            };
            for (var i = 0; i < LicenseGroup.MaxLicenseKeys; i++)
            {
                licenseGroup.Keys.Add(generator.CreateKey());
            }
            AvailableLicenseActivations.Add(new User
            {
                Email = "admin@host.com",
                PasswordHash = "admin"
            }, licenseGroup);

            licenseGroup = new LicenseGroup();
            for (var i = 0; i < LicenseGroup.MaxLicenseKeys; i++)
            {
                licenseGroup.Keys.Add(generator.CreateKey());
            }

            AvailableLicenseActivations.Add(new User
            {
                Email = "user@host.com",
                PasswordHash = "user"
            }, licenseGroup);
        }

        public static Key RequestLicenseKey(User user)
        {
            if (string.IsNullOrEmpty(user?.Email)
                || string.IsNullOrEmpty(user.PasswordHash)) return null;
            if (!AvailableLicenseActivations.ContainsKey(user)) return null;
            if (AvailableLicenseActivations[user].Available <= 0) return null;
            var index = AvailableLicenseActivations[user].Available -= 1;
            var key = AvailableLicenseActivations[user].Keys[index];
            key.Used = true;
            return key;
        }
    }
}
