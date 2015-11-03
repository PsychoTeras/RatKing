using System;
using System.Drawing;
using RK.Common.Classes.Common;

namespace RK.Common.Algo
{
    public static class Geometry
    {
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

        public static float GetAngleOfLine(Point p1, Point p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return (float) (Math.Atan2(yDiff, xDiff) * (180 / Math.PI));
        }
    }
}
