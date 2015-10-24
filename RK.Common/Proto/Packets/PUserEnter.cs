using RK.Common.Common;
using RK.Common.Win32;

namespace RK.Common.Proto.Packets
{
    public unsafe sealed class PUserEnter : BasePacket
    {
        public ShortSize ScreenRes;

        public override PacketType Type
        {
            get { return PacketType.UserEnter; }
        }

        protected override int SizeOf
        {
            get
            {
                return 
                    sizeof(ShortSize);
            }
        }

        protected override void DeserializeFromMemory(byte* bData, int pos)
        {
            Serializer.Read(bData, out ScreenRes, ref pos);
        }

        protected override void SerializeToMemory(byte* bData, int pos)
        {
            Serializer.Write(bData, ScreenRes, ref pos);
        }
    }
}
