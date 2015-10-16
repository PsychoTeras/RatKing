namespace RK.Common.Proto.Packets
{
    public sealed class PPlayerRotate : BasePacket
    {
        public float Angle;

        public override PacketType Type
        {
            get { return PacketType.PlayerRotate; }
        }
    }
}
