namespace CryptoKnight.Server.Console.Commands
{
    public class StartLicenseServerCommand : ICommand<Program>
    {
        public string Description => "Start License Server!";

        public void Execute(InputOutputHandler io, Program host)
        {
            var server = host.LicenseServer;
            if (!server.Running)
            {
                io.WriteLine("Attempting to start LicenseServer.");
                io.WriteLine(host.LicenseServer.Start()
                    ? "Started LicenseServer!"
                    : "An error occurred when starting the server.");
            }
            else
            {
                io.WriteLine("Cannot start server as it is already running.");
            }
        }
    }
}
