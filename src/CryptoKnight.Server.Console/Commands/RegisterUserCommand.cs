using CryptoKnight.Server.KeyGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using CryptoKnight.Library.Network;
using CryptoKnight.Server.Core;

namespace CryptoKnight.Server.Console.Commands
{
    class RegisterUserCommand : ICommand<Program>
    {
        public string Description => "Register User";

        public void Execute(InputOutputHandler io, Program host)
        {
            string email = null;
            while (string.IsNullOrEmpty(email = io.ReadString("Please type in the:"))) ; // wait for valid input
            var oneWayPassword = new Generator { Template = "kkkkkk" }.CreateKey();
            io.WriteLine($"Your one-way-password is: {oneWayPassword.Code}");
            host.LicenseService.RegisterUser(
                new User {
                Email = email,
                PasswordHash = oneWayPassword.Code.ComputeMd5Hash()
            });
        }
    }
}
