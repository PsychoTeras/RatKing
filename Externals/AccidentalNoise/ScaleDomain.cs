namespace AccidentalNoise
{
    public class ScaleDomain : ModuleBase
    {
        private double _sx = 1;
        private double _sy = 1;

        public ScaleDomain(ModuleBase source, double? x, double? y)
        {
            if (x != null)
                _sx = (double) x;

            if (y != null)
                _sy = (double) y;

            Source = source;
        }

        public override double Get(double x, double y)
        {
            return Source.Get(x*_sx, y*_sy);
        }
    }
}
