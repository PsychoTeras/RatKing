using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace RK.Common.Classes.Common
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ShortPoint : IEquatable<ShortPoint>
    {
        public ushort X;
        public ushort Y;

        public static ShortPoint Empty
        {
            get { return new ShortPoint(); }
        }

        public int Mark
        {
            get { return Y << 16 | X; }
        }

        public ShortPoint(int x, int y)
        {
            if (x < 0 || x > ushort.MaxValue || y < 0 || y > ushort.MaxValue)
            {
                throw new Exception("Params are out of range");
            }
            X = (ushort) x;
            Y = (ushort) y;
        }

        public ShortPoint(ushort x, ushort y)
        {
            X = x;
            Y = y;
        }

        public ShortPoint(ShortPoint source)
        {
            X = source.X;
            Y = source.Y;
        }

        public ShortPoint(Point source)
        {
            X = (ushort) source.X;
            Y = (ushort) source.Y;
        }

        public static bool operator ==(ShortPoint o1, ShortPoint o2)
        {
            return o1.Equals(o2);
        }

        public static bool operator !=(ShortPoint o1, ShortPoint o2)
        {
            return !o1.Equals(o2);
        }

        public bool Equals(ShortPoint other)
        {
            return other.X == X && other.Y == Y;
        }

        public override bool Equals(object other)
        {
            return other is TinySize && Equals((TinySize) other);
        }

        public override int GetHashCode()
        {
            return Mark;
        }

        public Point ToPoint()
        {
            return new Point(X, Y);
        }

        public Point ToPoint(int pixelSize)
        {
            return new Point(X * pixelSize, Y * pixelSize);
        }
    }

    public static class Extensions
    {
        public static bool EqualsTo(this Point source, int x, int y)
        {
            return source.X == x && source.Y == y;
        }

        public static ShortPoint ToShortPoint(this Point source)
        {
            return new ShortPoint(source);
        }
    }
}
