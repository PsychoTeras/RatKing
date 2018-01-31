using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using RK.Common.Classes.Common;
using RK.Common.Proto;
using System.Threading.Tasks;

namespace RK.Common.Net
{
    internal class ClientToken : IDisposable
    {
        public readonly int Id;

        public readonly int BufferOffsetReceive;
        public readonly int BufferOffsetSend;

        public Socket Socket;
        public SocketAsyncEventArgs ReceiveEvent;
        public SocketAsyncEventArgs SendEvent;

        public MemoryStream ReceivedData;
        public int ReceivedDataLength;

        public int SendBytesRemaining;
        public int BytesSentAlready;
        public byte[] DataToSend;

        public LightLock ActionSendSync;

        public volatile bool Closed;

        public ClientToken() { }

        public ClientToken(int id, Socket socket, SocketAsyncEventArgs receiveEvent, SocketAsyncEventArgs sendEvent)
        {
            Id = id;

            Socket = socket;

            ReceiveEvent = receiveEvent;
            BufferOffsetReceive = receiveEvent.Offset;

            SendEvent = sendEvent;
            BufferOffsetSend = sendEvent.Offset;

            ReceivedData = new MemoryStream();
            ActionSendSync = new LightLock();
        }

        public void AcceptConnection(SocketAsyncEventArgs e, bool cleanAcceptSocket)
        {
            ReceiveEvent.AcceptSocket = e.AcceptSocket;
            Socket = e.AcceptSocket;
            if (cleanAcceptSocket) e.AcceptSocket = null;
            Closed = false;
        }

        public void AcceptData(SocketAsyncEventArgs e, int bytesTransferred)
        {
            ReceivedDataLength += bytesTransferred;
            ReceivedData.Write(e.Buffer, BufferOffsetReceive, bytesTransferred);
        }

        public async Task AcceptDataAsync(SocketAsyncEventArgs e, int bytesTransferred)
        {
            ReceivedDataLength += bytesTransferred;
            await ReceivedData.WriteAsync(e.Buffer, BufferOffsetReceive, bytesTransferred);
        }

        public List<BasePacket> ProcessReceivedDataReq()
        {
            List<BasePacket> packets = new List<BasePacket>();

            //Parse packets
            short rSize;
            int pos = 0;

            byte[] buf = ReceivedData.GetBuffer();
            BasePacket packet;
            do
            {
                packet = BasePacket.Deserialize(buf, ReceivedDataLength, pos, out rSize);
                if (rSize > 0 && packet != null)
                {
                    packets.Add(packet);
                    pos += rSize;
                }
            } while (rSize > 0 && packet != null);

            //Shrink stream
            if (pos > 0)
            {
                ReceivedDataLength -= pos;
                Buffer.BlockCopy(buf, pos, buf, 0, ReceivedDataLength);
                ReceivedData.SetLength(ReceivedDataLength);
            }

            return packets;
        }

        public List<BaseResponse> ProcessReceivedDataRsp()
        {
            List<BaseResponse> packets = new List<BaseResponse>();

            byte[] buf = ReceivedData.GetBuffer();

            //Parse packets
            int rSize, pos = 0;
            BaseResponse packet;
            do
            {
                packet = BaseResponse.Deserialize(buf, ReceivedDataLength, pos, out rSize);
                if (rSize > 0 && packet != null)
                {
                    packets.Add(packet);
                    pos += rSize;
                }
            } while (rSize > 0 && packet != null);

            //Shrink stream
            if (pos > 0 && ReceivedDataLength > 0)
            {
                ReceivedDataLength -= pos;
                Buffer.BlockCopy(buf, pos, buf, 0, ReceivedDataLength);
                ReceivedData.SetLength(ReceivedDataLength);
            }
        
            return packets;
        }

        public void ResetReceive()
        {
            ReceivedData.SetLength(ReceivedDataLength = 0);
        }

        public void ResetSend()
        {
            DataToSend = null;
            SendBytesRemaining = BytesSentAlready = 0;
            ActionSendSync.Set();
        }

        public void Close()
        {
            lock (this)
            {
                if (Closed)
                {
                    return;
                }

                Closed = true;

                ActionSendSync.WaitOne();

                if (Socket != null && Socket.Connected)
                {
                    Socket.Disconnect(true);
                    Socket.Close();
                    Socket = null;
                }

                ResetReceive();
                ResetSend();
            }
        }

        public void Dispose()
        {
            Close();
            ReceivedData.Dispose();
        }
    }
}