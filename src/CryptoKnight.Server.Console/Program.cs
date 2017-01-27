using CryptoKnight.Server.Console.Commands;
using CryptoKnight.Server.Core;
using System;
using System.Net;

namespace CryptoKnight.Server.Console
{
    public class Program : IDisposable
    {
        private const string Localhost = "127.0.0.1";
        private const int Port = 1991;
        private static readonly IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse(Localhost), Port);

        public CommandManager<Program> CommandManager;
        public LicenseServer LicenseServer { get; set; }
        public ILicenseService LicenseService { get; set; }

        public void Run()
        {
            LicenseServer = new LicenseServer(EndPoint);
            LicenseService = new DefaultLicenseService();
            CommandManager = new CommandManager<Program>();
            CommandManager.Commands.Add(new TcpServerManagerCommand());
            CommandManager.Commands.Add(new KeyStoreManagerCommand());
            CommandManager.ProcessCommands(this);
        }

        private static void Main(string[] args)
        {
            using (var program = new Program())
            {
                program.Run();
            }
        }

        public void Dispose()
        {
            LicenseServer?.Stop();
        }
    }
}
