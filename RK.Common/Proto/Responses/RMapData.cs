using RK.Common.Classes.Common;
using RK.Common.Proto.Packets;
using RK.Common.Win32;

namespace RK.Common.Proto.Responses
{
    public unsafe sealed class RMapData : BaseResponse
    {
        public byte[] MapData;
        public ShortRect MapWindow;

        public override PacketType Type
        {
            get { return PacketType.MapData; }
        }

        protected override int SizeOf
        {
            get
            {
                return
                    Serializer.SizeOf(MapData) +
                    sizeof (ShortRect);
            }
        }

        protected override bool Compressed
        {
            get { return true; }
        }

        public override bool Private
        {
            get { return true; }
        }

        protected override void DeserializeFromMemory(byte* bData, int pos)
        {
            Serializer.Read(bData, out MapData, ref pos);
            Serializer.Read(bData, out MapWindow, ref pos);
        }

        protected override void SerializeToMemory(byte* bData, int pos)
        {
            Serializer.Write(bData, MapData, ref pos);
            Serializer.Write(bData, MapWindow, ref pos);
        }

        public RMapData() { }

        public RMapData(PMapData pMapData) 
            : base(pMapData) { }
    }
}
