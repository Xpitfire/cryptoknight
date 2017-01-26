using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CryptoKnight.Server.Console
{
    public class InputOutputHandler
    {
        private readonly TextReader _input = System.Console.In;
        private readonly TextWriter _output = System.Console.Out;

        public const string CancellationCommand = "exit";
        private const string EmailPattern = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        public int ReadInt(string name)
        {
            int value;
            WritePrompt(name);
            int.TryParse(_input.ReadLine(), out value);
            return value;
        }

        public string ReadString(string name = null)
        {
            WritePrompt(name);
            return _input.ReadLine();
        }

        public string ReadEmail(string info = null, string cancellation = CancellationCommand)
        {
            string email;
            while (!Regex.IsMatch(email = ReadString(info), EmailPattern)
                & !string.Equals(email, cancellation, StringComparison.InvariantCultureIgnoreCase)) ; // blocking read
            return email == cancellation ? null : email;
        }

        public void WriteLine(string str)
        {
            _output.WriteLine(str);
        }

        private void WritePrompt(string str = null)
        {
            if (str != null)
                _output.Write($"{str}> ");
        }
    }
}
