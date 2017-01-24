namespace CryptoKnight.Server.Console.Commands
{
    public class TcpServerManagerCommand : CommandManager<Program>
    {
        public override string Description => "Manage the TcpServer.";

        public TcpServerManagerCommand()
        {
            Commands.Add(new StartLicenseServerCommand());
            Commands.Add(new StopLicenseServerCommand());
            Commands.Add(new PrintLicenseServerInfosCommand());
        }
    }
}
