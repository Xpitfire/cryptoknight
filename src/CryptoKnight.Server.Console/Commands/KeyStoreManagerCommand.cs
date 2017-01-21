namespace CryptoKnight.Server.Console.Commands
{
    public class KeyStoreManagerCommand : CommandManager
    {
        public override string Description => "Manage the KeyStore.";

        public KeyStoreManagerCommand()
        {
            // TODO: Add additional commands (add new user, add new license key, remove, ...?)
            Commands.Add(new CreateKeyCommand());
        }
    }
}
