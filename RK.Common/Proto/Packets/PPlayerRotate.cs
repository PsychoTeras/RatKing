using RK.Common.Win32;

namespace RK.Common.Proto.Packets
{
    public unsafe sealed class PPlayerRotate : BasePacket
    {
        public float Angle;

        public override PacketType Type
        {
            get { return PacketType.PlayerRotate; }
        }

        protected override int SizeOf
        {
            get
            {
                return
                    BASE_SIZE +
                    sizeof(float);
            }
        }

        protected override void DeserializeFromMemory(byte* bData, int pos)
        {
            Serializer.Read(bData, out Angle, ref pos);
        }

        protected override void SerializeToMemory(byte* bData, int pos)
        {
            Serializer.Write(bData, Angle, ref pos);
        }
    }
}
