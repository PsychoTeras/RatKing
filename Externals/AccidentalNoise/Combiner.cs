using System;

namespace AccidentalNoise
{
    public enum CombinerTypes
    {
        ADD,
        MULT,
        MAX,
        MIN,
        AVG
    }

    public class Combiner : ModuleBase
    {
        private CombinerTypes _type;
        private ModuleBase _source2;

        public Combiner(CombinerTypes type, ModuleBase source1, ModuleBase source2)
        {
            _type = type;
            Source = source1;
            _source2 = source2;
        }

        public override double Get(double x, double y)
        {
            switch (_type)
            {
                case CombinerTypes.ADD:
                    return Add_Get(x, y);
                case CombinerTypes.MULT:
                    return Mult_Get(x, y);
                case CombinerTypes.MAX:
                    return Max_Get(x, y);
                case CombinerTypes.MIN:
                    return Min_Get(x, y);
                case CombinerTypes.AVG:
                    return Avg_Get(x, y);
                default:
                    throw new Exception();
            }
        }

        private double Add_Get(double x, double y)
        {
            return Source.Get(x, y) + _source2.Get(x, y);
        }

        private double Mult_Get(double x, double y)
        {
            double value = 1;
            value *= Source.Get(x, y);
            value *= _source2.Get(x, y);

            return value;
        }

        private double Max_Get(double x, double y)
        {
            double s1 = Source.Get(x, y);
            double s2 = _source2.Get(x, y);
            return s1 > s2 ? s1 : s2;
        }

        private double Min_Get(double x, double y)
        {
            double s1 = Source.Get(x, y);
            double s2 = _source2.Get(x, y);
            return s1 < s2 ? s1 : s2;
        }

        private double Avg_Get(double x, double y)
        {
            double val = Source.Get(x, y) + _source2.Get(x, y);
            return val/2.0f;
        }
    }
}