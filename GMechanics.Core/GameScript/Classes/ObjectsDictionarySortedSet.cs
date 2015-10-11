using System.Collections.Generic;

namespace GMechanics.Core.GameScript.Classes
{
    internal class ObjectsDictionarySortedSet
    {
        public SortedStringsSet Vars = new SortedStringsSet();
        public SortedStringsSet Idxs = new SortedStringsSet();
        public SortedStringsSet Fncs = new SortedStringsSet();
        public SortedStringsSet Lits = new SortedStringsSet();

        private void Fill(SortedStringsSet list, HashSet<string> hashSet)
        {
            foreach (string val in hashSet)
            {
                list.Add(val);
            }
        }

        public ObjectsDictionarySortedSet(ObjectsDictionaryHashSet hashSet)
        {
            Fill(Vars, hashSet.Vars);
            Fill(Idxs, hashSet.Idxs);
            Fill(Fncs, hashSet.Fncs);
            Fill(Lits, hashSet.Lits);
        }
    }
}
