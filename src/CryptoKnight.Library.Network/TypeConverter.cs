using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CryptoKnight.Library.Network
{
    public static class TypeConverter
    {
        public static byte[] ToBytes(this object obj)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }

        public static T ToType<T>(this byte[] bytes)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
