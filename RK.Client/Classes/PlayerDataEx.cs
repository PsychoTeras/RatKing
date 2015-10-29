using RK.Common.Classes.Units;

namespace RK.Win.Classes
{
    public sealed class PlayerDataEx : PlayerData
    {
        public bool GettingMapWindow;
        public bool NeedUpdatePosition;

        public PlayerDataEx(Player player) 
            : base(player) { }
    }
}
