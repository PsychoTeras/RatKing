using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using RK.Common.Classes.Common;
using RK.Common.Net.TCP;
using RK.Common.Proto;

namespace RK.Common.Net.TCP2.Server
{
    internal class TCPServer2 : TCPBase
    {

#region Delegates

        public delegate void OnClientConnection(int clientId);
        public delegate void OnClientDataReceived(int clientId, List<BasePacket> packets);
        public delegate void OnClientDataReceiveError(int clientId, SocketError error);
        public delegate void OnClientDataSent(int clientId, object packet);
        public delegate void OnClientDataSendError(int clientId, object packet, SocketError error);

#endregion

#region Private fields

        private Socket _listenSocket;

        private TCPServerSettings _settings;
        private BufferManager _bufferManager;
        private Semaphore _maxConnectionsEnforcer;

        private ClientToken[] _clients;
        private Pool<SocketAsyncEventArgs> _poolOfAcceptEventArgs;
        private Pool<ClientToken> _poolOfDataEventArgs;

        private int _numberOfAcceptedSockets;

        private volatile bool _disposed;

#endregion

#region Events

        public event OnClientConnection ClientConnected;
        public event OnClientConnection ClientDisonnected;
        public event OnClientDataReceived ClientDataReceived;
        public event OnClientDataReceiveError ClientDataReceiveError;
        public event OnClientDataSent ClientDataSent;
        public event OnClientDataSendError ClientDataSendError;

#endregion

#region Properties

        public int NumberOfAcceptedSockets
        {
            get { return _numberOfAcceptedSockets; }
        }

#endregion

#region Ctor

        public TCPServer2(TCPServerSettings settings)
        {
            _settings = settings;

            _clients = new ClientToken[_settings.MaxConnections];
            _bufferManager = new BufferManager(_settings.BufferSize, _settings.MaxConnections*2);
            _poolOfDataEventArgs = new Pool<ClientToken>(_settings.MaxConnections);
            _poolOfAcceptEventArgs = new Pool<SocketAsyncEventArgs>(_settings.MaxAcceptOps);
            _maxConnectionsEnforcer = new Semaphore(_settings.MaxConnections, _settings.MaxConnections);
        }

#endregion

#region Class methods

        public void Start()
        {
            for (int i = 0; i < _settings.MaxAcceptOps; i++)
            {
                SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                e.Completed += ProcessAccept;
                _poolOfAcceptEventArgs.Push(e);
            }

            for (int i = 0; i < _settings.MaxConnections; i++)
            {
                SocketAsyncEventArgs receiveEvent = new SocketAsyncEventArgs();
                _bufferManager.SetBuffer(receiveEvent);
                receiveEvent.Completed += ProcessReceive;

                SocketAsyncEventArgs sendEvent = new SocketAsyncEventArgs();
                _bufferManager.SetBuffer(sendEvent);
                sendEvent.Completed += ProcessSend;

                ClientToken clientToken = new ClientToken(receiveEvent, sendEvent);
                receiveEvent.UserToken = sendEvent.UserToken = clientToken;
                _clients[clientToken.Id] = clientToken;

                _poolOfDataEventArgs.Push(clientToken);
            }

            StartListen();
        }

        private void StartListen()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, _settings.Port);
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(endPoint);
            _listenSocket.Listen(_settings.Backlog);
            _listenSocket.Blocking = false;

            StartAccept();
        }

        private void StartAccept()
        {
            _maxConnectionsEnforcer.WaitOne();

#if DEBUG
            if (_disposed) return;
#endif

            SocketAsyncEventArgs e = _poolOfAcceptEventArgs.Pop();
            if (!_listenSocket.AcceptAsync(e))
            {
                ProcessAccept(this, e);
            }
        }

        private void ProcessAccept(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success && e.AcceptSocket != null)
            {
                e.AcceptSocket.Close();
                _poolOfAcceptEventArgs.Push(e);
                StartAccept();
                return;
            }

            Interlocked.Increment(ref _numberOfAcceptedSockets);

            StartAccept();

#if DEBUG
            if (_disposed) return;
#endif

            ClientToken clientToken = _poolOfDataEventArgs.Pop();
            clientToken.Prepare(e);

            _poolOfAcceptEventArgs.Push(e);

            //Fire ClientConnected event
            if (ClientConnected != null)
            {
                ClientConnected(clientToken.Id);
            }

            StartReceive(clientToken);
        }

        private void StartReceive(ClientToken clientToken)
        {
#if DEBUG
            if (_disposed) return;
#endif

            if (!clientToken.ReceiveEvent.AcceptSocket.ReceiveAsync(clientToken.ReceiveEvent))
            {
                ProcessReceive(this, clientToken.ReceiveEvent);
            }
        }

        private void ProcessReceive(object sender, SocketAsyncEventArgs e)
        {
            ClientToken clientToken = (ClientToken) e.UserToken;

            if (e.SocketError != SocketError.Success)
            {
                //Fire ClientDataReceiveError event
                if (ClientDataReceiveError != null)
                {
                    ClientDataReceiveError(clientToken.Id, e.SocketError);
                }

                CloseClientSocket(e);
                return;
            }

            if (e.BytesTransferred != 0)
            {
                clientToken.AcceptData(e);

                if (clientToken.ReceivedDataLength != 0)
                {
                    //Parse received data for packets
                    List<BasePacket> packets = clientToken.ProcessReceivedDataReq();

                    //Fire ClientDataReceived event
                    if (packets.Count > 0 && ClientDataReceived != null)
                    {
                        ClientDataReceived(clientToken.Id, packets);
                    }
                }
            }

            StartReceive(clientToken);
        }

        public void Send(int clientId, ITransferable[] packets)
        {
            int dataSize = 0, cnt = packets.Length;
            if (cnt == 1)
            {
                Send(clientId, packets[0].Serialize());
                return;
            }

            //Serialize all packets to one
            byte[][] data = new byte[cnt][];
            for (int i = 0; i < cnt; i++)
            {
                byte[] byteBuffer = packets[i].Serialize();
                data[i] = byteBuffer;
                dataSize += byteBuffer.Length;
            }

            int idx = 0;
            byte[] dataToSend = new byte[dataSize];
            for (int i = 0; i < cnt; i++)
            {
                byte[] array = data[i];
                int length = array.Length;
                Buffer.BlockCopy(array, 0, dataToSend, idx, length);
                idx += length;
            }

            //Send data
            Send(clientId, dataToSend);
        }

        public void Send(int clientId, ITransferable packet)
        {
            //Serialize data
            byte[] dataToSend = packet.Serialize();

            //Send data
            Send(clientId, dataToSend);
        }

        public void Send(int clientId, byte[] dataToSend)
        {
            //Get a client
            ClientToken clientToken = _clients[clientId];
            clientToken.SendSync.WaitOne();
            if (clientToken.Closed)
            {
                clientToken.SendSync.Set();
                return;
            }

            //Prepare data to send
            clientToken.DataToSend = dataToSend;
            clientToken.SendBytesRemainingCount = dataToSend.Length;
            clientToken.ObjectToSend = dataToSend;

            //Send data
            StartSend(clientToken, true);
        }

        public void SendAll(BaseResponse response)
        {
        }

        private void StartSend(ClientToken clientToken, bool newSend)
        {
#if DEBUG
            if (_disposed) return;
#endif
            if (clientToken.SendBytesRemainingCount == 0) return;

            if (clientToken.SendBytesRemainingCount <= _settings.BufferSize)
            {
                clientToken.SendEvent.SetBuffer(clientToken.BufferOffsetSend, clientToken.SendBytesRemainingCount);
                Buffer.BlockCopy(clientToken.DataToSend, clientToken.BytesSentAlreadyCount,
                    clientToken.SendEvent.Buffer, clientToken.BufferOffsetSend,
                    clientToken.SendBytesRemainingCount);
            }
            else
            {
                clientToken.SendEvent.SetBuffer(clientToken.BufferOffsetSend, _settings.BufferSize);
                Buffer.BlockCopy(clientToken.DataToSend, clientToken.BytesSentAlreadyCount,
                    clientToken.SendEvent.Buffer, clientToken.BufferOffsetSend, _settings.BufferSize);
            }

            if (!clientToken.SendEvent.AcceptSocket.SendAsync(clientToken.SendEvent))
            {
                ProcessSend(this, clientToken.SendEvent);
            }
        }

        private void ProcessSend(object sender, SocketAsyncEventArgs e)
        {
            ClientToken clientToken = (ClientToken) e.UserToken;
            if (e.SocketError == SocketError.Success)
            {
                clientToken.SendBytesRemainingCount = clientToken.SendBytesRemainingCount - e.BytesTransferred;
                if (clientToken.SendBytesRemainingCount == 0)
                {
                    clientToken.ResetSend();

                    //Fire DataSent event
                    if (ClientDataSent != null)
                    {
                        ClientDataSent(clientToken.Id, clientToken.ObjectToSend);
                    }
                }
                else
                {
                    clientToken.BytesSentAlreadyCount += e.BytesTransferred;
                    StartSend(clientToken, false);
                }
            }
            else
            {
                //Fire DataSentError event
                if (ClientDataSendError != null)
                {
                    ClientDataSendError(clientToken.Id, clientToken.ObjectToSend, e.SocketError);
                }
                CloseClientSocket(e);
            }
        }

        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
#if DEBUG
            if (_disposed) return;
#endif

            try
            {
                e.AcceptSocket.Shutdown(SocketShutdown.Both);
            }
            catch { }
            e.AcceptSocket.Close();

            Interlocked.Decrement(ref _numberOfAcceptedSockets);

            ClientToken clientToken = (ClientToken) e.UserToken;

            //Fire ClientDisonnected event
            if (ClientDisonnected != null)
            {
                ClientDisonnected(clientToken.Id);
            }

            clientToken.ResetForClose();
            _poolOfDataEventArgs.Push(clientToken);

            _maxConnectionsEnforcer.Release();
        }

        public override void Dispose()
        {
            _disposed = true;
            _listenSocket.Dispose();
            while (_poolOfAcceptEventArgs.Count > 0)
            {
                SocketAsyncEventArgs e = _poolOfAcceptEventArgs.Pop();
                e.Dispose();
            }
            while (_poolOfDataEventArgs.Count > 0)
            {
                ClientToken clientToken = _poolOfDataEventArgs.Pop();
                _bufferManager.FreeBuffer(clientToken.ReceiveEvent);
                clientToken.ReceiveEvent.Dispose();
                _bufferManager.FreeBuffer(clientToken.SendEvent);
                clientToken.SendEvent.Dispose();
            }
        }

#endregion

    }
}