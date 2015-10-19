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
            Serializer.ReadCollection<Player, List<Player>>(bData, out PlayersOnLocation, BASE_SIZE + 4);
            base.InitializeFromMemory(bData);
        }

        public override byte[] Serialize()
        {
            int pSize = BASE_SIZE + 4 +
                Serializer.CollectionLength(PlayersOnLocation);
            byte[] data = new byte[pSize];
            fixed (byte* bData = data)
            {
                SerializeHeader(bData, pSize);
                (*(int*)&bData[BASE_SIZE]) = MyPlayerId;
                Serializer.WriteCollection(PlayersOnLocation, bData, BASE_SIZE + 4);
            }
            return data;
        }
    }
}
