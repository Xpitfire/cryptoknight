using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CryptoKnight.Server.KeyGenerator
{
    public class Generator
    {
        private static readonly Random Random = new Random();
        private const char K = 'k';
        private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public string Template { get; set; }

        public Key CreateKey()
        {
            var key = new StringBuilder();
            using (var md5 = MD5.Create())
            {
                foreach (var c in Template.ToCharArray())
                {
                    switch (c)
                    {
                        case K:
                            key.Append(
                                Letters[Random.Next(Letters.Length)]);
                            break;
                        default:
                            key.Append(c);
                            break;
                    }
                }
                using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(Template)))
                {
                    var hash = md5.ComputeHash(stream);
                    return new Key
                    {
                        Code = key.ToString(),
                        Checksum = hash,
                        Used = false
                    };
                }
            }
        }
    }
}
