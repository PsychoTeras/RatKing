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
                return 
                    Serializer.SizeOf(UserName) +
                    Serializer.SizeOf(Password);
            }
        }

        protected override void DeserializeFromMemory(byte* bData, int pos)
        {
            Serializer.Read(bData, out UserName, ref pos);
            Serializer.Read(bData, out Password, ref pos);
        }

        protected override void SerializeToMemory(byte* bData, int pos)
        {
            Serializer.Write(bData, UserName, ref pos);
            Serializer.Write(bData, Password, ref pos);
        }
    }
}
