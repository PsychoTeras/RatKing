namespace RK.Common.Proto.Packets
{
    public sealed class PUserEnter : BasePacket
    {
        public override PacketType Type
        {
            get { return PacketType.UserEnter; }
        }
    }
}
