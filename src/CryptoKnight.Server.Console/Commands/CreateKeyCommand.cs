namespace CryptoKnight.Server.Console.Commands
{
    public class CreateKeyCommand : ICommand
    {
        public string Description => "Create a new license key.";

        public void Execute(InputOutputHandler io)
        {
            // TODO: Create a new license key and assign it to a specific user.
            io.WriteLine("Creating a new license key...");
        }
    }
}
