using System;
using System.Runtime.InteropServices;

namespace RK.Common.Common
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ShortSize : IEquatable<ShortSize>
    {
        public ushort Width;
        public ushort Height;

        public bool IsEmpty
        {
            get { return Width == 00 && Height == 0; }
        }

        public static ShortSize Empty
        {
            get { return new ShortSize(); }
        }

        public int Area
        {
            get { return Width*Height; }
        }

        public ushort HighValue
        {
            get { return Math.Max(Width, Height); }
        }

        public ShortSize(int width, int height)
        {
            if (width < 0 || width > ushort.MaxValue ||
                height < 0 || height > ushort.MaxValue)
            {
                throw new Exception("Params are out of range");
            }
            Width = (ushort) width;
            Height = (ushort) height;
        }

        public ShortSize(ushort width, ushort height)
        {
            Width = width;
            Height = height;
        }

        public ShortSize(ShortSize source)
        {
            Width = source.Width;
            Height = source.Height;
        }

        public static bool operator ==(ShortSize o1, ShortSize o2)
        {
            return o1.Equals(o2);
        }

        public static bool operator !=(ShortSize o1, ShortSize o2)
        {
            return !o1.Equals(o2);
        }

        public bool Equals(ShortSize other)
        {
            return other.Width == Width && other.Height == Height;
        }

        public override bool Equals(object other)
        {
            return other is ShortSize && Equals((ShortSize)other);
        }

        public override int GetHashCode()
        {
            return (Width.GetHashCode() * 0x18D) ^ Height.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("W = {0}, H = {1}", Width, Height);
        }
    }
}
