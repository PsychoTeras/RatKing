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

        public delegate void OnConnected(TCPClient client);
        public delegate void OnConnectionError(TCPClient client, SocketError error);
        public delegate void OnDisconnected(TCPClient client);
        public delegate void OnDataReceived(TCPClient client, IList<BaseResponse> packets);
        public delegate void OnDataReceiveError(TCPClient client);
        public delegate void OnDataSent(TCPClient client);
        public delegate void OnDataSendError(TCPClient client);

#endregion
        
#region Events

        public event OnDataReceived DataReceived;
        public event OnDataReceiveError DataReceiveError;
        public event OnDataSent DataSent;
        public event OnDataSendError DataSendError;
        public event OnConnected Connected;
        public event OnConnectionError ConnectionError;
        public event OnDisconnected Disconnected;

#endregion

#region Private fields

        private TCPClientSettings _settings;

        private ClientToken _clientToken;

        private SocketAsyncEventArgs _connectEvent;
        private SocketAsyncEventArgs _receiveEvent;
        private SocketAsyncEventArgs _sendEvent;

        private volatile bool _isConnected;

#endregion

#region Public fields

        public bool CallDataReceivedAsync = true;
        public bool CallDataReceiveErrorAsync = true;
        public bool CallDataSentAsync = true;
        public bool CallDataSendErrorAsync = true;
        public bool CallConnectedAsync = true;
        public bool CallConnectionErrorAsync = true;
        public bool CallDisconnectedAsync = true;

#endregion

#region Properties

        public bool IsConnected
        {
            get { return _isConnected; }
        }

#endregion

#region Ctor

        public TCPClient(TCPClientSettings settings)
        {
            _settings = settings;

            BufferManager bufferManager = new BufferManager(_settings.BufferSize, 2);

            _connectEvent = new SocketAsyncEventArgs();
            _connectEvent.Completed += IOCompleted;
            _connectEvent.RemoteEndPoint = _settings.EndPoint;

            _receiveEvent = new SocketAsyncEventArgs();
            _receiveEvent.Completed += IOCompleted;
            bufferManager.SetBuffer(_receiveEvent);

            _sendEvent = new SocketAsyncEventArgs();
            _sendEvent.Completed += IOCompleted;
            bufferManager.SetBuffer(_sendEvent);
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
            if (_isConnected) return;

            _connectEvent.AcceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            if (!_connectEvent.AcceptSocket.ConnectAsync(_connectEvent))
            {
                ProcessConnect(_connectEvent);
            }
        }

        private void ProcessConnect(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                _isConnected = true;

                _clientToken = new ClientToken(0, e.AcceptSocket, _receiveEvent, _sendEvent);
                _receiveEvent.UserToken = _sendEvent.UserToken = e.UserToken = _clientToken;

                StartReceive(_receiveEvent);

                //Fire Connected event
                if (Connected != null)
                {
                    if (CallConnectedAsync)
                    {
                        Connected.BeginInvoke(this, r => Connected.EndInvoke(r), null);
                    }
                    else
                    {
                        Connected(this);
                    }
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
                if (CallConnectionErrorAsync)
                {
                    ConnectionError.BeginInvoke(this, e.SocketError, r => ConnectionError.EndInvoke(r), null);
                }
                else
                {
                    ConnectionError(this, e.SocketError);
                }
            }

            ProcessDisconnect(e);
        }

        private void StartReceive(SocketAsyncEventArgs e)
        {
            ClientToken clientToken = (ClientToken)e.UserToken;
            if (!clientToken.Closed && !clientToken.Socket.ReceiveAsync(e))
            {
                ProcessReceive(e);
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                if (e.SocketError == SocketError.OperationAborted) return;

                //Fire ClientDataReceiveError event
                if (DataReceiveError != null)
                {
                    if (CallDataReceiveErrorAsync)
                    {
                        DataReceiveError.BeginInvoke(this, r => DataReceiveError.EndInvoke(r), null);
                    }
                    else
                    {
                        DataReceiveError(this);
                    }
                }

                ProcessDisconnect(e);
                return;
            }

            ClientToken clientToken = (ClientToken) e.UserToken;
            int bytesTransferred = e.BytesTransferred;
            if (bytesTransferred != 0)
            {
                clientToken.AcceptData(e, bytesTransferred);

                //Parse received data for packets
                List<BaseResponse> packets = clientToken.ProcessReceivedDataRsp();

                //Fire ClientDataReceived event
                if (packets.Count > 0 && DataReceived != null)
                {
                    if (CallDataReceivedAsync)
                    {
                        DataReceived.BeginInvoke(this, packets, r => DataReceived.EndInvoke(r), null);
                    }
                    else
                    {
                        DataReceived(this, packets);
                    }
                }

                StartReceive(e);

                //clientToken.AcceptDataAsync(e, bytesTransferred).ContinueWith
                //    (
                //        a =>
                //        {
                //            //Parse received data for packets
                //            List<BaseResponse> packets = clientToken.ProcessReceivedDataRsp();

                //            //Fire ClientDataReceived event
                //            if (packets.Count > 0 && DataReceived != null)
                //            {
                //                DataReceived.BeginInvoke(this, packets, r => DataReceived.EndInvoke(r), null);
                //            }

                //            StartReceive(e);
                //        }
                //    );

                return;
            }

            //Return of zero bytes transferred means that the server is no longer connected
            if (!clientToken.Closed && clientToken.Socket != null)
            {
                ProcessDisconnect(e);
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
            //Wait while sending in progress
            _clientToken.ActionSendSync.WaitOne();
            if (_clientToken.Closed)
            {
                _clientToken.ActionSendSync.Set();
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
            if (_clientToken.Closed) return;

            if (_clientToken.SendBytesRemaining <= _settings.BufferSize)
            {
                _clientToken.SendEvent.SetBuffer(_clientToken.BufferOffsetSend, _clientToken.SendBytesRemaining);
                Buffer.BlockCopy(_clientToken.DataToSend, _clientToken.BytesSentAlready, _clientToken.SendEvent.Buffer, 
                    _clientToken.BufferOffsetSend, _clientToken.SendBytesRemaining);
            }
            else
            {
                _clientToken.SendEvent.SetBuffer(_clientToken.BufferOffsetSend, _settings.BufferSize);
                Buffer.BlockCopy(_clientToken.DataToSend, _clientToken.BytesSentAlready, _clientToken.SendEvent.Buffer,
                    _clientToken.BufferOffsetSend, _settings.BufferSize);
            }

            if (!_clientToken.Closed && !_clientToken.Socket.SendAsync(_clientToken.SendEvent))
            {
                ProcessSend(_clientToken.SendEvent);
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            ClientToken clientToken = (ClientToken) e.UserToken;
            if (e.SocketError != SocketError.Success)
            {
                if (e.SocketError == SocketError.OperationAborted) return;

                //Fire DataSentError event
                if (DataSendError != null)
                {
                    if (CallDataSendErrorAsync)
                    {
                        DataSendError.BeginInvoke(this, r => DataSendError.EndInvoke(r), null);
                    }
                    else
                    {
                        DataSendError(this);
                    }
                }

                clientToken.ResetSend();
                ProcessDisconnect(e);
            }

            clientToken.SendBytesRemaining = clientToken.SendBytesRemaining - e.BytesTransferred;
            if (clientToken.SendBytesRemaining == 0)
            {
                //Fire DataSent event
                if (DataSent != null)
                {
                    if (CallDataSentAsync)
                    {
                        DataSent.BeginInvoke(this, r => DataSent.EndInvoke(r), null);
                    }
                    else
                    {
                        DataSent(this);
                    }
                }

                clientToken.ResetSend();
            }
            else
            {
                clientToken.BytesSentAlready += e.BytesTransferred;
                StartSend();
            }
        }

        public void Disconnect()
        {
            ProcessDisconnect(null);
        }

        private void ProcessDisconnect(SocketAsyncEventArgs e)
        {
            if (_isConnected)
            {
                _isConnected = false;

                ClientToken clientToken = (ClientToken) (e != null
                    ? e.UserToken
                    : _clientToken);
                clientToken.Close();

                //Fire Disconnected event
                if (Disconnected != null)
                {
                    if (CallDisconnectedAsync)
                    {
                        Disconnected.BeginInvoke(this, r => Disconnected.EndInvoke(r), null);
                    }
                    else
                    {
                        Disconnected(this);
                    }
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
            DisposeObject(ref _clientToken);
        }

#endregion

    }
}