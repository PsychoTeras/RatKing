using System.Collections.Generic;
using RK.Common.Classes.Units;
using RK.Common.Win32;

namespace RK.Common.Proto.Responses
{
    public unsafe sealed class RPlayerEnter : BaseResponse
    {
        public int MyPlayerId;
        public List<Player> PlayersOnLocation;

        internal override void InitializeFromMemory(byte* bData)
        {
            MyPlayerId = *(int*)&bData[BASE_SIZE];
//            Serializer.Read<Player, List<Player>>(bData, out PlayersOnLocation, BASE_SIZE + 4);
            base.InitializeFromMemory(bData);
        }

        public override byte[] Serialize()
        {
            int pSize = BASE_SIZE + 4 +
                Serializer.Length(PlayersOnLocation);
            byte[] data = new byte[pSize];
            fixed (byte* bData = data)
            {
                SerializeHeader(bData, pSize);
                (*(int*)&bData[BASE_SIZE]) = MyPlayerId;
//                Serializer.Write(bData, PlayersOnLocation, BASE_SIZE + 4);
//                Serializer.Write(bData, PlayersOnLocation, ref pSize);
            }
            return data;
        }
    }
}
