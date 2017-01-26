using CryptoKnight.Server.KeyGenerator;
using System;
using CryptoKnight.Library.Network;
using CryptoKnight.Server.Core;

namespace CryptoKnight.Server.Console.Commands
{
    class RegisterUserCommand : ICommand<Program>
    {
        public string Description => "Register new User.";
        private static readonly string CommandInfo = $"Please type in a valid email or '{InputOutputHandler.CancellationCommand}' to quit";

        public void Execute(InputOutputHandler io, Program host)
        {
            string email = io.ReadEmail(CommandInfo);
            if (email == null) return;
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
