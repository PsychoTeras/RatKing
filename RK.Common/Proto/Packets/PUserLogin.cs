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

        internal override void InitializeFromMemory(byte* bData)
        {
            int sSize = Serializer.ReadString(bData, out UserName, BASE_SIZE);
            Serializer.ReadString(bData, out Password, BASE_SIZE + sSize);
            base.InitializeFromMemory(bData);
        }

        public override byte[] Serialize()
        {
            int pSize = BASE_SIZE +
                        Serializer.StringLength(UserName) +
                        Serializer.StringLength(Password);
            byte[] data = new byte[pSize];
            fixed (byte* bData = data)
            {
                SerializeHeader(bData, pSize);
                int sSize = Serializer.WriteString(bData, UserName, BASE_SIZE);
                Serializer.WriteString(bData, Password, BASE_SIZE + sSize);
            }
            return data;
        }
    }
}
