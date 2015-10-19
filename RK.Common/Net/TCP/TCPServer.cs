using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RK.Common.Net.TCP
{
    internal sealed class TCPServer : TCPBase
    {

#region Delegates

        public delegate void OnClientConnetion(TCPClientEx tcpClient);
        public delegate void OnClientDataReceived(TCPClientEx tcpClient, string data);
        public delegate void OnClientDataReceiveError(TCPClientEx tcpClient, Exception ex);
        public delegate void OnClientDataSent(TCPClientEx tcpClient, string data, object userData);
        public delegate void OnClientDataSendError(TCPClientEx tcpClient, string data, object userData, Exception ex);

#endregion

#region Events

        public event Action Started;
        public event Action Stopped;

        public event OnClientConnetion ClientConnected;
        public event OnClientConnetion ClientDisonnected;
        public event OnClientDataReceived ClientDataReceived;
        public event OnClientDataReceiveError ClientDataReceiveError;
        public event OnClientDataSent ClientDataSent;
        public event OnClientDataSendError ClientDataSendError;

#endregion

#region Private fields

        private int _port;

        private volatile TcpListener _tcpListener;
        private Thread _listenerThread;
        private Dictionary<long, TCPClientEx> _connectedClientsId;

        private int _started;
        
#endregion

#region Properties

        public bool IsStarted
        {
            get { return _started == 1; }
        }

#endregion

#region Class methods

        public TCPServer()
        {
            _connectedClientsId = new Dictionary<long, TCPClientEx>();
        }

        public TCPServer(int port) : this()
        {
            Init(port);
        }

        public void Init(int port)
        {
            _port = port;
        }

        private void AddConnectedClient(TCPClientEx tcpClient)
        {
            lock (_connectedClientsId)
            {
                RemoveConnectedClient(tcpClient);
                _connectedClientsId.Add(tcpClient.Id, tcpClient);
            }
        }

        public TCPClientEx GetConnectedClient(long clientId)
        {
            lock (_connectedClientsId)
            {
                return _connectedClientsId.ContainsKey(clientId)
                           ? _connectedClientsId[clientId]
                           : null;
            }
        }

        private void RemoveConnectedClient(TCPClientEx tcpClient)
        {
            lock (_connectedClientsId)
            {
                if (_connectedClientsId.ContainsKey(tcpClient.Id))
                {
                    TcpClient oldTcpClient = _connectedClientsId[tcpClient.Id];
                    if (oldTcpClient.Connected)
                    {
                        oldTcpClient.Close();
                    }
                    _connectedClientsId.Remove(tcpClient.Id);
                }
            }
        }

        private void ClearConnectedClients()
        {
            lock (_connectedClientsId)
            {
                _connectedClientsId.Clear();
            }
        }

#endregion

#region Network methods

        #region Listener

        public bool Start()
        {
            try
            {
                //Stop listener
                Stop();

                //Clear connected clients list
                ClearConnectedClients();

                //Start listener
                _tcpListener = new TcpListener(IPAddress.Any, _port);

                //Create data thread
                _listenerThread = new Thread(ListenForClients);
                _listenerThread.Start();

                //Set state
                _started = 1;

                //Fire Started event
                if (Started != null)
                {
                    Started();
                }
            }
            catch (Exception ex)
            {
                OutLogMessage(string.Format("SRV START ERROR: {0}", ex));
            }

            //Done
            return IsStarted;
        }

        public void Stop()
        {
            if (Interlocked.CompareExchange(ref _started, 0, 1) == 1)
            {
                try
                {
                    //Terminate listener thread
                    if (_listenerThread != null && _listenerThread.IsAlive &&
                        Thread.CurrentThread != _listenerThread)
                    {
                        _listenerThread.Abort();
                        _listenerThread.Join();
                        _listenerThread = null;
                    }

                    //Terminate listener
                    if (_tcpListener != null)
                    {
                        _tcpListener.Stop();
                        _tcpListener = null;
                    }

                    //Clear connected clients list
                    ClearConnectedClients();

                    //Fire Started event
                    if (Stopped != null)
                    {
                        Stopped();
                    }
                }
                catch (Exception ex)
                {
                    OutLogMessage(string.Format("SRV STOP ERROR: {0}", ex));
                }
            }
        }

        private void ListenForClients()
        {
            //Start listener
            try
            {
                _tcpListener.Start();
            }
            catch (Exception ex)
            {
                OutLogMessage(string.Format("SRV START LISTENER ERROR: {0}", ex));
                return;
            }

            //While this thread is alive and listener is not destroyes
            while (Thread.CurrentThread.IsAlive && _tcpListener != null)
            {
                //Check for new clients
                while (_tcpListener != null && _tcpListener.Pending())
                {
                    try
                    {
                        //Accept the client
                        TCPClientEx tcpClient = new TCPClientEx(_tcpListener.AcceptTcpClient());
                        AddConnectedClient(tcpClient);

                        //Create input stream reader for the client
                        NetworkStream networkStream = ((TcpClient)tcpClient).GetStream();
                        ThreadPool.QueueUserWorkItem(IncomingClientPrc,
                            new object[]
                            {
                                tcpClient,
                                networkStream
                            });

                        //Fire ClientConnected event
                        if (ClientConnected != null)
                        {
                            ClientConnected(tcpClient);
                        }
                    }
                    catch (Exception ex)
                    {
                        OutLogMessage(string.Format("SRV ACCEPT CLIENT ERROR: {0}", ex));
                    }
                }
                Thread.Sleep(1);
            }
        }

        public bool DropClient(int clientId)
        {
            TCPClientEx tcpClient = GetConnectedClient(clientId);
            if (tcpClient == null)
            {
                OutLogMessage(string.Format("SRV DROP CLIENT ERROR, client ID is invalid: {0}", clientId));
                return false;
            }
            return DropClient(tcpClient);
        }

        public bool DropClient(TCPClientEx tcpClient)
        {
            try
            {
                ((TcpClient)tcpClient).Close();
                return true;
            }
            catch (Exception ex)
            {
                OutLogMessage(string.Format("SRV DROP CLIENT ERROR: {0}", ex));
            }
            return false;
        }

        #endregion

        #region Data

        public bool SendData(long clientId, string data)
        {
            TCPClientEx tcpClient = GetConnectedClient(clientId);
            if (tcpClient == null)
            {
                OutLogMessage(string.Format("SRV DATA SEND ERROR, client ID is invalid: {0}", clientId));
                return false;
            }
            return SendData(tcpClient, data);
        }

        public bool SendData(TCPClientEx tcpClient, string data, object userData = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    //Send data
                    byte[] byteBuffer = Encoding.UTF8.GetBytes(data);
                    int dataLength = byteBuffer.Length;
                    ((TcpClient)tcpClient).GetStream().Write(byteBuffer, 0, dataLength);

                    //Fire DataSent event
                    if (ClientDataSent != null)
                    {
                        ClientDataSent(tcpClient, data, userData);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ClientDataSendError != null)
                {
                    ClientDataSendError(tcpClient, data, userData, ex);
                }
            }
            return false;
        }

        private void IncomingClientPrc(object client)
        {
            //Get TCP objects
            TCPClientEx tcpClientEx = (TCPClientEx)((object[])client)[0];
            NetworkStream networkStream = (NetworkStream)((object[])client)[1];
            TcpClient tcpClient = tcpClientEx;

            try
            {
                byte[] buffer = new byte[tcpClient.ReceiveBufferSize];
                try
                {
                    int dataSize;
                    while ((dataSize = networkStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        //Receive data
                        string data = Encoding.UTF8.GetString(buffer, 0, dataSize);

                        //Fire ClientDataReceived event
                        if (ClientDataReceived != null)
                        {
                            ClientDataReceived(tcpClientEx, data);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("WSACancelBlockingCall") && 
                        ClientDataReceiveError != null)
                    {
                        ClientDataReceiveError(tcpClientEx, ex);
                    }
                }

                //Remove current client
                RemoveConnectedClient(tcpClientEx);

                //Disconnect TCP client
                if (tcpClientEx.Connected)
                {
                    tcpClient.Close();
                }

                //Fire ClientDisonnected event
                if (ClientDisonnected != null)
                {
                    ClientDisonnected(tcpClientEx);
                }
            }
            catch {}
        }

        #endregion

#endregion

#region IDisposable

        public override void Dispose()
        {
            Stop();
        }

#endregion

    }
}
