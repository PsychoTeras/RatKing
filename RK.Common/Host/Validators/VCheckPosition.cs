using System;
using System.Collections.Generic;
using RK.Common.Algo;
using RK.Common.Classes.Units;
using RK.Common.Proto;
using RK.Common.Proto.ErrorCodes;
using RK.Common.Proto.Packets;

namespace RK.Common.Host.Validators
{
    public sealed class VCheckPosition : BaseValidator
    {
        class ValidationInfo
        {
            private const double DIST_DIFF_THRESH = 0.9d;

            private int _lastX;
            private int _lastY;
            private DateTime _lastMoveTime;
            private float _lastPlayerSpeed;

            public ValidationInfo(Player player)
            {
                _lastX = player.Position.X;
                _lastY = player.Position.Y;
                _lastMoveTime = DateTime.Now;
                _lastPlayerSpeed = player.Speed;
            }

            private void AssertDistance(BasePacket packet, double dist)
            {
                double ms = DateTime.Now.Subtract(_lastMoveTime).TotalMilliseconds;
                if (ms/_lastPlayerSpeed/dist < DIST_DIFF_THRESH)
                {
                    BaseResponse.Throw(packet.ToString(), ECGeneral.ActionIsImposible);
                }
            }

            public void AssertAction(BasePacket packet)
            {
                switch (packet.Type)
                {
                    case PacketType.PlayerMove:
                    {
                        PPlayerMove p = (PPlayerMove) packet;
                        AssertDistance(packet, Geometry.GetDistance(p.X, p.Y, _lastX, _lastY));
                        break;
                    }
                }
            }
        }

        private Dictionary<long, ValidationInfo> _info;

        public VCheckPosition(GameHost host) : base(host)
        {
            _info = new Dictionary<long, ValidationInfo>();
        }

        public override void RegisterSession(long sessionMark)
        {
            lock (_info)
            {
                if (!_info.ContainsKey(sessionMark))
                {
                    Player player;
                    Host.World.Players.TryGetValue(sessionMark, out player);
                    if (player != null)
                    {
                        ValidationInfo info = new ValidationInfo(player);
                        _info.Add(sessionMark, info);
                    }
                }
            }
        }

        public override void UnregisterSession(long sessionMark)
        {
            lock (_info)
            {
                if (_info.ContainsKey(sessionMark))
                {
                    _info.Remove(sessionMark);
                }
            }
        }

        public override bool Validate(BasePacket packet)
        {
            ValidationInfo info;
            _info.TryGetValue(packet.SessionMark, out info);
            if (info == null)
            {
                return false;
            }
            info.AssertAction(packet);
            return true;
        }
    }
}
