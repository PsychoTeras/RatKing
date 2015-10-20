using System.Collections.Generic;
using System.Drawing;
using RK.Common.Common;
using RK.Common.Proto;

namespace RK.Common.Win32
{
    public unsafe static class Serializer
    {
        public static int Length(string str)
        {
            const int sSize = sizeof(short);
            short strLen = (short)(str == null ? -1 : str.Length * sizeof(char));
            return strLen > 0 ? sSize + strLen : sSize;
        }

        // ReSharper disable once PossibleNullReferenceException
        public static int Length<T>(IList<T> col) 
            where T : ISerializable
        {
            const int iSize = sizeof(int);
            int elsCount = (short)(col == null ? -1 : col.Count), elsSize = 0;
            for (int i = 0; i < elsCount; i++)
            {
                elsSize += col[i].SizeOf();
            }
            return iSize + elsSize;
        }

        public static void Read(byte* bData, out long val, ref int pos)
        {
            val = *(long*)&bData[pos];
            pos += sizeof(long);
        }

        public static void Read(byte* bData, out int val, ref int pos)
        {
            val = *(int*)&bData[pos];
            pos += sizeof(int);
        }

        public static void Read(byte* bData, out short val, ref int pos)
        {
            val = *(short*)&bData[pos];
            pos += sizeof(short);
        }

        public static void Read(byte* bData, out ushort val, ref int pos)
        {
            val = *(ushort*)&bData[pos];
            pos += sizeof(ushort);
        }

        public static void Read(byte* bData, out float val, ref int pos)
        {
            val = *(float*)&bData[pos];
            pos += sizeof(float);
        }

        public static void Read(byte* bData, out TinySize val, ref int pos)
        {
            val = *(TinySize*)&bData[pos];
            pos += sizeof(TinySize);
        }

        public static void Read(byte* bData, out Point val, ref int pos)
        {
            val = *(Point*)&bData[pos];
            pos += sizeof(Point);
        }

        public static void Read(byte* bData, out PacketType val, ref int pos)
        {
            val = *(PacketType*)&bData[pos];
            pos += sizeof(PacketType);
        }

        public static void Read(byte* bData, out Direction val, ref int pos)
        {
            val = *(Direction*)&bData[pos];
            pos += sizeof(Direction);
        }

        public static void Read(byte* bData, out string str, ref int pos)
        {
            short strLen = *(short*)&bData[pos];
            pos += sizeof(short);
            if (strLen == -1)
                str = null;
            else if (strLen == 0)
                str = "";
            else
            {
                str = new string((char*)&bData[pos], 0, strLen / sizeof(char));
                pos += strLen;
            }
        }

        public static void Read<T, TC>(byte* bData, out TC col, ref int pos)
            where T : ISerializable, new() where TC: class, IList<T>, new()
        {
            int elsCount = *(int*)&bData[pos];
            pos += sizeof (int);
            if (elsCount == -1)
                col = null;
            else
            {
                col = new TC();
                for (int i = 0; i < elsCount; i++)
                {
                    T obj = new T();
                    obj.Deserialize(bData, ref pos);
                    col.Add(obj);
                }
            }
        }

        public static void Write(byte* bData, long val, ref int pos)
        {
            (*(long*)&bData[pos]) = val;
            pos += sizeof(long);
        }

        public static void Write(byte* bData, int val, ref int pos)
        {
            (*(int*)&bData[pos]) = val;
            pos += sizeof (int);
        }

        public static void Write(byte* bData, short val, ref int pos)
        {
            (*(short*)&bData[pos]) = val;
            pos += sizeof(short);
        }

        public static void Write(byte* bData, ushort val, ref int pos)
        {
            (*(ushort*)&bData[pos]) = val;
            pos += sizeof(ushort);
        }

        public static void Write(byte* bData, float val, ref int pos)
        {
            (*(float*)&bData[pos]) = val;
            pos += sizeof(float);
        }

        public static void Write(byte* bData, TinySize val, ref int pos)
        {
            (*(TinySize*)&bData[pos]) = val;
            pos += sizeof(TinySize);
        }

        public static void Write(byte* bData, Point val, ref int pos)
        {
            (*(Point*)&bData[pos]) = val;
            pos += sizeof(Point);
        }

        public static void Write(byte* bData, PacketType val, ref int pos)
        {
            (*(PacketType*)&bData[pos]) = val;
            pos += sizeof(PacketType);
        }

        public static void Write(byte* bData, Direction val, ref int pos)
        {
            (*(Direction*)&bData[pos]) = val;
            pos += sizeof(Direction);
        }

        public static void Write(byte* bData, string str, ref int pos)
        {
            short strLen = (short)(str == null ? -1 : str.Length * sizeof(char));
            (*(short*)&bData[pos]) = strLen;
            pos += sizeof(short);
            if (strLen > 0)
            {
                fixed (char* c = str)
                {
                    Memory.Copy(&bData[pos], c, strLen);
                }
                pos += strLen;
            }
        }

        // ReSharper disable once PossibleNullReferenceException
        public static void Write<T>(byte* bData, IList<T> col, ref int pos)
            where T : ISerializable
        {
            int elsCount = col == null ? -1 : col.Count;
            (*(int*)&bData[pos]) = elsCount;
            pos += sizeof(int);
            for (int i = 0; i < elsCount; i++)
            {
                col[i].Serialize(bData, ref pos);
            }
        }
    }
}
