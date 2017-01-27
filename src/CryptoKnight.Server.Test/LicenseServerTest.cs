using System;
using CryptoKnight.Server.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptoKnight.Server.Test
{
    [TestClass]
    public class LicenseServerTest
    {
        private static readonly User Admin = new User
        {
            Email = "admin@host.com",
            PasswordHash = "test"
        };
        private static readonly User User = new User
        {
            Email = "user@host.com",
            PasswordHash = "user"
        };
        private static ILicenseService licencService;
        private static IAuthService authService;

        [TestMethod]
        public void TestActivations()
        {
            licencService = new DefaultLicenseService();
            for (var i = 0; i < LicenseGroup.MaxLicenseKeys; i++)
            {
                Assert.IsNotNull(licencService.RequestLicenseKey(Admin));
            }
            Assert.IsNull(licencService.RequestLicenseKey(Admin));
        }

        [TestMethod]
        public void TestKeyHash()
        {
            licencService = new DefaultLicenseService();
            System.Console.WriteLine(licencService.RequestLicenseKey(User).Code);
        }

        [TestMethod]
        public void TestSingleLogin()
        {
            licencService = new DefaultLicenseService();
            authService = new DefaultAuthService();
            var key = licencService.RequestLicenseKey(User);
            Assert.IsTrue(authService.Login(User, key));
            Assert.IsFalse(authService.Login(User, key));
        }
    }
}
