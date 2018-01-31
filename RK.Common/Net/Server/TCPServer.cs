using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using RK.Common.Classes.Common;
using RK.Common.Proto;

namespace RK.Common.Net.Server
{
    public class TCPServer : IDisposable
    {

#region Delegates

        public delegate void OnClientConnection(TCPServer server, int clientId);
        public delegate void OnClientDataReceived(TCPServer server, int clientId, List<BasePacket> packets);
        public delegate void OnClientDataReceiveError(TCPServer server, int clientId, SocketError error);
        public delegate void OnClientDataSent(TCPServer server, int clientId);
        public delegate void OnClientDataSendError(TCPServer server, int clientId, SocketError error);

#endregion

#region Private fields

        private int _idCounter;

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

#region Public fields

        public bool CallClientConnectedAsync = false;
        public bool CallClientDisonnectedAsync = false;
        public bool CallClientDataReceivedAsync = false;
        public bool CallClientDataReceiveErrorAsync = false;
        public bool CallClientDataSentAsync = false;
        public bool CallClientDataSendErrorAsync = false;

#endregion

#region Properties

        public int NumberOfAcceptedSockets
        {
            get { return _numberOfAcceptedSockets; }
        }

#endregion

#region Ctor

        public TCPServer(TCPServerSettings settings)
        {
            _settings = settings;

            _clients = new ClientToken[_settings.MaxConnections];
            _bufferManager = new BufferManager(_settings.BufferSize, _settings.MaxConnections*2);
            _poolOfDataEventArgs = new Pool<ClientToken>(_settings.MaxConnections, false);
            _poolOfAcceptEventArgs = new Pool<SocketAsyncEventArgs>(_settings.MaxAcceptOps, false);
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
                receiveEvent.Completed += IOCompleted;

                SocketAsyncEventArgs sendEvent = new SocketAsyncEventArgs();
                _bufferManager.SetBuffer(sendEvent);
                sendEvent.Completed += IOCompleted;

                unchecked
                {
                    ClientToken clientToken = new ClientToken(_idCounter++, null, receiveEvent, sendEvent);
                    receiveEvent.UserToken = sendEvent.UserToken = clientToken;
                    _clients[clientToken.Id] = clientToken;
                    _poolOfDataEventArgs.Push(clientToken);
                }
            }

            StartListen();
        }

        private void IOCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;

                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;

                case SocketAsyncOperation.Disconnect:
                    ProcessDisconnect(e);
                    break;
            }
        }

        private void StartListen()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, _settings.Port);
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(endPoint);
            _listenSocket.Listen(_settings.Backlog);

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
            if (e.SocketError != SocketError.Success && e.AcceptSocket != null && e.AcceptSocket.Connected)
            {
                e.AcceptSocket.Close();
                _poolOfAcceptEventArgs.Push(e);
                StartAccept();
                return;
            }

            ClientToken clientToken = _poolOfDataEventArgs.Pop();
            clientToken.AcceptConnection(e, true);
            _poolOfAcceptEventArgs.Push(e);

            Interlocked.Increment(ref _numberOfAcceptedSockets);

            //Fire ClientConnected event
            if (ClientConnected != null)
            {
                if (CallClientConnectedAsync)
                {
                    ClientConnected.BeginInvoke(this, clientToken.Id, r => ClientConnected.EndInvoke(r), null);
                }
                else
                {
                    ClientConnected(this, clientToken.Id);
                }
            }

            StartReceive(clientToken);

            StartAccept();
        }

        private void StartReceive(ClientToken clientToken)
        {
#if DEBUG
            if (_disposed) return;
#endif
            if (!clientToken.Closed && !clientToken.Socket.ReceiveAsync(clientToken.ReceiveEvent))
            {
                ProcessReceive(clientToken.ReceiveEvent);
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            ClientToken clientToken = (ClientToken)e.UserToken;

            if (e.SocketError != SocketError.Success)
            {
                if (e.SocketError == SocketError.OperationAborted) return;

                //Fire ClientDataReceiveError event
                if (ClientDataReceiveError != null)
                {
                    if (CallClientDataReceiveErrorAsync)
                    {
                        ClientDataReceiveError.BeginInvoke(this, clientToken.Id, e.SocketError, 
                            r => ClientDataReceiveError.EndInvoke(r), null);
                    }
                    else
                    {
                        ClientDataReceiveError(this, clientToken.Id, e.SocketError);
                    }
                }

                if (!clientToken.Closed && !clientToken.Socket.DisconnectAsync(e))
                {
                    CloseClientSocket(clientToken);
                }

                return;
            }

            //Read client bytes transferred
            int bytesTransferred = e.BytesTransferred;
            if (bytesTransferred != 0)
            {
                clientToken.AcceptData(e, bytesTransferred);

                //Parse received data for packets
                List<BasePacket> packets = clientToken.ProcessReceivedDataReq();

                //Fire ClientDataReceived event
                if (packets.Count > 0 && ClientDataReceived != null)
                {
                    if (CallClientDataReceivedAsync)
                    {
                        ClientDataReceived.BeginInvoke(this, clientToken.Id, packets,
                            r => ClientDataReceived.EndInvoke(r), null);
                    }
                    else
                    {
                        ClientDataReceived(this, clientToken.Id, packets);
                    }
                }

                StartReceive(clientToken);

                //clientToken.AcceptDataAsync(e, bytesTransferred).ContinueWith
                //    (
                //        a =>
                //        {
                //            //Parse received data for packets
                //            List<BasePacket> packets = clientToken.ProcessReceivedDataReq();

                //            //Fire ClientDataReceived event
                //            if (packets.Count > 0 && ClientDataReceived != null)
                //            {
                //                ClientDataReceived.BeginInvoke(this, clientToken.Id, packets, 
                //                    r => ClientDataReceived.EndInvoke(r), null);
                //            }

                //            StartReceive(clientToken);
                //        }
                //    );

                return;
            }

            //Return of zero bytes transferred means that the client is no longer connected
            if (!clientToken.Closed && !clientToken.Socket.DisconnectAsync(e))
            {
                CloseClientSocket(clientToken);
            }
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
            clientToken.ActionSendSync.WaitOne();
            if (clientToken.Closed)
            {
                clientToken.ActionSendSync.Set();
                return;
            }

            //Prepare data to send
            clientToken.DataToSend = dataToSend;
            clientToken.SendBytesRemaining = dataToSend.Length;

            //Send data
            StartSend(clientToken);
        }

        public void SendAll(BaseResponse response)
        {
        }

        private void StartSend(ClientToken clientToken)
        {
#if DEBUG
            if (_disposed) return;
#endif

            if (clientToken.Socket == null || clientToken.Closed)
            {
                return;
            }
        
            if (clientToken.SendBytesRemaining <= _settings.BufferSize)
            {
                clientToken.SendEvent.SetBuffer(clientToken.BufferOffsetSend, clientToken.SendBytesRemaining);
                Buffer.BlockCopy(clientToken.DataToSend, clientToken.BytesSentAlready,
                    clientToken.SendEvent.Buffer, clientToken.BufferOffsetSend,
                    clientToken.SendBytesRemaining);
            }
            else
            {
                if (clientToken.SendEvent.Count != _settings.BufferSize)
                {
                    clientToken.SendEvent.SetBuffer(clientToken.BufferOffsetSend, _settings.BufferSize);
                }
                Buffer.BlockCopy(clientToken.DataToSend, clientToken.BytesSentAlready,
                    clientToken.SendEvent.Buffer, clientToken.BufferOffsetSend, _settings.BufferSize);
            }

            if (!clientToken.Closed && !clientToken.Socket.SendAsync(clientToken.SendEvent))
            {
                ProcessSend(clientToken.SendEvent);
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            ClientToken clientToken = (ClientToken) e.UserToken;

            if (e.SocketError != SocketError.Success)
            {
                //Fire DataSentError event
                if (ClientDataSendError != null)
                {
                    if (CallClientDataSendErrorAsync)
                    {
                        ClientDataSendError.BeginInvoke(this, clientToken.Id, e.SocketError,
                            r => ClientDataSendError.EndInvoke(r), null);
                    }
                    else
                    {
                        ClientDataSendError(this, clientToken.Id, e.SocketError);
                    }
                }

                if (!clientToken.Closed && !clientToken.Socket.DisconnectAsync(e))
                {
                    CloseClientSocket(clientToken);
                }

                return;
            }

            clientToken.SendBytesRemaining = clientToken.SendBytesRemaining - e.BytesTransferred;
            if (clientToken.SendBytesRemaining == 0)
            {
                //Fire DataSent event
                if (ClientDataSent != null)
                {
                    if (CallClientDataSentAsync)
                    {
                        ClientDataSent.BeginInvoke(this, clientToken.Id, r => ClientDataSent.EndInvoke(r), null);
                    }
                    else
                    {
                        ClientDataSent(this, clientToken.Id);
                    }
                }

                clientToken.ResetSend();
            }
            else
            {
                clientToken.BytesSentAlready += e.BytesTransferred;
                StartSend(clientToken);
            }
        }

        private void ProcessDisconnect(SocketAsyncEventArgs e)
        {
#if DEBUG
            if (_disposed) return;
#endif
            CloseClientSocket((ClientToken)e.UserToken);
        }

        private void CloseClientSocket(ClientToken clientToken)
        {
#if DEBUG
            if (_disposed) return;
#endif
            if (!clientToken.Closed)
            {
                clientToken.Close();

                _poolOfDataEventArgs.Push(clientToken);

                _maxConnectionsEnforcer.Release();

                Interlocked.Decrement(ref _numberOfAcceptedSockets);

                //Fire ClientDisonnected event
                if (ClientDisonnected != null)
                {
                    if (CallClientDisonnectedAsync)
                    {
                        ClientDisonnected.BeginInvoke(this, clientToken.Id, r => ClientDisonnected.EndInvoke(r), null);
                    }
                    else
                    {
                        ClientDisonnected(this, clientToken.Id);
                    }
                }
            }
        }

        public void DropClient(int clientId)
        {
            //Get a client
            ClientToken clientToken = _clients[clientId];

            //Drop the client
            if (!clientToken.Closed && !clientToken.Socket.DisconnectAsync(clientToken.ReceiveEvent))
            {
                CloseClientSocket(clientToken);
            }
        }

        public void Dispose()
        {
            _disposed = true;

            _listenSocket.Dispose();

            while (_poolOfAcceptEventArgs.Count > 0)
            {
                SocketAsyncEventArgs e = _poolOfAcceptEventArgs.Pop();
                if (e != null) e.Dispose();
            }

            while (_poolOfDataEventArgs.Count > 0)
            {
                ClientToken clientToken = _poolOfDataEventArgs.Pop();
                clientToken.ReceiveEvent.Dispose();
                clientToken.SendEvent.Dispose();
                clientToken.Dispose();
            }
        }

#endregion

    }
}