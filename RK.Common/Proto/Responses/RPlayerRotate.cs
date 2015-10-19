using RK.Common.Proto.Packets;

namespace RK.Common.Proto.Responses
{
    public unsafe sealed class RPlayerRotate : BaseResponse
    {
        public int PlayerId;
        public float Angle;

        internal override void InitializeFromMemory(byte* bData)
        {
            Angle = *(float*)&bData[BASE_SIZE];
            PlayerId = *(int*)&bData[BASE_SIZE + 4];
            base.InitializeFromMemory(bData);
        }

        public override byte[] Serialize()
        {
            const int pSize = BASE_SIZE + 8;
            byte[] data = new byte[pSize];
            fixed (byte* bData = data)
            {
                SerializeHeader(bData, pSize);
                (*(float*)&bData[BASE_SIZE]) = Angle;
                (*(int*)&bData[BASE_SIZE + 4]) = PlayerId;
            }
            return data;
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
