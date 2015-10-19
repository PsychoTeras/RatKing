using System;
using System.Collections.Generic;
using System.Drawing;
using RK.Common.Classes.Common;
using RK.Common.Classes.Units;
using RK.Common.Classes.Users;
using RK.Common.Classes.World;
using RK.Common.Const;
using RK.Common.Host.Validators;
using RK.Common.Net.TCP;
using RK.Common.Proto;
using RK.Common.Proto.ErrorCodes;
using RK.Common.Proto.Events;
using RK.Common.Proto.Packets;
using RK.Common.Proto.Responses;

namespace RK.Common.Host
{
    public sealed class GameHost : IDisposable
    {

#region Delegates

        private delegate BaseResponse OnAcceptPacket<in T>(T packet) 
            where T: BasePacket;

        public delegate void OnGameHostEvent(BaseEvent e);

#endregion

#region Private fields

        private TCPServer _netServer;

        private Dictionary<PacketType, OnAcceptPacket<BasePacket>> _actions;
        private List<BaseValidator> _validators;

        private Dictionary<long, int> _loggedPlayers;

#endregion

#region Public fields

        public GameWorld World;

        public event OnGameHostEvent GameHostEvent;

#endregion

#region Ctor

        public GameHost()
        {
            _netServer = new TCPServer(15051);

            _loggedPlayers = new Dictionary<long, int>();

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

        private void SendGameHostEvent(BaseEvent e)
        {
            if (GameHostEvent != null)
            {
                GameHostEvent(e);
            }
        }

#endregion

#region User methods

        private BaseResponse Login(BasePacket packet)
        {
            lock (_loggedPlayers)
            {
                PUserLogin pUserLogin = (PUserLogin)packet;

                User user = new User(pUserLogin.UserName, pUserLogin.Password);
                packet.SessionToken = BasePacket.NewSessionToken(user.Id);

                Player player = Player.Create(user.UserName);
                ShortPoint? startPoint = World.MapFindPlayerStartPoint(player);
                if (!startPoint.HasValue)
                {
                    BaseResponse.Throw("Cannot get start point for player", ECGeneral.ServerError);
                    return null;
                }
                player.Position = startPoint.Value.ToPoint(ConstMap.PIXEL_SIZE);

                World.PlayerAdd(packet.SessionToken, player);

                _loggedPlayers.Add(packet.SessionToken, player.Id);

                return new RUserLogin(packet.SessionToken);
            }
        }

        private BaseResponse Logout(BasePacket packet)
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

        private BaseResponse PlayerEnter(BasePacket packet)
        {
            PPlayerEnter pPlayerEnter = (PPlayerEnter) packet;

            Player player = World.PlayerGet(pPlayerEnter.SessionToken);
            if (player == null)
            {
                ThrowSessionError(packet.Type, packet.SessionToken);
                return null;
            }

            RPlayerEnter response = new RPlayerEnter();
            response.PlayersOnLocation = World.PlayersGetNearest(player);
            response.MyPlayerId = player.Id;

            return response;
        }

        private BaseResponse PlayerRotate(BasePacket packet)
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

                SendGameHostEvent(new EPlayerRotate(player.Id, pPlayerRotate));
            }

            return null;
        }

        private BaseResponse PlayerMove(BasePacket packet)
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

                SendGameHostEvent(new EPlayerMove(player.Id, pPlayerMove));
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

        public BaseResponse ProcessPacket(BasePacket packet)
        {
            try
            {
                AssertSession(packet);
                return _actions[packet.Type](packet);
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

    }
}
