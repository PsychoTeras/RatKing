using System.Collections.Generic;
using RK.Common.Classes.Units;
using RK.Common.Common;
using RK.Common.Proto.Packets;
using RK.Common.Win32;

namespace RK.Common.Proto.Responses
{
    public unsafe sealed class RUserEnter : BaseResponse
    {
        public int MyPlayerId;
        public List<Player> PlayersOnLocation;

        public byte[] MapData;
        public ShortRect MapWindow;

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
            Serializer.Read(bData, out MyPlayerId, ref pos);
            Serializer.Read<Player, List<Player>>(bData, out PlayersOnLocation, ref pos);

            Serializer.Read(bData, out MapData, ref pos);
            Serializer.Read(bData, out MapWindow, ref pos);
        }

        protected override void SerializeToMemory(byte* bData, int pos)
        {
            Serializer.Write(bData, MyPlayerId, ref pos);
            Serializer.Write<Player, List<Player>>(bData, PlayersOnLocation, ref pos);

            Serializer.Write(bData, MapData, ref pos);
            Serializer.Write(bData, MapWindow, ref pos);
        }

        public RUserEnter() { }

        public RUserEnter(int myPlayerId, PUserEnter pUserEnter)
            : base(pUserEnter)
        {
            MyPlayerId = myPlayerId;
        }
    }
}
