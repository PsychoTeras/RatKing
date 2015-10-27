namespace RK.Common.Map
{
    internal static unsafe class TileBorders
    {
        public static byte GetBorders(int x, int y, ushort w, ushort h, TileType areaType, ServerMap map)
        {
            if (IsNotValidTile(x, y, w, h, areaType, map))
            {
                return 0;
            }

            byte flag = 0;

            if (IsNotValidTile(x - 1, y, w, h, areaType, map)) flag |= 1;
            if (IsNotValidTile(x + 1, y, w, h, areaType, map)) flag |= 2;
            if (IsNotValidTile(x, y - 1, w, h, areaType, map)) flag |= 4;
            if (IsNotValidTile(x, y + 1, w, h, areaType, map)) flag |= 8;

            return flag;
        }

        public static bool IsNotValidTile(int x, int y, ushort w, ushort h, TileType areaType, ServerMap map)
        {
            return x < 0 || x >= w || y < 0 || y >= h || (*map[(ushort)x, (ushort)y]).Type != areaType;
        }
    }
}
