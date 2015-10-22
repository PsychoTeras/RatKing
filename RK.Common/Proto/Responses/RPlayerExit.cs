using RK.Common.Win32;

namespace RK.Common.Proto.Responses
{
    public unsafe sealed class RPlayerExit : BaseResponse
    {
        public int PlayerId;

        protected override int SizeOf
        {
            get
            {
                return
                    BASE_SIZE +
                    sizeof(int);
            }
        }

        protected override void DeserializeFromMemory(byte* bData, int pos)
        {
            Serializer.Read(bData, out PlayerId, ref pos);
        }

        protected override void SerializeToMemory(byte* bData, int pos)
        {
            Serializer.Write(bData, PlayerId, ref pos);
        }

        public RPlayerExit() { }

        public RPlayerExit(int playerId) : base(PacketType.PlayerExit)
        {
            PlayerId = playerId;
        }
    }
}
