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

        private Dictionary<long, int> _loggedPlayers;
        private Dictionary<long, long> _tcpClients;

#endregion

#region Public fields

        public GameWorld World;

#endregion

#region Ctor

        public GameHost()
        {
            _netServer = new TCPServer(15051);
            _netServer.ClientConnected += TCPClientConnected;
            _netServer.ClientDataReceived += TCPClientDataReceived;
            _netServer.Start();

            _loggedPlayers = new Dictionary<long, int>();
            _tcpClients = new Dictionary<long, long>();

            _actions = new Dictionary<PacketType, OnAcceptPacket<BasePacket>>
            {
                {PacketType.UserLogin, Login},
                {PacketType.UserLogout, Logout},

                {PacketType.PlayerEnter, PlayerEnter},
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
            lock (_loggedPlayers)
            {
                PUserLogin pUserLogin = (PUserLogin)packet;

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

                _loggedPlayers.Add(pUserLogin.SessionToken, player.Id);
                _tcpClients.Add(pUserLogin.SessionToken, tcpClient.Id);

                return new RUserLogin(pUserLogin);
            }
        }

        private BaseResponse Logout(TCPClientEx tcpClient, BasePacket packet)
        {
            lock (_loggedPlayers)
            {
                if (!_loggedPlayers.ContainsKey(packet.SessionToken))
                {
                    ThrowSessionError(packet.Type, packet.SessionToken);
                }
                _loggedPlayers.Remove(packet.SessionToken);
                World.PlayerRemove(packet.SessionToken);
            }
            return BaseResponse.Successful(packet);
        }

#endregion

#region Player methods

        private BaseResponse PlayerEnter(TCPClientEx tcpClient, BasePacket packet)
        {
            PPlayerEnter pPlayerEnter = (PPlayerEnter) packet;

            Player player = World.PlayerGet(pPlayerEnter.SessionToken);
            if (player == null)
            {
                ThrowSessionError(packet.Type, packet.SessionToken);
                return null;
            }

            List<Player> playersOnLocation = World.PlayersGetNearest(player);
            return new RPlayerEnter(player.Id, playersOnLocation, pPlayerEnter);
        }

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
                return BaseResponse.FromException<BaseResponse>(packet, ex);
            }
        }

        public void Dispose()
        {
            _netServer.Dispose();
        }

#endregion

#region TCP

        private void TCPClientConnected(TCPClientEx tcpClient)
        {
        }

        private void TCPClientDataReceived(TCPClientEx tcpClient, IList<BasePacket> packets)
        {
            foreach (BasePacket packet in packets)
            {
                BaseResponse response = ProcessPacket(tcpClient, packet);
                if (response != null)
                {
                    _netServer.SendData(tcpClient, response);
                }
            }
        }

#endregion

    }
}
