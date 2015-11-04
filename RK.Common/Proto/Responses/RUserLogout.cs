namespace RK.Common.Proto.Responses
{
    public sealed class RUserLogout : BaseResponse
    {
        public override PacketType Type
        {
            get { return PacketType.UserLogout; }
        }
    }
}
