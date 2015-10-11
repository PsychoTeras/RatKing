using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Interfaces;
using GMechanics.Core.Helpers;

namespace GMechanics.Core.Classes.Storages.BaseStorages
{
    public class BaseContainer<TObjType, TParentType> : 
        Dictionary<string, TObjType>, IBaseContainer
        where TObjType : Atom
        where TParentType : class, new()
    {
        private static readonly TParentType StaticInstance = new TParentType();

        internal BaseContainer() {}

        internal BaseContainer(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public static TParentType Instance
        {
            get { return StaticInstance; }
        }

        public virtual string[] AtomsNamesList
        {
            get
            {
                List<string> names = new List<string>(Values.Count);
                foreach (TObjType obj in Values)
                {
                    names.Add(obj.Name);
                }
                names.Sort();
                return names.ToArray();
            }
        }

        public virtual TObjType[] ToArray()
        {
            List<TObjType> list = new List<TObjType>();
            foreach (TObjType obj in Values)
            {
                list.Add(obj);
            }
            return list.ToArray();
        }

        public new virtual TObjType this[string name]
        {
            get
            {
                return !string.IsNullOrEmpty(name) && ContainsKey(name)
                           ? base[name]
                           : null;
            }
            set
            {
                Remove(name);
                if (value != null)
                {
                    Add(value);
                }
            }
        }

        public virtual TObjType this[long objectId]
        {
            get { return Values.FirstOrDefault(o => o.ObjectId == objectId); }
        }

        public virtual bool Add(TObjType obj)
        {
            if (obj != null && !string.IsNullOrEmpty(obj.Name) &&
                !ContainsKey(obj.Name))
            {
                Add(obj.Name, obj);
                return true;
            }
            return false;
        }

        public virtual void AddRange(IEnumerable<TObjType> list)
        {
            foreach (TObjType obj in list)
            {
                Add(obj);
            }
        }

        public virtual void AddRange(BaseContainer<TObjType, TParentType> list)
        {
            if (list != null)
            {
                foreach (TObjType obj in list.Values)
                {
                    Add(obj);
                }
            }
        }

        public bool Remove(string name, bool destroy)
        {
            if (!string.IsNullOrEmpty(name) && ContainsKey(name))
            {
                TObjType obj = this[name];
                if (obj != null)
                {
                    if (destroy)
                    {
                        obj.Destroy();
                    }
                    base.Remove(name);
                    return true;
                }
            }
            return false;
        }

        public new bool Remove(string name)
        {
            return Remove(name, true);
        }

        public virtual bool Remove(TObjType obj, bool destroy)
        {
            if (obj != null)
            {
                if (destroy)
                {
                    obj.Destroy();
                }
                base.Remove(obj.Name);
                return true;
            }
            return false;
        }

        public virtual bool Remove(TObjType obj)
        {
            return Remove(obj, true);
        }

        public virtual bool ChangeName(string oldName)
        {
            if (!string.IsNullOrEmpty(oldName))
            {
                TObjType obj = this[oldName];
                if (obj != null)
                {
                    Remove(oldName, false);
                    Add(obj);
                }
            }
            return false;
        }

        public virtual void RemoveDestroyedItems()
        {
            List<string> forRemoving = new List<string>();
            foreach (string key in Keys)
            {
                TObjType obj = this[key];
                if (obj.IsDestroyed)
                {
                    forRemoving.Add(key);
                }
            }
            foreach (string key in forRemoving)
            {
                Remove(key, false);
            }
        }

        public virtual byte[] Serialize()
        {
            Serializer<TObjType> serializer = new Serializer<TObjType>();
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    foreach (TObjType obj in Values)
                    {
                        byte[] data = serializer.SerializeObject(obj);
                        bw.Write(data.Length);
                        bw.Write(data);
                    }
                    return ms.ToArray();
                }
            }
        }

        public virtual bool Deserialize(byte[] data)
        {
            Clear();
            Serializer<TObjType> serializer = new Serializer<TObjType>();
            try
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        while (ms.Position < ms.Length)
                        {
                            int length = br.ReadInt32();
                            byte[] objData = new byte[length];
                            br.Read(objData, 0, length);
                            TObjType obj = serializer.DeserializeObject(objData);
                            if (obj != null)
                            {
                                Add(obj.Name, obj);
                            }
                        }
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}