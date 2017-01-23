using CryptoKnight.Library.Network;
using CryptoKnight.Library.Network.ProtocolMessages;
using CryptoKnight.Library.Network.ProtocolMessages.Client;
using CryptoKnight.Library.Network.ProtocolMessages.Server;
using CryptoKnight.Library.Network.ProtocolMessages.Server.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace CryptoKnight.Server.Core
{
    public class LicenseServer : TcpServer
    {
        private readonly IDictionary<TcpSocket, User> _loggedInUsers = new ConcurrentDictionary<TcpSocket, User>();

        public LicenseServer(IPEndPoint endPoint) : base(endPoint)
        {
            ClientSentData += OnClientSentData;
            ClientDisconnected += OnClientDisconnected;
        }

        private new void OnClientDisconnected(TcpSocket client)
        {
            var user = default(User);
            if (_loggedInUsers.TryGetValue(client, out user))
            {
                // TODO: Interact with IAuthService
                // IAuthService.Logout
                _loggedInUsers.Remove(client);
            }
        }

        private new void OnClientSentData(TcpSocket client, byte[] data)
        {
            try
            {
                var message = data.ToType<IMessage>();
                switch (message.Type)
                {
                    case MessageType.Login:
                        HandleData<IMessage, LoginMessage>(client, message, OnLogin);
                        break;

                    case MessageType.VerifyLicense:
                        HandleData<IMessage, VerifyLicenseMessage>(client, message, OnVerifyLicense);
                        break;

                    default:
                        // unknown message (invalid data)
                        client.Close();
                        break;
                }
            }
            catch (Exception)
            {
                // client sent invalid data
                client.Close();
            }
        }

        private void OnLogin(TcpSocket client, LoginMessage message)
        {
            Debug.WriteLine($"Client: {client.Id} tries to login with {message.Email} // {message.PasswordHash}");
            var loginUser = KeyStore.AvailableLicenseActivations.Keys.FirstOrDefault(user =>
                user.Email.ToLower().Equals(message.Email.ToLower()) &&
                user.PasswordHash.Equals(message.PasswordHash)
            );
            var loginStatus = LoginStatus.WrongEmailOrPassword;
            if (loginUser != null)
            {
                loginStatus = LoginStatus.LoggedIn;
                _loggedInUsers.Add(client, loginUser);
            }
            client.SendData(new LoginResponseMessage
            {
                Status = loginStatus
            });
        }

        private void OnVerifyLicense(TcpSocket client, VerifyLicenseMessage message)
        {
            Debug.WriteLine($"Client: {client.Id} tries to verify license {message.Code}");
            var user = default(User);
            if (_loggedInUsers.TryGetValue(client, out user))
            {
                Debug.WriteLine($"User: {user.Email} tries to verify a license!");
                var loggedIn = true; // TODO: Interact with IAuthService
                client.SendData(new VerifyLicenseResponseMessage
                {
                    Status = loggedIn ? LicenseStatus.Accepted : LicenseStatus.Denied,
                    Code = message.Code
                });
            }
            else
            {
                Debug.WriteLine($"Client: {client.Id} sent VerifyLicense before logging in");
                // user is not logged in
                // TODO: Decide how to handle it, disconnect the user? or allow invalid verify request?
            }
        }

    }
}
