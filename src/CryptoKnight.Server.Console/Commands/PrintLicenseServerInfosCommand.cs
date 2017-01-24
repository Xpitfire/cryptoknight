namespace CryptoKnight.Server.Console.Commands
{
    public class PrintLicenseServerInfosCommand : ICommand<Program>
    {
        public string Description => "Print LicenseServer Infos!";

        public void Execute(InputOutputHandler io, Program host)
        {
            var server = host.LicenseServer;
            io.WriteLine($"Running: {server.Running}");
            io.WriteLine($"ActiveClients: {server.ActiveClients}");
        }
    }
}
