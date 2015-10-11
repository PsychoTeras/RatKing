using System;
using GMechanics.Core.Classes.Types;

namespace GMechanics.Core.Helpers
{
    internal unsafe static class TypesConverter
    {
        private static byte[] FloatToBytes(float value)
        {
            int i = *(int*)&value;
            return new[] { (byte) (i & 0xFF),
                           (byte) ((i >> 8 ) & 0xFF),
                           (byte) ((i >> 16) & 0xFF),
                           (byte) ((i >> 24) & 0xFF)
                         };
        }

        private static float BytesToFloat(byte[] value)
        {
            int i = value[0] << 0  | value[1] << 8 |
                    value[2] << 16 | value[3] << 24;
            return *(float*)&i;
        }

        public static byte[] Size3DToBytes(Size3D size3D)
        {
            byte[] result = new byte[16];
            Array.Copy(FloatToBytes(size3D.X), 0, result, 0, 4);
            Array.Copy(FloatToBytes(size3D.Y), 0, result, 4, 4);
            Array.Copy(FloatToBytes(size3D.Z), 0, result, 8, 4);
            return result;
        }

        public static Size3D BytesToSize3D(byte[] bytes)
        {
            Size3D result = new Size3D();
            result.X = BytesToFloat(bytes);
            Array.Copy(bytes, 4, bytes, 0, 4);
            result.Y = BytesToFloat(bytes);
            Array.Copy(bytes, 8, bytes, 0, 4);
            result.Z = BytesToFloat(bytes);
            return result;
        }

        public static byte[] Location3DToBytes(Location3D location3D)
        {
            byte[] result = new byte[16];
            Array.Copy(FloatToBytes(location3D.X), 0, result, 0, 4);
            Array.Copy(FloatToBytes(location3D.Y), 0, result, 4, 4);
            Array.Copy(FloatToBytes(location3D.Z), 0, result, 8, 4);
            return result;
        }

        public static Location3D BytesToLocation3D(byte[] bytes)
        {
            Location3D result = new Location3D();
            result.X = BytesToFloat(bytes);
            Array.Copy(bytes, 4, bytes, 0, 4);
            result.Y = BytesToFloat(bytes);
            Array.Copy(bytes, 8, bytes, 0, 4);
            result.Z = BytesToFloat(bytes);
            return result;
        }
    }
}
