﻿using RK.Common.Classes.Common;

namespace RK.Common.Proto.Packets
{
    public class PPlayerMove : BasePacket
    {
        public int X;
        public int Y;
        public Direction D;

        public override PacketType Type
        {
            get { return PacketType.PlayerMove; }
        }
    }
}