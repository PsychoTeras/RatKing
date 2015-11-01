﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RK.Common.Classes.Common;
using RK.Common.Net.TCP;
using RK.Common.Proto;
using RK.Common.Proto.Packets;
using RK.Common.Proto.Responses;

namespace RK.Client.Classes
{
    public sealed class WorldBot : IDisposable
    {
        private TCPClient _tcpClient;
        private int _sessionToken;
        private Random _rnd = new Random(Environment.TickCount);

        public bool Connected
        {
            get { return _tcpClient.IsConnected; }
        }

        public WorldBot()
        {
            _tcpClient = new TCPClient("192.168.1.32", 15051);
            _tcpClient.DataReceived += TCPClientDataReceived;
        }

        public void Connect()
        {
            _tcpClient.Connect();
            TCPClientDataSend(new PUserLogin
            {
                UserName = "PsychoTeras",
                Password = "password"
            });
        }

        private void TCPClientDataSend(BasePacket packet)
        {
            if (_sessionToken != 0 || packet.Type == PacketType.UserLogin)
            {
                _tcpClient.SendData(packet);
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
            switch (_rnd.Next(0, 3))
            {
                default:
                {
                    TCPClientDataSend(new PPlayerRotate
                    {
                        SessionToken = _sessionToken,
                        Angle = _rnd.Next(0, 360)
                    });
                    break;
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
