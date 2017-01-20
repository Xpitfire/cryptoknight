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
        private static ILicenseService licencService;

        [TestMethod]
        public void TestActivations()
        {
            licencService = new DefaultLicenseServiceImpl();
            for (var i = 0; i < 10; i++)
            {
                Assert.IsNotNull(licencService.RequestLicenseKey(Admin));
            }
            Assert.IsNull(licencService.RequestLicenseKey(Admin));
        }

        [TestMethod]
        public void TestKeyHash()
        {
            licencService = new DefaultLicenseServiceImpl();
            System.Console.WriteLine(licencService.RequestLicenseKey(Admin).Code);
        }
    }
}
