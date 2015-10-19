using RK.Common.Classes.Common;
using RK.Common.Proto.Packets;

namespace RK.Common.Proto.Responses
{
    public unsafe sealed class RPlayerMove : BaseResponse
    {
        public int X;
        public int Y;
        public Direction D;

        public int PlayerId;

        internal override void InitializeFromMemory(byte* bData)
        {
            X = *(int*)&bData[BASE_SIZE];
            Y = *(int*)&bData[BASE_SIZE + 4];
            D = *(Direction*)&bData[BASE_SIZE + 8];
            PlayerId = *(int*)&bData[BASE_SIZE + 9];
            base.InitializeFromMemory(bData);
        }

        public override byte[] Serialize()
        {
            const int pSize = BASE_SIZE + 13;
            byte[] data = new byte[pSize];
            fixed (byte* bData = data)
            {
                SerializeHeader(bData, pSize);
                (*(int*)&bData[BASE_SIZE]) = X;
                (*(int*)&bData[BASE_SIZE + 4]) = Y;
                (*(Direction*)&bData[BASE_SIZE + 8]) = D;
                (*(int*)&bData[BASE_SIZE + 9]) = PlayerId;
            }
            return data;
        }

        public RPlayerMove() { } 

        public RPlayerMove(int playerId, PPlayerMove pPlayerMove)
            : base(PacketType.PlayerMove)
        {
            X = pPlayerMove.X;
            Y = pPlayerMove.Y;
            D = pPlayerMove.D;
            PlayerId = playerId;
        }
    }
}
