using RK.Common.Classes.Common;
using RK.Common.Proto.Packets;

namespace RK.Common.Proto.Events
{
    public sealed class EPlayerMove : BaseEvent
    {
        public int PlayerId;

        public int X;
        public int Y;
        public Direction D;

        public override PacketType Type
        {
            get { return PacketType.PlayerMove; }
        }

        public EPlayerMove(int playerId, PPlayerMove pPlayerMove)
        {
            PlayerId = playerId;
            X = pPlayerMove.X;
            Y = pPlayerMove.Y;
            D = pPlayerMove.D;
        }
    }
}
