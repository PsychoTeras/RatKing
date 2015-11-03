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
using Direction = RK.Common.Classes.Common.Direction;

namespace RK.Common.World
{
    internal sealed class GameWorld : IDisposable
    {

#region Constants

        private const int MIN_FREE_AREA_SIZE = 500 * ConstMap.PIXEL_SIZE_SQR;
        private const int NEAREST_AREA_HALF_SIZE = 100 * ConstMap.PIXEL_SIZE;

        private const float MAP_WINDOW_RES_COEF = 1.5f;

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
                int playersCnt = _players.Count;
                for (int i = 0; i < playersCnt; i++)
                {
                    PlayerDataEx p = _players[i];
                    if (p.Player.MapId == player.MapId &&
                        p.Player.Position.CloseTo(player.Position, NEAREST_AREA_HALF_SIZE))
                    {
                        players.Add(p.Player);
                    }
                }
                return players;
            }
        }

#endregion

#region Map & Minimap

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
                ? Game.FindPlayerStartPoint(map, player, MIN_FREE_AREA_SIZE)
                : null;
        }

        public byte[] MapWindowGet(PlayerDataEx playerData, out ShortRect mapWindow)
        {
            const float resCoef = MAP_WINDOW_RES_COEF/ConstMap.PIXEL_SIZE;

            float dStartX, dStartY;
            float dWidth = playerData.ScreenRes.Width*resCoef;
            float dHeight = playerData.ScreenRes.Height*resCoef;
            ShortPoint pos = playerData.Player.Position.ToShortPoint(ConstMap.PIXEL_SIZE);
            switch (playerData.Player.Direction)
            {
                case Direction.N:
                    dStartX = pos.X - dWidth/2;
                    dStartY = pos.Y - dHeight;
                    break;
                case Direction.S:
                    dStartX = pos.X - dWidth/2;
                    dStartY = pos.Y;
                    break;
                case Direction.W:
                    dStartX = pos.X - dWidth;
                    dStartY = pos.Y - dHeight/2;
                    break;
                case Direction.E:
                    dStartX = pos.X;
                    dStartY = pos.Y - dHeight/2;
                    break;
                case Direction.NW:
                    dStartX = pos.X - dWidth/1.1f;
                    dStartY = pos.Y - dHeight/1.1f;
                    break;
                case Direction.NE:
                    dStartX = pos.X - dWidth*0.1f;
                    dStartY = pos.Y - dHeight/1.1f;
                    break;
                case Direction.SW:
                    dStartX = pos.X - dWidth/1.1f;
                    dStartY = pos.Y - dHeight*0.1f;
                    break;
                case Direction.SE:
                    dStartX = pos.X - dWidth*0.1f;
                    dStartY = pos.Y - dHeight*0.1f;
                    break;
                default:
                    dStartX = pos.X - dWidth/2;
                    dStartY = pos.Y - dHeight/2;
                    break;
            }

            int mapWidth = playerData.Map.Width, mapHeight = playerData.Map.Height;

            ushort startX = (ushort) Math.Max(dStartX, 0);
            ushort startY = (ushort) Math.Max(dStartY, 0);
            ushort width = (ushort) Math.Floor(startX + dWidth > mapWidth
                ? dWidth - (startX + dWidth - mapWidth)
                : dWidth);
            ushort height = (ushort) Math.Floor(startY + dHeight > mapHeight
                ? dHeight - (startY + dHeight - mapHeight)
                : dHeight);

            mapWindow = new ShortRect(startX, startY, width, height);
            return playerData.Map.GetWindow(mapWindow.X, mapWindow.Y, mapWindow.Width, mapWindow.Height);
        }

        public byte[] MiniMapGet(PlayerDataEx playerData, out ShortSize miniMapSize)
        {
            miniMapSize = playerData.Map.MiniMapSize;
            return playerData.Map.GetMiniMap();
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
