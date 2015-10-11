using System;
using System.Collections.Generic;
using GMechanics.Core.Helpers;

namespace GMechanics.Core.GameScript.Classes
{
    public class SortedStringsSet : List<uint>
    {
        private bool _isSorted;
        private readonly object _lock = new object();
        private readonly HashSet<uint> _hashSet = new HashSet<uint>();

        protected int GetIndex(uint hash)
        {
            int size = Count - 1, l = 0;
            while (l <= size)
            {
                int m = (l + size) / 2;
                uint h = this[m];
                if (hash < h)
                    size = m - 1;
                else 
                if (hash > h)
                    l = m + 1;
                else 
                    return m;
            }
            return -1;
        }

        public new void Add(uint item)
        {
            throw new NotImplementedException();
        }

        public bool Add(string str)
        {
            uint hash = Helper.HashX65599(str);
            if (_hashSet.Add(hash))
            {
                base.Add(hash);
                return true;
            }
            return false;
        }

        public new void Remove(uint item)
        {
            base.Remove(item);
            _hashSet.Remove(item);
        }

        public void Remove(string str)
        {
            uint hash = Helper.HashX65599(str);
            Remove(hash);
            _hashSet.Remove(hash);
        }

        public new void Clear()
        {
            base.Clear();
            _hashSet.Clear();
        }

        public short this[string str]
        {
            get
            {
                lock (_lock)
                {
                    if (!_isSorted)
                    {
                        Sort();
                        _isSorted = true;
                    }
                    uint hash = Helper.HashX65599(str);
                    return (short) GetIndex(hash);
                }
            }
        }
    }
}
