namespace RK.Common.Classes
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
}
