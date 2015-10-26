using System.Drawing;
using RK.Common.Classes.Common;
using RK.Common.Win32;

namespace RK.Common.Proto.Packets
{
    public sealed unsafe class PPlayerMove : BasePacket
    {
        public Point Position;
        public Direction Direction;

        public override PacketType Type
        {
            get { return PacketType.PlayerMove; }
        }

        protected override int SizeOf
        {
            get
            {
                return
                    sizeof(Point) +
                    sizeof(Direction);
            }
        }

        protected override void DeserializeFromMemory(byte* bData, int pos)
        {
            Serializer.Read(bData, out Position, ref pos);
            Serializer.Read(bData, out Direction, ref pos);
        }

        protected override void SerializeToMemory(byte* bData, int pos)
        {
            Serializer.Write(bData, Position, ref pos);
            Serializer.Write(bData, Direction, ref pos);
        }
    }
}