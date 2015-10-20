using System.Collections.Generic;
using RK.Common.Classes.Units;
using RK.Common.Proto.Packets;
using RK.Common.Win32;

namespace RK.Common.Proto.Responses
{
    public unsafe sealed class RPlayerEnter : BaseResponse
    {
        public int MyPlayerId;
        public List<Player> PlayersOnLocation;

        protected override int SizeOf
        {
            get
            {
                return
                    BASE_SIZE +
                    sizeof (int) +
                    Serializer.Length(PlayersOnLocation);
            }
        }

        protected override void DeserializeFromMemory(byte* bData, int pos)
        {
            Serializer.Read(bData, out MyPlayerId, ref pos);
            Serializer.Read<Player, List<Player>>(bData, out PlayersOnLocation, ref pos);
        }

        protected override void SerializeToMemory(byte* bData, int pos)
        {
            Serializer.Write(bData, MyPlayerId, ref pos);
            Serializer.Write(bData, PlayersOnLocation, ref pos);
        }

        public RPlayerEnter() { }

        public RPlayerEnter(int myPlayerId, List<Player> playersOnLocation, PPlayerEnter pPlayerEnter)
            : base(pPlayerEnter)
        {
            MyPlayerId = myPlayerId;
            PlayersOnLocation = playersOnLocation;
        }
    }
}
