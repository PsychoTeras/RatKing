using System;
using System.Collections.Generic;
using RK.Common.Classes.Common;
using RK.Common.Map;

namespace RK.Common.Classes.Units
{
    internal sealed class PlayerDataEx : PlayerData
    {
        public ServerMap Map;
        public ShortSize ScreenRes;

        public PlayerDataEx(Player player, Dictionary<int, ServerMap> maps)
            : base(player)
        {
            if (!maps.TryGetValue(player.MapId, out Map))
            {
                throw new Exception(string.Format("Player [{0}] has a wrong map ID", player.Id));
            }
        }
    }
}
