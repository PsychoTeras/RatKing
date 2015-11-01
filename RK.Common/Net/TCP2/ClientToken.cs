using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using RK.Common.Classes.Common;
using RK.Common.Proto;

namespace RK.Common.Net.TCP2
{
    internal class ClientToken
    {
        private static int _idCounter;

        public readonly int BufferOffsetReceive;
        public readonly int BufferOffsetSend;

        public int Id;

        public SocketAsyncEventArgs ReceiveEvent;
        public SocketAsyncEventArgs SendEvent;

        public MemoryStream ReceivedData;
        public int ReceivedDataLength;

        public int SendBytesRemainingCount;
        public int BytesSentAlreadyCount;
        public byte[] DataToSend;
        public object ObjectToSend;
        public HybridLock SendSync;

        public volatile bool Closed;

        public ClientToken(SocketAsyncEventArgs receiveEvent, SocketAsyncEventArgs sendEvent)
        {
            Id = _idCounter++;

            ReceiveEvent = receiveEvent;
            BufferOffsetReceive = receiveEvent.Offset;

            SendEvent = sendEvent;
            BufferOffsetSend = sendEvent.Offset;

            ReceivedData = new MemoryStream();
            SendSync = new HybridLock();
        }

        public void Prepare(SocketAsyncEventArgs e)
        {
            ReceiveEvent.AcceptSocket = e.AcceptSocket;
            SendEvent.AcceptSocket = e.AcceptSocket;
            e.AcceptSocket = null;
            Closed = false;
        }

        public void AcceptData(SocketAsyncEventArgs e)
        {
            ReceivedDataLength = e.BytesTransferred;
            ReceivedData.Write(e.Buffer, BufferOffsetReceive, ReceivedDataLength);
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

            //Parse packets
            int rSize;
            int pos = 0;

            byte[] buf = ReceivedData.GetBuffer();
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
            if (pos > 0)
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
            ObjectToSend = null;
            SendBytesRemainingCount = BytesSentAlreadyCount = 0;
            SendSync.Set();
        }

        public void ResetForClose()
        {
            Closed = true;
            ResetSend();
            ResetReceive();
        }
    }
}