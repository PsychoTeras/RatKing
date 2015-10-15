using System.Drawing;

namespace RK.Common.Algo
{
    public static class Extensions
    {
        public static bool CloseTo(this Point point, Point dest, int areaHalfSize)
        {
            return point.X >= dest.X - areaHalfSize && point.X <= dest.X + areaHalfSize &&
                   point.Y >= dest.Y - areaHalfSize && point.Y <= dest.Y + areaHalfSize;
        }
    }
}
