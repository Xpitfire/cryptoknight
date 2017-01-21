using CryptoKnight.Server.Console.Commands;

namespace CryptoKnight.Server.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmdManager = new CommandManager();
            cmdManager.Commands.Add(new TcpServerManagerCommand());
            cmdManager.Commands.Add(new KeyStoreManagerCommand());
            cmdManager.ProcessCommands();
        }
    }
}
