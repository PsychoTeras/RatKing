using System;
using System.Collections.Generic;
using System.Drawing;
using RK.Common.Classes.Units;
using RK.Common.Classes.Users;
using RK.Common.Classes.World;
using RK.Common.Common;
using RK.Common.Const;
using RK.Common.Host.Validators;
using RK.Common.Net.TCP;
using RK.Common.Proto;
using RK.Common.Proto.ErrorCodes;
using RK.Common.Proto.Packets;
using RK.Common.Proto.Responses;

namespace RK.Common.Host
{
    public sealed class GameHost : IDisposable
    {

#region Delegates

        private delegate BaseResponse OnAcceptPacket<in T>(TCPClientEx tcpClient, T packet) 
            where T: BasePacket;

        public delegate void OnGameHostResponse(BaseResponse e);

#endregion

#region Private fields

        private TCPServer _netServer;

        private Dictionary<PacketType, OnAcceptPacket<BasePacket>> _actions;
        private List<BaseValidator> _validators;

        private Dictionary<long, LoggedUser> _loggedUsers;
        private Dictionary<long, int> _loggedPlayers;
        private Dictionary<TCPClientEx, long> _tcpClients;

#endregion

#region Public fields

        public GameWorld World;

#endregion

#region Ctor

        public GameHost()
        {
            if (Environment.CommandLine.Trim().Split(new [] {' '}).Length == 1)
            {
                _netServer = new TCPServer(15051);
                _netServer.ClientConnected += TCPClientConnected;
                _netServer.ClientDataReceived += TCPClientDataReceived;
                _netServer.ClientDisonnected += TCPClientDisonnected;
                _netServer.Start();
            }

            _loggedUsers = new Dictionary<long, LoggedUser>();
            _loggedPlayers = new Dictionary<long, int>();
            _tcpClients = new Dictionary<TCPClientEx, long>();

            _actions = new Dictionary<PacketType, OnAcceptPacket<BasePacket>>
            {
                {PacketType.UserLogin, Login},
                {PacketType.UserEnter, Enter},
                {PacketType.UserLogout, Logout},

                {PacketType.PlayerRotate, PlayerRotate},
                {PacketType.PlayerMove, PlayerMove},
            };

            _validators = new List<BaseValidator>
            {
                new VCheckPosition(this)
            };
        }

        public GameHost(GameWorld world) : this()
        {
            World = world;
        }

#endregion

#region Class methods

        private void ThrowSessionError(params object[] args)
        {
            BaseResponse.Throw("Invalid session", ECGeneral.SessionError);
        }

#endregion

#region User methods

        private BaseResponse Login(TCPClientEx tcpClient, BasePacket packet)
        {
            PUserLogin pUserLogin = (PUserLogin) packet;

            User user = new User(pUserLogin.UserName, pUserLogin.Password);
            pUserLogin.SessionToken = BasePacket.NewSessionToken(user.Id);
            
            LoggedUser loggedUser = new LoggedUser(user);

            Player player = Player.Create(user.UserName);
            ShortPoint? startPoint = World.MapFindPlayerStartPoint(player);
            if (!startPoint.HasValue)
            {
                BaseResponse.Throw("Cannot get start point for player", ECGeneral.ServerError);
                return null;
            }
            player.Position = startPoint.Value.ToPoint(ConstMap.PIXEL_SIZE);

            World.PlayerAdd(pUserLogin.SessionToken, player);

            lock (_loggedPlayers)
            {
                _loggedUsers.Add(pUserLogin.SessionToken, loggedUser);
                _loggedPlayers.Add(pUserLogin.SessionToken, player.Id);
            }

            lock (_tcpClients)
            {
                _tcpClients.Add(tcpClient, pUserLogin.SessionToken);
            }

            return new RUserLogin(pUserLogin);
        }

        private BaseResponse Enter(TCPClientEx tcpClient, BasePacket packet)
        {
            PUserEnter pUserEnter = (PUserEnter)packet;

            LoggedUser user;
            Player player = World.PlayerGet(pUserEnter.SessionToken);
            if (player == null || !_loggedUsers.TryGetValue(pUserEnter.SessionToken, out user))
            {
                ThrowSessionError(packet.Type, packet.SessionToken);
                return null;
            }

            user.ScreenRes = pUserEnter.ScreenRes;

            List<Player> playersOnLocation = World.PlayersGetNearest(player);
            ShortRect mapWindow;
            byte[] mapData = World.MapWindowGet(player, user.ScreenRes, out mapWindow);

            SendResponse(new RPlayerEnter(player));

            return new RUserEnter(player.Id, pUserEnter)
            {
                PlayersOnLocation = playersOnLocation,
                MapData = mapData,
                MapWindow = mapWindow
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
                    }

                    World.PlayerRemove(sessionToken);

                    SendResponse(new RPlayerExit(playerId));
                }
            }
            return null;
        }

#endregion

#region Player methods

        private BaseResponse PlayerRotate(TCPClientEx tcpClient, BasePacket packet)
        {
            PPlayerRotate pPlayerRotate = (PPlayerRotate)packet;
            Player player = World.PlayerGet(pPlayerRotate.SessionToken);
            if (player == null)
            {
                ThrowSessionError(packet.Type, packet.SessionToken);
                return null;
            }

            if (player.Angle != pPlayerRotate.Angle)
            {
                player.Angle = pPlayerRotate.Angle;
                return new RPlayerRotate(player.Id, pPlayerRotate);
            }

            return null;
        }

        private BaseResponse PlayerMove(TCPClientEx tcpClient, BasePacket packet)
        {
            PPlayerMove pPlayerMove = (PPlayerMove)packet;
            Player player = World.PlayerGet(pPlayerMove.SessionToken);
            if (player == null)
            {
                ThrowSessionError(packet.Type, packet.SessionToken);
                return null;
            }

            if (!player.Position.EqualsTo(pPlayerMove.X, pPlayerMove.Y) ||
                player.Direction != pPlayerMove.D)
            {
                player.Position = new Point(pPlayerMove.X, pPlayerMove.Y);
                player.Direction = pPlayerMove.D;
                return new RPlayerMove(player.Id, pPlayerMove);
            }

            return null;
        }

#endregion

#region Class methods

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

        public BaseResponse ProcessPacket(TCPClientEx tcpClient, BasePacket packet)
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

        public void Dispose()
        {
            if (_netServer != null)
            {
                _netServer.Dispose();
            }
        }

#endregion

#region TCP

        private void TCPClientConnected(TCPClientEx tcpClient) { }

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
            if (response.Private)
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
            foreach (BasePacket packet in packets)
            {
                BaseResponse response = ProcessPacket(tcpClient, packet);
                if (response != null)
                {
                    SendResponse(response, tcpClient);
                }
            }
        }

#endregion

    }
}
