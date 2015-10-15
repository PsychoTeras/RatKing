namespace RK.Common.Proto.Packets
{
    public sealed class PPlayerEnter : BasePacket
    {
        public override PacketType Type
        {
            get { return PacketType.PlayerEnter; }
        }
    }
}
