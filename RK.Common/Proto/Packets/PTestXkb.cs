using RK.Common.Win32;

namespace RK.Common.Proto.Packets
{
    public unsafe class PTestXkb : BasePacket
    {
        private const int SIZE = 0;

        private static byte[] _data = new byte[SIZE];

        protected override int SizeOf
        {
            get { return SIZE; }
        }

        public override PacketType Type
        {
            get { return PacketType.TestXkb; }
        }

        protected override void SerializeToMemory(byte* bData, int pos)
        {
            Serializer.Write(bData, _data, ref pos);
        }
    }
}
