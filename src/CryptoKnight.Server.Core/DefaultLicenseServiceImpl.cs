using System.Collections.Concurrent;
using System.Collections.Generic;
using CryptoKnight.Server.KeyGenerator;

namespace CryptoKnight.Server.Core
{
    public class DefaultLicenseServiceImpl : ILicenseService
    {
        private readonly IDictionary<User, LicenseGroup> _availableLicenseActivations = 
            new ConcurrentDictionary<User, LicenseGroup>();

        public DefaultLicenseServiceImpl()
        {
            var lg = new LicenseGroup();
            var generator = new Generator
            {
                Template = "kkkk-kkkkkkk-kk-kkkkkkk"
            };
            for (var i = 0; i < LicenseGroup.MaxLicenseKeys; i++)
            {
                lg.Keys.Add(generator.CreateKey());
            }

            _availableLicenseActivations.Add(new User
            {
                Email = "admin@host.com",
                PasswordHash = "test"
            }, lg);
        }

        public Key RequestLicenseKey(User user)
        {
            if (string.IsNullOrEmpty(user?.Email) 
                || string.IsNullOrEmpty(user.PasswordHash)) return null;
            if (!_availableLicenseActivations.ContainsKey(user)) return null;
            if (_availableLicenseActivations[user].Available <= 0) return null;
            var index = _availableLicenseActivations[user].Available -= 1;
            var key = _availableLicenseActivations[user].Keys[index];
            key.Used = true;
            return key;
        }
    }
}
