using System.Collections.Generic;
using RK.Common.Classes.Common;
using RK.Common.Classes.Units;
using RK.Common.Proto.Packets;
using RK.Common.Win32;

namespace RK.Common.Proto.Responses
{
    public unsafe sealed class RUserEnter : BaseResponse
    {
        public int MyPlayerId;
        public List<Player> PlayersOnLocation;

        public ShortSize MapSize;
        public byte[] MapData;
        public ShortRect MapWindow;

        public byte[] MiniMapData;
        public ShortSize MiniMapSize;

        public override PacketType Type
        {
            get { return PacketType.UserEnter; }
        }

        protected override int SizeOf
        {
            get
            {
                return
                    sizeof (int) +
                    Serializer.SizeOf(PlayersOnLocation) +

                    sizeof(ShortSize) +
                    Serializer.SizeOf(MapData) +
                    sizeof (ShortRect) +

                    Serializer.SizeOf(MiniMapData) +
                    sizeof (ShortSize);
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
            Serializer.Read(bData, out MyPlayerId, ref pos);
            Serializer.Read<Player, List<Player>>(bData, out PlayersOnLocation, ref pos);

            Serializer.Read(bData, out MapSize, ref pos);
            Serializer.Read(bData, out MapData, ref pos);
            Serializer.Read(bData, out MapWindow, ref pos);

            Serializer.Read(bData, out MiniMapData, ref pos);
            Serializer.Read(bData, out MiniMapSize, ref pos);
        }

        protected override void SerializeToMemory(byte* bData, int pos)
        {
            Serializer.Write(bData, MyPlayerId, ref pos);
            Serializer.Write<Player, List<Player>>(bData, PlayersOnLocation, ref pos);

            Serializer.Write(bData, MapSize, ref pos);
            Serializer.Write(bData, MapData, ref pos);
            Serializer.Write(bData, MapWindow, ref pos);

            Serializer.Write(bData, MiniMapData, ref pos);
            Serializer.Write(bData, MiniMapSize, ref pos);
        }

        public RUserEnter() { }

        public RUserEnter(PUserEnter pUserEnter)
            : base(pUserEnter) { }
    }
}
