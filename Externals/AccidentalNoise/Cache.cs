namespace AccidentalNoise
{
    public struct SCache
    {
        public double X, Y, Val;
        public bool Valid;
    }

    public class Cache : ModuleBase
    {
        private SCache _cache;

        public Cache(ModuleBase source)
        {
            Source = source;
        }

        public override double Get(double x, double y)
        {
            if (!_cache.Valid || _cache.X != x || _cache.Y != y)
            {
                _cache.X = x;
                _cache.Y = y;
                _cache.Valid = true;
                _cache.Val = Source.Get(x, y);
            }

            return _cache.Val;
        }
    }
}
