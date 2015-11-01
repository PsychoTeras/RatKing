using System;
using System.Collections.Generic;
using System.Net.Sockets;
using RK.Common.Classes.Common;
using RK.Common.Net.TCP;
using RK.Common.Proto;

namespace RK.Common.Net.TCP2.Client
{
    public sealed class TCPClient2 : TCPBase
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

        private ClientToken _clientToken;
        private SocketAsyncEventArgs _connectEvent;
        private SocketAsyncEventArgs _receiveEvent;
        private SocketAsyncEventArgs _sendEvent;

        private volatile bool _disposed;

#endregion

#region Properties

        public bool IsConnected { get; private set; }

#endregion

#region Ctor

        public TCPClient2(TCPClientSettings settings)
        {
            _settings = settings;
            _bufferManager = new BufferManager(_settings.BufferSize, 2);

            _receiveEvent = new SocketAsyncEventArgs();
            _bufferManager.SetBuffer(_receiveEvent);
            _receiveEvent.Completed += IO_Completed;

            _sendEvent = new SocketAsyncEventArgs();
            _bufferManager.SetBuffer(_sendEvent);
            _sendEvent.Completed += IO_Completed;

            _clientToken = new ClientToken(_receiveEvent, _sendEvent);
        }

#endregion

#region Class methods

        private void IO_Completed(object sender, SocketAsyncEventArgs e)
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
                    ProcessDisconnectAndCloseSocket(e);
                    break;
                default:
                    throw new ArgumentException("\r\nError in I/O Completed");
            }
        }

        public void Connect()
        {
            _connectEvent = new SocketAsyncEventArgs();
            _connectEvent.Completed += IO_Completed;
            _connectEvent.RemoteEndPoint = _settings.EndPoint;
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
                _clientToken.Prepare(e);
                DisposeEventObject(ref _connectEvent);

                IsConnected = true;

                //Fire Connected event
                if (Connected != null)
                {
                    Connected();
                }
            }
            else
            {
                ProcessConnectionError(e);
            }
        }

        private void ProcessConnectionError(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.ConnectionRefused &&
                e.SocketError != SocketError.TimedOut &&
                e.SocketError != SocketError.HostUnreachable)
            {
                CloseSocket(e.AcceptSocket);
            }

            //Fire ConnectionError event
            if (ConnectionError != null)
            {
                ConnectionError(e.SocketError);
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
            _clientToken.SendBytesRemainingCount = dataToSend.Length;
            _clientToken.ObjectToSend = dataToSend;

            //Send data
            StartSend(true);
        }

        private void StartSend(bool newSend)
        {
#if DEBUG
            if (_disposed) return;
#endif
            if (_clientToken.SendBytesRemainingCount == 0) return;

            if (_clientToken.SendBytesRemainingCount <= _settings.BufferSize)
            {
                _sendEvent.SetBuffer(_clientToken.BufferOffsetSend, _clientToken.SendBytesRemainingCount);
                Buffer.BlockCopy(_clientToken.DataToSend, _clientToken.BytesSentAlreadyCount,
                    _sendEvent.Buffer, _clientToken.BufferOffsetSend,
                    _clientToken.SendBytesRemainingCount);
            }
            else
            {
                _sendEvent.SetBuffer(_clientToken.BufferOffsetSend, _settings.BufferSize);
                Buffer.BlockCopy(_clientToken.DataToSend, _clientToken.BytesSentAlreadyCount,
                    _sendEvent.Buffer, _clientToken.BufferOffsetSend, _settings.BufferSize);
            }

            if (!_sendEvent.AcceptSocket.SendAsync(_sendEvent))
            {
                ProcessSend(_sendEvent);
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                _clientToken.SendBytesRemainingCount = _clientToken.SendBytesRemainingCount - e.BytesTransferred;
                if (_clientToken.SendBytesRemainingCount == 0)
                {
                    _clientToken.ResetSend();

                    //Fire DataSent event
                    if (DataSent != null)
                    {
                        DataSent((ITransferable) _clientToken.ObjectToSend);
                    }
                }
                else
                {
                    _clientToken.BytesSentAlreadyCount += e.BytesTransferred;
                    StartSend(false);
                }
            }
            else
            {
                //Fire DataSentError event
                if (DataSendError != null)
                {
                    DataSendError((ITransferable) _clientToken.ObjectToSend);
                }
                StartDisconnect(e);
            }
        }

        private void StartReceive()
        {
#if DEBUG
            if (_disposed) return;
#endif

            if (!_receiveEvent.AcceptSocket.ReceiveAsync(_receiveEvent))
            {
                ProcessReceive(_receiveEvent);
            }
        }
        
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                //Fire ClientDataReceiveError event
                if (DataReceiveError != null)
                {
                    DataReceiveError();
                }
                StartDisconnect(e);
                return;
            }

            if (e.BytesTransferred != 0)
            {
                _clientToken.AcceptData(e);

                if (_clientToken.ReceivedDataLength != 0)
                {
                    //Parse received data for packets
                    List<BaseResponse> packets = _clientToken.ProcessReceivedDataRsp();

                    //Fire ClientDataReceived event
                    if (packets.Count > 0 && DataReceived != null)
                    {
                        DataReceived(packets);
                    }
                }
            }

            StartReceive();
        }

        private void StartDisconnect(SocketAsyncEventArgs e)
        {
            e.AcceptSocket.Shutdown(SocketShutdown.Both);
            bool willRaiseEvent = e.AcceptSocket.DisconnectAsync(e);
            if (!willRaiseEvent)
            {
                ProcessDisconnectAndCloseSocket(e);
            }
        }
        
        private void ProcessDisconnectAndCloseSocket(SocketAsyncEventArgs e)
        {
            e.AcceptSocket.Close();
        }

        private void CloseSocket(Socket theSocket)
        {
            try
            {
                theSocket.Shutdown(SocketShutdown.Both);
            }
            catch { }
            theSocket.Close();

            IsConnected = false;

            //Fire Disconnected event
            if (Disconnected != null)
            {
                Disconnected();
            }
        }

        private void DisposeEventObject(ref SocketAsyncEventArgs e)
        {
            if (e != null)
            {
                e.Dispose();
                e = null;
            }
        }

        public override void Dispose()
        {
            _disposed = true;
            DisposeEventObject(ref _connectEvent);
            DisposeEventObject(ref _receiveEvent);
            DisposeEventObject(ref _sendEvent);
        }

#endregion

    }
}
 


