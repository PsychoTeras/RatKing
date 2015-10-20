namespace RK.Common.Proto
{
    public enum PacketType : short
    {
        //Error
        Error = -1,

        //0. User
        UserLogin = 0,
        UserLogout,

        //100. Player
        PlayerEnter = 100,
        PlayerMove,
        PlayerRotate
    }
}
