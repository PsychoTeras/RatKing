using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using RK.Common.Classes.Common;
using RK.Common.Proto;

namespace RK.Common.Net.TCP
{
    public sealed class TCPClient : TCPBase
    {

#region Delegates

        public delegate void OnDataReceived(IList<BaseResponse> packets);
        public delegate void OnDataReceiveError(Exception ex);
        public delegate void OnDataSent(ITransferable packet, object userData);
        public delegate void OnDataSendError(ITransferable packet, object userData, Exception ex);

#endregion
        
#region Events

        public event OnDataReceived DataReceived;
        public event OnDataReceiveError DataReceiveError;
        public event OnDataSent DataSent;
        public event OnDataSendError DataSendError;

        public event Action Connected;
        public event Action Disconnected;

#endregion

#region Private fields

        private string _server;
        private int _port;

        private TCPClientEx _tcpClient;
        private NetworkStream _networkStream;

        private Thread _dataThread;
        private MemoryStream _receivedData;

        private int _connected;

#endregion

#region Properties

        public bool IsConnected
        {
            get { return _connected == 1; }
        }

        public string ServerAddress
        {
            get { return string.Format("{0}:{1}", _server, _port); }
        }

#endregion

#region Class methods

        public TCPClient() { }

        public TCPClient(string server, int port)
            : this()
        {
            Init(server, port);
        }

        public void Init(string server, int port)
        {
            _server = server;
            _port = port;
        }

#endregion

#region Network methods

        #region Connection

        public bool Connect()
        {
            try
            {
                //Terminate current connection
                Disconnect();

                //Establish TCP connection
                _tcpClient = new TCPClientEx(_server, _port);
                _networkStream = ((TcpClient)_tcpClient).GetStream();

                //Create data thread
                _receivedData = new MemoryStream();
                _dataThread = new Thread(() => ReceiveData(_tcpClient, _networkStream));
                _dataThread.Start();

                //Set connection state
                _connected = 1;

                //Fire Connected event
                if (Connected != null)
                {
                    Connected();
                }
            }
            catch (Exception ex)
            {
                OutLogMessage(string.Format("CLN CONNECTION ERROR: {0}", ex));
            }

            //Done
            return IsConnected;
        }
        
        public bool Disconnect()
        {
            if (Interlocked.CompareExchange(ref _connected, 0, 1) == 1)
            {
                try
                {
                    //Close existing connection
                    if (_tcpClient.Connected)
                    {
                        ((TcpClient)_tcpClient).Close();
                    }

                    //Terminate data thread
                    if (_dataThread != null && _dataThread.IsAlive &&
                        Thread.CurrentThread != _dataThread)
                    {

                        _dataThread.Abort();
                        _dataThread.Join();
                        _dataThread = null;
                    }
                    _receivedData.Dispose();

                    //Fire Disconnected event
                    if (Disconnected != null)
                    {
                        Disconnected();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    OutLogMessage(string.Format("CLN DISCONNECTION ERROR: {0}", ex));
                }
            }
            return false;
        }

        #endregion
        
        #region Data

        public bool SendData(ITransferable packet, object userData = null)
        {
            if (_connected == 0)
            {
                return false;
            }

            try
            {
                //Send data
                byte[] byteBuffer = packet.Serialize();
                int dataLength = byteBuffer.Length;
                _networkStream.Write(byteBuffer, 0, dataLength);

                //Fire DataSent event
                if (DataSent != null)
                {
                    DataSent(packet, userData);
                }

                return true;
            }
            catch (Exception ex)
            {
                if (DataSendError != null)
                {
                    DataSendError(packet, userData, ex);
                }
                Disconnect();
            }

            return false;
        }

        private void ProcessReceivedData(MemoryStream stream)
        {
            List<BaseResponse> packets = new List<BaseResponse>();
            byte[] buf = stream.GetBuffer();

            //Parse packets
            int rSize, pos = 0;
            int dataSize = (int) stream.Length;
            BaseResponse packet;
            do
            {
                packet = BaseResponse.Deserialize(buf, dataSize, pos, out rSize);
                if (rSize > 0 && packet != null)
                {
                    packets.Add(packet);
                    pos += rSize;
                }
            } while (rSize > 0 && packet != null);

            //Shrink stream
            if (pos > 0)
            {
                Buffer.BlockCopy(buf, pos, buf, 0, dataSize - pos);
                stream.SetLength(dataSize - pos);
            }

            //Fire DataReceived event
            if (DataReceived != null && packets.Count > 0)
            {
                DataReceived(packets);
            }
        }

        private void ReceiveData(TCPClientEx tcpClientEx, NetworkStream networkStream)
        {
            TcpClient tcpClient = tcpClientEx;
            try
            {
                int dataSize;
                byte[] buffer = new byte[tcpClient.ReceiveBufferSize];
                while ((dataSize = networkStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    _receivedData.Write(buffer, 0, dataSize);
                    ProcessReceivedData(_receivedData);
                }
            }
            catch (Exception ex)
            {
                if (IsConnected && DataReceiveError != null)
                {
                    DataReceiveError(ex);
                }
            }
            finally
            {
                //Disconnect from a server
                if (IsConnected)
                {
                    Disconnect();
                }
            }
        }

        #endregion

#endregion

#region IDisposable

        public override void Dispose()
        {
            Disconnect();
        }

#endregion

    }
}
