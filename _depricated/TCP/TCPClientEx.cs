using System.Net.Sockets;

namespace RK.Common.Net.TCP
{
    public sealed class TCPClientEx
    {

#region Private fields

        private TcpClient _client;
        private string _hostInfo;

#endregion

#region Properties

        public long Id { get; private set; }

        public bool Connected
        {
            get { return _client.Connected && _client.Client.Connected; }
        }

#endregion

#region Ctor

        public TCPClientEx(TcpClient client)
        {
            _client = client;
            _hostInfo = _client.Client.RemoteEndPoint.ToString();
            Id = _client.GetId();
        }

        public TCPClientEx(string hostName, int port)
        {
            _client = new TcpClient(hostName, port);
            _hostInfo = _client.Client.RemoteEndPoint.ToString();
            Id = _client.GetId();
        }

#endregion

#region Class methods

        public static implicit operator TcpClient(TCPClientEx client)
        {
            return client._client;
        }

        public override string ToString()
        {
            return _hostInfo;
        }

#endregion

    }
}
