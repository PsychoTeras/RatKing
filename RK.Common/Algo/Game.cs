using System;
using RK.Common.Classes.Common;
using RK.Common.Classes.Units;
using RK.Common.Const;
using RK.Common.Map;

namespace RK.Common.Algo
{
    internal static class Game
    {
        private static Random _rnd = new Random(Environment.TickCount);

        public static ShortPoint? FindPlayerStartPoint(ServerMap map, Player player, int minAreaSpace)
        {
            var areas = map.SpaceAreas;
            int iStart = _rnd.Next(areas.Count), iEnd = areas.Count;

            bool secondIter = false;
            for (int i = iStart; i <= iEnd; i++)
            {
                if (i == iEnd)
                {
                    if (secondIter)
                    {
                        return null;
                    }
                    i = 0;
                    iEnd = iStart - 1;
                    secondIter = true;
                }
                MapArea area = areas[i];
                if (area.CellsCount * ConstMap.PIXEL_SIZE_SQR >= minAreaSpace)
                {
                    int playerMargin = (int)Math.Floor((float)player.Size.HighValue / ConstMap.PIXEL_SIZE);
                    ShortPoint? cell = area.FindFreeCell(map, playerMargin, _rnd);
                    if (cell != null)
                    {
                        return cell;
                    }
                }
            }

            return null;
        }
    }
}
