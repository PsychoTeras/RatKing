using System;
using System.Runtime.InteropServices;

namespace RK.Common.Common
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ShortRect : IEquatable<ShortRect>
    {
        public ushort X;
        public ushort Y;
        public ushort Width;
        public ushort Height;

        public ushort Left
        {
            get { return X; }
        }

        public ushort Right
        {
            get { return (ushort) (X + Width); }
        }

        public ushort Top
        {
            get { return Y; }
        }

        public ushort Bottom
        {
            get { return (ushort)(Y + Height); }
        }

        public ShortSize Size 
        {
            get { return new ShortSize(Width, Height); }
        }

        public static ShortRect Empty
        {
            get { return new ShortRect(); }
        }

        public ShortRect(int x, int y, int w, int h)
        {
            if (x < 0 || x > ushort.MaxValue || y < 0 || y > ushort.MaxValue ||
                w < 0 || w > ushort.MaxValue || h < 0 || h > ushort.MaxValue ||
                x + w > ushort.MaxValue || y + h > ushort.MaxValue)
            {
                throw new Exception("Params are out of range");
            }
            X = (ushort) x;
            Y = (ushort) y;
            Width = (ushort) w;
            Height = (ushort) h;
        }

        public ShortRect(ushort x, ushort y, ushort w, ushort h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        public ShortRect(ShortRect source)
        {
            X = source.X;
            Y = source.Y;
            Width = source.Width;
            Height = source.Height;
        }

        public static bool operator ==(ShortRect o1, ShortRect o2)
        {
            return o1.Equals(o2);
        }

        public static bool operator !=(ShortRect o1, ShortRect o2)
        {
            return !o1.Equals(o2);
        }

        public bool Equals(ShortRect other)
        {
            return other.X == X && other.Y == Y && other.Width == Width && other.Height == Height;
        }

        public override bool Equals(object other)
        {
            return other is TinySize && Equals((TinySize) other);
        }

        public override int GetHashCode()
        {
            return (X.GetHashCode()*0x18D) ^
                   (Y.GetHashCode()*0x18D) ^
                   (Width.GetHashCode()*0x18D) ^
                   (Height.GetHashCode());
        }

        public override string ToString()
        {
            return string.Format("X = {0}, Y = {1}, W = {2}, H = {3}", X, Y, Width, Height);
        }
    }
}
