using CryptoKnight.Client.Core.Plugin;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptoKnight.Client.Plugin.Test
{
    [TestClass]
    public class TestPlugin
    {
        private IPlugin _plugin;
        private const string TestText = "Hello, CryptoKnight!";

        [TestMethod]
        public void TestAesPlugin()
        {
            _plugin = new Aes.Plugin();
            var data = _plugin.Encrypt(TestText);
            Assert.AreEqual(TestText, _plugin.Decrypt(data));
        }

        [TestMethod]
        public void TestDESPlugin()
        {
            _plugin = new DES.Plugin();
            var data = _plugin.Encrypt(TestText);
            Assert.AreEqual(TestText, _plugin.Decrypt(data));
        }

        [TestMethod]
        public void TestRC2Plugin()
        {
            _plugin = new RC2.Plugin();
            var data = _plugin.Encrypt(TestText);
            Assert.AreEqual(TestText, _plugin.Decrypt(data));
        }

        [TestMethod]
        public void TestRijindaelPlugin()
        {
            _plugin = new Rijindael.Plugin();
            var data = _plugin.Encrypt(TestText);
            Assert.AreEqual(TestText, _plugin.Decrypt(data));
        }

        [TestMethod]
        public void TestTripleDESPlugin()
        {
            _plugin = new TripleDES.Plugin();
            var data = _plugin.Encrypt(TestText);
            Assert.AreEqual(TestText, _plugin.Decrypt(data));
        }
    }
}
