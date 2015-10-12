using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RK.Common.Classes.Map;
using RK.Common.Classes.Units;

namespace RK.Common.Classes.World
{
    public sealed class GameWorld : IDisposable
    {

#region Public fields

        public Dictionary<long, GameMap> Maps;
        public Dictionary<long, Player> Players;

#endregion

#region Properties

        public GameMap FirstMap
        {
            get { return Maps.Values.FirstOrDefault(); }
        }

#endregion

#region Ctor

        public GameWorld()
        {
            Maps = new Dictionary<long, GameMap>();
            Players = new Dictionary<long, Player>();
        }

#endregion

#region !!!Temporary

        public void LoadMap()
        {
            foreach (GameMap map in Maps.Values)
            {
                map.Dispose();
            }
            Maps.Clear();
            if (File.Exists("d:\\RK.save"))
            {
                GameMap map = GameMap.LoadFromFile("d:\\RK.save");
                Maps.Add(map.Id, map);
            }
        }

#endregion

#region Players

        public bool PlayerAdd(Player player)
        {
            if (!Players.ContainsKey(player.Id))
            {
                Players.Add(player.Id, player);
                return true;
            }
            return false;
        }

        public bool PlayerChangeMap(long playerId, long mapId)
        {
            Player player;
            if (Players.TryGetValue(playerId, out player) && Maps.ContainsKey(mapId) &&
                player.MapId != mapId)
            {
                player.MapId = mapId;
                return true;
            }
            return false;
        }

#endregion

#region Class methods

        public void Dispose()
        {
            lock (Maps)
            {
                if (Maps != null)
                {
                    foreach (GameMap map in Maps.Values)
                    {
                        map.Dispose();
                    }
                    Maps = null;
                }
            }
        }

#endregion

    }
}
