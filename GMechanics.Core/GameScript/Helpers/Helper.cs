using System;
using System.Globalization;
using System.IO;
using GMechanics.Core.GameScript.Classes;
using GMechanics.Core.GameScript.Runtime;

namespace GMechanics.Core.GameScript.Helpers
{
    public static class Helper
    {
        public delegate string GetValue(int idx);

        public static void WriteBinaryIndex(BinaryWriter bw, object obj, SortedStringsSet indexes)
        {
            bool isNotNull = obj != null && obj != NullReference.Instance;
            bw.Write((short)(isNotNull ? indexes[obj.ToString()] : -1));
        }

        public static void WriteTypedBinaryIndex(BinaryWriter bw, object obj, SortedStringsSet indexes)
        {
            sbyte typeIdx = -1;
            if (obj != null)
            {
                typeIdx = (sbyte) (obj == NullReference.Instance ? 0 : Type.GetTypeCode(obj.GetType()));
            }
            bw.Write(typeIdx);
            if (obj != null && obj != NullReference.Instance)
            {
                bw.Write(indexes[obj.ToString()]);
            }
        }

        public static short ReadBinaryIndex(BinaryReader br)
        {
            return br.ReadInt16();
        }

        public static short ReadTypedBinaryIndex(BinaryReader br, out sbyte type)
        {
            type = br.ReadSByte();
            return (short) (type > 0 ? br.ReadInt16() : -1);
        }

        public static string ReadBinaryString(BinaryReader br, GetValue getValueFunc)
        {
            short idx = ReadBinaryIndex(br);
            return getValueFunc(idx);
        }

        public static object ReadTypedBinaryObject(BinaryReader br, GetValue getValueFunc)
        {
            sbyte type;
            short idx = ReadTypedBinaryIndex(br, out type);
            switch (type)
            {
                case -1:
                    return null;
                case 0:
                    return NullReference.Instance;
                default:
                    {
                        string str = getValueFunc(idx);
                        switch ((TypeCode) type)
                        {
                            case TypeCode.Boolean:
                                return Boolean.Parse(str);
                            case TypeCode.Int32:
                                return Int32.Parse(str);
                            case TypeCode.Single:
                                return float.Parse(str.Replace(",", "."), CultureInfo.InvariantCulture.NumberFormat); //!!! Replace
                            case TypeCode.String:
                                return str;
                            case TypeCode.Char:
                                return Char.Parse(str);
                            default:
                                throw new Exception("Projoban type code...");
                        }
                    }
            }
        }
    }
}