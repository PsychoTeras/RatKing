using System;
using System.Collections.Generic;
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
using RK.Common.World;

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

            _loggedPlayers = new Dictionary<long, int>();
            _tcpClients = new Dictionary<TCPClientEx, long>();

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
            World.Dispose();
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

            lock (_loggedPlayers)
            {
                _loggedPlayers.Add(pUserLogin.SessionToken, player.Id);
            }

            lock (_tcpClients)
            {
                _tcpClients.Add(tcpClient, pUserLogin.SessionToken);
            }

            return new RUserLogin(pUserLogin)
            {
                SessionToken = pUserLogin.SessionToken
            };
        }

        private BaseResponse Enter(TCPClientEx tcpClient, BasePacket packet)
        {
            PUserEnter pUserEnter = (PUserEnter)packet;

            PlayerDataEx playerData = World.PlayerDataGet(pUserEnter.SessionToken);
            if (playerData == null)
            {
                ThrowSessionError(packet.Type, packet.SessionToken);
                return null;
            }

            playerData.ScreenRes = pUserEnter.ScreenRes;

            List<Player> playersOnLocation = World.PlayersGetNearest(playerData);
            ShortRect mapWindow;
            byte[] mapData = World.MapWindowGet(playerData, playerData.ScreenRes, out mapWindow);

            SendResponse(new RPlayerEnter
            {
                Player = playerData
            });

            return new RUserEnter(pUserEnter)
            {
                MyPlayerId = playerData.Player.Id,
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
            Player player = World.PlayerGet(pPlayerRotate.SessionToken);
            if (player == null)
            {
                ThrowSessionError(packet.Type, packet.SessionToken);
                return null;
            }

            if (player.Angle != pPlayerRotate.Angle)
            {
                player.Angle = pPlayerRotate.Angle;
                return new RPlayerRotate
                {
                    PlayerId = player.Id,
                    Angle = pPlayerRotate.Angle
                };
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
                return new RPlayerMove
                {
                    PlayerId = playerData.Player.Id,
                    Position = pPlayerMove.Position,
                    Direction = pPlayerMove.Direction,
                };
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
            byte[] mapData = World.MapWindowGet(playerData, playerData.ScreenRes, out mapWindow);

            return new RMapData(pMapData)
            {
                MapData = mapData,
                MapWindow =  mapWindow
            };
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
            for (int i = 0; i < packets.Count; i++)
            {
                BasePacket packet = packets[i];
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
