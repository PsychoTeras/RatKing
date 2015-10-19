using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RK.Common.Net.TCP
{
    internal sealed class TCPClient : TCPBase
    {

#region Delegates

        public delegate void OnDataReceived(string data);
        public delegate void OnDataReceiveError(Exception ex);
        public delegate void OnDataSent(string data, object userData);
        public delegate void OnDataSendError(string data, object userData, Exception ex);

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

        public bool SendData(string data, object userData = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    //Send data
                    byte[] byteBuffer = Encoding.UTF8.GetBytes(data);
                    int dataLength = byteBuffer.Length;
                    _networkStream.Write(byteBuffer, 0, dataLength);

                    //Fire DataSent event
                    if (DataSent != null)
                    {
                        DataSent(data, userData);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                if (DataSendError != null)
                {
                    DataSendError(data, userData, ex);
                }
                Disconnect();
            }

            return false;
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
                    //Receive data
                    string data = Encoding.UTF8.GetString(buffer, 0, dataSize);

                    //Fire DataReceived event
                    if (DataReceived != null)
                    {
                        DataReceived(data);
                    }
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
