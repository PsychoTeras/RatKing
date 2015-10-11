using System;
using System.ComponentModel;
using System.Globalization;
using GMechanics.Core.Helpers;

namespace GMechanics.Core.Classes.Types
{
    [TypeConverter(typeof(Location3DConverter))]
    public class Location3D
    {

#region Properties

        [RefreshProperties(RefreshProperties.Repaint)]
        public float X { get; set; }

        [RefreshProperties(RefreshProperties.Repaint)]
        public float Y { get; set; }

        [RefreshProperties(RefreshProperties.Repaint)]
        public float Z { get; set; }

#endregion

#region Class functions

        public Location3D() { }

        public Location3D(Location3D location3D)
        {
            X = location3D.X;
            Y = location3D.Y;
            Z = location3D.Z;
        }

        public Location3D(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        internal byte[] ToBytes()
        {
            return TypesConverter.Location3DToBytes(this);
        }

        internal static Location3D FromBytes(byte[] bytes)
        {
            return TypesConverter.BytesToLocation3D(bytes);
        }

        public override string ToString()
        {
            return string.Format("{0} x {1} x {2}", X, Y, Z);
        }

        public static Location3D FromString(string str)
        {
            str = (str ?? string.Empty).Replace(" ", "").Replace("X", "x");
            string[] values = str.Split(new[] { "x" }, StringSplitOptions.RemoveEmptyEntries);
            if (string.IsNullOrEmpty(str) || values.Length != 3)
            {
                throw new Exception(string.Format("Incorrect input string '{0}'", str));
            }

            float x, y, z;
            if (!float.TryParse(values[0], out x) || !float.TryParse(values[1], out y) ||
                !float.TryParse(values[2], out z))
            {
                throw new Exception(string.Format("Incorrect input string '{0}'", str));
            }

            return new Location3D(x, y, z);
        }

#endregion

#region ShouldSerialize

        private bool ShouldSerializeX()
        {
            return false;
        }

        private bool ShouldSerializeY()
        {
            return false;
        }

        private bool ShouldSerializeZ()
        {
            return false;
        }

#endregion

    }

#region Location3DConverter

    public class Location3DConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type type)
        {
            if (type == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, type);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo info,
                                           object value)
        {
            string str = value as string;
            if (str != null)
            {
                return Location3D.FromString(str);
            }
            return base.ConvertFrom(context, info, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
                                         object value, Type destType)
        {
            if (destType == typeof(string) && value is Location3D)
            {
                return value.ToString();
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }

#endregion

}
