using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoKnight.Client.Core.Plugin;

namespace CryptoKnight.Client.Plugin.DES
{
    public sealed class DESPlugin : MarshalByRefObject, IPlugin
    {
        public byte[] Encrypt(string data)
        {
            Console.WriteLine("blib");
            return null;
        }

        public string Decrypt(byte[] data)
        {
            Console.WriteLine("bleb");
            return null;
        }
    }
}
