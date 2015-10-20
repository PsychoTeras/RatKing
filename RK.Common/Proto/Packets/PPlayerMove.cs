using RK.Common.Classes.Common;
using RK.Common.Win32;

namespace RK.Common.Proto.Packets
{
    public sealed unsafe class PPlayerMove : BasePacket
    {
        public int X;
        public int Y;
        public Direction D;

        public override PacketType Type
        {
            get { return PacketType.PlayerMove; }
        }

        protected override int SizeOf
        {
            get
            {
                return
                    BASE_SIZE +
                    sizeof(int) +
                    sizeof(int) +
                    sizeof(Direction);
            }
        }

        protected override void DeserializeFromMemory(byte* bData, int pos)
        {
            Serializer.Read(bData, out X, ref pos);
            Serializer.Read(bData, out Y, ref pos);
            Serializer.Read(bData, out D, ref pos);
        }

        protected override void SerializeToMemory(byte* bData, int pos)
        {
            Serializer.Write(bData, X, ref pos);
            Serializer.Write(bData, Y, ref pos);
            Serializer.Write(bData, D, ref pos);
        }
    }
}