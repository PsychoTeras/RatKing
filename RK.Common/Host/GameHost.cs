using System;
using System.Collections.Generic;
using RK.Common.Classes.Units;
using RK.Common.Classes.Users;
using RK.Common.Classes.World;
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

        private Dictionary<long, User> _loggedUsers;
        private Dictionary<long, int> _userPlayers;

#endregion

#region Public fields

        public GameWorld World;

#endregion

#region Ctor

        public GameHost()
        {
            _loggedUsers = new Dictionary<long, User>();
            _userPlayers = new Dictionary<long, int>();

            _actions = new Dictionary<PacketType, OnAcceptPacket<BasePacket>>
            {
                {PacketType.UserLogin, Login},
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

#region User methods

        public BaseResponse Login(BasePacket packet)
        {
            lock (_loggedUsers) lock (_userPlayers)
            {
                PUserLogin pUserLogin = (PUserLogin)packet;

                User user = new User(pUserLogin.UserName, pUserLogin.Password);
                packet.SessionToken = BasePacket.NewSessionToken(user.Id);
                _loggedUsers.Add(packet.SessionToken, user);

                Player player = Player.Create(user.UserName);
                _userPlayers.Add(packet.SessionToken, player.Id);

                World.PlayerAdd(player);

                return new RUserLogin(packet.SessionToken);
            }
        }

#endregion

#region Player methods

        public BaseResponse PlayerEnter(BasePacket packet)
        {
            lock (_userPlayers)
            {
                PPlayerEnter pPlayerEnter = (PPlayerEnter)packet;

                RPlayerEnter response = new RPlayerEnter();

                return response;
            }
        }

#endregion

#region Class methods

        private void AssertSession(BasePacket packet)
        {
            if (packet.Type != PacketType.UserLogin)
            {
                if (!_loggedUsers.ContainsKey(packet.SessionToken))
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
                return BaseResponse.FromException<BaseResponse>(packet, ex);
            }
        }

#endregion

    }
}
