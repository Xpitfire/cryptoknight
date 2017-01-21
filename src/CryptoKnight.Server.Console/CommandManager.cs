using System;
using System.Collections.Generic;

namespace CryptoKnight.Server.Console
{
    public class CommandManager : ICommand
    {
        public virtual string Description => string.Empty;
        public IList<ICommand> Commands { get; }
        private readonly InputOutputHandler _io = new InputOutputHandler();

        public CommandManager()
        {
            Commands = new List<ICommand>();
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

        public void ProcessCommands()
        {
            var index = 0;
            while (index != Commands.Count + 1)
            {
                PrintCommands();
                index = _io.ReadInt("Command");
                if (index > 0 && index <= Commands.Count)
                {
                    Commands[index - 1].Execute(_io);
                }
            }
        }

        public void Execute(InputOutputHandler io)
        {
            ProcessCommands();
        }
    }
}
