using RK.Common.Classes.Common;

namespace RK.Common.Proto.Packets
{
    public sealed class PPlayerMove : BasePacket
    {
        public int X;
        public int Y;
        public Direction D;

        public override PacketType Type
        {
            get { return PacketType.PlayerMove; }
        }
    }
}