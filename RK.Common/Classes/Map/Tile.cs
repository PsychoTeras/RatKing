﻿using System;
using System.Runtime.InteropServices;

namespace RK.Common.Classes.Map
{
    public enum TileType : byte
    {
        Undefined,
        Nothing,
        Wall
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Tile : IEquatable<Tile>
    {

#region Static fields
        
        public static readonly int SizeOf = Marshal.SizeOf(typeof(Tile));

#endregion

#region Public fields

        public TileType Type;
        public byte TypeIndex;

        public int Flags;

        //Flags for real-time using. 
        //4 bits for wall borders. 0 - clear. 1 - left. 2 - right. 3. top. 4. bottom.
        public int RTFlags;

#endregion

#region Properties

        public static Tile Empty
        {
            get { return new Tile();}
        }

        public int Borders
        {
            get { return RTFlags & 0x0000000F; }
        }

#endregion

#region Flags operations

        public void FlagClearBorders()
        {
            RTFlags = (int)(RTFlags & 0xFFFFFFF0);
        }

        public void FlagSetBorders(byte borders)
        {
            RTFlags = (int)(RTFlags & 0xFFFFFFF0 | borders);
        }

#endregion

#region Class methods

        public static bool operator ==(Tile o1, Tile o2)
        {
            return o1.Equals(o2);
        }

        public static bool operator !=(Tile o1, Tile o2)
        {
            return !o1.Equals(o2);
        }

        public bool Equals(Tile other)
        {
            return other.Type == Type &&
                   other.TypeIndex == TypeIndex &&
                   other.Flags == Flags;
        }

        public override bool Equals(object other)
        {
            return other is Tile && Equals((Tile)other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Type.GetHashCode();
                hashCode = (hashCode*397) ^ TypeIndex.GetHashCode();
                hashCode = (hashCode*397) ^ Flags.GetHashCode();
                return hashCode;
            }
        }

#endregion

    }
}