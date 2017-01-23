using CryptoKnight.Library.Network;
using CryptoKnight.Library.Network.ProtocolMessages;
using CryptoKnight.Library.Network.ProtocolMessages.Client;
using CryptoKnight.Library.Network.ProtocolMessages.Server;
using CryptoKnight.Library.Network.ProtocolMessages.Server.Enums;
using CryptoKnight.Server.KeyGenerator;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace CryptoKnight.Server.Core
{
    // implemented the server with the following existing classes
    // LicenseRuntime, KeyStore (can and should be replaced by a service)
    public class LicenseServer : TcpServer
    {

        public const int MaxLicenseActivations = 1;

        private readonly IDictionary<TcpSocket, ClientData> _loggedInUsers = new ConcurrentDictionary<TcpSocket, ClientData>();

        public LicenseServer(IPEndPoint endPoint) : base(endPoint)
        {
            ClientSentData += OnClientSentData;
            ClientDisconnected += OnClientDisconnected;
        }

        private new void OnClientDisconnected(TcpSocket client)
        {
            var clientData = default(ClientData);
            if (!_loggedInUsers.TryGetValue(client, out clientData)) return;
            _loggedInUsers.Remove(client);

            // TODO: can be replaced with any IAuthService if needed
            // IAuthService.Logout
            foreach (var key in clientData.LicenseGroup.Keys)
            {
                if (!LicenseRuntime.ActiveInstance.ContainsKey(key)) continue;
                LicenseRuntime.ActiveInstance[key] -= 1;
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
                _loggedInUsers.Add(client, new ClientData
                {
                    User = loginUser,
                    LicenseGroup = new LicenseGroup()
                });
            }
            client.SendData(new LoginResponseMessage
            {
                Status = loginStatus
            });
        }

        private void OnVerifyLicense(TcpSocket client, VerifyLicenseMessage message)
        {
            Debug.WriteLine($"Client: {client.Id} tries to verify license {message.Code}");
            var clientData = default(ClientData);
            if (_loggedInUsers.TryGetValue(client, out clientData))
            {
                Debug.WriteLine($"User: {clientData.User.Email} tries to verify a license!");

                // TODO: can be replaced with any IAuthService if needed
                // IAuthService.Login
                var verifyKey = default(Key);
                foreach (var key in KeyStore.AvailableLicenseActivations[clientData.User].Keys)
                {
                    if (!key.Code.Equals(message.Code)) continue;
                    verifyKey = key;
                    break;
                }

                var isActive = LicenseRuntime.ActiveInstance.ContainsKey(verifyKey);
                if (!isActive)
                {
                    LicenseRuntime.ActiveInstance[verifyKey] = 0;
                }
                var accept = LicenseRuntime.ActiveInstance[verifyKey] < MaxLicenseActivations;
                if (accept)
                {
                    LicenseRuntime.ActiveInstance[verifyKey] += 1;
                    clientData.LicenseGroup.Keys.Add(verifyKey);
                }
                client.SendData(new VerifyLicenseResponseMessage
                {
                    Status = accept ? LicenseStatus.Accepted : LicenseStatus.Denied,
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
