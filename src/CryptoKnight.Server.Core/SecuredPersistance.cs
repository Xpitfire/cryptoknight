using System;
using System.IO;
using System.Security.Cryptography;

namespace CryptoKnight.Server.Core
{
    internal static class SecuredPersistance
    {
        public static byte[] CreateRandomEntropy()
        {
            // Create a byte array to hold the random value.
            var entropy = new byte[16];

            // Create a new instance of the RNGCryptoServiceProvider.
            // Fill the array with a random value.
            new RNGCryptoServiceProvider().GetBytes(entropy);

            // Return the array.
            return entropy;
        }

        public static int SaveData(string fileName, byte[] data, byte[] entropy)
        {
            // Create a file.
            var fStream = new FileStream(fileName, FileMode.OpenOrCreate);

            // Encrypt a copy of the data to the stream.
            var bytesWritten = EncryptDataToStream(data, entropy, DataProtectionScope.CurrentUser, fStream);
            fStream.Close();
            return bytesWritten;
        }

        public static byte[] LoadData(string fileName, byte[] entropy, int bytesToRead)
        {
            // Open the file.
            var fStream = new FileStream(fileName, FileMode.Open);

            // Read from the stream and decrypt the data.
            var decryptData = DecryptDataFromStream(entropy, DataProtectionScope.CurrentUser, fStream, bytesToRead);

            fStream.Close();
            return decryptData;
        }

        public static int EncryptDataToStream(byte[] buffer, byte[] entropy, DataProtectionScope scope, Stream stream)
        {
            if (buffer.Length <= 0)
                throw new ArgumentException(nameof(buffer));
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (entropy.Length <= 0)
                throw new ArgumentException(nameof(entropy));
            if (entropy == null)
                throw new ArgumentNullException(nameof(entropy));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var length = 0;

            // Encrypt the data in memory. The result is stored in the same same array as the original data.
            var encrptedData = ProtectedData.Protect(buffer, entropy, scope);

            // Write the encrypted data to a stream.
            if (!stream.CanWrite) return length;
            stream.Write(encrptedData, 0, encrptedData.Length);

            length = encrptedData.Length;

            // Return the length that was written to the stream. 
            return length;
        }

        public static byte[] DecryptDataFromStream(byte[] entropy, DataProtectionScope scope, Stream stream, int length)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (length <= 0)
                throw new ArgumentException(nameof(length));
            if (entropy == null)
                throw new ArgumentNullException(nameof(entropy));
            if (entropy.Length <= 0)
                throw new ArgumentException(nameof(entropy));

            var inBuffer = new byte[length];
            byte[] outBuffer;

            // Read the encrypted data from a stream.
            if (stream.CanRead)
            {
                stream.Read(inBuffer, 0, length);

                outBuffer = ProtectedData.Unprotect(inBuffer, entropy, scope);
            }
            else
            {
                throw new IOException("Could not read the stream.");
            }

            // Return the length that was written to the stream. 
            return outBuffer;
        }
    }
}
