namespace RK.Common.Proto.Packets
{
    public unsafe sealed class PPlayerRotate : BasePacket
    {
        public float Angle;

        public override PacketType Type
        {
            get { return PacketType.PlayerRotate; }
        }

        internal override void InitializeFromMemory(byte* bData)
        {
            Angle = *(float*)&bData[BASE_SIZE];
            base.InitializeFromMemory(bData);
        }

        public override byte[] Serialize()
        {
            const int pSize = BASE_SIZE + 4;
            byte[] data = new byte[pSize];
            fixed (byte* bData = data)
            {
                SerializeHeader(bData, pSize);
                (*(float*)&bData[BASE_SIZE]) = Angle;
            }
            return data;
        }
    }
}
