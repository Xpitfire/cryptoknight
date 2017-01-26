using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoKnight.Server.Console.Commands
{
    class RequestUserKeyCommand : ICommand<Program>
    {
        public string Description => "Print all User Keys.";

        public void Execute(InputOutputHandler io, Program host)
        {
            host.LicenseService.ShowInfo();
        }
    }
}
