namespace CryptoKnight.Server.Console
{
    public interface ICommand<T>
    {
        string Description { get; }
        void Execute(InputOutputHandler io, T host);
    }
}
