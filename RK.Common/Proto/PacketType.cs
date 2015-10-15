namespace RK.Common.Proto
{
    public enum PacketType : short
    {
        //0. User
        UserLogin,
        UserLogout,

        //100. Player
        PlayerEnter,
        PlayerMove
    }
}
