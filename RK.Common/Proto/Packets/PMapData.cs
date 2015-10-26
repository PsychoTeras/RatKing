namespace RK.Common.Proto.Packets
{
    public sealed class PMapData : BasePacket
    {
        public override PacketType Type
        {
            get { return PacketType.MapData; }
        }
    }
}
