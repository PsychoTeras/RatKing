namespace RK.Common.Proto.User
{
    public sealed class RUserLogin : BaseResponse
    {
        public int SessionId;

        public RUserLogin(int sessionId)
        {
            SessionId = sessionId;
        }
    }
}
