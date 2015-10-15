using System;
using System.Collections.Generic;
using RK.Common.Classes.Common;
using RK.Common.Classes.Units;
using RK.Common.Classes.Users;
using RK.Common.Classes.World;
using RK.Common.Const;
using RK.Common.Host.Validators;
using RK.Common.Proto;
using RK.Common.Proto.ErrorCodes;
using RK.Common.Proto.Packets;
using RK.Common.Proto.Responses;

namespace RK.Common.Host
{
    public sealed class GameHost
    {

#region Delegates

        private delegate BaseResponse OnAcceptPacket<in T>(T packet) 
            where T: BasePacket;

#endregion

#region Private fields

        private Dictionary<PacketType, OnAcceptPacket<BasePacket>> _actions;
        private List<BaseValidator> _validators;

        private Dictionary<long, int> _loggedPlayers;

#endregion

#region Public fields

        public GameWorld World;

#endregion

#region Ctor

        public GameHost()
        {
            _loggedPlayers = new Dictionary<long, int>();

            _actions = new Dictionary<PacketType, OnAcceptPacket<BasePacket>>
            {
                {PacketType.UserLogin, Login},
                {PacketType.UserLogout, Logout},
                {PacketType.PlayerEnter, PlayerEnter},
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

#endregion

    }
}
