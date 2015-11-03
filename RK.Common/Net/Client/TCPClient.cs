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
        public delegate void OnDataSent();

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
            if (IsConnected) return;

            _connectEvent = new SocketAsyncEventArgs();
            _connectEvent.Completed += IOCompleted;
            _connectEvent.RemoteEndPoint = _settings.EndPoint;
            _socket = _connectEvent.AcceptSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            _socket.LingerState = new LingerOption(true, 1000);

            if (!_connectEvent.AcceptSocket.ConnectAsync(_connectEvent))
            {
                ProcessConnect(_connectEvent);
            }
        }

        private void ProcessConnect(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                _clientToken.Closed = false;
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
            if (!IsConnected) return;
            _clientToken.ReceiveSync.WaitOne();
            _clientToken.ReceiveSync.Set();

            if (IsConnected && !_socket.ReceiveAsync(_receiveEvent))
            {
                ProcessReceive(_receiveEvent);
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            _clientToken.ReceiveSync.WaitOne();
            if (_clientToken.Closed)
            {
                _clientToken.ReceiveSync.Set();
                return;
            }

            if (e.SocketError != SocketError.Success)
            {
                //Fire ClientDataReceiveError event
                if (DataReceiveError != null)
                {
                    DataReceiveError.BeginInvoke(null, null);
                }

                _clientToken.ResetReceive();
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

                _clientToken.ReceiveSync.Set();
                StartReceive();
                return;
            }

            _clientToken.ReceiveSync.Set();
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
            //Wait while sending in progress
            _clientToken.SendSync.WaitOne();
            if (_clientToken.Closed)
            {
                _clientToken.SendSync.Set();
                return;
            }

            //Prepare data to send
            _clientToken.DataToSend = dataToSend;
            _clientToken.SendBytesRemaining = dataToSend.Length;

            //Send data
            StartSend();
        }

        private void StartSend()
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

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (IsConnected && e.SocketError == SocketError.Success)
            {
                _clientToken.SendBytesRemaining = _clientToken.SendBytesRemaining - e.BytesTransferred;
                if (_clientToken.SendBytesRemaining == 0)
                {
                    //Fire DataSent event
                    if (DataSent != null)
                    {
                        DataSent.BeginInvoke(null, null);
                    }

                    _clientToken.ResetSend();
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
                    DataSendError.BeginInvoke(null, null);
                }

                _clientToken.ResetSend();
                StartDisconnect(e);
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
            if (IsConnected)
            {
                IsConnected = false;

                _clientToken.PrepareForClose();

                _socket.Close();
                DisposeObject(ref _socket);

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