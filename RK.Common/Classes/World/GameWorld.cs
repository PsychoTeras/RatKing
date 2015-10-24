using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RK.Common.Algo;
using RK.Common.Classes.Map;
using RK.Common.Classes.Units;
using RK.Common.Common;
using RK.Common.Const;

namespace RK.Common.Classes.World
{
    public sealed class GameWorld : IDisposable
    {

#region Constants

        private const int MIN_FREE_AREA_SIZE = 500 * ConstMap.PIXEL_SIZE_SQR;
        private const int NEAREST_AREA_HALF_SIZE_SIZE = 100 * ConstMap.PIXEL_SIZE;

        private const float MAP_WINDOW_RES_COEF = 2f;

#endregion

#region Private fields

        private Dictionary<int, ServerMap> _maps;
        private Dictionary<long, Player> _players;

#endregion

#region Properties

        public ServerMap FirstMap
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
            _maps = new Dictionary<int, ServerMap>();
            _players = new Dictionary<long, Player>();
        }

#endregion

#region !!!Temporary

        public void LoadMap()
        {
            foreach (ServerMap map in _maps.Values)
            {
                map.Dispose();
            }
            _maps.Clear();
            if (File.Exists("RK.save"))
            {
                ServerMap map = ServerMap.LoadFromFile("RK.save");
                _maps.Add(map.Id, map);
            }
        }

#endregion
        
#region Class methods

        public void Dispose()
        {
            if (_maps != null)
            {
                lock (_maps)
                {
                    foreach (ServerMap map in _maps.Values)
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
            Player player;
            if (_players.TryGetValue(sessionToken, out player) &&
                _maps.ContainsKey(mapId) && player.MapId != mapId)
            {
                player.MapId = mapId;
                return true;
            }
            return false;
        }

        public Player PlayerGet(long sessionToken)
        {
            Player player;
            _players.TryGetValue(sessionToken, out player);
            return player;
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
                        players.Add(p);
                    }
                }
                return players;
            }
        }

#endregion

#region Map

        public ServerMap MapGet(int mapId)
        {
            ServerMap map;
            _maps.TryGetValue(mapId, out map);
            return map;
        }

        public ShortPoint? MapFindPlayerStartPoint(Player player)
        {
            ServerMap map;
            return _maps.TryGetValue(player.MapId, out map)
                ? Geometry.FindPlayerStartPoint(map, player, MIN_FREE_AREA_SIZE)
                : null;
        }

        public byte[] MapWindowGet(Player player, ShortSize screenRes, out ShortRect mapWindow)
        {
            ServerMap map;
            if (_maps.TryGetValue(player.MapId, out map))
            {
                const float resCoef = MAP_WINDOW_RES_COEF/ConstMap.PIXEL_SIZE;
                double width =
                    Math.Min(Math.Ceiling(screenRes.Width * resCoef), map.Width);
                double height =
                    (int)Math.Min(Math.Ceiling(screenRes.Height * resCoef), map.Height);
                int startX =
                    (int) Math.Max(Math.Floor((float) player.Position.X/ConstMap.PIXEL_SIZE) - width/2, 0);
                int startY =
                    (int) Math.Max(Math.Floor((float) player.Position.Y/ConstMap.PIXEL_SIZE) - height/2, 0);
                mapWindow = new ShortRect(startX, startY, (int) width, (int) height);
                return map.GetWindow(startX, startY, mapWindow.Width, mapWindow.Height);
            }
            mapWindow = ShortRect.Empty;
            return null;
        }

#endregion

    }
}
