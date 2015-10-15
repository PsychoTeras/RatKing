namespace RK.Common.Proto.Packets
{
    public sealed class PUserLogin : BasePacket
    {
        public string UserName;
        public string Password;

        public override PacketType Type
        {
            get { return PacketType.UserLogin; }
        }
    }
}
