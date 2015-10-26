using System;
using System.Drawing;
using RK.Common.Classes.Common;
using RK.Common.Classes.Units;
using RK.Common.Const;
using RK.Common.Map;

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

        public static ShortPoint? FindPlayerStartPoint(ServerMap map, Player player, 
            int minAreaSpace)
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
                    int playerMargin = (int) Math.Floor((float)player.Size.HighValue/ConstMap.PIXEL_SIZE);
                    ShortPoint? cell = area.FindFreeCell(map, playerMargin);
                    if (cell != null)
                    {
                        return cell;
                    }
                }
            }

            return null;
        }

        public static float GetAngleOfLine(Point p1, Point p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return (float) (Math.Atan2(yDiff, xDiff) * (180 / Math.PI));
        }
    }
}
