using RK.Common.Win32;

namespace RK.Common.Proto.Packets
{
    public unsafe sealed class PUserLogin : BasePacket
    {
        public string UserName;
        public string Password;

        public override PacketType Type
        {
            get { return PacketType.UserLogin; }
        }

        protected override int SizeOf
        {
            get
            {
                return BASE_SIZE +
                       Serializer.StringLength(UserName) +
                       Serializer.StringLength(Password);
            }
        }

        protected override void DeserializeFromMemory(byte* bData, int pos)
        {
            pos += Serializer.ReadString(bData, out UserName, pos);
            Serializer.ReadString(bData, out Password, pos);
        }

        protected override void SerializeToMemory(byte* bData, int pos)
        {
            pos += Serializer.WriteString(bData, UserName, pos);
            Serializer.WriteString(bData, Password, pos);
        }
    }
}
