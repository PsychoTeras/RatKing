using RK.Common.Common;
using RK.Common.Proto.Packets;
using RK.Common.Win32;

namespace RK.Common.Proto.Responses
{
    public unsafe sealed class RPlayerMove : BaseResponse
    {
        public int X;
        public int Y;
        public Direction D;

        public int PlayerId;

        public override PacketType Type
        {
            get { return PacketType.PlayerMove; }
        }

        protected override int SizeOf
        {
            get
            {
                return
                    sizeof (int) +
                    sizeof (int) +
                    sizeof (Direction) +
                    sizeof (int);
            }
        }

        protected override void DeserializeFromMemory(byte* bData, int pos)
        {
            Serializer.Read(bData, out PlayerId, ref pos);
            Serializer.Read(bData, out X, ref pos);
            Serializer.Read(bData, out Y, ref pos);
            Serializer.Read(bData, out D, ref pos);
        }

        protected override void SerializeToMemory(byte* bData, int pos)
        {
            Serializer.Write(bData, PlayerId, ref pos);
            Serializer.Write(bData, X, ref pos);
            Serializer.Write(bData, Y, ref pos);
            Serializer.Write(bData, D, ref pos);
        }

        public RPlayerMove() { } 

        public RPlayerMove(int playerId, PPlayerMove pPlayerMove)
        {
            X = pPlayerMove.X;
            Y = pPlayerMove.Y;
            D = pPlayerMove.D;
            PlayerId = playerId;
        }
    }
}
