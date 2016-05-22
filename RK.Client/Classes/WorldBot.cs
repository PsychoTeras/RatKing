using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RK.Common.Classes.Common;
using RK.Common.Net.Client;
using RK.Common.Proto;
using RK.Common.Proto.Packets;
using RK.Common.Proto.Responses;

namespace RK.Client.Classes
{
    public sealed class WorldBot : IDisposable
    {
        private TCPClient _tcpClient;
        private int _sessionToken;
        private volatile bool _changingConnection;

        public bool Connected { get; private set; }

        public WorldBot()
        {
            TCPClientSettings settings = new TCPClientSettings
                (   
                    ushort.MaxValue, "127.0.0.1", 15051, true
                );
            _tcpClient = new TCPClient(settings);
            _tcpClient.Connected += TCPConnected;
            _tcpClient.DataReceived += TCPClientDataReceived;
            _tcpClient.Disconnected += TCPClientDisconnected;
        }

        private void TCPClientDisconnected()
        {
            _changingConnection = Connected = false;
        }

        private void TCPConnected()
        {
            TCPClientDataSend(new PUserLogin
            {
                UserName = "PsychoTeras",
                Password = "password"
            });
        }

        public void Connect()
        {
            if (_changingConnection || Connected) return;
            _changingConnection = true;
            _tcpClient.Connect();
        }

        private void TCPClientDataSend(BasePacket packet)
        {
            if (_sessionToken != 0 || packet.Type == PacketType.UserLogin)
            {
                _tcpClient.Send(packet);
            }
        }

        private void TCPClientDataReceived(IList<BaseResponse> packets)
        {
            foreach (BaseResponse packet in packets)
            {
                GameHostResponse(packet);
            }
        }

        private void GameHostResponse(BaseResponse e)
        {
            switch (e.Type)
            {
                case PacketType.UserEnter:
                    Connected = true;
                    _changingConnection = false;
                    break;
                case PacketType.UserLogin:
                    RUserLogin userLogin = (RUserLogin) e;
                    _sessionToken = userLogin.SessionToken;
                    ShortSize screenRes = new ShortSize
                        (
                        Screen.PrimaryScreen.Bounds.Width,
                        Screen.PrimaryScreen.Bounds.Height
                        );
                    TCPClientDataSend(new PUserEnter
                    {
                        SessionToken = _sessionToken,
                        ScreenRes = screenRes
                    });
                    break;
            }
        }

        public void DoSimulate()
        {
            if (_changingConnection || !Connected) return;
            Random rnd = new Random(Environment.TickCount);
            switch (rnd.Next(0, 5))
            {
                default:
                {
                    TCPClientDataSend(new PPlayerRotate
                    {
                        SessionToken = _sessionToken,
                        Angle = rnd.Next(0, 360)
                    });
                    break;
                }
                case 0:
                {
                    _changingConnection = true;
                    TCPClientDataSend(new PUserLogout
                    {
                        SessionToken = _sessionToken
                    });
                    _sessionToken = 0;
                    _tcpClient.Disconnect();
                    return;
                }
            }
        }

        public void Dispose()
        {
            if (_sessionToken != 0)
            {
                TCPClientDataSend(new PUserLogout
                {
                    SessionToken = _sessionToken
                });
            }
            _tcpClient.Dispose();
        }
    }
}
