using CryptoKnight.Library.Network;
using CryptoKnight.Library.Network.ProtocolMessages;
using CryptoKnight.Library.Network.ProtocolMessages.Client;
using CryptoKnight.Library.Network.ProtocolMessages.Common;
using CryptoKnight.Library.Network.ProtocolMessages.Server;
using System;
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

        public delegate void RequestLicenseResponseHandler(RequestLicenseResponseMessage message);

        public event RequestLicenseResponseHandler RequestLicenseResponse;

        public delegate void LoginResponseHandler(LoginResponseMessage message);

        public event LoginResponseHandler LoginResponse;

        public delegate void PluginResponseHandler(PluginResponseMessage message);
        public event PluginResponseHandler PluginResponse;

        public LicenseClient(IPEndPoint endPoint) : base(endPoint)
        {
            ReceivedData += OnReceivedData;
        }

        public void Login(User user, Key key)
        {
            SendData(new LoginMessage
            {
                User = user,
                Key = key
            });
        }

        public void RequestLicense(User user)
        {
            SendData(new RequestLicenseMessage
            {
                User = user
            });
        }

        private void OnReceivedData(TcpSocket server, byte[] data)
        {
            try
            {
                var message = data.ToType<IMessage>();
                switch (message.Type)
                {
                    case MessageType.LoginResponse:
                        HandleData<LoginResponseMessage>(server, message, OnLoginResponse);
                        break;

                    case MessageType.RequestLicenseResponse:
                        HandleData<RequestLicenseResponseMessage>(server, message, OnRequestLicenseResponse);
                        break;

                    case MessageType.PluginResponse:
                        HandleData<PluginResponseMessage>(server, message, OnPluginResponse);
                        break;

                    default:
                        // unknown message (invalid data)
                        server.Close();
                        break;
                }

            }
            catch
            {
                // server sent invalid data
                server.Close();
            }
        }

        protected virtual void OnPluginResponse(PluginResponseMessage message)
        {
            PluginResponse?.Invoke(message);
        }

        protected virtual void OnRequestLicenseResponse(RequestLicenseResponseMessage message)
        {
            RequestLicenseResponse?.Invoke(message);
        }

        protected virtual void OnLoginResponse(LoginResponseMessage message)
        {
            LoginResponse?.Invoke(message);
        }
    }
}
