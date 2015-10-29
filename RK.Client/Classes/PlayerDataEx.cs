using RK.Common.Classes.Units;

namespace RK.Client.Classes
{
    public sealed class PlayerDataEx : PlayerData
    {
        public bool GettingMapWindow;
        public bool NeedUpdatePosition;

        public PlayerDataEx(Player player) 
            : base(player) { }
    }
}
