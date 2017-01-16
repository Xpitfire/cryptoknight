using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoKnight.Client.Core.Plugin
{
    public interface IPlugin
    {
        byte[] Encrypt(string data);
        string Decrypt(byte[] data);
    }
}
