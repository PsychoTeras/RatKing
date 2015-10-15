using RK.Common.Classes.Units;

namespace RK.Common.Proto.Responses
{
    public sealed class RPlayerEnter : BaseResponse
    {
        public Player[] PlayersOnLocation;
        public int MyPlayerId;

        public RPlayerEnter()
        {
            Type = PacketType.PlayerEnter;
        }
    }
}
