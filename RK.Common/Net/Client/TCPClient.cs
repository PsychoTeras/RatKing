using System;
using System.Collections.Generic;
using System.Net.Sockets;
using RK.Common.Classes.Common;
using RK.Common.Proto;

namespace RK.Common.Net.Client
{
    public sealed class TCPClient : IDisposable
    {

#region Delegates

        public delegate void OnConnectionError(SocketError error);
        public delegate void OnDataReceived(IList<BaseResponse> packets);
        public delegate void OnDataSent(ITransferable packet);

#endregion
        
#region Events

        public event OnDataReceived DataReceived;
        public event Action DataReceiveError;
        public event OnDataSent DataSent;
        public event OnDataSent DataSendError;

        public event Action Connected;
        public event OnConnectionError ConnectionError;
        public event Action Disconnected;

#endregion

#region Private fields

        private BufferManager _bufferManager;
        private TCPClientSettings _settings;

        private Socket _socket;
        private ClientToken _clientToken;
        private SocketAsyncEventArgs _connectEvent;
        private SocketAsyncEventArgs _receiveEvent;
        private SocketAsyncEventArgs _sendEvent;

        private object _syncActions = new object();

#endregion

#region Properties

        public bool IsConnected { get; private set; }

#endregion

#region Ctor

        public TCPClient(TCPClientSettings settings)
        {
            _settings = settings;
            _bufferManager = new BufferManager(_settings.BufferSize, 2);

            _receiveEvent = new SocketAsyncEventArgs();
            _bufferManager.SetBuffer(_receiveEvent);
            _receiveEvent.Completed += IOCompleted;
            _receiveEvent.UserToken = "R";

            _sendEvent = new SocketAsyncEventArgs();
            _bufferManager.SetBuffer(_sendEvent);
            _sendEvent.Completed += IOCompleted;
            _sendEvent.UserToken = "S";

            _clientToken = new ClientToken(_receiveEvent, _sendEvent);
        }

#endregion

#region Class methods

        private void IOCompleted(object sender, SocketAsyncEventArgs e)
        {
            lock (_syncActions)
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    ProcessConnect(e);
                    break;

                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;

                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;

                case SocketAsyncOperation.Disconnect:
                    ProcessDisconnect(e);
                    break;

                default:
                    throw new ArgumentException("Error in I/O Completed");
            }
        }

        public void Connect()
        {
            lock (_syncActions)
            {
                if (IsConnected) return;

                _connectEvent = new SocketAsyncEventArgs();
                _connectEvent.Completed += IOCompleted;
                _connectEvent.RemoteEndPoint = _settings.EndPoint;
                _socket = _connectEvent.AcceptSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                if (!_connectEvent.AcceptSocket.ConnectAsync(_connectEvent))
                {
                    ProcessConnect(_connectEvent);
                }
            }
        }

        private void ProcessConnect(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                IsConnected = true;

                StartReceive();

                //Fire Connected event
                if (Connected != null)
                {
                    Connected.BeginInvoke(null, null);
                }
            }
            else
            {
                ProcessConnectionError(e);
            }
        }

        private void ProcessConnectionError(SocketAsyncEventArgs e)
        {
            //Fire ConnectionError event
            if (ConnectionError != null)
            {
                ConnectionError.BeginInvoke(e.SocketError, null, null);
            }

            StartDisconnect(e);
        }

        private void StartReceive()
        {
            lock (_syncActions)
            {
                if (IsConnected && !_socket.ReceiveAsync(_receiveEvent))
                {
                    ProcessReceive(_receiveEvent);
                }
            }
        }
        
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            lock (_syncActions)
            {
                if (!IsConnected) return;

                if (e.SocketError != SocketError.Success)
                {
                    //Fire ClientDataReceiveError event
                    if (DataReceiveError != null)
                    {
                        DataReceiveError.BeginInvoke(null, null);
                    }

                    StartDisconnect(e);
                    return;
                }

                int bytesTransferred = e.BytesTransferred;
                if (bytesTransferred != 0)
                {
                    _clientToken.AcceptData(e, bytesTransferred);

                    if (_clientToken.ReceivedDataLength != 0)
                    {
                        //Parse received data for packets
                        List<BaseResponse> packets = _clientToken.ProcessReceivedDataRsp();

                        //Fire ClientDataReceived event
                        if (packets.Count > 0 && DataReceived != null)
                        {
                            DataReceived.BeginInvoke(packets, null, null);
                        }
                    }
                    StartReceive();
                }
            }
        }

        public void Send(ITransferable packet)
        {
            //Serialize data
            byte[] dataToSend = packet.Serialize();

            //Send data
            Send(dataToSend);
        }

        public void Send(byte[] dataToSend)
        {
            //Get a client
            _clientToken.SendSync.WaitOne();

            //Prepare data to send
            _clientToken.DataToSend = dataToSend;
            _clientToken.SendBytesRemaining = dataToSend.Length;
            _clientToken.ObjectToSend = dataToSend;

            //Send data
            StartSend();
        }

        private void StartSend()
        {
            if (_clientToken.SendBytesRemaining == 0 || _clientToken.DataToSend == null) return;
            lock (_syncActions)
            {
                if (_clientToken.SendBytesRemaining <= _settings.BufferSize)
                {
                    _sendEvent.SetBuffer(_clientToken.BufferOffsetSend, _clientToken.SendBytesRemaining);
                    Buffer.BlockCopy(_clientToken.DataToSend, _clientToken.BytesSentAlready,
                        _sendEvent.Buffer, _clientToken.BufferOffsetSend,
                        _clientToken.SendBytesRemaining);
                }
                else
                {
                    _sendEvent.SetBuffer(_clientToken.BufferOffsetSend, _settings.BufferSize);
                    Buffer.BlockCopy(_clientToken.DataToSend, _clientToken.BytesSentAlready,
                        _sendEvent.Buffer, _clientToken.BufferOffsetSend, _settings.BufferSize);
                }

                if (IsConnected && !_socket.SendAsync(_sendEvent))
                {
                    ProcessSend(_sendEvent);
                }
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            lock (_syncActions)
            {
                if (IsConnected && e.SocketError == SocketError.Success)
                {
                    _clientToken.SendBytesRemaining = _clientToken.SendBytesRemaining - e.BytesTransferred;
                    if (_clientToken.SendBytesRemaining == 0)
                    {
                        _clientToken.ResetSend();

                        //Fire DataSent event
                        if (DataSent != null)
                        {
                            ITransferable obj = (ITransferable) _clientToken.ObjectToSend;
                            DataSent.BeginInvoke(obj, null, null);
                        }
                    }
                    else
                    {
                        _clientToken.BytesSentAlready += e.BytesTransferred;
                        StartSend();
                    }
                }
                else
                {
                    //Fire DataSentError event
                    if (DataSendError != null)
                    {
                        ITransferable obj = (ITransferable) _clientToken.ObjectToSend;
                        DataSendError.BeginInvoke(obj, null, null);
                    }

                    StartDisconnect(e);
                }
            }
        }

        public void Disconnect()
        {
            ProcessDisconnect(_connectEvent);
        }

        private void StartDisconnect(SocketAsyncEventArgs e)
        {
            if (IsConnected)
            {
                ProcessDisconnect(e);
            }
        }

        private void ProcessDisconnect(SocketAsyncEventArgs e)
        {
            lock (_syncActions)
            {
                if (!IsConnected) return;
                IsConnected = false;

                _socket.Close();
                DisposeObject(ref _socket);
                _clientToken.ResetForClose();

                DisposeObject(ref _connectEvent);

                //Fire Disconnected event
                if (Disconnected != null)
                {
                    Disconnected.BeginInvoke(null, null);
                }
            }
        }

        private void DisposeObject<T>(ref T o) 
            where T: class, IDisposable
        {
            if (o != null)
            {
                o.Dispose();
                o = null;
            }
        }

        public void Dispose()
        {
            Disconnect();
            _socket = null;
            DisposeObject(ref _connectEvent);
            DisposeObject(ref _receiveEvent);
            DisposeObject(ref _sendEvent);
            DisposeObject(ref _clientToken);
        }

#endregion

    }
}