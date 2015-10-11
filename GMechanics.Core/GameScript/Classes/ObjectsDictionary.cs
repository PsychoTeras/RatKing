using System.Collections.Generic;
using System.IO;

namespace GMechanics.Core.GameScript.Classes
{
    public class ObjectsDictionary
    {
        private readonly List<string> _vars;
        private readonly List<string> _idxs;
        private readonly List<string> _fncs;
        private readonly List<string> _lits;

        private void InitializeAndReadList(out List<string> list, BinaryReader br)
        {
            int count = br.ReadUInt16();

            list = new List<string>(count);
            List<string> tmpList = new List<string>(count);
            SortedStringsSet sortedSet = new SortedStringsSet();

            for (int i = 0; i < count; i++)
            {
                string str = br.ReadString();
                sortedSet.Add(str);
                tmpList.Add(str);
                list.Add(null);
            }
            
            for (int i = 0; i < count; i++)
            {
                int idx = sortedSet[tmpList[i]];
                list[idx] = tmpList[i];
            }
        }

        public ObjectsDictionary(BinaryReader br)
        {
            InitializeAndReadList(out _vars, br);
            InitializeAndReadList(out _idxs, br);
            InitializeAndReadList(out _fncs, br);
            InitializeAndReadList(out _lits, br);
        }

        public string Vars(int idx)
        {
            return idx != -1 ? _vars[idx] : null;
        }

        public string Idxs(int idx)
        {
            return idx != -1 ? _idxs[idx] : null;
        }

        public string Fncs(int idx)
        {
            return idx != -1 ? _fncs[idx] : null;
        }

        public string Lits(int idx)
        {
            return idx != -1 ? _lits[idx] : null;
        }
    }
}
