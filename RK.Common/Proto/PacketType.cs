namespace RK.Common.Proto
{
    public enum PacketType : short
    {
        //Error
        Error = -1,

        //0. Test
        TestXkb = 0,

        //1. User
        UserLogin = 1,
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
