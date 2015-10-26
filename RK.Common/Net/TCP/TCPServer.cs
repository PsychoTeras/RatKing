using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using RK.Common.Classes.Common;
using RK.Common.Proto;

namespace RK.Common.Net.TCP
{
    public sealed class TCPServer : TCPBase
    {

#region Delegates

        public delegate void OnClientConnetion(TCPClientEx tcpClient);
        public delegate void OnClientDataReceived(TCPClientEx tcpClient, IList<BasePacket> packets);
        public delegate void OnClientDataReceiveError(TCPClientEx tcpClient, Exception ex);
        public delegate void OnClientDataSent(TCPClientEx tcpClient, ITransferable packet, object userData);
        public delegate void OnClientDataSendError(TCPClientEx tcpClient, ITransferable packet, object userData, Exception ex);

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
        private Dictionary<long, TCPClientEx> _connectedClients;
        private Dictionary<long, MemoryStream> _clientsData;

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
            _connectedClients = new Dictionary<long, TCPClientEx>();
            _clientsData = new Dictionary<long, MemoryStream>();
        }

        public TCPServer(int port) : this()
        {
            Init(port);
        }

        public void Init(int port)
        {
            _port = port;
        }

        private MemoryStream AddConnectedClient(TCPClientEx tcpClient)
        {
            lock (_clientsData)
            {
                long clietnId = tcpClient.Id;
                RemoveConnectedClient(clietnId);
                _connectedClients.Add(clietnId, tcpClient);
                
                MemoryStream data = new MemoryStream();
                _clientsData.Add(clietnId, data);
                return data;
            }
        }

        public TCPClientEx GetConnectedClient(long clientId)
        {
            TCPClientEx clientEx;
            _connectedClients.TryGetValue(clientId, out clientEx);
            return clientEx;
        }

        private void RemoveConnectedClient(long clientId)
        {
            TCPClientEx tcpClient;
            if (_connectedClients.TryGetValue(clientId, out tcpClient))
            {
                lock (_clientsData)
                {
                    TcpClient oldTcpClient = tcpClient;
                    if (oldTcpClient.Connected)
                    {
                        oldTcpClient.Close();
                    }
                    _connectedClients.Remove(clientId);

                    _clientsData[clientId].Dispose();
                    _clientsData.Remove(clientId);
                }
            }
        }

        private void ClearConnectedClients()
        {
            lock (_clientsData)
            {
                _clientsData.Clear();
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
                        MemoryStream clientData = AddConnectedClient(tcpClient);

                        //Create input stream reader for the client
                        NetworkStream networkStream = ((TcpClient)tcpClient).GetStream();
                        ThreadPool.QueueUserWorkItem(IncomingClientPrc,
                            new object[]
                            {
                                tcpClient,
                                clientData,
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

        public bool SendData(long clientId, ITransferable packet)
        {
            TCPClientEx tcpClient = GetConnectedClient(clientId);
            if (tcpClient == null)
            {
                OutLogMessage(string.Format("SRV DATA SEND ERROR, client ID is invalid: {0}", clientId));
                return false;
            }
            return SendData(tcpClient, packet);
        }

        public bool SendAll(ITransferable packet, object userData = null)
        {
            lock (_clientsData)
            {
                Parallel.ForEach(_connectedClients.Values, c => SendData(c, packet, userData));
                return true;
            }
        }

        public bool SendData(TCPClientEx tcpClient, ITransferable packet, object userData = null)
        {
            if (!tcpClient.Connected)
            {
                return false;
            }

            try
            {
                //Send data
                byte[] byteBuffer = packet.Serialize();
                int dataLength = byteBuffer.Length;
                ((TcpClient) tcpClient).GetStream().Write(byteBuffer, 0, dataLength);

                //Fire DataSent event
                if (ClientDataSent != null)
                {
                    ClientDataSent(tcpClient, packet, userData);
                }

                return true;
            }
            catch (Exception ex)
            {
                if (ClientDataSendError != null)
                {
                    ClientDataSendError(tcpClient, packet, userData, ex);
                }
            }
            return false;
        }

        private void ProcessReceivedData(TCPClientEx tcpClientEx, MemoryStream stream)
        {
            List<BasePacket> packets = new List<BasePacket>();
            byte[] buf = stream.GetBuffer();

            //Parse packets
            short rSize;
            int pos = 0, dataSize = (int) stream.Length;
            BasePacket packet;
            do
            {
                packet = BasePacket.Deserialize(buf, dataSize, pos, out rSize);
                if (rSize > 0)
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

            //Fire ClientDataReceived event
            if (ClientDataReceived != null && packets.Count > 0)
            {
                ClientDataReceived(tcpClientEx, packets);
            }
        }

        private void IncomingClientPrc(object client)
        {
            //Get TCP objects
            TCPClientEx tcpClientEx = (TCPClientEx)((object[])client)[0];
            MemoryStream clientData = (MemoryStream)((object[])client)[1];
            NetworkStream networkStream = (NetworkStream)((object[])client)[2];
            TcpClient tcpClient = tcpClientEx;

            try
            {
                byte[] buffer = new byte[tcpClient.ReceiveBufferSize];
                try
                {
                    int dataSize;
                    while ((dataSize = networkStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        clientData.Write(buffer, 0, dataSize);
                        ProcessReceivedData(tcpClientEx, clientData);
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
                RemoveConnectedClient(tcpClientEx.Id);

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
