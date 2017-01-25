using CryptoKnight.Client.Core.Plugin;
using System;
using System.IO;
using System.Text;
using SysCrypto = System.Security.Cryptography;

namespace CryptoKnight.Client.Plugin.DES
{
    public sealed class Plugin : MarshalByRefObject, IPlugin
    {
        public string Name => "DES";

        private static readonly byte[] Salt = Encoding.ASCII.GetBytes("elephants can swim");

        public byte[] Encrypt(string data, string password)
        {
            try
            {
                // Check arguments.
                if (data == null || data.Length <= 0)
                    throw new ArgumentNullException(nameof(data));
                byte[] encrypted;
                // Create an algorithm object
                // with the specified _key and IV.
                using (var cryptoAlgorithm = SysCrypto.DES.Create())
                {
                    if (cryptoAlgorithm == null) return null;
                    var key = new SysCrypto.Rfc2898DeriveBytes(password, Salt);
                    cryptoAlgorithm.Key = key.GetBytes(cryptoAlgorithm.KeySize / 8);

                    // Create a decrytor to perform the stream transform.
                    var encryptor = cryptoAlgorithm.CreateEncryptor(cryptoAlgorithm.Key, cryptoAlgorithm.IV);

                    // Create the streams used for encryption.
                    using (var msEncrypt = new MemoryStream())
                    {
                        // prepend the IV
                        msEncrypt.Write(BitConverter.GetBytes(cryptoAlgorithm.IV.Length), 0, sizeof(int));
                        msEncrypt.Write(cryptoAlgorithm.IV, 0, cryptoAlgorithm.IV.Length);

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

        public string Decrypt(byte[] data, string password)
        {
            try
            {
                // Check arguments.
                if (data == null || data.Length <= 0)
                    throw new ArgumentNullException(nameof(data));

                // Declare the string used to hold
                // the decrypted text.
                string plaintext;

                // Create an algorithm object
                // with the specified _key and IV.
                using (var cryptoAlgorithm = SysCrypto.DES.Create())
                {
                    if (cryptoAlgorithm == null) return null;

                    // Create the streams used for decryption.
                    using (var msDecrypt = new MemoryStream(data))
                    {
                        var key = new SysCrypto.Rfc2898DeriveBytes(password, Salt);
                        cryptoAlgorithm.Key = key.GetBytes(cryptoAlgorithm.KeySize / 8);
                        cryptoAlgorithm.IV = ReadByteArray(msDecrypt);

                        // Create a decrytor to perform the stream transform.
                        var decryptor = cryptoAlgorithm.CreateDecryptor(cryptoAlgorithm.Key, cryptoAlgorithm.IV);

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


        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }
    }
}
