using System.Collections.Generic;
using RK.Common.Classes.Common;

namespace RK.Common.Win32
{
    public unsafe static class Serializer
    {
        public static int StringLength(string str)
        {
            const int sSize = sizeof(short);
            short strLen = (short)(str == null ? -1 : str.Length * sizeof(char));
            return strLen > 0 ? sSize + strLen : sSize;
        }

        public static int WriteString(byte* bData, string str, int pos)
        {
            const int sSize = sizeof(short);
            short strLen = (short)(str == null ? -1 : str.Length * sizeof(char));
            (*(short*)&bData[pos]) = strLen;
            if (strLen > 0)
            {
                fixed (char* c = str)
                {
                    Memory.Copy(&bData[pos + sSize], c, strLen);
                }
            }
            return strLen > 0 ? sSize + strLen : sSize;
        }

        public static int ReadString(byte* bData, out string str, int pos)
        {
            const int sSize = sizeof(short);
            short strLen = *(short*)&bData[pos];
            if (strLen == -1)
                str = null;
            else if (strLen == 0)
                str = "";
            else
            {
                str = new string((char*)&bData[pos + sSize], 0, strLen / sizeof(char));
            }
            return strLen > 0 ? sSize + strLen : sSize;
        }

        // ReSharper disable once PossibleNullReferenceException
        public static int CollectionLength<T>(IList<T> col)
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

        // ReSharper disable once PossibleNullReferenceException
        public static int WriteCollection<T>(IList<T> col, byte* bData, int pos)
            where T : ISerializable
        {
            const int iSize = sizeof (int);
            int elsCount = (short) (col == null ? -1 : col.Count), elsSize = 0;
            (*(int*)&bData[pos]) = elsCount;
            for (int i = 0; i < elsCount; i++)
            {
                elsSize += col[i].Serialize(bData, pos + iSize + elsSize);
            }
            return iSize + elsSize;
        }

        public static int ReadCollection<T, TC>(byte* bData, out TC col, int pos)
            where T : ISerializable, new() where TC: class, IList<T>, new()
        {
            const int iSize = sizeof(int);
            int elsCount = *(int*)&bData[pos], elsSize = 0;
            if (elsCount == -1)
                col = null;
            else
            {
                col = new TC();
                for (int i = 0; i < elsCount; i++)
                {
                    T obj = new T();
                    elsSize += obj.Deserialize(bData, pos + iSize + elsSize);
                    col.Add(obj);
                }
            }
            return iSize + elsSize;
        }
    }
}
