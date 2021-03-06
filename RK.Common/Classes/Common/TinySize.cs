﻿using System;
using System.Runtime.InteropServices;

namespace RK.Common.Classes.Common
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TinySize : IEquatable<TinySize>
    {
        public byte Width;
        public byte Height;

        public bool IsEmpty
        {
            get { return Width == 00 && Height == 0; }
        }

        public static TinySize Empty
        {
            get { return new TinySize(); }
        }

        public int Area
        {
            get { return Width*Height; }
        }

        public byte HighValue
        {
            get { return Math.Max(Width, Height); }
        }

        public TinySize(byte width, byte height)
        {
            Width = width;
            Height = height;
        }

        public TinySize(TinySize source)
        {
            Width = source.Width;
            Height = source.Height;
        }

        public static bool operator ==(TinySize o1, TinySize o2)
        {
            return o1.Equals(o2);
        }

        public static bool operator !=(TinySize o1, TinySize o2)
        {
            return !o1.Equals(o2);
        }

        public bool Equals(TinySize other)
        {
            return other.Width == Width && other.Height == Height;
        }

        public override bool Equals(object other)
        {
            return other is TinySize && Equals((TinySize) other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return
                    (Width*0x18D) ^
                    Height;
            }
        }

        public override string ToString()
        {
            return string.Format("W = {0}, H = {1}", Width, Height);
        }
    }
}
