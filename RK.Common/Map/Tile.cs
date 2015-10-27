using System;
using System.Runtime.InteropServices;
using RK.Common.Classes.Common;
using RK.Common.Win32;

namespace RK.Common.Map
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

#endregion

#region Properties

        public static Tile Empty
        {
            get { return new Tile();}
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
                return ((int) Type*0x18D) ^
                       (TypeIndex*0x18D) ^
                       Flags;
            }
        }

        public int GetHashCode(TileType newType)
        {
            unchecked
            {
                return ((int) newType*0x18D) ^
                       (TypeIndex*0x18D) ^
                       Flags;
            }
        }

        public int GetHashCode(byte newTypeIndex)
        {
            unchecked
            {
                return ((int) Type*0x18D) ^
                       (newTypeIndex*0x18D) ^
                       Flags;
            }
        }

        public int GetHashCode(int newFlags)
        {
            unchecked
            {
                return ((int) Type*0x18D) ^
                       (TypeIndex*0x18D) ^
                       newFlags;
            }
        }

#endregion

#region ISerializable

        public int SizeOf()
        {
            return sizeof (TileType) +
                   sizeof (byte) +
                   sizeof (int);
        }

        public void Serialize(byte* bData, ref int pos)
        {
            Serializer.Write(bData, Type, ref pos);
            Serializer.Write(bData, TypeIndex, ref pos);
            Serializer.Write(bData, Flags, ref pos);
        }

        public void Deserialize(byte* bData, ref int pos)
        {
            Serializer.Read(bData, out Type, ref pos);
            Serializer.Read(bData, out TypeIndex, ref pos);
            Serializer.Read(bData, out Flags, ref pos);
        }

#endregion

    }
}
