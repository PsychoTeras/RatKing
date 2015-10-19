using System;
using System.Net;
using System.Net.Sockets;

namespace RK.Common.Net.TCP
{
    internal static class TCPExtentions
    {
        public static int GetIp(this TcpClient client)
        {
            IPEndPoint endPoint = (IPEndPoint)client.Client.RemoteEndPoint;
            return BitConverter.ToInt32(endPoint.Address.GetAddressBytes(), 0);
        }

        public static long GetId(this TcpClient client)
        {
            int address = GetIp(client);
            IPEndPoint endPoint = (IPEndPoint) client.Client.RemoteEndPoint;
            long id = (((long) address) << 32) | endPoint.Port;
            return id;
        }
    }
}
