namespace RK.Common.Map
{
    public unsafe interface IBaseMap
    {
        ushort Width { get; }
        ushort Height { get; }
        Tile* this[ushort x, ushort y] { get; }
    }
}
