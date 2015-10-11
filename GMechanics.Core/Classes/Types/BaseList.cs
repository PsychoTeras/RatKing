using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using GMechanics.Core.Classes.Interfaces;

namespace GMechanics.Core.Classes.Types
{
    [Serializable]
    public abstract class BaseList<TObjType, TParentType> : List<TObjType>, IBaseList
        where TObjType : IClassAsAtom
        where TParentType : List<TObjType>, new()
    {
        [XmlIgnore]
        public new int Count
        {
            get { return base.Count; }
        }

        [XmlIgnore]
        public new int Capacity
        {
            get { return base.Capacity; }
            set { base.Capacity = value; }
        }

        [XmlIgnore]
        public new TObjType this[int index]
        {
            get { return base[index]; }
            set { base[index] = value; }
        }

        [XmlIgnore]
        public virtual TObjType this[string name]
        {
            get { return Find(v => v.ClassAsAtom.Name == name); }
        }

        [XmlIgnore]
        public virtual TObjType this[TObjType obj]
        {
            set
            {
                int idx = IndexOf(obj);
                if (idx != -1)
                {
                    this[idx] = value;
                }
            }
        }

        public override string ToString()
        {
            string result = string.Empty;
            foreach (TObjType obj in this)
            {
                bool lastValue = result.Length > 200;
                result += string.Format("{0}{1}", result == "" ? "" : ", ",
                    lastValue ? "..." : obj.ClassAsAtom.Name);
                if (lastValue)
                {
                    break;
                }
            }
            return string.Format("[{0}]", result);
        }

        public virtual void Destroy()
        {
            foreach (TObjType obj in this)
            {
                obj.Destroy();
            }
        }

        public virtual void RemoveDestroyedItems()
        {
            RemoveAll(obj => obj.ClassAsAtom == null || obj.ClassAsAtom.IsDestroyed);
        }

        public virtual TParentType Clone()
        {
            TParentType result = new TParentType();
            foreach (TObjType obj in this)
            {
                result.Add((TObjType) obj.ClassAsAtom.Clone());
            }
            return result;
        }
    }
}