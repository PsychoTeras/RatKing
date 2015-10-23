using System.Collections.Generic;
using RK.Common.Classes.Map;
using RK.Common.Common;
using RK.Common.Win32;

namespace RK.Common.Proto.Responses
{
    public unsafe sealed class RMapBuffer : BaseResponse
    {
        public List<Tile> MapBuffer;
        public ShortSize MapSize;
        public ShortRect MapWindow;

        protected override int SizeOf
        {
            get
            {
                return
                    BASE_SIZE +
                    Serializer.SizeOf(MapBuffer) +
                    sizeof (ShortSize) +
                    sizeof (ShortRect);
            }
        }

        protected override void DeserializeFromMemory(byte* bData, int pos)
        {
            Serializer.Read<Tile, List<Tile>>(bData, out MapBuffer, ref pos);
            Serializer.Read(bData, out MapSize, ref pos);
            Serializer.Read(bData, out MapWindow, ref pos);
        }

        protected override void SerializeToMemory(byte* bData, int pos)
        {
            Serializer.Write<Tile, List<Tile>>(bData, MapBuffer, ref pos);
            Serializer.Write(bData, MapSize, ref pos);
            Serializer.Write(bData, MapWindow, ref pos);
        }

        public RMapBuffer() : base(PacketType.MapBuffer) { }
    }
}
