namespace RK.Common.Proto.Responses
{
    public unsafe sealed class RUserLogin : BaseResponse
    {
        public long SessionToken;

        internal override void InitializeFromMemory(byte* bData)
        {
            SessionToken = *(long*)&bData[BASE_SIZE];
            base.InitializeFromMemory(bData);
        }

        public override byte[] Serialize()
        {
            const int pSize = BASE_SIZE + 4;
            byte[] data = new byte[pSize];
            fixed (byte* bData = data)
            {
                SerializeHeader(bData, pSize);
                (*(long*)&bData[BASE_SIZE]) = SessionToken;
            }
            return data;
        }

        protected override BaseResponse Set(BasePacket p)
        {
            SessionToken = p.SessionToken;
            return base.Set(p);
        }
    }
}
