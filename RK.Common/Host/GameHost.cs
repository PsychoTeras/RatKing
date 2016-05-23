using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using IPCLogger.Core.Loggers;
using RK.Common.Classes.Common;
using RK.Common.Classes.Units;
using RK.Common.Classes.Users;
using RK.Common.Const;
using RK.Common.Host.Validators;
using RK.Common.Net.Server;
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

        private const int NET_PORT = 15051;
        private const int NET_MAX_ACCEPT_OPS = 10;
        private const int NET_MAX_CONNECTIONS = 3000;
        private const int NET_SOCKET_BACKLOG = 100;
        private const int NET_BUFFER_SIZE = 1024;

        private const int WORLD_DELAY_BETWEEN_FRAMES_MS = 30;

#endregion

#region Delegates

        private delegate BaseResponse OnAcceptPacket<in T>(int clientId, T packet) 
            where T: BasePacket;

        public delegate void OnGameHostResponse(BaseResponse e);

#endregion

#region Private fields

        private TCPServer _netServer;

        private Dictionary<PacketType, OnAcceptPacket<BasePacket>> _actions;
        private ValidatorList _validators;

        private Dictionary<int, int> _loggedPlayers; //SessionId   | UserId
        private Dictionary<int, int> _tcpClients;    //TCPClientId | SessionId
        private Dictionary<int, int> _playerClients; //UserId      | TCPClientId

        private Thread _threadSendResponses;
        private volatile bool _unsentResponsesAvailable;
        private Pool<Pair<int, BaseResponse>> _responsesPool;
        private List<Pair<int, BaseResponse>> _responses;

        private volatile bool _terminating;

        private VCheckPosition _vCheckPosition;

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

            TCPServerSettings setting = new TCPServerSettings(
                NET_MAX_CONNECTIONS, NET_SOCKET_BACKLOG, NET_MAX_ACCEPT_OPS, 
                NET_BUFFER_SIZE, NET_PORT);

            _netServer = new TCPServer(setting);
            _netServer.ClientConnected += TCPClientConnected;
            _netServer.ClientDisonnected += TCPClientDisonnected;
            _netServer.ClientDataReceived += TCPClientDataReceived;
            _netServer.ClientDataReceiveError += TCPClientDataReceiveError;
            _netServer.ClientDataSent += TCPClientDataSent;
            _netServer.ClientDataSendError += NetServerOnClientDataSendError;
            _netServer.Start();

            _loggedPlayers = new Dictionary<int, int>();
            _tcpClients = new Dictionary<int, int>();
            _playerClients = new Dictionary<int, int>();

            _responses = new List<Pair<int, BaseResponse>>();

            _actions = new Dictionary<PacketType, OnAcceptPacket<BasePacket>>
            {
                {PacketType.UserLogin, Login},
                {PacketType.UserEnter, Enter},
                {PacketType.UserLogout, Logout},

                {PacketType.PlayerRotate, PlayerRotate},
                {PacketType.PlayerMove, PlayerMove},

                {PacketType.MapData, MapData}
            };

            _validators = new ValidatorList
            {
                (_vCheckPosition = new VCheckPosition(this))
            };

            _responsesPool = new Pool<Pair<int, BaseResponse>>(1000, true);

            _threadSendResponses = new Thread(SendResponsesProc);
            _threadSendResponses.Priority = ThreadPriority.AboveNormal;
            _threadSendResponses.IsBackground = true;
            _threadSendResponses.Start();
        }

#endregion

#region Class methods

        private void ThrowSessionError(params object[] args)
        {
            BaseResponse.Throw("Invalid session", ECGeneral.SessionError);
        }

        private void AssertSession(BasePacket packet)
        {
            if (packet.Type != PacketType.UserLogin &&
                !_loggedPlayers.ContainsKey(packet.SessionToken))
            {
                ThrowSessionError(packet.Type, packet.SessionToken);
            }
        }

        private BaseResponse ProcessPacket(int clientId, BasePacket packet)
        {
            try
            {
                AssertSession(packet);
                return _actions[packet.Type](clientId, packet);
            }
            catch (Exception ex)
            {
                return BaseResponse.FromException(packet, ex);
            }
        }

        private void WriteLog(string msg)
        {
            LFactory.Instance.Write(msg);
        }

        private void WriteLog(LogEventType eventType, string msg)
        {
            LFactory.Instance.Write(eventType, msg);
        }

        public void Dispose()
        {
            _terminating = true;
            if (_netServer != null)
            {
                _netServer.Dispose();
            }
            _threadSendResponses.Join(100);
            World.Dispose();
            LFactory.Instance.Deinitialize();
        }

#endregion

#region World

        private void RegisterWorldResponseForNearest(Player player, BaseResponse response)
        {
            List<Player> nearestPlayers = World.PlayersGetNearest(player);
            int cnt = nearestPlayers.Count;
            if (cnt > 0)
            {
                lock (_responsesPool)
                {
                    for (int i = 0; i < cnt; i++)
                    {
                        Pair<int, BaseResponse> item = _responsesPool.PopExpandAsync();
                        item.Key = nearestPlayers[i].Id;
                        item.Value = response;
                        _responses.Add(item);
//                        _responses.Add(new Pair<int, BaseResponse>(nearestPlayers[i].Id, response));
                    }
                    _unsentResponsesAvailable = true;
                }
            }
        }

#endregion

#region User

        private BaseResponse Login(int clientId, BasePacket packet)
        {
            PUserLogin pUserLogin = (PUserLogin) packet;

            User user = new User(pUserLogin.UserName, pUserLogin.Password);
            pUserLogin.SessionToken = BasePacket.NewSessionToken();

            Player player = Player.Create(user.UserName);
            ShortPoint? startPoint = World.MapFindPlayerStartPoint(player);
            if (!startPoint.HasValue)
            {
                BaseResponse.Throw("Cannot get start point for the player", ECGeneral.ServerError);
                return null;
            }
            player.Position = startPoint.Value.ToPoint(ConstMap.PIXEL_SIZE);

            lock (_loggedPlayers)
            {
                _loggedPlayers.Add(pUserLogin.SessionToken, player.Id);
            }

            lock (_tcpClients)
            {
                _tcpClients.Add(clientId, pUserLogin.SessionToken);
                _playerClients.Add(player.Id, clientId);
            }

            World.PlayerAdd(pUserLogin.SessionToken, player);

            _validators.RegisterSession(pUserLogin.SessionToken);

            return new RUserLogin(pUserLogin)
            {
                SessionToken = pUserLogin.SessionToken
            };
        }

        private BaseResponse Enter(int clientId, BasePacket packet)
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

        private BaseResponse Logout(int clientId, BasePacket packet)
        {
            return Logout(clientId, packet.SessionToken);
        }

        private BaseResponse Logout(int clientId, int sessionToken)
        {
            lock (_loggedPlayers)
            {
                int playerId;
                if (_loggedPlayers.TryGetValue(sessionToken, out playerId))
                {
                    _loggedPlayers.Remove(sessionToken);

                    lock (_tcpClients)
                    {
                        _tcpClients.Remove(clientId);
                        _playerClients.Remove(playerId);
                    }

                    World.PlayerRemove(sessionToken);

                    _validators.UnregisterSession(sessionToken);

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

        private BaseResponse PlayerRotate(int clientId, BasePacket packet)
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

        private BaseResponse PlayerMove(int clientId, BasePacket packet)
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
                _vCheckPosition.Validate(packet);

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

        private BaseResponse MapData(int clientId, BasePacket packet)
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

        private void TCPClientConnected(int clientId)
        {
            WriteLog(string.Format("TCPClientConnected: {0}", clientId));
            WriteLog(LogEventType.TCPConnections, _netServer.NumberOfAcceptedSockets.ToString());
        }

        private void TCPClientDisonnected(int clientId)
        {
            WriteLog(string.Format("TCPClientDisonnected: {0}", clientId));

            int sessionToken;
            if (_tcpClients.TryGetValue(clientId, out sessionToken))
            {
                Logout(clientId, sessionToken);
            }
            WriteLog(LogEventType.TCPConnections, _netServer.NumberOfAcceptedSockets.ToString());
        }

        private void SendResponse(BaseResponse response, int clientId = 0)
        {
            if (response.Private || response.HasError)
            {
                _netServer.Send(clientId, response);
            }
            else
            {
                _netServer.SendAll(response);
            }
        }

        private void TCPClientDataReceived(int clientId, List<BasePacket> packets)
        {
            int cnt = packets.Count;
            for (int i = 0; i < cnt; i++)
            {
                BasePacket packet = packets[i];
                BaseResponse response = ProcessPacket(clientId, packet);
                if (response != null)
                {
                    SendResponse(response, clientId);
                }
            }
        }

        private void SendResponsesProc()
        {
            const float timeToCall = 1000/WORLD_DELAY_BETWEEN_FRAMES_MS - 1;

            HRTimer timer = new HRTimer();
            DateTime opTime = DateTime.UtcNow;
            while (!_terminating)
            {
                if (_unsentResponsesAvailable)
                {
                    timer.StartWatch();
                    Pair<int, BaseResponse>[] responses;
                    lock (_responsesPool)
                    {
                        responses = _responses.ToArray();
                        _responses.Clear();
                        _unsentResponsesAvailable = false;
                    }
                    int cnt = responses.Length;
                    Parallel.For(0, cnt, i =>
                    {
                        int clientId;
                        Pair<int, BaseResponse> response = responses[i];
                        if (_playerClients.TryGetValue(response.Key, out clientId))
                        {
                            _netServer.Send(clientId, response.Value);
                        }
                        _responsesPool.Push(response);
                    });
                    WriteLog(LogEventType.TCPResponsesSend, timer.StopWatch().ToString("F"));
                }

                DateTime curTime = DateTime.UtcNow;
                TimeSpan elapsed = curTime - opTime;
                int timeToIdle = (int) (timeToCall - elapsed.TotalMilliseconds);
                while (timeToIdle > 0)
                {
                    Thread.Sleep(timeToIdle/2);
                    elapsed = (curTime = DateTime.UtcNow) - opTime;
                    timeToIdle = (int) (timeToCall - elapsed.TotalMilliseconds);
                }

                elapsed = curTime - opTime;
                opTime = curTime;

                WriteLog(LogEventType.TCPResponsesProc, elapsed.TotalMilliseconds.ToString("F"));
            }
        }

        private void TCPClientDataReceiveError(int clientId, SocketError error)
        {
            WriteLog(string.Format("TCPClientDataReceiveError: {0}, {1}", clientId, error));
        }

        private void TCPClientDataSent(int clientId)
        {
        }

        private void NetServerOnClientDataSendError(int clientId, SocketError error)
        {
            WriteLog(string.Format("NetServerOnClientDataSendError: {0}, {1}", clientId, error));
        }

#endregion

    }
}