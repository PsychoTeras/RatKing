using RK.Common.Classes.Common;

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
            X = *(int*)&bData[pos];
            Y = *(int*)&bData[pos + 4];
            D = *(Direction*)&bData[pos + 8];
        }

        protected override void SerializeToMemory(byte* bData, int pos)
        {
            (*(int*)&bData[pos]) = X;
            (*(int*)&bData[pos + 4]) = Y;
            (*(Direction*)&bData[pos + 8]) = D;
        }
    }
}