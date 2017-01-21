using System.IO;

namespace CryptoKnight.Server.Console
{
    public class InputOutputHandler
    {
        private readonly TextReader _input = System.Console.In;
        private readonly TextWriter _output = System.Console.Out;

        public int ReadInt(string name)
        {
            int value;
            WritePrompt(name);
            int.TryParse(_input.ReadLine(), out value);
            return value;
        }

        public string ReadString(string name)
        {
            WritePrompt(name);
            return _input.ReadLine();
        }

        public void WriteLine(string str)
        {
            _output.WriteLine(str);
        }

        private void WritePrompt(string str)
        {
            _output.Write(str + "> ");
        }
    }
}
