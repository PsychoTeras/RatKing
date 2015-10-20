namespace RK.Common.Common
{
    public unsafe interface ISerializable
    {
        int SizeOf();
        void Serialize(byte* bData, ref int pos);
        void Deserialize(byte* bData, ref int pos);
    }
}
