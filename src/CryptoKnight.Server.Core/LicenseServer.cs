using CryptoKnight.Library.Network;
using CryptoKnight.Library.Network.ProtocolMessages;
using CryptoKnight.Library.Network.ProtocolMessages.Client;
using CryptoKnight.Library.Network.ProtocolMessages.Server;
using CryptoKnight.Server.KeyGenerator;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Common = CryptoKnight.Library.Network.ProtocolMessages.Common;

namespace CryptoKnight.Server.Core
{
    public class LicenseServer : TcpServer
    {
        private const int DefaultExpireMinutes = 5;
        private const string KeyTemplate = "kkkk-kkk-kk-k-kk-kkk-kkkk";
        private readonly IAuthService _authService = new DefaultAuthService();
        private readonly ILicenseService _licenseService = new DefaultLicenseServiceImpl();
        private readonly Generator _generator = new Generator { Template = KeyTemplate };

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

            var encryptedKey = default(byte[]);
            var clientData = default(ClientData);
            var loginStatus = _authService.Login(user, key);
            if (loginStatus)
            {
                clientData = new ClientData
                {
                    User = user,
                    Key = key,
                    PluginPassword = _generator.CreateKey().Code
                };
                var fileKey = new Common.FileKey
                {
                    Password = clientData.PluginPassword,
                    Expire = DateTime.Now.AddMinutes(DefaultExpireMinutes)
                };
                encryptedKey = DataProtection.Encrypt(fileKey.ToBytes(), key.Code);
                _loggedInUsers.Add(client, clientData);
            }

            client.SendData(new LoginResponseMessage { Key = encryptedKey });
            if (loginStatus)
            {
                Task.Run(() => SendPlugins(client, clientData));
            }
        }

        private void SendPlugins(TcpSocket client, ClientData data)
        {
            foreach (var file in new DirectoryInfo(Path.Combine(".", "Plugins")).GetFiles("*.dll"))
            {
                client.SendData(new PluginResponseMessage
                {
                    Plugin = DataProtection.EncryptFile(file.FullName, data.PluginPassword)
                });
            }
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
