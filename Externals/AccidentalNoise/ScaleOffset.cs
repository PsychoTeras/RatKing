namespace AccidentalNoise
{
    public class ScaleOffset : ModuleBase
    {
        private double _scale, _offset;

        public ScaleOffset(double scale, double offset, ModuleBase mbase)
        {
            _scale = scale;
            _offset = offset;
            Source = mbase;
        }

        public override double Get(double x, double y)
        {
            return Source.Get(x, y)*_scale + _offset;
        }
    }
}