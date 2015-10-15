namespace RK.Common.Proto.Packets
{
    public sealed class PUserLogout : BasePacket
    {
        public override PacketType Type
        {
            get { return PacketType.UserLogout;  }
        }
    }
}
