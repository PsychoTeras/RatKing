using System;
using System.Collections.Generic;
using RK.Common.Classes.Units;
using RK.Common.Classes.Users;
using RK.Common.Classes.World;
using RK.Common.Proto;
using RK.Common.Proto.ErrorCodes;
using RK.Common.Proto.User;

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

        private GameWorld _world;
        private Dictionary<long, User> _loggedUsers;
        private Dictionary<long, long> _userPlayers;

#endregion

#region Ctor

        public GameHost()
        {
            _loggedUsers = new Dictionary<long, User>();
            _userPlayers = new Dictionary<long, long>();

            _actions = new Dictionary<PacketType, OnAcceptPacket<BasePacket>>
            {
                {PacketType.UserLogin, Login}
            };
        }

        public GameHost(GameWorld world) : this()
        {
            _world = world;
        }

#endregion

#region User methods

        public BaseResponse Login(BasePacket packet)
        {
            PUserLogin pUserLogin = (PUserLogin) packet;

            pUserLogin.NewSessionId();

            User user = new User(pUserLogin.UserName, pUserLogin.Password);
            _loggedUsers.Add(pUserLogin.SessionMark, user);

            Player player = Player.Create(user.UserName);
            _world.PlayerAdd(player);

            _userPlayers.Add(user.Id, player.Id);

            return new RUserLogin(pUserLogin.SessionId);
        }

#endregion

#region Player methods

#endregion

#region Class methods

        private void AssertSession(BasePacket packet)
        {
            if (packet.Type != PacketType.UserLogin)
            {
                if (!_loggedUsers.ContainsKey(packet.SessionMark))
                {
                    BaseResponse.Throw("Invalid session", ECGeneral.SessionError);
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
                return BaseResponse.FromException<BaseResponse>(ex);
            }
        }

#endregion

    }
}
