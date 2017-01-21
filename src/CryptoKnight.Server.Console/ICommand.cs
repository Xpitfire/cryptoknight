namespace CryptoKnight.Server.Console
{
    public interface ICommand
    {
        string Description { get; }
        void Execute(InputOutputHandler io);
    }
}
