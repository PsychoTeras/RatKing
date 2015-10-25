using System;
using System.Runtime.InteropServices;
using RK.Common.Common;
using RK.Common.Win32;

namespace RK.Common.Classes.Map
{
    public enum TileType : byte
    {
        Undefined,
        Nothing,
        Wall
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Tile : IEquatable<Tile>, ISerializable
    {

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

        public long Mark
        {
            get
            {
                unchecked
                {
                    int mark = (int) Type;
                    mark = (mark * 0x18D) ^ TypeIndex;
                    mark = (mark * 0x18D) ^ Flags;
                    mark = (mark * 0x18D) ^ RTFlags;
                    return mark;
                }
            }
        }

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

        public bool MarkEquals(ref Tile other)
        {
            return other.Type == Type &&
                   other.TypeIndex == TypeIndex &&
                   other.Flags == Flags &&
                   other.RTFlags == RTFlags;
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
                hashCode = (hashCode*0x18D) ^ TypeIndex.GetHashCode();
                hashCode = (hashCode*0x18D) ^ Flags.GetHashCode();
                return hashCode;
            }
        }

#endregion

#region ISerializable

        public int SizeOf()
        {
            return sizeof (TileType) +
                   sizeof (byte) +
                   sizeof (int) +
                   sizeof (int);
        }

        public void Serialize(byte* bData, ref int pos)
        {
            Serializer.Write(bData, Type, ref pos);
            Serializer.Write(bData, TypeIndex, ref pos);
            Serializer.Write(bData, Flags, ref pos);
            Serializer.Write(bData, RTFlags, ref pos);
        }

        public void Deserialize(byte* bData, ref int pos)
        {
            Serializer.Read(bData, out Type, ref pos);
            Serializer.Read(bData, out TypeIndex, ref pos);
            Serializer.Read(bData, out Flags, ref pos);
            Serializer.Read(bData, out RTFlags, ref pos);
        }

#endregion

    }
}
