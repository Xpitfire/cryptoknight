using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CryptoKnight.Server.Core
{
    public class DataProtection
    {
        private static readonly byte[] Salt = Encoding.ASCII.GetBytes("abcdefghijklmnopqrstuvwxyz");
        private static readonly byte[] IV = new byte[16];

        public static byte[] EncryptFile(string filePath, string password)
        {
            var readBytes = File.ReadAllBytes(filePath);
            return Encrypt(readBytes, password);
        }

        public static byte[] DecryptFile(string filePath, string password)
        {
            var readBytes = File.ReadAllBytes(filePath);
            return Decrypt(readBytes, password);
        }

        public static byte[] Encrypt(byte[] data, string password)
        {
            try
            {
                using (var cryptoAlgorithm = Aes.Create())
                {
                    if (cryptoAlgorithm == null) return null;
                    var key = new Rfc2898DeriveBytes(password, Salt);
                    cryptoAlgorithm.Key = key.GetBytes(cryptoAlgorithm.KeySize / 8);
                    cryptoAlgorithm.IV = IV;

                    var encryptor = cryptoAlgorithm.CreateEncryptor(cryptoAlgorithm.Key, cryptoAlgorithm.IV);
                    return PerformCryptography(encryptor, data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            return null;
        }

        public static byte[] Decrypt(byte[] data, string password)
        {
            try
            {
                if (data == null || data.Length <= 0)
                    throw new ArgumentNullException(nameof(data));

                using (var cryptoAlgorithm = System.Security.Cryptography.Aes.Create())
                {
                    if (cryptoAlgorithm == null) return null;
                    var key = new Rfc2898DeriveBytes(password, Salt);
                    cryptoAlgorithm.Key = key.GetBytes(cryptoAlgorithm.KeySize / 8);
                    cryptoAlgorithm.IV = IV;

                    var decryptor = cryptoAlgorithm.CreateDecryptor(cryptoAlgorithm.Key, cryptoAlgorithm.IV);
                    return PerformCryptography(decryptor, data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            return null;
        }

        private static byte[] PerformCryptography(ICryptoTransform cryptoTransform, byte[] data)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                    return memoryStream.ToArray();
                }
            }
        }
    }
}
