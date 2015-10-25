namespace RK.Common.Proto
{
    public enum PacketType : short
    {
        //Error
        Error = -1,

        //0. User
        UserLogin = 0,
        UserEnter,
        UserLogout,

        //100. Player
        PlayerEnter = 100,
        PlayerExit,
        PlayerMove,
        PlayerRotate,

        //200. Map
        MapData = 200,
    }
}
