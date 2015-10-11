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

#region Private fields

        private Dictionary<long, GameMap> _maps;
        private Dictionary<long, Player> _players;

#endregion

#region Properties

        public GameMap FirstMap
        {
            get { return _maps.Values.FirstOrDefault(); }
        }

#endregion

#region Ctor

        public GameWorld()
        {
            _maps = new Dictionary<long, GameMap>();
            _players = new Dictionary<long, Player>();
        }

#endregion

#region !!!Temporary

        public void LoadMap()
        {
            foreach (GameMap map in _maps.Values)
            {
                map.Dispose();
            }
            _maps.Clear();
            if (File.Exists("d:\\RK.save"))
            {
                GameMap map = GameMap.LoadFromFile("d:\\RK.save");
                _maps.Add(map.Id, map);
            }
        }

#endregion

#region Players

        public bool PlayerAdd(Player player)
        {
            if (!_players.ContainsKey(player.Id))
            {
                _players.Add(player.Id, player);
                return true;
            }
            return false;
        }

        public bool PlayerChangeMap(long playerId, long mapId)
        {
            Player player;
            if (_players.TryGetValue(playerId, out player) && _maps.ContainsKey(mapId) &&
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
            lock (_maps)
            {
                if (_maps != null)
                {
                    foreach (GameMap map in _maps.Values)
                    {
                        map.Dispose();
                    }
                    _maps = null;
                }
            }
        }

#endregion

    }
}
