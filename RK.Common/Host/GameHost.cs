﻿using System;using System.Collections.Generic;using System.Threading;using System.Threading.Tasks;using IPCLogger.Core.Loggers;using RK.Common.Classes.Common;using RK.Common.Classes.Units;using RK.Common.Classes.Users;using RK.Common.Const;using RK.Common.Host.Validators;using RK.Common.Net.TCP2.Server;using RK.Common.Proto;using RK.Common.Proto.ErrorCodes;using RK.Common.Proto.Packets;using RK.Common.Proto.Responses;using RK.Common.Win32;using RK.Common.World;namespace RK.Common.Host{    public sealed class GameHost : IDisposable    {#region Constants        private const int NET_PORT= 15051;        private const int NET_MAX_ACCEPT_OPS = 3000;        private const int NET_MAX_CONNECTIONS = 3000;        private const int NET_SOCKET_BACKLOG = 100;        private const int NET_BUFFER_SIZE = 1024;        private const int WORLD_DELAY_BETWEEN_FRAMES_MS = 30;#endregion#region Delegates        private delegate BaseResponse OnAcceptPacket<in T>(int clientId, T packet)             where T: BasePacket;        public delegate void OnGameHostResponse(BaseResponse e);#endregion#region Private fields        private TCPServer2 _netServer;        private Dictionary<PacketType, OnAcceptPacket<BasePacket>> _actions;        private List<BaseValidator> _validators;        private Dictionary<int, int> _loggedPlayers; //SessionId   | UserId        private Dictionary<int, int> _tcpClients;    //TCPClientId | SessionId        private Dictionary<int, int> _playerClients; //UserId      | TCPClientId        private Thread _threadSendResponses;        private bool _unsentResponsesAvailable;        private List<Pair<int, BaseResponse>> _responses;        private volatile bool _terminating;#endregion#region Public fields        internal GameWorld World;#endregion#region Ctor        static GameHost()        {            LFactory.Instance.Setup("RK.Server.config");        }        public GameHost()        {            World = new GameWorld();            World.LoadMap();            TCPServerSettings setting = new TCPServerSettings(                NET_MAX_CONNECTIONS, NET_SOCKET_BACKLOG, NET_MAX_ACCEPT_OPS,                 NET_BUFFER_SIZE, NET_PORT);            _netServer = new TCPServer2(setting);            _netServer.ClientConnected += TCPClientConnected;            _netServer.ClientDataReceived += TCPClientDataReceived;            _netServer.ClientDisonnected += TCPClientDisonnected;            _netServer.Start();            _loggedPlayers = new Dictionary<int, int>();            _tcpClients = new Dictionary<int, int>();            _playerClients = new Dictionary<int, int>();            _responses = new List<Pair<int, BaseResponse>>();            _actions = new Dictionary<PacketType, OnAcceptPacket<BasePacket>>            {                {PacketType.UserLogin, Login},                {PacketType.UserEnter, Enter},                {PacketType.UserLogout, Logout},                {PacketType.PlayerRotate, PlayerRotate},                {PacketType.PlayerMove, PlayerMove},                {PacketType.MapData, MapData}            };            _validators = new List<BaseValidator>            {                new VCheckPosition(this)            };            _threadSendResponses = new Thread(SendResponsesProc);            _threadSendResponses.Priority = ThreadPriority.AboveNormal;            _threadSendResponses.IsBackground = true;            _threadSendResponses.Start();        }#endregion#region Class methods        private void ThrowSessionError(params object[] args)        {            BaseResponse.Throw("Invalid session", ECGeneral.SessionError);        }        private void AssertSession(BasePacket packet)        {            if (packet.Type != PacketType.UserLogin)            {                if (!_loggedPlayers.ContainsKey(packet.SessionToken))                {                    ThrowSessionError(packet.Type, packet.SessionToken);                }            }        }        private BaseResponse ProcessPacket(int clientId, BasePacket packet)        {            try            {                AssertSession(packet);                return _actions[packet.Type](clientId, packet);            }            catch (Exception ex)            {                return BaseResponse.FromException(packet, ex);            }        }        private void WriteLog(string msg)        {            LFactory.Instance.Write(msg);        }        private void WriteLog(LogEventType eventType, string msg)        {            LFactory.Instance.Write(eventType, msg);        }        public void Dispose()        {            _terminating = true;            if (_netServer != null)            {                _netServer.Dispose();            }            _threadSendResponses.Join(100);            World.Dispose();            LFactory.Instance.Deinitialize();        }#endregion#region World        private void RegisterWorldResponseForNearest(Player player, BaseResponse response)        {            List<Player> nearestPlayers = World.PlayersGetNearest(player);            if (nearestPlayers.Count > 0)            {                lock (_responses)                {                    foreach (Player nearest in nearestPlayers)                    {                        _responses.Add(new Pair<int, BaseResponse>(nearest.Id, response));                    }                    _unsentResponsesAvailable = true;                }            }        }#endregion#region User        private BaseResponse Login(int clientId, BasePacket packet)        {            PUserLogin pUserLogin = (PUserLogin) packet;            User user = new User(pUserLogin.UserName, pUserLogin.Password);            pUserLogin.SessionToken = BasePacket.NewSessionToken();            Player player = Player.Create(user.UserName);            ShortPoint? startPoint = World.MapFindPlayerStartPoint(player);            if (!startPoint.HasValue)            {                BaseResponse.Throw("Cannot get start point for player", ECGeneral.ServerError);                return null;            }            player.Position = startPoint.Value.ToPoint(ConstMap.PIXEL_SIZE);            lock (_loggedPlayers)            {                _loggedPlayers.Add(pUserLogin.SessionToken, player.Id);            }            lock (_tcpClients)            {                _tcpClients.Add(clientId, pUserLogin.SessionToken);                _playerClients.Add(player.Id, clientId);            }            World.PlayerAdd(pUserLogin.SessionToken, player);            return new RUserLogin(pUserLogin)            {                SessionToken = pUserLogin.SessionToken            };        }        private BaseResponse Enter(int clientId, BasePacket packet)        {            PUserEnter pUserEnter = (PUserEnter) packet;            PlayerDataEx playerData = World.PlayerDataGet(pUserEnter.SessionToken);            if (playerData == null)            {                ThrowSessionError(packet.Type, packet.SessionToken);                return null;            }            int maxScreenSide = Math.Max(pUserEnter.ScreenRes.Width, pUserEnter.ScreenRes.Height);            playerData.ScreenRes = new ShortSize(maxScreenSide, maxScreenSide);            List<Player> nearestPlayers = World.PlayersGetNearest(playerData);            ShortRect mapWindow;            byte[] mapData = World.MapWindowGet(playerData, out mapWindow);            ShortSize mapSize = new ShortSize(playerData.Map.Width, playerData.Map.Height);            ShortSize miniMapSize;            byte[] miniMapData = World.MiniMapGet(playerData, out miniMapSize);            RegisterWorldResponseForNearest(playerData, new RPlayerEnter            {                Player = playerData            });            return new RUserEnter(pUserEnter)            {                MyPlayerId = playerData.Player.Id,                PlayersOnLocation = nearestPlayers,                MapSize = mapSize,                MapData = mapData,                MapWindow = mapWindow,                MiniMapData = miniMapData,                MiniMapSize = miniMapSize            };        }        private BaseResponse Logout(int clientId, BasePacket packet)        {            return Logout(clientId, packet.SessionToken);        }        private BaseResponse Logout(int clientId, int sessionToken)        {            lock (_loggedPlayers)            {                int playerId;                if (_loggedPlayers.TryGetValue(sessionToken, out playerId))                {                    _loggedPlayers.Remove(sessionToken);                    lock (_tcpClients)                    {                        _tcpClients.Remove(clientId);                        _playerClients.Remove(playerId);                    }                    World.PlayerRemove(sessionToken);                    SendResponse(new RPlayerExit                    {                        PlayerId = playerId                    });                }            }            return null;        }#endregion#region Player        private BaseResponse PlayerRotate(int clientId, BasePacket packet)        {            PPlayerRotate pPlayerRotate = (PPlayerRotate)packet;            PlayerDataEx playerData = World.PlayerDataGet(pPlayerRotate.SessionToken);            if (playerData == null)            {                ThrowSessionError(packet.Type, packet.SessionToken);                return null;            }            if (playerData.Player.Angle != pPlayerRotate.Angle)            {                playerData.Player.Angle = pPlayerRotate.Angle;                RegisterWorldResponseForNearest(playerData, new RPlayerRotate                {                    PlayerId = playerData.Player.Id,                    Angle = pPlayerRotate.Angle                });            }            return null;        }        private BaseResponse PlayerMove(int clientId, BasePacket packet)        {            PPlayerMove pPlayerMove = (PPlayerMove)packet;            PlayerDataEx playerData = World.PlayerDataGet(pPlayerMove.SessionToken);            if (playerData == null)            {                ThrowSessionError(packet.Type, packet.SessionToken);                return null;            }            if (playerData.Player.Position != pPlayerMove.Position ||                playerData.Player.Direction != pPlayerMove.Direction)            {                if (pPlayerMove.Direction == Direction.None)                {                    playerData.StopMoving(pPlayerMove.Position);                }                else                {                    playerData.StartMoving(pPlayerMove.Position, pPlayerMove.Direction);                }                RegisterWorldResponseForNearest(playerData, new RPlayerMove                {                    PlayerId = playerData.Player.Id,                    Position = pPlayerMove.Position,                    Direction = pPlayerMove.Direction,                });            }            return null;        }#endregion#region Map        private BaseResponse MapData(int clientId, BasePacket packet)        {            PMapData pMapData = (PMapData) packet;            PlayerDataEx playerData = World.PlayerDataGet(pMapData.SessionToken);            if (playerData == null)            {                ThrowSessionError(packet.Type, packet.SessionToken);                return null;            }            ShortRect mapWindow;            byte[] mapData = World.MapWindowGet(playerData, out mapWindow);            return new RMapData(pMapData)            {                MapData = mapData,                MapWindow =  mapWindow            };        }#endregion#region TCP        private void TCPClientConnected(int clientId)        {            WriteLog(LogEventType.TCPConnections, _netServer.NumberOfAcceptedSockets.ToString());        }        private void TCPClientDisonnected(int clientId)        {            int sessionToken;            if (_tcpClients.TryGetValue(clientId, out sessionToken))            {                Logout(clientId, sessionToken);            }            WriteLog(LogEventType.TCPConnections, _netServer.NumberOfAcceptedSockets.ToString());        }        private void SendResponse(BaseResponse response, int clientId = 0)        {            if (response.Private || response.HasError)            {                _netServer.Send(clientId, response);            }            else            {                _netServer.SendAll(response);            }        }        private void TCPClientDataReceived(int clientId, List<BasePacket> packets)        {            for (int i = 0; i < packets.Count; i++)            {                BasePacket packet = packets[i];                BaseResponse response = ProcessPacket(clientId, packet);                if (response != null)                {                    SendResponse(response, clientId);                }            }        }        private void SendResponsesProc()        {            const float timeToCall = 1000 / WORLD_DELAY_BETWEEN_FRAMES_MS;            HRTimer timer = new HRTimer();            DateTime opTime = DateTime.UtcNow;            while (!_terminating)            {                if (_unsentResponsesAvailable)                {                    timer.StartWatch();                    int cnt = _responses.Count;                    Parallel.For(0, cnt, i =>                    {                        int clientId;                        Pair<int, BaseResponse> response = _responses[i];                        if (_playerClients.TryGetValue(response.Key, out clientId))                        {                            _netServer.Send(clientId, response.Value);                        }                    });                    _responses.Clear();                    _unsentResponsesAvailable = false;                    WriteLog(LogEventType.TCPResponsesSend, timer.StopWatch().ToString("F"));                }                TimeSpan elapsed = DateTime.UtcNow - opTime;                int timeToIdle = (int) Math.Max(timeToCall - elapsed.TotalMilliseconds - 1, 1);                Thread.Sleep(timeToIdle);                DateTime curTime = DateTime.UtcNow;                elapsed = curTime - opTime;                opTime = curTime;                WriteLog(LogEventType.TCPResponsesProc, elapsed.TotalMilliseconds.ToString("F"));            }        }#endregion    }}