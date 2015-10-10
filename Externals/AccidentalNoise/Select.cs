namespace AccidentalNoise
{
    public class Select : ModuleBase
    {
        private double _lowD;
        private double _highD;
        private ModuleBase _lowM;
        private ModuleBase _highM;
        private double _threshold;
        private double _falloff;

        public Select(ModuleBase controlsource, double low, double high, double threshold, double? falloff)
        {
            Source = controlsource;
            _lowD = low;
            _highD = high;
            _threshold = threshold;

            if (falloff != null)
                _falloff = (double) falloff;
        }

        public Select(ModuleBase controlsource, ModuleBase low, ModuleBase high, double threshold, double? falloff)
        {
            Source = controlsource;
            _lowM = low;
            _highM = high;
            _threshold = threshold;

            if (falloff != null)
                _falloff = (double) falloff;
        }

        public override double Get(double x, double y)
        {
            double control = Source.Get(x, y);

            double low = _lowM == null ? _lowD : _lowM.Get(x, y);
            double high = _highM == null ? _highD : _highM.Get(x, y);

            if (_falloff > 0.0)
            {
                if (control < (_threshold - _falloff))
                    return low;
                if (control > (_threshold + _falloff))
                    return high;
                double lower = _threshold - _falloff;
                double upper = _threshold + _falloff;
                double blend = Helper.Quintic_Blend((control - lower)/(upper - lower));
                return Helper.Lerp(blend, low, high);
            }
            return control < _threshold ? low : high;
        }
    }
}