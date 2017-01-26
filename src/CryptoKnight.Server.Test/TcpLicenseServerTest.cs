using CryptoKnight.Client.UI;
using CryptoKnight.Library.Network;
using CryptoKnight.Library.Network.ProtocolMessages;
using CryptoKnight.Library.Network.ProtocolMessages.Server;
using CryptoKnight.Server.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Common = CryptoKnight.Library.Network.ProtocolMessages.Common;

namespace CryptoKnight.Server.Test
{
    // TODO: Fix project dependencies
    // Due to the structure where the LicenseClient is located a reference to
    // "CryptoKnight.Client.UI" had to be made to test the server with a proper client
    // read the suggestions stated in LicenseClient.cs (located in CryptoKnight.Client.UI)
    [TestClass]
    public class TcpLicenseServerTest
    {
        private const int TestTimeout = 15000;
        private const int SleepInterval = 100;

        private const string Localhost = "127.0.0.1";
        private const int Port = 1991;
        private static readonly IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse(Localhost), Port);
        private static readonly Common.User User = new Common.User
        {
            Email = "user@host.com",
            PasswordHash = "user"
        };

        private static LicenseServer _server;
        private static LicenseClient _client;

        private static Common.Key _key;
        private static bool _loggedIn;

        [TestMethod]
        public void TestLogin()
        {
            // this test currently also tests the requests license key functionality
            // which should be done in a separate test
            // the KeyStore currently generates keys upon start
            // as a result I do not know which key is available and therefore 
            // I have to request a key and use that key to test the login
            // instead of using a static fixed key
            _server = new LicenseServer(EndPoint);
            _client = new LicenseClient(EndPoint);

            _key = null;
            _loggedIn = false;
            _client.Connected += ClientOnConnected;
            _client.ReceivedData += ClientOnReceivedDataTestRequestLicense;

            _server.Start();
            _client.Start();

            TimeoutLoop(() => _loggedIn);
            Assert.IsTrue(_loggedIn);

            _client.Stop();
            _server.Stop();
        }

        private void ClientOnReceivedDataTestRequestLicense(TcpSocket server, byte[] data)
        {
            var message = data.ToType<IMessage>();
            switch (message.Type)
            {
                case MessageType.LoginResponse:
                    var loginMessage = message as LoginResponseMessage;
                    Assert.IsNotNull(loginMessage);
                    _loggedIn = loginMessage.Key != null;
                    break;

                case MessageType.RequestLicenseResponse:
                    var requestLicenseMessage = message as RequestLicenseResponseMessage;
                    Assert.IsNotNull(requestLicenseMessage);
                    Assert.IsNotNull(requestLicenseMessage.Key);
                    _key = requestLicenseMessage.Key;
                    _client.Login(User, _key);
                    break;
            }
        }

        private void ClientOnConnected(TcpSocket server)
        {
            _client.RequestLicense(User);
        }

        private static void TimeoutLoop(Func<bool> condition)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < TestTimeout)
            {
                Thread.Sleep(SleepInterval);
                if (condition())
                    break;
            }
        }
    }
}
