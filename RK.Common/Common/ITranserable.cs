namespace RK.Common.Common
{
    public unsafe interface ITranserable
    {
        int SizeOf();
        void Serialize(byte* bData, ref int pos);
        void Deserialize(byte* bData, ref int pos);
    }
}
