using System.Threading.Tasks;

namespace RK.Common.Classes.Map
{
    public static unsafe class TileBorders
    {
        public static void ScanAndSetBorders(MapAreas areas, TileType areaType, GameMap map)
        {
            ushort w = map.Width, h = map.Height;
            Parallel.For(0, h, y => Parallel.For(0, w, x =>
            {
                Tile* tile = map[(ushort)x, (ushort)y];
                if ((*tile).Type == areaType)
                {
                    (*tile).FlagClearBorders();
                }
            }));

            Parallel.ForEach(areas, a => Parallel.ForEach(a, p =>
            {
                byte borders = GetBorders(p.X, p.Y, w, h, areaType, map);
                (*map[p.X, p.Y]).FlagSetBorders(borders);
            }));
        }

        public static void ScanAndSetBorders(ushort x, ushort y, TileType areaType, GameMap map)
        {
            ushort w = map.Width, h = map.Height;
            byte borders = GetBorders(x, y, w, h, areaType, map);
            (*map[x, y]).FlagSetBorders(borders);
        }

        public static byte GetBorders(int x, int y, ushort w, ushort h, TileType areaType, GameMap map)
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

        public static bool IsNotValidTile(int x, int y, ushort w, ushort h, TileType areaType, GameMap map)
        {
            return x < 0 || x >= w || y < 0 || y >= h || (*map[(ushort)x, (ushort)y]).Type != areaType;
        }
    }
}
