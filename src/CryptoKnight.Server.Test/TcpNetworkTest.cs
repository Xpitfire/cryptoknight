using CryptoKnight.Library.Network;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;

namespace CryptoKnight.Server.Test
{
    // TODO: implement proper unit tests (currently just "quicktests")
    [TestClass]
    public class TcpNetworkTest
    {

        private const string Localhost = "127.0.0.1";
        private const int Port = 1991;
        private const int NoOfClients = 10;
        private const int TestTimeout = 15000;
        private const int SleepInterval = 100;
        private const int NoOfTestMessages = 10;

        private static readonly IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse(Localhost), Port);
        private static readonly Random Random = new Random();

        private int _noOfMessages = 0;

        private int _clientConnected = 0;
        private int _clientDisconnected = 0;
        private int _clientReceivedData = 0;
        private readonly ConcurrentStack<byte[]> _clientReceivedDataStack = new ConcurrentStack<byte[]>();

        private int _serverClientConnected = 0;
        private int _serverClientDisconnected = 0;
        private int _serverClientReceivedData = 0;
        private readonly ConcurrentStack<byte[]> _serverClientReceivedDataStack = new ConcurrentStack<byte[]>();


        private void ResetStatistics()
        {
            _noOfMessages = 0;
            _clientConnected = 0;
            _clientDisconnected = 0;
            _clientReceivedData = 0;
            _clientReceivedDataStack.Clear();
            _serverClientConnected = 0;
            _serverClientDisconnected = 0;
            _serverClientReceivedData = 0;
            _serverClientReceivedDataStack.Clear();
        }

        private TcpServer CreateTcpServer()
        {
            var tcpServer = new TcpServer(EndPoint);
            tcpServer.ClientConnected += TcpServerOnClientConnected;
            tcpServer.ClientDisconnected += TcpServerOnClientDisconnected;
            tcpServer.ClientSentData += TcpServerOnClientSentData;
            tcpServer.Start();
            return tcpServer;
        }

        private TcpClient CreateTcpClient()
        {
            var tcpClient = new TcpClient(EndPoint);
            tcpClient.Connected += TcpClientOnConnected;
            tcpClient.Disconnected += TcpClientOnDisconnected;
            tcpClient.ReceivedData += TcpClientOnReceivedData;
            tcpClient.Start();
            return tcpClient;
        }

        [TestMethod]
        public void TestSingleClient()
        {
            ResetStatistics();
            using (var server = CreateTcpServer())
            using (var client = CreateTcpClient())
            {
                TimeoutLoop(() => _clientConnected != 0
                               && _serverClientConnected != 0
                               && _clientConnected == _serverClientConnected);
            }
            Assert.AreNotEqual(_clientConnected, 0);
            Assert.AreNotEqual(_serverClientConnected, 0);
            Assert.AreEqual(_clientConnected, _serverClientConnected);
        }

        [TestMethod]
        public void TestMultipleClients()
        {
            ResetStatistics();
            ICollection<TcpClient> clients = new List<TcpClient>(NoOfClients);
            using (var server = CreateTcpServer())
            {
                for (int i = 0; i < NoOfClients; ++i)
                {
                    clients.Add(CreateTcpClient());
                }
                TimeoutLoop(() => _clientConnected != 0
                               && _serverClientConnected != 0
                               && _clientConnected == NoOfClients
                               && _serverClientConnected == NoOfClients);
                foreach (var client in clients)
                {
                    client.Dispose();
                }
            }
            Assert.AreNotEqual(_clientConnected, 0);
            Assert.AreNotEqual(_serverClientConnected, 0);
            Assert.AreEqual(_clientConnected, NoOfClients);
            Assert.AreEqual(_serverClientConnected, NoOfClients);
        }

        [TestMethod]
        public void TestSendReceiveSingleClient()
        {
            ResetStatistics();
            _noOfMessages = NoOfTestMessages;
            using (var server = CreateTcpServer())
            using (var client = CreateTcpClient())
            {
                TimeoutLoop(() => _clientReceivedDataStack.Count != 0
                               && _serverClientReceivedDataStack.Count != 0
                               && _clientReceivedDataStack.Count == _noOfMessages
                               && _serverClientReceivedDataStack.Count == _noOfMessages);
            }
            Assert.AreNotEqual(_clientReceivedDataStack.Count, 0);
            Assert.AreNotEqual(_serverClientReceivedDataStack.Count, 0);
            Assert.AreEqual(_clientReceivedDataStack.Count, _noOfMessages);
            Assert.AreEqual(_serverClientReceivedDataStack.Count, _noOfMessages);
            // compare the stack content
            while (!_serverClientReceivedDataStack.IsEmpty)
            {
                byte[] clientData, serverData;
                _clientReceivedDataStack.TryPop(out clientData);
                _serverClientReceivedDataStack.TryPop(out serverData);
                Assert.IsTrue(clientData.SequenceEqual(serverData));
            }
        }

        [TestMethod]
        public void TestSendReceiveMultipleClients()
        {
            ResetStatistics();
            _noOfMessages = NoOfTestMessages;
            ICollection<TcpClient> clients = new List<TcpClient>(NoOfClients);
            using (var server = CreateTcpServer())
            {
                for (var i = 0; i < NoOfClients; ++i)
                {
                    clients.Add(CreateTcpClient());
                }
                TimeoutLoop(() => _clientReceivedDataStack.Count != 0
                       && _serverClientReceivedDataStack.Count != 0
                       && _clientReceivedDataStack.Count == _noOfMessages * NoOfClients
                       && _serverClientReceivedDataStack.Count == _noOfMessages * NoOfClients);
                foreach (var client in clients)
                {
                    client.Dispose();
                }
            }
            Assert.AreNotEqual(_clientReceivedDataStack.Count, 0);
            Assert.AreNotEqual(_serverClientReceivedDataStack.Count, 0);
            Assert.AreEqual(_clientReceivedDataStack.Count, _noOfMessages * NoOfClients);
            Assert.AreEqual(_serverClientReceivedDataStack.Count, _noOfMessages * NoOfClients);
            // TODO: due to multiple clients the stack comparison has to be adjusted
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

        #region TcpClient Events
        private void TcpClientOnConnected(TcpSocket server)
        {
            Interlocked.Increment(ref _clientConnected);
            for (var i = 0; i < _noOfMessages; ++i)
            {
                var size = Random.Next(1, 100);
                var data = new byte[size];
                Random.NextBytes(data);
                server.SendData(data);
            }
        }

        private void TcpClientOnDisconnected(TcpSocket server)
        {
            Interlocked.Increment(ref _clientDisconnected);
        }

        private void TcpClientOnReceivedData(TcpSocket server, byte[] data)
        {
            Interlocked.Increment(ref _clientReceivedData);
            _clientReceivedDataStack.Push(data);
        }
        #endregion // TcpClient Events


        #region TcpServer Events
        private void TcpServerOnClientConnected(TcpSocket socket)
        {
            Interlocked.Increment(ref _serverClientConnected);
        }

        private void TcpServerOnClientDisconnected(TcpSocket socket)
        {
            Interlocked.Increment(ref _serverClientDisconnected);
        }

        private void TcpServerOnClientSentData(TcpSocket socket, byte[] data)
        {
            Interlocked.Increment(ref _serverClientReceivedData);
            socket.SendData(data);
            _serverClientReceivedDataStack.Push(data);
        }
        #endregion // TcpServer Events
    }
}
