namespace CryptoKnight.Server.Console.Commands
{
    public class StopLicenseServerCommand : ICommand<Program>
    {
        public string Description => "Stop License Server!";

        public void Execute(InputOutputHandler io, Program host)
        {
            var server = host.LicenseServer;
            if (server.Running)
            {
                server.Stop();
                io.WriteLine("Stopped LicenseServer!");
            }
            else
            {
                io.WriteLine("Server is not running.");
            }
        }
    }
}
