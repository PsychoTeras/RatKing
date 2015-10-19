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

        internal override void InitializeFromMemory(byte* bData)
        {
            X = *(int*)&bData[BASE_SIZE];
            Y = *(int*)&bData[BASE_SIZE + 4];
            D = *(Direction*)&bData[BASE_SIZE + 8];
            base.InitializeFromMemory(bData);
        }

        public override byte[] Serialize()
        {
            const int pSize = BASE_SIZE + 9;
            byte[] data = new byte[pSize];
            fixed (byte* bData = data)
            {
                SerializeHeader(bData, pSize);
                (*(int*) &bData[BASE_SIZE]) = X;
                (*(int*) &bData[BASE_SIZE + 4]) = Y;
                (*(Direction*) &bData[BASE_SIZE + 8]) = D;
            }
            return data;
        }
    }
}