using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RK.Common.Algo;
using RK.Common.Classes.Common;
using RK.Common.Classes.Map;
using RK.Common.Classes.Units;
using RK.Common.Const;

namespace RK.Common.Classes.World
{
    public sealed class GameWorld : IDisposable
    {

#region Constants

        private const int MIN_FREE_AREA_SIZE = 100 * ConstMap.PIXEL_SIZE_SQR;
        private const int NEAREST_AREA_HALF_SIZE_SIZE = 100 * ConstMap.PIXEL_SIZE;

#endregion

#region Private fields

        private Dictionary<int, GameMap> _maps;
        private Dictionary<long, Player> _players;

#endregion

#region Properties

        public GameMap FirstMap
        {
            get { return _maps.Values.FirstOrDefault(); }
        }

        public Player FirstPlayer
        {
            get { return _players.Values.FirstOrDefault(); }
        }

#endregion

#region Ctor

        public GameWorld()
        {
            _maps = new Dictionary<int, GameMap>();
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

#region Player

        public bool PlayerAdd(long sessionToken, Player player)
        {
            lock (_players)
            {
                if (!_players.ContainsKey(sessionToken))
                {
                    _players.Add(sessionToken, player);
                    return true;
                }
                return false;
            }
        }

        public bool PlayerRemove(long sessionToken)
        {
            lock (_players)
            {
                if (_players.ContainsKey(sessionToken))
                {
                    _players.Remove(sessionToken);
                    return true;
                }
                return false;
            }
        }

        public bool PlayerChangeMap(long sessionToken, int mapId)
        {
            lock (_players) lock (_maps)
            {
                Player player;
                if (_players.TryGetValue(sessionToken, out player) && 
                    _maps.ContainsKey(mapId) &&
                    player.MapId != mapId)
                {
                    player.MapId = mapId;
                    return true;
                }
                return false;
            }
        }

        public Player PlayerGet(long sessionToken)
        {
            lock (_players)
            {
                Player player;
                _players.TryGetValue(sessionToken, out player);
                return player;
            }
        }

        public List<Player> PlayersGetNearest(Player player)
        {
            lock (_players)
            {
                List<Player> players = new List<Player>();
                foreach (Player p in _players.Values)
                {
                    if (p.MapId == player.MapId &&
                        p.Position.CloseTo(player.Position, NEAREST_AREA_HALF_SIZE_SIZE))
                    {
                        players.Add(player);
                    }
                }
                return players;
            }
        }

#endregion

#region Map

        public GameMap MapGet(int mapId)
        {
            lock (_maps)
            {
                GameMap map;
                _maps.TryGetValue(mapId, out map);
                return map;
            }
        }

        public ShortPoint? MapFindPlayerStartPoint(Player player)
        {
            GameMap map;
            return _maps.TryGetValue(player.MapId, out map)
                ? Geometry.FindPlayerStartPoint(map.SpaceAreas, player, MIN_FREE_AREA_SIZE)
                : null;
        }

#endregion

    }
}
