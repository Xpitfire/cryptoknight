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
        private static readonly User Admin = new User
        {
            Email = "admin@host.com",
            PasswordHash = "admin"
        };

        private static readonly LicenseServer Server = new LicenseServer(EndPoint);
        private static readonly LicenseClient Client = new LicenseClient(EndPoint);

        private static bool _loggedIn;

        [TestMethod]
        public void TestLogin()
        {
            _loggedIn = false;
            Client.Connected += ClientOnConnected;
            Client.ReceivedData += ClientOnReceivedData;

            Server.Start();
            Client.Start();

            TimeoutLoop(() => _loggedIn);
            Assert.IsTrue(_loggedIn);

            Client.Stop();
            Server.Stop();
        }

        private void ClientOnReceivedData(TcpSocket server, byte[] data)
        {
            var message = data.ToType<IMessage>();
            Assert.AreEqual(message.Type, MessageType.LoginResponse);
            var loginMessage = message as LoginResponseMessage;
            Assert.IsNotNull(loginMessage);
            Assert.AreEqual(loginMessage.Status, LoginStatus.LoggedIn);
            _loggedIn = true;
        }

        private void ClientOnConnected(TcpSocket server)
        {
            Client.Login(Admin.Email, Admin.PasswordHash);
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
