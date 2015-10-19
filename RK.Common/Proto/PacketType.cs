namespace RK.Common.Proto
{
    public enum PacketType : short
    {
        //Generic
        Generic = -1,

        //0. User
        UserLogin,
        UserLogout,

        //100. Player
        PlayerEnter,
        PlayerMove,
        PlayerRotate
    }
}
