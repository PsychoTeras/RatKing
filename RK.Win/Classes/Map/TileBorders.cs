using RK.Common.Classes.Map;

namespace RK.Win.Classes.Map
{
    public static unsafe class TileBorders
    {
        public static int ScanAndSetBorders(ushort x, ushort y, TileType areaType, 
            ClientMap map)
        {
            ushort w = map.Width, h = map.Height;

            byte borders = 0;

            if (!IsOppositeTile(x, y, w, h, areaType, map))
            {
                if (IsOppositeTile(x - 1, y, w, h, areaType, map)) borders |= 2;
                if (IsOppositeTile(x + 1, y, w, h, areaType, map)) borders |= 4;
                if (IsOppositeTile(x, y - 1, w, h, areaType, map)) borders |= 8;
                if (IsOppositeTile(x, y + 1, w, h, areaType, map)) borders |= 16;
            }

            if (borders == 0) borders = 1;

            map.FlagSetBorders(x, y, borders);

            return borders;
        }

        public static bool IsOppositeTile(int x, int y, ushort w, ushort h, TileType areaType, 
            ClientMap map)
        {
            if (x < 0 || x >= w || y < 0 || y >= h)
            {
                return true;
            }
            Tile* tile;
            return (tile = map[(ushort) x, (ushort) y]) != null && (*tile).Type != areaType;
        }
    }
}
