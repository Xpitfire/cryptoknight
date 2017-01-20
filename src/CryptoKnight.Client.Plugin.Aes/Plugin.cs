using System;
using System.IO;
using CryptoKnight.Client.Core.Plugin;
using SysCrypto = System.Security.Cryptography;

namespace CryptoKnight.Client.Plugin.Aes
{
    public sealed class Plugin : MarshalByRefObject, IPlugin
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public Plugin()
        {
            using (var cryptoAlgorithm = SysCrypto.Aes.Create())
            {
                // Encrypt the string to an array of bytes.
                if (cryptoAlgorithm == null)
                    throw new InvalidProgramException("Require encryption instance!");
                _key = cryptoAlgorithm.Key;
                _iv = cryptoAlgorithm.IV;
            }
        }

        public byte[] Encrypt(string data)
        {
            try
            {
                // Check arguments.
                if (data == null || data.Length <= 0)
                    throw new ArgumentNullException(nameof(data));
                if (_key == null || _key.Length <= 0)
                    throw new ArgumentNullException(nameof(_key));
                if (_iv == null || _iv.Length <= 0)
                    throw new ArgumentNullException(nameof(_iv));
                byte[] encrypted;
                // Create an algorithm object
                // with the specified _key and IV.
                using (var cryptoAlgorithm = SysCrypto.Aes.Create())
                {
                    if (cryptoAlgorithm == null) return null;
                    cryptoAlgorithm.Key = _key;
                    cryptoAlgorithm.IV = _iv;

                    // Create a decrytor to perform the stream transform.
                    var encryptor = cryptoAlgorithm.CreateEncryptor(cryptoAlgorithm.Key, cryptoAlgorithm.IV);

                    // Create the streams used for encryption.
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt =
                            new SysCrypto.CryptoStream(msEncrypt, encryptor, SysCrypto.CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(data);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }
                // Return the encrypted bytes from the memory stream.
                return encrypted;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            return null;
        }

        public string Decrypt(byte[] data)
        {
            try
            {
                // Check arguments.
                if (data == null || data.Length <= 0)
                    throw new ArgumentNullException(nameof(data));
                if (_key == null || _key.Length <= 0)
                    throw new ArgumentNullException(nameof(_key));
                if (_iv == null || _iv.Length <= 0)
                    throw new ArgumentNullException(nameof(_iv));

                // Declare the string used to hold
                // the decrypted text.
                string plaintext;

                // Create an algorithm object
                // with the specified _key and IV.
                using (var cryptoAlgorithm = SysCrypto.Aes.Create())
                {
                    if (cryptoAlgorithm == null) return null;
                    cryptoAlgorithm.Key = _key;
                    cryptoAlgorithm.IV = _iv;

                    // Create a decrytor to perform the stream transform.
                    var decryptor = cryptoAlgorithm.CreateDecryptor(cryptoAlgorithm.Key, cryptoAlgorithm.IV);

                    // Create the streams used for decryption.
                    using (var msDecrypt = new MemoryStream(data))
                    {
                        using (var csDecrypt =
                            new SysCrypto.CryptoStream(msDecrypt, decryptor, SysCrypto.CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                return plaintext;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            return null;
        }
    }
}
