namespace CryptoKnight.Server.Console.Commands
{
    public class KeyStoreManagerCommand : CommandManager<Program>
    {
        public override string Description => "Manage the KeyStore.";

        public KeyStoreManagerCommand()
        {
            Commands.Add(new RegisterUserCommand());
            Commands.Add(new RequestUserKeyCommand());
        }
    }
}
