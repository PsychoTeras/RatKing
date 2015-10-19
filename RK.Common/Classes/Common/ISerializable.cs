namespace RK.Common.Classes.Common
{
    public unsafe interface ISerializable
    {
        int SizeOf();
        int Serialize(byte* bData, int pos);
        int Deserialize(byte* bData, int pos);
    }
}
