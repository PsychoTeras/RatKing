using System;
using System.Collections.Generic;
using System.Threading;
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
        private long _sessionToken;
        private Thread _thread;

        private volatile bool _disposiong;

        public WorldBot()
        {
            _thread = new Thread(DoSimulate);
            _thread.IsBackground = true;
            _thread.Priority = ThreadPriority.Lowest;
            _thread.Start();

            _tcpClient = new TCPClient("192.168.1.114", 15051);
            _tcpClient.DataReceived += TCPClientDataReceived;
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

        private void DoSimulate()
        {
            Random rnd = new Random(Environment.TickCount);
            try
            {
                while (!_disposiong)
                {
                    switch (rnd.Next(0, 3))
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
                    }
                    Thread.Sleep(rnd.Next(100, 1000));
                }
            }
            catch { }
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
            _disposiong = true;
            _thread.Abort();
            _tcpClient.Dispose();
        }
    }
}
