using System.Collections.Generic;
using System.IO;

namespace GMechanics.Core.GameScript.Classes
{
    internal class ObjectsDictionaryHashSet
    {
        public HashSet<string> Vars = new HashSet<string>();
        public HashSet<string> Idxs = new HashSet<string>();
        public HashSet<string> Fncs = new HashSet<string>();
        public HashSet<string> Lits = new HashSet<string>();

        private void Write(HashSet<string> hashSet, BinaryWriter bw)
        {
            bw.Write((ushort)hashSet.Count);
            foreach (string val in hashSet)
            {
                bw.Write(val);
            }
        }

        public void Serialize(BinaryWriter bw)
        {
            Write(Vars, bw);
            Write(Idxs, bw);
            Write(Fncs, bw);
            Write(Lits, bw);
        }
    }
}
