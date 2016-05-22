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

        private ClientToken _clientToken;

#endregion

#region Properties

        public bool IsConnected { get; private set; }

#endregion

#region Ctor

        public TCPClient(TCPClientSettings settings)
        {
            _settings = settings;
            _bufferManager = new BufferManager(_settings.BufferSize, 2);
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

            SocketAsyncEventArgs connectEvent = new SocketAsyncEventArgs();
            connectEvent.Completed += IOCompleted;
            connectEvent.RemoteEndPoint = _settings.EndPoint;
            connectEvent.AcceptSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            connectEvent.AcceptSocket.LingerState = new LingerOption(true, 1000);

            if (!connectEvent.AcceptSocket.ConnectAsync(connectEvent))
            {
                ProcessConnect(connectEvent);
            }
        }

        private void ProcessConnect(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                IsConnected = true;

                _bufferManager.Reset();

                SocketAsyncEventArgs receiveEvent = new SocketAsyncEventArgs();
                _bufferManager.SetBuffer(receiveEvent);
                receiveEvent.Completed += IOCompleted;

                SocketAsyncEventArgs sendEvent = new SocketAsyncEventArgs();
                _bufferManager.SetBuffer(sendEvent);
                sendEvent.Completed += IOCompleted;

                _clientToken = new ClientToken(e.AcceptSocket, receiveEvent, sendEvent);
                receiveEvent.UserToken = sendEvent.UserToken = e.UserToken = _clientToken;

                StartReceive(receiveEvent);

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

            ProcessDisconnect(e);
        }

        private void StartReceive(SocketAsyncEventArgs e)
        {
            ClientToken clientToken = (ClientToken)e.UserToken;
            if (clientToken.Socket.Connected && !clientToken.Socket.ReceiveAsync(e))
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
                    DataReceiveError.BeginInvoke(null, null);
                }

                ProcessDisconnect(e);
                return;
            }

            ClientToken clientToken = (ClientToken) e.UserToken;
            int bytesTransferred = e.BytesTransferred;
            if (bytesTransferred != 0)
            {
                clientToken.AcceptData(e, bytesTransferred);

                if (clientToken.ReceivedDataLength != 0)
                {
                    //Parse received data for packets
                    List<BaseResponse> packets = clientToken.ProcessReceivedDataRsp();

                    //Fire ClientDataReceived event
                    if (packets.Count > 0 && DataReceived != null)
                    {
                        DataReceived.BeginInvoke(packets, null, null);
                    }
                }

                StartReceive(e);
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
                _clientToken.SendEvent.SetBuffer(_clientToken.BufferOffsetSend, _clientToken.SendBytesRemaining);
                Buffer.BlockCopy(_clientToken.DataToSend, _clientToken.BytesSentAlready,
                    _clientToken.SendEvent.Buffer, _clientToken.BufferOffsetSend,
                    _clientToken.SendBytesRemaining);
            }
            else
            {
                _clientToken.SendEvent.SetBuffer(_clientToken.BufferOffsetSend, _settings.BufferSize);
                Buffer.BlockCopy(_clientToken.DataToSend, _clientToken.BytesSentAlready,
                    _clientToken.SendEvent.Buffer, _clientToken.BufferOffsetSend, _settings.BufferSize);
            }

            if (_clientToken.Socket.Connected && !_clientToken.Socket.SendAsync(_clientToken.SendEvent))
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
                    DataSendError.BeginInvoke(null, null);
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
                    DataSent.BeginInvoke(null, null);
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
            if (IsConnected)
            {
                IsConnected = false;

                ClientToken clientToken = (ClientToken) (e != null
                    ? e.UserToken
                    : _clientToken);
                clientToken.Close();

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
            DisposeObject(ref _clientToken);
        }

#endregion

    }
}