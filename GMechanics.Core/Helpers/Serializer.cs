using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace GMechanics.Core.Helpers
{
    public class Serializer<T> where T : class
    {
        private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

        public byte[] SerializeObject(object obj, Type type, MemoryStream ms, BinaryFormatter bf)
        {
            try
            {
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
                IEnumerable<PropertyInfo> properties = type.GetProperties(flags).
                    Where(pi => pi.CanRead && pi.CanWrite);
                foreach (PropertyInfo property in properties)
                {
                    object[] attributes = property.GetCustomAttributes(true);
                    if (!Array.Exists(attributes, a => a.ToString().Contains("XmlIgnore")))
                    {
                        Type propertyType = property.PropertyType;
                        object value = property.GetValue(obj, null);

                        if (propertyType.IsClass)
                        {
                            ms.WriteByte((byte)(value != null ? 1 : 0));
                        }

                        if (value != null)
                        {
                            if (Equals(Assembly.GetAssembly(propertyType), Assembly))
                            {
                                SerializeObject(value, propertyType, ms, bf);
                                IList list = value as IList;
                                if (list != null && !(list is Array))
                                {
                                    ushort count = (ushort) list.Count;
                                    byte[] bytes = BitConverter.GetBytes(count);
                                    ms.Write(bytes, 0, 2);
                                    if (count > 0)
                                    {
                                        Type elemType = list[0].GetType();
                                        bf.Serialize(ms, elemType);
                                        foreach (object elem in list)
                                        {
                                            SerializeObject(elem, elemType, ms, bf);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                bf.Serialize(ms, value);
                            }
                        }
                    }
                }
                return ms.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public byte[] SerializeObject(T obj)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    SerializeObject(obj, typeof(T), ms, bf);
                    return ms.ToArray();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object DeserializeObject(Type type, MemoryStream ms, BinaryFormatter bf)
        {
            try
            {
                object obj = Activator.CreateInstance(type);
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
                IEnumerable<PropertyInfo> properties = type.GetProperties(flags).
                    Where(pi => pi.CanWrite);
                foreach (PropertyInfo property in properties)
                {
                    object[] attributes = property.GetCustomAttributes(true);
                    if (!Array.Exists(attributes, a => a.ToString().Contains("XmlIgnore")))
                    {
                        Type propertyType = property.PropertyType;

                        int notNull = 1;
                        object value = null;
                        if (propertyType.IsClass)
                        {
                            notNull = ms.ReadByte();
                        }

                        if (notNull == 1)
                        {
                            value = Equals(Assembly.GetAssembly(propertyType), Assembly)
                                        ? DeserializeObject(propertyType, ms, bf)
                                        : bf.Deserialize(ms);
                            IList list = value as IList;
                            if (list != null && !(list is Array))
                            {
                                byte[] bytes = new byte[2];
                                ms.Read(bytes, 0, 2);
                                ushort count = BitConverter.ToUInt16(bytes, 0);
                                if (count > 0)
                                {
                                    Type elemType = (Type) bf.Deserialize(ms);
                                    for (int i = 0; i < count; i++)
                                    {
                                        list.Add(DeserializeObject(elemType, ms, bf));
                                    }
                                }
                            }
                        }

                        property.SetValue(obj, value, null);
                    }
                }
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public T DeserializeObject(byte[] data)
        {
            try
            {
                if (data != null && data.Length > 0)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        return (T) DeserializeObject(typeof(T), ms, bf);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }
    }
}