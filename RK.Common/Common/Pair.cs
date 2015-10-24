namespace RK.Common.Common
{
    public struct Pair<TK, TV>
    {
        public TK Key;
        public TV Value;

        public Pair(TK key, TV value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("[{0}], [{1}]", Key, Value);
        } 
    }

    public unsafe struct PairP1<TV>
    {
        public void* Key;
        public TV Value;

        public PairP1(void* key, TV value)
        {
            Key = key;
            Value = value;
        }
    }

    public unsafe struct PairP2<TK>
    {
        public TK Key;
        public void* Value;

        public PairP2(TK key, void* value)
        {
            Key = key;
            Value = value;
        }
    }
}
