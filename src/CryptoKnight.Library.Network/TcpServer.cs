using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;

namespace CryptoKnight.Library.Network
{
    public class TcpServer : IDisposable
    {

        public int ActiveClients => _clients.Count;
        public bool Running { get; private set; }

        private TcpSocket _listenerSocket;
        private readonly ConcurrentDictionary<Guid, TcpSocket> _clients;
        private readonly IPEndPoint _endPoint;


        public delegate void ClientConnectedHandler(TcpSocket socket);

        public event ClientConnectedHandler ClientConnected;

        public delegate void ClientDisconnectedHandler(TcpSocket socket);

        public event ClientDisconnectedHandler ClientDisconnected;

        public delegate void ClientSentDataHandler(TcpSocket socket, byte[] data);

        public event ClientSentDataHandler ClientSentData;


        public TcpServer(IPEndPoint endPoint)
        {
            Running = false;
            _endPoint = endPoint;
            _clients = new ConcurrentDictionary<Guid, TcpSocket>();
        }

        public bool Start()
        {
            if (Running) return false;
            _listenerSocket = new TcpSocket(_endPoint);
            _listenerSocket.AcceptedConnection += OnClientConnected;
            _listenerSocket.StartListening();
            Running = true;
            return true;
        }

        public void Stop()
        {
            if (!Running) return;
            foreach (var client in _clients.ToArray())
            {
                client.Value.Close();
            }
            _clients.Clear();
            _listenerSocket.Close();
            _listenerSocket.AcceptedConnection -= OnClientConnected;
            Running = false;
        }

        public void BroadcastData(byte[] data)
        {
            foreach (var client in _clients)
            {
                client.Value.SendData(data);
            }
        }

        protected virtual void OnClientConnected(TcpSocket socket)
        {
            if (_clients.TryAdd(socket.Id, socket))
            {
                socket.ReceivedData += OnClientSentData;
                socket.Disconnected += OnClientDisconnected;
                ClientConnected?.Invoke(socket);
                socket.StartReceiving();
            }
            else
            {
                socket.Close();
            }
        }

        protected virtual void OnClientDisconnected(TcpSocket socket)
        {
            TcpSocket tmpSocket;
            _clients.TryRemove(socket.Id, out tmpSocket);
            socket.ReceivedData -= OnClientSentData;
            socket.Disconnected -= OnClientDisconnected;
            ClientDisconnected?.Invoke(socket);
        }

        protected virtual void OnClientSentData(TcpSocket socket, byte[] data)
        {
            if (data.Length == 0)
            {
                Debug.WriteLine($"Processing heartbeat for client {socket.Id}");
            }
            else
            {
                ClientSentData?.Invoke(socket, data);
            }
        }

        protected static void HandleData<TInterface, TData>(
            TcpSocket socket,
            TInterface message,
            Action<TcpSocket, TData> handler) where TData : class
        {
            var convertedMessage = message as TData;
            if (convertedMessage != null)
            {
                handler(socket, convertedMessage);
            }
            else
            {
                socket.Close();
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
