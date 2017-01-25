using CryptoKnight.Library.Network;
using CryptoKnight.Library.Network.ProtocolMessages;
using CryptoKnight.Library.Network.ProtocolMessages.Client;
using CryptoKnight.Library.Network.ProtocolMessages.Server;
using CryptoKnight.Server.KeyGenerator;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Common = CryptoKnight.Library.Network.ProtocolMessages.Common;

namespace CryptoKnight.Server.Core
{
    public class LicenseServer : TcpServer
    {
        public const int MaxLicenseActivations = 1;

        private readonly IAuthService _authService = new DefaultAuthService();
        private readonly ILicenseService _licenseService = new DefaultLicenseServiceImpl();

        private readonly IDictionary<TcpSocket, ClientData> _loggedInUsers = new ConcurrentDictionary<TcpSocket, ClientData>();

        public LicenseServer(IPEndPoint endPoint) : base(endPoint)
        {
            ClientSentData += OnClientSentData;
            ClientDisconnected += OnClientDisconnected;
        }

        private void Logout(TcpSocket client)
        {
            var clientData = default(ClientData);
            if (!_loggedInUsers.TryGetValue(client, out clientData)) return;
            _loggedInUsers.Remove(client);
            _authService.Logout(clientData.User, clientData.Key);
        }

        private void OnClientDisconnected(TcpSocket client)
        {
            Logout(client);
        }

        private void OnClientSentData(TcpSocket client, byte[] data)
        {

            try
            {
                var message = data.ToType<IMessage>();
                switch (message.Type)
                {
                    case MessageType.Login:
                        HandleData<LoginMessage>(client, message, OnLogin);
                        break;

                    case MessageType.RequestLicense:
                        HandleData<RequestLicenseMessage>(client, message, OnRequestLicense);
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
            Debug.WriteLine($"Client: {client.Id} ({message.User.Email} / {message.User.PasswordHash}) tries to login.");
            Logout(client); // to avoid multiple key reservation from one connection

            var user = new User
            {
                Email = message.User.Email,
                PasswordHash = message.User.PasswordHash
            };
            var key = new Key { Code = message.Key.Code };

            var loginStatus = _authService.Login(user, key);
            if (loginStatus)
            {
                _loggedInUsers.Add(client, new ClientData
                {
                    User = user,
                    Key = key
                });
            }
            client.SendData(new LoginResponseMessage { LoggedIn = loginStatus });
        }

        private void OnRequestLicense(TcpSocket client, RequestLicenseMessage message)
        {
            Debug.WriteLine($"Client: {client.Id} ({message.User.Email} / {message.User.PasswordHash}) requests a license key.");
            var user = new User
            {
                Email = message.User.Email,
                PasswordHash = message.User.PasswordHash
            };
            var key = _licenseService.RequestLicenseKey(user);
            client.SendData(new RequestLicenseResponseMessage
            {
                Key = key == null ? null : new Common.Key { Code = key.Code }
            });
        }

    }
}
