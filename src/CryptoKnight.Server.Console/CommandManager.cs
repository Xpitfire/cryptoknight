using System;
using System.Collections.Generic;

namespace CryptoKnight.Server.Console
{
    public class CommandManager<T> : ICommand<T> where T : class
    {
        public virtual string Description => string.Empty;
        public IList<ICommand<T>> Commands { get; }
        private readonly InputOutputHandler _io = new InputOutputHandler();

        public CommandManager()
        {
            Commands = new List<ICommand<T>>();
        }

        private void PrintCommands()
        {
            var index = 1;
            _io.WriteLine($"{Environment.NewLine}Commands:");
            foreach (var command in Commands)
            {
                _io.WriteLine($"{index++}. {command.Description}");
            }
            _io.WriteLine($"{index}. Exit{Environment.NewLine}");
        }

        public void ProcessCommands(T host)
        {
            var index = 0;
            while (index != Commands.Count + 1)
            {
                PrintCommands();
                index = _io.ReadInt("Command");
                if (index > 0 && index <= Commands.Count)
                {
                    Commands[index - 1].Execute(_io, host);
                }
            }
        }

        public void Execute(InputOutputHandler io, T host)
        {
            ProcessCommands(host);
        }
    }
}
