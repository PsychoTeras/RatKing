using System;
using System.Drawing;
using RK.Common.Classes.Common;
using RK.Common.Classes.Map;
using RK.Common.Classes.Units;
using RK.Common.Const;

namespace RK.Common.Algo
{
    public static class Geometry
    {
        private static Random _rnd = new Random(Environment.TickCount);

        public static double GetDistance(Point p1, Point p2)
        {
            float x = p2.X - p1.X;
            float y = p2.Y - p1.Y;
            return Math.Sqrt(x*x + y*y);
        }

        public static double GetDistance(ShortPoint p1, ShortPoint p2)
        {
            float x = p2.X - p1.X;
            float y = p2.Y - p1.Y;
            return Math.Sqrt(x * x + y * y);
        }

        public static double GetDistance(int x1, int y1, int x2, int y2)
        {
            float x = x2 - x1;
            float y = y2 - y1;
            return Math.Sqrt(Math.Abs(x * x + y * y));
        }

        public static ShortPoint? FindPlayerStartPoint(MapAreas areas, Player player,
            int minAreaSpace)
        {
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
                    int playerMargin = (int) Math.Floor((float)player.Size.HighValue/ConstMap.PIXEL_SIZE);
                    ShortPoint? cell = area.FindFreeCell(playerMargin + 1);
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
