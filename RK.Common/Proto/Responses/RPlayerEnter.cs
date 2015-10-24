using RK.Common.Classes.Units;
using RK.Common.Win32;

namespace RK.Common.Proto.Responses
{
    public unsafe sealed class RPlayerEnter : BaseResponse
    {
        public Player Player;

        public override PacketType Type
        {
            get { return PacketType.PlayerEnter; }
        }

        protected override int SizeOf
        {
            get
            {
                return
                    Serializer.SizeOf(Player);
            }
        }

        protected override void DeserializeFromMemory(byte* bData, int pos)
        {
            Serializer.Read(bData, out Player, ref pos);
        }

        protected override void SerializeToMemory(byte* bData, int pos)
        {
            Serializer.Write(bData, Player, ref pos);
        }

        public RPlayerEnter() { }

        public RPlayerEnter(Player player)
        {
            Player = player;
        }
    }
}
