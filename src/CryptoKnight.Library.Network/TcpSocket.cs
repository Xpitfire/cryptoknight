using System;
using System.Net;
using System.Net.Sockets;

namespace CryptoKnight.Library.Network
{
    public class TcpSocket : IDisposable
    {
        public Guid Id = Guid.NewGuid();
        public Socket Socket { get; set; }
        public byte[] SocketBuffer { get; set; }
        public int BufferSize { get; set; }
        public IPEndPoint EndPoint { get; set; }

        public bool IsConnected {
            get {
                try
                {
                    return !(Socket.Poll(1, SelectMode.SelectRead) && Socket.Available == 0);
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private readonly byte[] _lengthBuffer = new byte[sizeof(int)];
        private byte[] _dataBuffer;
        private int _bytesReceived;


        public delegate void AcceptedConnectionHandler(TcpSocket socket);

        public event AcceptedConnectionHandler AcceptedConnection;

        public delegate void ReceivedDataHandler(TcpSocket socket, byte[] data);

        public event ReceivedDataHandler ReceivedData;

        public delegate void SentDataHandler(TcpSocket socket, byte[] data);

        public event SentDataHandler SentData;

        public delegate void ConnectedHandler(TcpSocket socket);

        public event ConnectedHandler Connected;

        public delegate void DisconnectedHandler(TcpSocket socket);

        public event DisconnectedHandler Disconnected;


        public TcpSocket(IPEndPoint endPoint, int bufferSize = 1024)
        {
            EndPoint = endPoint;
            BufferSize = bufferSize;
            SocketBuffer = new byte[BufferSize];
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect()
        {
            Socket.BeginConnect(EndPoint, ProcessConnect, null);
        }

        public void StartListening()
        {
            Socket.Bind(EndPoint);
            Socket.Listen(100);
            Socket.BeginAccept(ProcessAccept, null);
        }

        public void StartReceiving()
        {
            if (!Socket.Connected)
                return;
            Socket.BeginReceive(SocketBuffer, 0, BufferSize, SocketFlags.None, ProcessReceive, this);
        }

        public void SendData(byte[] data)
        {
            if (!Socket.Connected)
                return;
            var dataToSend = AddLengthPrefix(data);
            SendBytes(dataToSend);
        }

        public void SendHeartbeat()
        {
            var heartbeat = BitConverter.GetBytes((int)0);
            SendBytes(heartbeat);
        }

        public void Close()
        {
            if (Socket.Connected)
            {
                Socket.Shutdown(SocketShutdown.Both);
            }
            Socket.Close();
            Socket.Dispose();
        }

        private void SendBytes(byte[] bytes)
        {
            Socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, ProcessSend, bytes);
        }

        private void ProcessConnect(IAsyncResult ar)
        {
            try
            {
                Socket.EndConnect(ar);
                OnConnected(this);
            }
            catch (Exception)
            {
                // Debug.WriteLine($"ProcessConnect: {e}");
                Close();
                OnDisconnected(this);
            }
        }

        private void ProcessAccept(IAsyncResult ar)
        {
            try
            {
                var connection = new TcpSocket(null)
                {
                    Socket = Socket.EndAccept(ar)
                };
                OnAcceptedConnection(connection);
                Socket.BeginAccept(ProcessAccept, null);
            }
            catch (Exception)
            {
                // Debug.WriteLine($"ProcessAccept: {e}");
                Close();
                OnDisconnected(this);
            }
        }

        private void ProcessReceive(IAsyncResult ar)
        {
            try
            {
                var readBytes = Socket.EndReceive(ar);
                var receivedData = new byte[readBytes];
                Array.Copy(SocketBuffer, receivedData, readBytes);
                ProcessIncomingData(receivedData);
                Socket.BeginReceive(SocketBuffer, 0, BufferSize, SocketFlags.None, ProcessReceive, this);
            }
            catch (Exception)
            {
                // Debug.WriteLine($"ProcessReceive: {e}");
                Close();
                OnDisconnected(this);
            }
        }

        private void ProcessIncomingData(byte[] data)
        {
            var i = 0;
            while (i != data.Length)
            {
                var bytesAvailable = data.Length - i;
                var bytesRequested = (_dataBuffer?.Length ?? _lengthBuffer.Length) - _bytesReceived;
                var bytesTransferred = Math.Min(bytesRequested, bytesAvailable);
                Array.Copy(data, i, _dataBuffer ?? _lengthBuffer, _bytesReceived, bytesTransferred);
                i += bytesTransferred;
                _bytesReceived += bytesTransferred;
                if (_dataBuffer == null)
                {
                    if (_bytesReceived != sizeof(int))
                        return;
                    var length = BitConverter.ToInt32(_lengthBuffer, 0);
                    if (length < 0)
                        throw new ProtocolViolationException("Invalid message length (less than zero)");
                    if (length == 0)
                    {
                        _bytesReceived = 0;
                        OnReceivedData(this, new byte[0]);
                    }
                    else
                    {
                        _dataBuffer = new byte[length];
                        _bytesReceived = 0;
                    }
                }
                else
                {
                    if (_bytesReceived != _dataBuffer.Length)
                        return;
                    OnReceivedData(this, _dataBuffer);
                    _dataBuffer = null;
                    _bytesReceived = 0;
                }
            }
        }

        private void ProcessSend(IAsyncResult ar)
        {
            try
            {
                var sentData = (byte[])ar.AsyncState;
                Socket.EndSend(ar);
                OnSentData(this, sentData);
            }
            catch (Exception)
            {
                // Debug.WriteLine($"ProcessSend: {e}");
                Close();
                OnDisconnected(this);
            }
        }

        private static byte[] AddLengthPrefix(byte[] message)
        {
            byte[] lengthPrefix = BitConverter.GetBytes(message.Length);
            byte[] ret = new byte[lengthPrefix.Length + message.Length];
            lengthPrefix.CopyTo(ret, 0);
            message.CopyTo(ret, lengthPrefix.Length);
            return ret;
        }

        protected virtual void OnAcceptedConnection(TcpSocket socket)
        {
            AcceptedConnection?.Invoke(socket);
        }

        protected virtual void OnReceivedData(TcpSocket socket, byte[] data)
        {
            ReceivedData?.Invoke(socket, data);
        }

        protected virtual void OnSentData(TcpSocket socket, byte[] data)
        {
            SentData?.Invoke(socket, data);
        }

        protected virtual void OnConnected(TcpSocket socket)
        {
            Connected?.Invoke(socket);
        }

        protected virtual void OnDisconnected(TcpSocket socket)
        {
            Disconnected?.Invoke(socket);
        }

        public void Dispose()
        {
            Close();
        }
    }
}
