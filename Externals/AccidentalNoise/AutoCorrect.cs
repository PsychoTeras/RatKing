using System;

namespace AccidentalNoise
{
    public class AutoCorrect : ModuleBase
    {
        private double _low, _high;
        private double _scale2, _offset2;

        public AutoCorrect(ModuleBase source, double low, double high)
        {
            Source = source;
            _low = low;
            _high = high;

            Calculate();
        }

        public override double Get(double x, double y)
        {
            double v = Source.Get(x, y);

            return Math.Max(_low, Math.Min(_high, v*_scale2 + _offset2));
        }

        public void Calculate()
        {
            Random r = new Random();

            // Calculate 2D
            double mn = 10000.0;
            double mx = -10000.0;
            for (int c = 0; c < 10000; ++c)
            {
                double nx = r.NextDouble()*4.0 - 2.0;
                double ny = r.NextDouble()*4.0 - 2.0;

                double v = Source.Get(nx, ny);
                if (v < mn) mn = v;
                if (v > mx) mx = v;
            }

            _scale2 = (_high - _low)/(mx - mn);
            _offset2 = _low - mn*_scale2;
        }
    }
}