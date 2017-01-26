using CryptoKnight.Server.KeyGenerator;
using System;
using CryptoKnight.Library.Network;
using CryptoKnight.Server.Core;

namespace CryptoKnight.Server.Console.Commands
{
    class RegisterUserCommand : ICommand<Program>
    {
        public string Description => "Register User";
        private const string CancellationCommand = "exit";
        private static readonly string CommandInfo = $"Please type in a valid email or '{CancellationCommand}' to quit >";

        public void Execute(InputOutputHandler io, Program host)
        {
            string email = null;
            // wait for valid input
            while (string.IsNullOrEmpty(email = io.ReadString(CommandInfo)))
            {
                if (string.Equals(email, CancellationCommand, StringComparison.InvariantCultureIgnoreCase)) return;
                io.WriteLine(CommandInfo);
            }

            var oneWayPassword = new Generator { Template = "kkkkkk" }.CreateKey();
            io.WriteLine($"Your one-way-password is: {oneWayPassword.Code}");
            host.LicenseService.RegisterUser(
                new User
                {
                    Email = email,
                    PasswordHash = oneWayPassword.Code.ComputeMd5Hash()
                });
        }
    }
}
