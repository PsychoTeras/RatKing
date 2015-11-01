using System.Net;
using System.Net.Sockets;

namespace RK.Common.Net.TCP2.Client
{
    public class TCPClientSettings
    {
        private string _host;
        private int _port;
        private bool _autoReconnect;

        private int _receiveBufferSize;

        private IPEndPoint _endPoint;

        public TCPClientSettings(int receiveBufferSize, string host, int port, bool autoReconnect)
        {
            _receiveBufferSize = receiveBufferSize;
            _host = host;
            _port = port;
            _autoReconnect = autoReconnect;
            _endPoint = GetServerEndpointUsingIpAddress() ?? GetServerEndpointUsingMachineName();
        }

        private IPEndPoint GetServerEndpointUsingMachineName()
        {
            try
            {
                IPHostEntry theIpHostEntry = Dns.GetHostEntry(_host);
                IPAddress[] serverAddressList = theIpHostEntry.AddressList;

                bool gotIpv4Address = false;
                int count = -1;
                for (int i = 0; i < serverAddressList.Length; i++)
                {
                    count++;
                    AddressFamily addressFamily = serverAddressList[i].AddressFamily;
                    if (addressFamily == AddressFamily.InterNetwork)
                    {
                        gotIpv4Address = true;
                        i = serverAddressList.Length;
                    }
                }

                if (gotIpv4Address)
                    return new IPEndPoint(serverAddressList[count], _port);
            }
            catch {}

            return null;
        }

        private IPEndPoint GetServerEndpointUsingIpAddress()
        {
            try
            {
                IPAddress theIpAddress = IPAddress.Parse(_host);
                return new IPEndPoint(theIpAddress, _port);
            }
            catch
            {
                return null;
            }
        }

        public IPEndPoint EndPoint
        {
            get { return _endPoint; }
        }

        public string Host
        {
            get { return _host; }
        }

        public int Port
        {
            get { return _port; }
        }

        public bool Autoreconnect
        {
            get { return _autoReconnect; }
        }

        public int BufferSize
        {
            get { return _receiveBufferSize; }
        }
    }
}