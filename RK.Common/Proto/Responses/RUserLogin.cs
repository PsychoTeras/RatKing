namespace RK.Common.Proto.Responses
{
    public sealed class RUserLogin : BaseResponse
    {
        public long SessionToken;

        public RUserLogin(long sessionToken) 
            : base(PacketType.UserLogin)
        {
            SessionToken = sessionToken;
        }
    }
}
