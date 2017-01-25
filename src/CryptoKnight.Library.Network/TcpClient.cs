using System;
using System.Net;
using System.Timers;
using Timer = System.Timers.Timer;

namespace CryptoKnight.Library.Network
{
    public class TcpClient : IDisposable
    {
        private const int DefaultHeartbeatInterval = 5000;

        public int HeartbeatInterval { get; set; }

        public IPEndPoint EndPoint;

        private TcpSocket _serverSocket;

        private readonly Timer _heartbeatTimer;

        public delegate void ConnectedHandler(TcpSocket server);

        public event ConnectedHandler Connected;

        public delegate void ReceivedDataHandler(TcpSocket server, byte[] data);

        public event ReceivedDataHandler ReceivedData;

        public delegate void DisconnectedHandler(TcpSocket server);

        public event DisconnectedHandler Disconnected;

        public TcpClient(IPEndPoint endPoint, bool heartbeat = true, int heartbeatInterval = DefaultHeartbeatInterval)
        {
            EndPoint = endPoint;
            HeartbeatInterval = heartbeatInterval;
            if (!heartbeat) return;
            _heartbeatTimer = new Timer(HeartbeatInterval)
            {
                AutoReset = true
            };
            _heartbeatTimer.Elapsed += HeartbeatTimerOnElapsed;
        }

        private void HeartbeatTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _serverSocket?.SendHeartbeat();
        }

        public void Start()
        {
            _serverSocket = new TcpSocket(EndPoint);
            _serverSocket.Connected += OnConnected;
            _serverSocket.ReceivedData += OnReceivedData;
            _serverSocket.Disconnected += OnDisconnected;
            _serverSocket.Connect();
        }

        public void Stop()
        {
            if (_heartbeatTimer != null && _heartbeatTimer.Enabled)
            {
                _heartbeatTimer.Stop();
            }
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

        private void OnConnected(TcpSocket server)
        {
            _heartbeatTimer?.Start();
            _serverSocket.StartReceiving();
            Connected?.Invoke(server);
        }

        private void OnDisconnected(TcpSocket server)
        {
            Stop();
            Disconnected?.Invoke(server);
        }

        private void OnReceivedData(TcpSocket server, byte[] data)
        {
            if (data.Length != 0)
                ReceivedData?.Invoke(server, data);
        }

        protected static void HandleData<TData>(
            TcpSocket socket,
            object data,
            Action<TData> handler) where TData : class
        {
            var convertedData = data as TData;
            if (convertedData != null)
            {
                handler(convertedData);
            }
            else
            {
                socket.Close();
            }
        }

        public void Dispose()
        {
            _heartbeatTimer.Elapsed -= HeartbeatTimerOnElapsed;
            Stop();
        }
    }
}
