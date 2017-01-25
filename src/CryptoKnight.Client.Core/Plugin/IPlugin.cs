namespace CryptoKnight.Client.Core.Plugin
{
    public interface IPlugin
    {
        string Name { get; }

        byte[] Encrypt(string data, string password);
        string Decrypt(byte[] data, string password);
    }
}
