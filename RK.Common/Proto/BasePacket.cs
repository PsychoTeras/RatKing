using System;
using System.Threading;
using RK.Common.Classes.Common;
using RK.Common.Proto.Packets;
using RK.Common.Win32;

namespace RK.Common.Proto
{
    public abstract unsafe class BasePacket : ITransferable
    {

#region Constants

        public const int ERR_PARTIAL_PACKET = 0;
        public const int ERR_INVALID_PACKET_SIZE = -1;
        public const int ERR_INVALID_PACKET_TYPE = -2;

        private const int BASE_SIZE = 20;

#endregion

#region Static fields

        private static int _packetIdCounter;
        private static int _sessionIdCounter;

#endregion

#region Public fields

        public int Id;
        public int SessionToken;
        public long TimeStamp;

#endregion

#region Properties

        public abstract PacketType Type { get; }

        protected virtual int SizeOf
        {
            get { return 0; }
        }

        protected virtual bool Compressed 
        {
            get { return false; }
        }

#endregion

#region Class methods

        public void Setup()
        {
            Id = Interlocked.Increment(ref _packetIdCounter);
            TimeStamp = DateTime.UtcNow.ToBinary();
        }

        public static int NewSessionToken()
        {
            return Interlocked.Increment(ref _sessionIdCounter);
        }

#endregion

#region Serialize

        protected virtual void SerializeToMemory(byte* bData, int pos) {}

        public byte[] Serialize()
        {
            int packetSize = BASE_SIZE + SizeOf;
            byte[] data = new byte[packetSize];
            fixed (byte* bData = data)
            {
                (*(short*)bData) = (short)packetSize;
                (*(PacketType*) &bData[2]) = Type;
                (*(long*) &bData[4]) = Id;
                (*(long*) &bData[8]) = SessionToken;
                (*(long*) &bData[12]) = TimeStamp;
                SerializeToMemory(bData, BASE_SIZE);

                if (Compressed)
                {
                    byte[] compressed = Compression.Compress(data, BASE_SIZE);
                    int compressedLength = compressed.Length;
                    packetSize = BASE_SIZE + compressedLength;
                    Array.Resize(ref data, packetSize);
                    Buffer.BlockCopy(compressed, 0, data, BASE_SIZE, compressedLength);
                    fixed (byte* bNewData = data)
                    {
                        (*(short*)bNewData) = (short)packetSize;
                    }
                }
            }
            return data;
        }

#endregion

#region Deserialize

        private static BasePacket AllocNew(PacketType packetType)
        {
            switch (packetType)
            {
                    //Test
                case PacketType.TestXkb:
                    return new PTestXkb();

                    //User
                case PacketType.UserLogin:
                    return new PUserLogin();
                case PacketType.UserLogout:
                    return new PUserLogout();

                    //Player
                case PacketType.UserEnter:
                    return new PUserEnter();
                case PacketType.PlayerMove:
                    return new PPlayerMove();
                case PacketType.PlayerRotate:
                    return new PPlayerRotate();

                    //Map
                case PacketType.MapData:
                    return new PMapData();

                    //Error
                default:
                    return null;
            }
        }

        protected virtual void DeserializeFromMemory(byte* bData, int pos) { }

        public static BasePacket Deserialize(byte[] data, int dataSize, int pos, out short packetSize)
        {
            dataSize -= pos;
            if (dataSize < BASE_SIZE)
            {
                packetSize = 0;
                return null;
            }

            BasePacket packet;

            fixed (byte* bData = &data[pos])
            {
                packetSize = *((short*) bData);
                if (packetSize <= 0)
                {
                    packetSize = ERR_INVALID_PACKET_SIZE;
                    return null;
                }

                if (dataSize < packetSize)
                {
                    packetSize = ERR_PARTIAL_PACKET;
                    return null;
                }

                PacketType packetType = (PacketType) (*(short*) &bData[2]);
                packet = AllocNew(packetType);
                if (packet == null)
                {
                    packetSize = ERR_INVALID_PACKET_TYPE;
                    return null;
                }

                packet.Id = *(int*) &bData[4];
                packet.SessionToken = *(int*) &bData[8];
                packet.TimeStamp = *(long*) &bData[12];

                if (!packet.Compressed)
                {
                    packet.DeserializeFromMemory(bData, BASE_SIZE);
                    return packet;
                }
            }

            if (packet.Compressed)
            {
                byte[] decompressed = Compression.Decompress(data, pos + BASE_SIZE);
                fixed (byte* pDecompressed = decompressed)
                {
                    packet.DeserializeFromMemory(pDecompressed, 0);
                }
            }

            return packet;
        }

#endregion

    }
}
