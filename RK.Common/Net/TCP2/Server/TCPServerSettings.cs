namespace RK.Common.Net.TCP2.Server
{
    internal class TCPServerSettings
    {
        private int _port;
        private int _backlog;
        private int _maxConnections;
        
        private int _maxAcceptOps;
        private int _receiveBufferSize;

        public TCPServerSettings(int maxConnections, int backlog, int maxAcceptOps, 
            int receiveBufferSize, int port)
        {
            _maxConnections = maxConnections;
            _backlog = backlog;
            _maxAcceptOps = maxAcceptOps;
            _receiveBufferSize = receiveBufferSize;
            _port = port;
        }

        public int Port
        {
            get { return _port; }
        }

        public int MaxConnections
        {
            get { return _maxConnections; }
        }

        public int Backlog
        {
            get { return _backlog; }
        }

        public int MaxAcceptOps
        {
            get { return _maxAcceptOps; }
        }

        public int BufferSize
        {
            get { return _receiveBufferSize; }
        }
    }
}