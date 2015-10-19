using System;
using System.Threading;
using RK.Common.Proto.Packets;

namespace RK.Common.Proto
{
    public unsafe abstract class BasePacket
    {

#region Constants

        protected const int BASE_SIZE = 30;

#endregion

#region Static fields

        private static long _packetIdCounter;
        private static int _sessionIdCounter = Environment.TickCount;

#endregion

#region Public fields

        public long Id;
        public long SessionToken;
        public long TimeStamp;

#endregion

#region Properties

        public abstract PacketType Type { get; }

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

        protected void SerializeHeader(byte* bData, int packetSize)
        {
            (*(int*)bData) = packetSize;
            (*(PacketType*)&bData[4]) = Type;
            (*(long*)&bData[6]) = Id;
            (*(long*)&bData[14]) = SessionToken;
            (*(long*)&bData[22]) = TimeStamp;
        }

        public virtual byte[] Serialize()
        {
            byte[] data = new byte[BASE_SIZE];
            fixed (byte* bData = data)
            {
                SerializeHeader(bData, BASE_SIZE);
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

                default:
                    throw new Exception(string.Format("Unknown packet type: {0}", packetType));
            }
        }

        internal virtual void InitializeFromMemory(byte* bData)
        {
            Id = *(long*)&bData[6];
            SessionToken = *(long*)&bData[14];
            TimeStamp = *(long*)&bData[22];
        }

        public static BasePacket Deserialize(byte[] data, out int packetSize)
        {
            int dataLength = data.Length;
            if (dataLength < BASE_SIZE)
            {
                packetSize = -1;
                return null;
            }

            fixed (byte* bData = data)
            {
                packetSize = *((int*) bData);
                if (packetSize < dataLength)
                {
                    return null;
                }

                PacketType packetType = (PacketType) (*(short*)&bData[4]);
                BasePacket packet = AllocNew(packetType);
                packet.InitializeFromMemory(bData);
                return packet;
            }
        }

#endregion

    }
}
