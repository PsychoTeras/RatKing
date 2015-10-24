using RK.Common.Common;
using RK.Common.Win32;

namespace RK.Common.Proto.Responses
{
    public unsafe sealed class RMapBuffer : BaseResponse
    {
        public byte[] MapBuffer;
        public ShortRect MapWindow;

        public override PacketType Type
        {
            get { return PacketType.MapBuffer; }
        }

        protected override int SizeOf
        {
            get
            {
                return
                    Serializer.SizeOf(MapBuffer) +
                    sizeof (ShortRect);
            }
        }

        protected override bool Compressed
        {
            get { return true; }
        }

        protected override void DeserializeFromMemory(byte* bData, int pos)
        {
            Serializer.Read(bData, out MapBuffer, ref pos);
            Serializer.Read(bData, out MapWindow, ref pos);
        }

        protected override void SerializeToMemory(byte* bData, int pos)
        {
            Serializer.Write(bData, MapBuffer, ref pos);
            Serializer.Write(bData, MapWindow, ref pos);
        }
    }
}
