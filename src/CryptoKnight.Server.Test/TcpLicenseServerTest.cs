using CryptoKnight.Client.UI;
using CryptoKnight.Library.Network;
using CryptoKnight.Library.Network.ProtocolMessages;
using CryptoKnight.Library.Network.ProtocolMessages.Server;
using CryptoKnight.Library.Network.ProtocolMessages.Server.Enums;
using CryptoKnight.Server.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

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
        private static readonly User User = new User
        {
            Email = "user@host.com",
            PasswordHash = "user"
        };

        private static LicenseServer _server;
        private static LicenseClient _client;

        private static bool _loggedIn;
        private static bool _verified;


        private static ILicenseService _licencService;

        [TestMethod]
        public void TestLogin()
        {
            _server = new LicenseServer(EndPoint);
            _client = new LicenseClient(EndPoint);

            _loggedIn = false;
            _client.Connected += ClientOnConnected;
            _client.ReceivedData += ClientOnReceivedDataTestLogin;

            _server.Start();
            _client.Start();

            TimeoutLoop(() => _loggedIn);
            Assert.IsTrue(_loggedIn);

            _client.Stop();
            _server.Stop();
        }

        private void ClientOnReceivedDataTestLogin(TcpSocket server, byte[] data)
        {
            var message = data.ToType<IMessage>();
            Assert.AreEqual(message.Type, MessageType.LoginResponse);
            var loginMessage = message as LoginResponseMessage;
            Assert.IsNotNull(loginMessage);
            Assert.AreEqual(loginMessage.Status, LoginStatus.LoggedIn);
            _loggedIn = true;
        }

        [TestMethod]
        public void TestVerifyLicense()
        {
            _server = new LicenseServer(EndPoint);
            _client = new LicenseClient(EndPoint);

            _verified = false;
            _client.Connected += ClientOnConnected;
            _client.ReceivedData += ClientOnReceivedDataTestVerifyLicense;

            _server.Start();
            _client.Start();

            TimeoutLoop(() => _verified);
            Assert.IsTrue(_verified);

            _client.Stop();
            _server.Stop();
        }


        private void ClientOnReceivedDataTestVerifyLicense(TcpSocket server, byte[] data)
        {
            var message = data.ToType<IMessage>();
            if (message.Type == MessageType.VerifyLicenseResponse)
            {
                Assert.AreEqual(message.Type, MessageType.VerifyLicenseResponse);
                var verifyLicenseResponse = message as VerifyLicenseResponseMessage;
                Assert.IsNotNull(verifyLicenseResponse);
                Assert.AreEqual(verifyLicenseResponse.Status, LicenseStatus.Accepted);
                _verified = true;
            }
            else if (message.Type == MessageType.LoginResponse)
            {
                _licencService = new DefaultLicenseServiceImpl();
                var code = _licencService.RequestLicenseKey(User).Code;
                _client.VerifyLicense(code);
            }
        }

        private void ClientOnConnected(TcpSocket server)
        {
            _client.Login(User.Email, User.PasswordHash);
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
