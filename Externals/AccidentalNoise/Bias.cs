namespace AccidentalNoise
{
    public class Bias : ModuleBase
    {
        private double _bias;

        public Bias(ModuleBase source, double? bias)
        {
            Source = source;

            if (bias != null)
                _bias = (double) bias;
        }

        public override double Get(double x, double y)
        {
            double va = Source.Get(x, y);
            return Helper.Bias(_bias, va);
        }
    }
}