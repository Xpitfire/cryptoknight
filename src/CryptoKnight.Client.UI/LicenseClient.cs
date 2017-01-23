using CryptoKnight.Library.Network;
using CryptoKnight.Library.Network.ProtocolMessages;
using CryptoKnight.Library.Network.ProtocolMessages.Client;
using CryptoKnight.Library.Network.ProtocolMessages.Server;
using System;
using System.Diagnostics;
using System.Net;

namespace CryptoKnight.Client.UI
{
    // this class should preferably be located in CryptoKnight.Client.Core
    // but since CryptoKnight.Client.Core is used as the "Plugin" interface 
    // between the client and the plugins it is not recommended to put the
    // license client code in there
    // TODO: preferably renaming CryptoKnight.Client.Core to CryptoKnight.Client.Plugin
    // and creating a new CryptoKnight.Client.Core class library that serves as core
    // for the client the same way as CryptoKnight.Server.Core serves as core for the server
    public class LicenseClient : TcpClient
    {
        public LicenseClient(IPEndPoint endPoint) : base(endPoint)
        {
            Connected += OnConnected;
            Disconnected += OnDisconnected;
            ReceivedData += OnReceivedData;
        }

        public void Login(string email, string passwordHash)
        {
            SendData(new LoginMessage
            {
                Email = email,
                PasswordHash = passwordHash
            });
        }

        public void VerifyLicense(string code)
        {
            SendData(new VerifyLicenseMessage
            {
                Code = code
            });
        }

        private new void OnConnected(TcpSocket server)
        {
            // TODO: Implement the handling of the on connected event
            // dummy implementation (login as user)
            // Login("daniel.glaser@crexis.com", "12345");
        }

        private new void OnDisconnected(TcpSocket server)
        {
            // TODO: Implement the handling of the on disconnected event
        }

        private new void OnReceivedData(TcpSocket server, byte[] data)
        {
            try
            {
                var message = data.ToType<IMessage>();
                switch (message.Type)
                {
                    case MessageType.LoginResponse:
                        HandleData<IMessage, LoginResponseMessage>(server, message, OnLoginResponse);
                        break;

                    case MessageType.VerifyLicenseResponse:
                        HandleData<IMessage, VerifyLicenseResponseMessage>(server, message, OnVerifyLicenseResponse);
                        break;

                    default:
                        // unknown message (invalid data)
                        server.Close();
                        break;
                }

            }
            catch (Exception)
            {
                // server sent invalid data
                server.Close();
            }
        }

        private void OnVerifyLicenseResponse(VerifyLicenseResponseMessage response)
        {
            // TODO: Implement the handling of the verify license response
            Debug.WriteLine($@"License {response.Code} got {response.Status}!");
        }

        private void OnLoginResponse(LoginResponseMessage response)
        {
            // TODO: Implement the handling of the login response
            Debug.WriteLine($@"Login response status: {response.Status}");
            // dummy implementation: when logged in - verify a license
            // if (response.Status == LoginStatus.LoggedIn)
            // {
            //     VerifyLicense("abcdefghijklmnopqrstuvwxyz");
            // }
        }
    }
}
