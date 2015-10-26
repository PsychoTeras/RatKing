using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RK.Common.Algo;
using RK.Common.Classes.Common;
using RK.Common.Classes.Units;
using RK.Common.Const;
using RK.Common.Map;

namespace RK.Common.World
{
    public sealed class GameWorld : IDisposable
    {

#region Constants

        private const int MIN_FREE_AREA_SIZE = 500 * ConstMap.PIXEL_SIZE_SQR;
        private const int NEAREST_AREA_HALF_SIZE_SIZE = 100 * ConstMap.PIXEL_SIZE;

        private const float MAP_WINDOW_RES_COEF = 100f;

#endregion

#region Private fields

        private Thread _worldEngine;

        private Dictionary<int, ServerMap> _maps;

        private List<PlayerDataEx> _players;
        private Dictionary<long, PlayerDataEx> _sessionPlayers;

        private volatile bool _disposing;

#endregion

#region Ctor

        public GameWorld()
        {
            _maps = new Dictionary<int, ServerMap>();

            _players = new List<PlayerDataEx>();
            _sessionPlayers = new Dictionary<long, PlayerDataEx>();
            
            _worldEngine = new Thread(WorldEngineProc);
            _worldEngine.IsBackground = true;
            _worldEngine.Start();
        }

#endregion

#region !!!Temporary

        public ServerMap FirstMap
        {
            get { return _maps.Values.FirstOrDefault(); }
        }

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
                _disposing = true;

                foreach (ServerMap map in _maps.Values)
                {
                    map.Dispose();
                }
                _maps = null;

                _worldEngine.Abort();
                _worldEngine.Join(1000);
            }
        }

#endregion

#region Player

        public bool PlayerAdd(long sessionToken, Player player)
        {
            if (!_sessionPlayers.ContainsKey(sessionToken))
            {
                PlayerDataEx playerData = new PlayerDataEx(player, _maps);
                lock (_sessionPlayers) lock (_players)
                {
                    _players.Add(playerData);
                    _sessionPlayers.Add(sessionToken, playerData);
                    return true;
                }
            }
            return false;
        }

        public bool PlayerRemove(long sessionToken)
        {
            PlayerDataEx playerData;
            if (_sessionPlayers.TryGetValue(sessionToken, out playerData))
            {
                lock (_sessionPlayers) lock (_players)
                {
                    _players.Remove(playerData);
                    _sessionPlayers.Remove(sessionToken);
                    return true;
                }
            }
            return false;
        }

        public bool PlayerChangeMap(long sessionToken, int mapId)
        {
            ServerMap map;
            PlayerDataEx playerData;
            if (_sessionPlayers.TryGetValue(sessionToken, out playerData) &&
                playerData.Player.MapId != mapId &&
                _maps.TryGetValue(mapId, out map))
            {
                playerData.Map = map;
                playerData.Player.MapId = mapId;
                return true;
            }
            return false;
        }

        internal PlayerDataEx PlayerDataGet(long sessionToken)
        {
            PlayerDataEx playerData;
            return _sessionPlayers.TryGetValue(sessionToken, out playerData)
                ? playerData
                : null;
        }

        public Player PlayerGet(long sessionToken)
        {
            PlayerDataEx playerData;
            return _sessionPlayers.TryGetValue(sessionToken, out playerData)
                ? playerData.Player
                : null;
        }

        public List<Player> PlayersGetNearest(Player player)
        {
            lock (_players)
            {
                List<Player> players = new List<Player>();
                foreach (PlayerDataEx p in _players)
                {
                    if (p.Player.MapId == player.MapId &&
                        p.Player.Position.CloseTo(player.Position, NEAREST_AREA_HALF_SIZE_SIZE))
                    {
                        players.Add(p.Player);
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

                int mapWidth = map.Width, mapHeight = map.Height;

                float dWidth = screenRes.Width*resCoef;
                float dHeight = screenRes.Height * resCoef;
                float dStartX = ((float)player.Position.X / ConstMap.PIXEL_SIZE) - dWidth / 2;
                float dStartY = ((float)player.Position.Y / ConstMap.PIXEL_SIZE) - dHeight / 2;

                ushort startX = (ushort) Math.Max(dStartX, 0);
                ushort startY = (ushort) Math.Max(dStartY, 0);
                ushort width = (ushort)Math.Ceiling(startX + dWidth > mapWidth
                    ? dWidth - (startX + dWidth - mapWidth)
                    : dWidth);
                ushort height = (ushort)Math.Ceiling(startY + dHeight > mapHeight
                    ? dHeight - (startY + dHeight - mapHeight)
                    : dHeight);

                mapWindow = new ShortRect(startX, startY, width, height);
                return map.GetWindow(mapWindow.X, mapWindow.Y, mapWindow.Width, mapWindow.Height);
            }
            mapWindow = ShortRect.Empty;
            return null;
        }

#endregion

#region World engine

        private void EngineCheckPlayerPosition(PlayerDataEx playerData)
        {
            Engine.ValidateNewPlayerPosition(playerData, playerData.Map);
        }

        private void WorldEngineProc()
        {
            while (Thread.CurrentThread.IsAlive && !_disposing)
            {
                lock (_players)
                {
                    Parallel.For(0, _players.Count, i =>
                    {
                        PlayerDataEx playerData = _players[i];
                        EngineCheckPlayerPosition(playerData);
                    });
                }
                Thread.Sleep(1);
            }
        }

#endregion

    }
}
