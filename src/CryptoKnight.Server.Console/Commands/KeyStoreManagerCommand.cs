namespace CryptoKnight.Server.Console.Commands
{
    public class KeyStoreManagerCommand : CommandManager<Program>
    {
        public override string Description => "Manage the KeyStore.";

        public KeyStoreManagerCommand()
        {
            // TODO: Add additional commands (add new user, add new license key, remove, ...?)
            Commands.Add(new CreateKeyCommand());
        }
    }
}
