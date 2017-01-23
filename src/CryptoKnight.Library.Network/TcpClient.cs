using System;
using System.Net;
using System.Threading;

namespace CryptoKnight.Library.Network
{
    public class TcpClient : IDisposable
    {
        public bool Heartbeat { get; set; }
        public int HeartbeatInterval { get; set; }
        private bool _heartbeatRunning;

        private TcpSocket _serverSocket;
        private readonly IPEndPoint _endPoint;
        private Thread _heartbeatThread;

        public delegate void ConnectedHandler(TcpSocket server);

        public event ConnectedHandler Connected;

        public delegate void ReceivedDataHandler(TcpSocket server, byte[] data);

        public event ReceivedDataHandler ReceivedData;

        public delegate void DisconnectedHandler(TcpSocket server);

        public event DisconnectedHandler Disconnected;

        public TcpClient(IPEndPoint endPoint, bool heartbeat = true, int heartbeatInterval = 5000)
        {
            _endPoint = endPoint;
            Heartbeat = heartbeat;
            HeartbeatInterval = heartbeatInterval;
        }

        public void Start()
        {
            _serverSocket = new TcpSocket(_endPoint);
            _serverSocket.Connected += OnConnected;
            _serverSocket.ReceivedData += OnReceivedData;
            _serverSocket.Disconnected += OnDisconnected;
            _serverSocket.Connect();
            _heartbeatThread = new Thread(HeartbeatStart);
            _heartbeatThread.Start();
        }

        private void HeartbeatStart()
        {
            _heartbeatRunning = true;
            while (_heartbeatRunning)
            {
                Thread.Sleep(HeartbeatInterval);
                if (Heartbeat)
                {
                    _serverSocket?.SendHeartbeat();
                }
            }
        }

        public void Stop()
        {
            StopHeartbeat();
            _serverSocket.Close();
            _serverSocket.Connected -= OnConnected;
            _serverSocket.ReceivedData -= OnReceivedData;
            _serverSocket.Disconnected -= OnDisconnected;
        }

        public void SendData(byte[] data)
        {
            _serverSocket?.SendData(data);
        }

        public void SendData<T>(T data)
        {
            SendData(data.ToBytes());
        }

        private void StopHeartbeat()
        {
            _heartbeatRunning = false;
            _heartbeatThread.Abort();
        }

        protected virtual void OnConnected(TcpSocket server)
        {
            _serverSocket.StartReceiving();
            Connected?.Invoke(server);
        }

        protected virtual void OnDisconnected(TcpSocket server)
        {
            Stop();
            Disconnected?.Invoke(server);
        }

        protected virtual void OnReceivedData(TcpSocket server, byte[] data)
        {
            ReceivedData?.Invoke(server, data);
        }

        protected static void HandleData<TInterface, TData>(
            TcpSocket socket,
            TInterface message,
            Action<TData> handler) where TData : class
        {
            var convertedMessage = message as TData;
            if (convertedMessage != null)
            {
                handler(convertedMessage);
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
