using CryptoKnight.Client.Core.Plugin;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptoKnight.Client.Plugin.Test
{
    [TestClass]
    public class TestPlugin
    {
        private IPlugin _plugin;
        private const string TestText = "Hello, CryptoKnight!";
        private const string Password = "blub";
        private const string OtherPassword = "blob";

        [TestMethod]
        public void TestAesPlugin()
        {
            _plugin = new Aes.Plugin();
            var data = _plugin.Encrypt(TestText, Password);
            Assert.AreEqual(TestText, _plugin.Decrypt(data, Password));
            Assert.AreNotEqual(TestText, _plugin.Decrypt(data, OtherPassword));
        }

        [TestMethod]
        public void TestDESPlugin()
        {
            _plugin = new DES.Plugin();
            var data = _plugin.Encrypt(TestText, Password);
            Assert.AreEqual(TestText, _plugin.Decrypt(data, Password));
            Assert.AreNotEqual(TestText, _plugin.Decrypt(data, OtherPassword));
        }

        [TestMethod]
        public void TestRC2Plugin()
        {
            _plugin = new RC2.Plugin();
            var data = _plugin.Encrypt(TestText, Password);
            Assert.AreEqual(TestText, _plugin.Decrypt(data, Password));
            Assert.AreNotEqual(TestText, _plugin.Decrypt(data, OtherPassword));
        }

        [TestMethod]
        public void TestRijindaelPlugin()
        {
            _plugin = new Rijindael.Plugin();
            var data = _plugin.Encrypt(TestText, Password);
            Assert.AreEqual(TestText, _plugin.Decrypt(data, Password));
            Assert.AreNotEqual(TestText, _plugin.Decrypt(data, OtherPassword));
        }

        [TestMethod]
        public void TestTripleDESPlugin()
        {
            _plugin = new TripleDES.Plugin();
            var data = _plugin.Encrypt(TestText, Password);
            Assert.AreEqual(TestText, _plugin.Decrypt(data, Password));
            Assert.AreNotEqual(TestText, _plugin.Decrypt(data, OtherPassword));
        }
    }
}
