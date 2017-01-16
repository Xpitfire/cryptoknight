using System;
using System.Diagnostics;
using CryptoKnight.Client.Core.Plugin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sandbox.AddIns.Simple;

namespace CryptoKnight.Client.Plugin.Test
{
    [TestClass]
    public class TestAesPlugin
    {
        [TestMethod]
        public void TestPlugin()
        {
            var plugin = new AesPlugin();
            var text = "Hello CryptoKnight!";
            var data = plugin.Encrypt(text);
            Assert.AreEqual(text, plugin.Decrypt(data));
        }
    }
}
