using System;
using System.Threading;
using RK.Common.Common;
using RK.Common.Proto.Packets;

namespace RK.Common.Proto
{
    public unsafe abstract class BasePacket : ITransferable
    {

#region Constants

        public const int ERR_PARTIAL_PACKET = 0;
        public const int ERR_INVALID_PACKET_SIZE = -1;
        public const int ERR_INVALID_PACKET_TYPE = -2;

        protected const int BASE_SIZE = 24;

#endregion

#region Static fields

        private static int _packetIdCounter;
        private static int _sessionIdCounter = Environment.TickCount;

#endregion

#region Public fields

        public int Id;
        public long SessionToken;
        public long TimeStamp;

#endregion

#region Properties

        public abstract PacketType Type { get; }

        protected virtual int SizeOf
        {
            get { return BASE_SIZE; }
        }

#endregion

#region Class methods

        public void Setup()
        {
            Id = Interlocked.Increment(ref _packetIdCounter);
            TimeStamp = DateTime.UtcNow.ToBinary();
        }

        public static long NewSessionToken(int userId)
        {
            return ((long)userId << 32) | Interlocked.Increment(ref _sessionIdCounter);
        }

#endregion

#region Serialize

        protected virtual void SerializeToMemory(byte* bData, int pos) {}

        public byte[] Serialize()
        {
            int packetSize = SizeOf;
            byte[] data = new byte[packetSize];
            fixed (byte* bData = data)
            {
                (*(short*)bData) = (short)packetSize;
                (*(PacketType*)&bData[2]) = Type;
                (*(long*)&bData[4]) = Id;
                (*(long*)&bData[8]) = SessionToken;
                (*(long*)&bData[16]) = TimeStamp;
                SerializeToMemory(bData, BASE_SIZE);
            }
            return data;
        }

#endregion

#region Deserialize

        private static BasePacket AllocNew(PacketType packetType)
        {
            switch (packetType)
            {
                    //User
                case PacketType.UserLogin:
                    return new PUserLogin();
                case PacketType.UserLogout:
                    return new PUserLogout();

                    //Player
                case PacketType.PlayerEnter:
                    return new PPlayerEnter();
                case PacketType.PlayerMove:
                    return new PPlayerMove();
                case PacketType.PlayerRotate:
                    return new PPlayerRotate();

                    //Error
                default:
                    return null;
            }
        }

        protected virtual void DeserializeFromMemory(byte* bData, int pos) { }

        public static BasePacket Deserialize(byte[] data, int pos, out short packetSize)
        {
            int dataLength = data.Length - pos;
            if (dataLength < BASE_SIZE)
            {
                packetSize = 0;
                return null;
            }

            fixed (byte* bData = &data[pos])
            {
                packetSize = *((short*) bData);
                if (packetSize <= 0)
                {
                    packetSize = ERR_INVALID_PACKET_SIZE;
                    return null;
                }

                if (dataLength < packetSize)
                {
                    packetSize = ERR_PARTIAL_PACKET;
                    return null;
                }

                PacketType packetType = (PacketType) (*(short*) &bData[2]);
                BasePacket packet = AllocNew(packetType);
                if (packet == null)
                {
                    packetSize = ERR_INVALID_PACKET_TYPE;
                    return null;
                }

                packet.Id = *(int*) &bData[4];
                packet.SessionToken = *(long*) &bData[8];
                packet.TimeStamp = *(long*) &bData[16];

                packet.DeserializeFromMemory(bData, BASE_SIZE);

                return packet;
            }
        }

#endregion

    }
}
