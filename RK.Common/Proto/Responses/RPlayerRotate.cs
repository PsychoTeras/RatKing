using RK.Common.Proto.Packets;
using RK.Common.Win32;

namespace RK.Common.Proto.Responses
{
    public unsafe sealed class RPlayerRotate : BaseResponse
    {
        public int PlayerId;
        public float Angle;

        protected override int SizeOf
        {
            get
            {
                return
                    BASE_SIZE +
                    sizeof (int) +
                    sizeof (float);
            }
        }

        protected override void DeserializeFromMemory(byte* bData, int pos)
        {
            Serializer.Read(bData, out PlayerId, ref pos);
            Serializer.Read(bData, out Angle, ref pos);
        }

        protected override void SerializeToMemory(byte* bData, int pos)
        {
            Serializer.Write(bData, PlayerId, ref pos);
            Serializer.Write(bData, Angle, ref pos);
        }

        public RPlayerRotate() { }

        public RPlayerRotate(int playerId, PPlayerRotate pPlayerRotate) 
            : base(PacketType.PlayerRotate)
        {
            PlayerId = playerId;
            Angle = pPlayerRotate.Angle;
        }
    }
}
