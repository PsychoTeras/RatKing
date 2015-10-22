using RK.Common.Proto.Packets;
using RK.Common.Win32;

namespace RK.Common.Proto.Responses
{
    public unsafe sealed class RUserLogin : BaseResponse
    {
        public long SessionToken;

        protected override int SizeOf
        {
            get
            {
                return
                    BASE_SIZE +
                    sizeof(long);
            }
        }

        protected override void DeserializeFromMemory(byte* bData, int pos)
        {
            Serializer.Read(bData, out SessionToken, ref pos);
        }

        protected override void SerializeToMemory(byte* bData, int pos)
        {
            Serializer.Write(bData, SessionToken, ref pos);
        }

        public RUserLogin() { }

        public RUserLogin(PUserLogin rUserLogin) 
            : base(rUserLogin)
        {
            Private = true;
            SessionToken = rUserLogin.SessionToken;
        }
    }
}
