namespace RK.Common.Proto.Player
{
    public class PPlayerMove : BasePacket
    {
        public override PacketType Type
        {
            get { return PacketType.PlayerMove; }
        }
    }
}
