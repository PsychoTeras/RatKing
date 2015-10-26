using RK.Common.Classes.Units;

namespace RK.Win.Classes
{
    internal sealed class PlayerDataEx : PlayerData
    {
        public bool NeedUpdatePosition;

        public PlayerDataEx(Player player) 
            : base(player) { }
    }
}
