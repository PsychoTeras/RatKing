using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IPCLogger.Core.Loggers;
using RK.Common.Classes;
using RK.Common.Classes.Common;
using RK.Common.Classes.Units;
using RK.Common.Classes.Users;
using RK.Common.Const;
using RK.Common.Host.Validators;
using RK.Common.Net.TCP;
using RK.Common.Proto;
using RK.Common.Proto.ErrorCodes;
using RK.Common.Proto.Packets;
using RK.Common.Proto.Responses;
using RK.Common.Win32;
using RK.Common.World;

namespace RK.Common.Host
{
    public sealed class GameHost : IDisposable
    {

#region Constants

        private const int WORLD_DELAY_BETWEEN_FRAMES_MS = 30;

#endregion

#region Delegates

        private delegate BaseResponse OnAcceptPacket<in T>(TCPClientEx tcpClient, T packet) 
            where T: BasePacket;

        public delegate void OnGameHostResponse(BaseResponse e);

#endregion

#region Private fields

//        private static LFactory
        
        private TCPServer _netServer;

        private Dictionary<PacketType, OnAcceptPacket<BasePacket>> _actions;
        private List<BaseValidator> _validators;

        private Dictionary<long, int> _loggedPlayers;
        private Dictionary<TCPClientEx, long> _tcpClients;
        private Dictionary<int, TCPClientEx> _playerClients;

        private Thread _threadWorldProcessor;
        private bool _worldHasUnsentResponses;
        private HashSet<int> _playersThatHaveWorldResponses;
        private Dictionary<int, List<BaseResponse>> _playersWorldResponses;

        private volatile bool _terminating;

#endregion

#region Public fields

        internal GameWorld World;

#endregion

#region Ctor

        static GameHost()
        {
            LFactory.Instance.Setup("RK.Server.config");
        }

        public GameHost()
        {
            World = new GameWorld();
            World.LoadMap();

            _netServer = new TCPServer(15051);
            _netServer.ClientDataReceived += TCPClientDataReceived;
            _netServer.ClientDisonnected += TCPClientDisonnected;
            _netServer.Start();

            _loggedPlayers = new Dictionary<long, int>();
            _tcpClients = new Dictionary<TCPClientEx, long>();
            _playerClients = new Dictionary<int, TCPClientEx>();
            
            _playersThatHaveWorldResponses = new HashSet<int>();
            _playersWorldResponses = new Dictionary<int, List<BaseResponse>>();

            _actions = new Dictionary<PacketType, OnAcceptPacket<BasePacket>>
            {
                {PacketType.UserLogin, Login},
                {PacketType.UserEnter, Enter},
                {PacketType.UserLogout, Logout},

                {PacketType.PlayerRotate, PlayerRotate},
                {PacketType.PlayerMove, PlayerMove},

                {PacketType.MapData, MapData}
            };

            _validators = new List<BaseValidator>
            {
                new VCheckPosition(this)
            };

            _threadWorldProcessor = new Thread(SendWorldResponsesProc);
            _threadWorldProcessor.IsBackground = true;
            _threadWorldProcessor.Start();
        }

#endregion

#region Class methods

        private void ThrowSessionError(params object[] args)
        {
            BaseResponse.Throw("Invalid session", ECGeneral.SessionError);
        }

        private void AssertSession(BasePacket packet)
        {
            if (packet.Type != PacketType.UserLogin)
            {
                if (!_loggedPlayers.ContainsKey(packet.SessionToken))
                {
                    ThrowSessionError(packet.Type, packet.SessionToken);
                }
            }
        }

        private BaseResponse ProcessPacket(TCPClientEx tcpClient, BasePacket packet)
        {
            try
            {
                AssertSession(packet);
                return _actions[packet.Type](tcpClient, packet);
            }
            catch (Exception ex)
            {
                return BaseResponse.FromException(packet, ex);
            }
        }

        private void WriteLog(LogEventType eventType, string msg)
        {
            msg = string.Format("{0}\x4{1}", eventType, msg);
            LFactory.Instance.Write(msg);
        }

        public void Dispose()
        {
            _terminating = true;
            if (_netServer != null)
            {
                _netServer.Dispose();
            }
            _threadWorldProcessor.Join(100);
            World.Dispose();
            LFactory.Instance.Deinitialize();
        }

#endregion

#region World

        private void RegisterWorldResponseForNearest(Player player, BaseResponse response)
        {
            List<Player> nearestPlayers = World.PlayersGetNearest(player);
            if (nearestPlayers.Count > 0)
            {
                lock (_playersWorldResponses)
                {
                    foreach (Player nearest in nearestPlayers)
                    {
                        if (!_playersThatHaveWorldResponses.Contains(nearest.Id))
                            _playersThatHaveWorldResponses.Add(nearest.Id);
                        _playersWorldResponses[nearest.Id].Add(response);
                    }
                }
                _worldHasUnsentResponses = true;
            }
        }

#endregion

#region User

        private BaseResponse Login(TCPClientEx tcpClient, BasePacket packet)
        {
            PUserLogin pUserLogin = (PUserLogin) packet;

            User user = new User(pUserLogin.UserName, pUserLogin.Password);
            pUserLogin.SessionToken = BasePacket.NewSessionToken(user.Id);

            Player player = Player.Create(user.UserName);
            ShortPoint? startPoint = World.MapFindPlayerStartPoint(player);
            if (!startPoint.HasValue)
            {
                BaseResponse.Throw("Cannot get start point for player", ECGeneral.ServerError);
                return null;
            }
            player.Position = startPoint.Value.ToPoint(ConstMap.PIXEL_SIZE);

            World.PlayerAdd(pUserLogin.SessionToken, player);

            lock (_playersWorldResponses)
            {
                _playersWorldResponses.Add(player.Id, new List<BaseResponse>());
            }

            lock (_loggedPlayers)
            {
                _loggedPlayers.Add(pUserLogin.SessionToken, player.Id);
            }

            lock (_tcpClients)
            {
                _tcpClients.Add(tcpClient, pUserLogin.SessionToken);
                _playerClients.Add(player.Id, tcpClient);
            }

            return new RUserLogin(pUserLogin)
            {
                SessionToken = pUserLogin.SessionToken
            };
        }

        private BaseResponse Enter(TCPClientEx tcpClient, BasePacket packet)
        {
            PUserEnter pUserEnter = (PUserEnter) packet;

            PlayerDataEx playerData = World.PlayerDataGet(pUserEnter.SessionToken);
            if (playerData == null)
            {
                ThrowSessionError(packet.Type, packet.SessionToken);
                return null;
            }

            int maxScreenSide = Math.Max(pUserEnter.ScreenRes.Width, pUserEnter.ScreenRes.Height);
            playerData.ScreenRes = new ShortSize(maxScreenSide, maxScreenSide);

            List<Player> nearestPlayers = World.PlayersGetNearest(playerData);

            ShortRect mapWindow;
            byte[] mapData = World.MapWindowGet(playerData, out mapWindow);
            ShortSize mapSize = new ShortSize(playerData.Map.Width, playerData.Map.Height);

            ShortSize miniMapSize;
            byte[] miniMapData = World.MiniMapGet(playerData, out miniMapSize);

            RegisterWorldResponseForNearest(playerData, new RPlayerEnter
            {
                Player = playerData
            });

            return new RUserEnter(pUserEnter)
            {
                MyPlayerId = playerData.Player.Id,
                PlayersOnLocation = nearestPlayers,

                MapSize = mapSize,
                MapData = mapData,
                MapWindow = mapWindow,

                MiniMapData = miniMapData,
                MiniMapSize = miniMapSize
            };
        }

        private BaseResponse Logout(TCPClientEx tcpClient, BasePacket packet)
        {
            return Logout(tcpClient, packet.SessionToken);
        }

        private BaseResponse Logout(TCPClientEx tcpClient, long sessionToken)
        {
            lock (_loggedPlayers)
            {
                int playerId;
                if (_loggedPlayers.TryGetValue(sessionToken, out playerId))
                {
                    _loggedPlayers.Remove(sessionToken);

                    lock (_tcpClients)
                    {
                        _tcpClients.Remove(tcpClient);
                        _playerClients.Remove(playerId);
                    }

                    lock (_playersWorldResponses)
                    {
                        _playersWorldResponses.Remove(playerId);
                    }

                    World.PlayerRemove(sessionToken);

                    SendResponse(new RPlayerExit
                    {
                        PlayerId = playerId
                    });
                }
            }
            return null;
        }

#endregion

#region Player

        private BaseResponse PlayerRotate(TCPClientEx tcpClient, BasePacket packet)
        {
            PPlayerRotate pPlayerRotate = (PPlayerRotate)packet;
            PlayerDataEx playerData = World.PlayerDataGet(pPlayerRotate.SessionToken);
            if (playerData == null)
            {
                ThrowSessionError(packet.Type, packet.SessionToken);
                return null;
            }

            if (playerData.Player.Angle != pPlayerRotate.Angle)
            {
                playerData.Player.Angle = pPlayerRotate.Angle;

                RegisterWorldResponseForNearest(playerData, new RPlayerRotate
                {
                    PlayerId = playerData.Player.Id,
                    Angle = pPlayerRotate.Angle
                });
            }

            return null;
        }

        private BaseResponse PlayerMove(TCPClientEx tcpClient, BasePacket packet)
        {
            PPlayerMove pPlayerMove = (PPlayerMove)packet;
            PlayerDataEx playerData = World.PlayerDataGet(pPlayerMove.SessionToken);
            if (playerData == null)
            {
                ThrowSessionError(packet.Type, packet.SessionToken);
                return null;
            }

            if (playerData.Player.Position != pPlayerMove.Position ||
                playerData.Player.Direction != pPlayerMove.Direction)
            {
                if (pPlayerMove.Direction == Direction.None)
                {
                    playerData.StopMoving(pPlayerMove.Position);
                }
                else
                {
                    playerData.StartMoving(pPlayerMove.Position, pPlayerMove.Direction);
                }

                RegisterWorldResponseForNearest(playerData, new RPlayerMove
                {
                    PlayerId = playerData.Player.Id,
                    Position = pPlayerMove.Position,
                    Direction = pPlayerMove.Direction,
                });
            }

            return null;
        }

#endregion

#region Map

        private BaseResponse MapData(TCPClientEx tcpclient, BasePacket packet)
        {
            PMapData pMapData = (PMapData) packet;

            PlayerDataEx playerData = World.PlayerDataGet(pMapData.SessionToken);
            if (playerData == null)
            {
                ThrowSessionError(packet.Type, packet.SessionToken);
                return null;
            }

            ShortRect mapWindow;
            byte[] mapData = World.MapWindowGet(playerData, out mapWindow);

            return new RMapData(pMapData)
            {
                MapData = mapData,
                MapWindow =  mapWindow
            };
        }

#endregion

#region TCP

        private void TCPClientDisonnected(TCPClientEx tcpClient)
        {
            long sessionToken;
            if (_tcpClients.TryGetValue(tcpClient, out sessionToken))
            {
                Logout(tcpClient, sessionToken);
            }
        }

        private void SendResponse(BaseResponse response, TCPClientEx tcpClient = null)
        {
            if (response.Private || response.HasError)
            {
                _netServer.SendData(tcpClient, response);
            }
            else
            {
                _netServer.SendAll(response);
            }
        }

        private void TCPClientDataReceived(TCPClientEx tcpClient, IList<BasePacket> packets)
        {
            Parallel.For(0, packets.Count, i =>
            {
                BasePacket packet = packets[i];
                BaseResponse response = ProcessPacket(tcpClient, packet);
                if (response != null)
                {
                    SendResponse(response, tcpClient);
                }
            });
        }

        private void SendWorldResponsesProc()
        {
            HRTimer timer = new HRTimer();
            int timeToCall = 1000/WORLD_DELAY_BETWEEN_FRAMES_MS;
            int lostElapsedChunk = 0, needsToWait = timeToCall;
            DateTime? lastFrameRenderTime = null;

            while (!_terminating)
            {
                Thread.Sleep(needsToWait - lostElapsedChunk);
                try
                {
                    if (lastFrameRenderTime != null)
                    {
                        TimeSpan elapsed = DateTime.UtcNow - lastFrameRenderTime.Value;
                        lostElapsedChunk = (int) (elapsed.TotalMilliseconds%timeToCall);
                    }

                    if (!_worldHasUnsentResponses)
                    {
                        lastFrameRenderTime = DateTime.UtcNow;
                        continue;
                    }

                    timer.StartWatch();

                    lock (_playersWorldResponses)
                    {
                        KeyValuePair<int, List<BaseResponse>>[] responses = _playersWorldResponses.
                            Where(r => _playersThatHaveWorldResponses.Contains(r.Key)).
                            ToArray();
                        int responsesCnt = responses.Length;
                        Parallel.For(0, responsesCnt, i =>
                        {
                            TCPClientEx tcpClient;
                            var response = responses[i];
                            if (_playerClients.TryGetValue(response.Key, out tcpClient))
                            {
                                _netServer.SendData(tcpClient, response.Value);
                            }
                            response.Value.Clear();
                        });
                        _playersThatHaveWorldResponses.Clear();
                        _worldHasUnsentResponses = false;
                    }

                    WriteLog(LogEventType.SendWorldResponses, timer.StopWatch().ToString("F"));

                    lastFrameRenderTime = DateTime.UtcNow;
                }
                catch
                {
                    Thread.Sleep(0);
                }
            }
        }

#endregion

    }
}
