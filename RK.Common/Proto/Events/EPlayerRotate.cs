using RK.Common.Proto.Packets;

namespace RK.Common.Proto.Events
{
    public sealed class EPlayerRotate : BaseEvent
    {
        public int PlayerId;
        public float Angle;

        public override PacketType Type
        {
            get { return PacketType.PlayerRotate; }
        }

        public EPlayerRotate(int playerId, PPlayerRotate pPlayerRotate)
        {
            PlayerId = playerId;
            Angle = pPlayerRotate.Angle;
        }
    }
}
