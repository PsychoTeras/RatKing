using System;

namespace AccidentalNoise
{
    public enum FractalType
    {
        FBM,
        RIDGEDMULTI,
        BILLOW,
        MULTI,
        HYBRIDMULTI
    }

    public class Fractal : ModuleBase
    {
        private FractalType _type;
        private int _numoctaves;
        private double _h;
        private double _gain;
        private double _offset;
        private double _lacunarity;
        private double _frequency;
        private BasisFunction[] _basis = new BasisFunction[MaxSources];
        private double[] _exparray = new double[MaxSources];
        private double[,] _correct = new double[MaxSources, 2];

        public Fractal(FractalType type, BasisTypes basisType, InterpTypes interpType,
            int? octaves = null, double? frequency = null, uint? seed = null, 
            double? angle = null, double? lacunarity = null)
        {
            if (octaves == null)
            {
                octaves = 1;
            }

            if (frequency == null)
            {
                frequency = 1.0;
            }

            if (seed == null)
            {
                seed = 10000;
            }

            if (angle == null)
            {
                angle = 0;
            }

            if (lacunarity == null)
            {
                lacunarity = 2;
            }

            _frequency = frequency.Value;
            _lacunarity = lacunarity.Value;
            
            SetOctaves(octaves.Value);
            SetType(type);
            SetAllSourceTypes(basisType, interpType, seed.Value, angle.Value);
        }

        private void SetOctaves(int octaves)
        {
            if (octaves >= MaxSources)
                octaves = MaxSources - 1;
            _numoctaves = octaves;
        }

        private void SetType(FractalType type)
        {
            _type = type;

            switch (type)
            {
                case FractalType.FBM:
                    _h = 1.0;
                    _gain = 0;
                    _offset = 0;
                    fB_calcWeights();
                    break;
                case FractalType.RIDGEDMULTI:
                    _h = 0.9;
                    _gain = 2;
                    _offset = 1;
                    RidgedMulti_calcWeights();
                    break;
                case FractalType.BILLOW:
                    _h = 1;
                    _gain = 0;
                    _offset = 0;
                    Billow_calcWeights();
                    break;
                case FractalType.MULTI:
                    _h = 1;
                    _offset = 0;
                    _gain = 0;
                    Multi_calcWeights();
                    break;
                case FractalType.HYBRIDMULTI:
                    _h = 0.25;
                    _gain = 1;
                    _offset = 0.7;
                    HybridMulti_calcWeights();
                    break;
                default:
                    _h = 1.0;
                    _gain = 0;
                    _offset = 0;
                    fB_calcWeights();
                    break;
            }
        }

        private void SetAllSourceTypes(BasisTypes basisType, InterpTypes interp, uint seed,
                                       double angle)
        {
            for (int i = 0; i < MaxSources; i++)
            {
                _basis[i] = new BasisFunction(basisType, interp, (uint) (seed + i * 300), angle);
            }
        }

        #region Weight Calculations

        private void fB_calcWeights()
        {
            //std::cout << "Weights: ";
            for (int i = 0; i < MaxSources; ++i)
            {
                _exparray[i] = Math.Pow(_lacunarity, -i*_h);
            }

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            double minvalue = 0.0, maxvalue = 0.0;

            for (int i = 0; i < MaxSources; ++i)
            {
                minvalue += -1.0*_exparray[i];
                maxvalue += 1.0*_exparray[i];

                double a = -1.0, b = 1.0;
                double scale = (b - a)/(maxvalue - minvalue);
                double bias = a - minvalue*scale;
                _correct[i, 0] = scale;
                _correct[i, 1] = bias;
            }
        }

        private void RidgedMulti_calcWeights()
        {
            for (int i = 0; i < MaxSources; ++i)
            {
                _exparray[i] = Math.Pow(_lacunarity, -i*_h);
            }

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            double minvalue = 0.0, maxvalue = 0.0;
            for (int i = 0; i < MaxSources; ++i)
            {
                minvalue += (_offset - 1.0)*(_offset - 1.0)*_exparray[i];
                maxvalue += (_offset)*(_offset)*_exparray[i];

                double a = -1.0, b = 1.0;
                double scale = (b - a)/(maxvalue - minvalue);
                double bias = a - minvalue*scale;
                _correct[i, 0] = scale;
                _correct[i, 1] = bias;
            }
        }

        private void Billow_calcWeights()
        {
            for (int i = 0; i < MaxSources; ++i)
            {
                _exparray[i] = Math.Pow(_lacunarity, -i*_h);
            }

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            double minvalue = 0.0, maxvalue = 0.0;
            for (int i = 0; i < MaxSources; ++i)
            {
                minvalue += -1.0*_exparray[i];
                maxvalue += 1.0*_exparray[i];

                double a = -1.0, b = 1.0;
                double scale = (b - a)/(maxvalue - minvalue);
                double bias = a - minvalue*scale;
                _correct[i, 0] = scale;
                _correct[i, 1] = bias;
            }
        }

        private void Multi_calcWeights()
        {
            for (int i = 0; i < MaxSources; ++i)
                _exparray[i] = Math.Pow(_lacunarity, -i*_h);

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            double minvalue = 1.0, maxvalue = 1.0;
            for (int i = 0; i < MaxSources; ++i)
            {
                minvalue *= -1.0*_exparray[i] + 1.0;
                maxvalue *= 1.0*_exparray[i] + 1.0;

                double a = -1.0, b = 1.0;
                double scale = (b - a)/(maxvalue - minvalue);
                double bias = a - minvalue*scale;
                _correct[i, 0] = scale;
                _correct[i, 1] = bias;
            }

        }

        private void HybridMulti_calcWeights()
        {
            for (int i = 0; i < MaxSources; ++i)
            {
                _exparray[i] = Math.Pow(_lacunarity, -i*_h);
            }

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            double a = -1.0, b = 1.0;

            double minvalue = _offset - 1.0;
            double maxvalue = _offset + 1.0;
            double weightmin = _gain*minvalue;
            double weightmax = _gain*maxvalue;

            double scale = (b - a)/(maxvalue - minvalue);
            double bias = a - minvalue*scale;
            _correct[0, 0] = scale;
            _correct[0, 1] = bias;

            for (int i = 1; i < MaxSources; ++i)
            {
                if (weightmin > 1.0) weightmin = 1.0;
                if (weightmax > 1.0) weightmax = 1.0;

                double signal = (_offset - 1.0)*_exparray[i];
                minvalue += signal*weightmin;
                weightmin *= _gain*signal;

                signal = (_offset + 1.0)*_exparray[i];
                maxvalue += signal*weightmax;
                weightmax *= _gain*signal;

                scale = (b - a)/(maxvalue - minvalue);
                bias = a - minvalue*scale;
                _correct[i, 0] = scale;
                _correct[i, 1] = bias;
            }
        }

        #endregion

        public override double Get(double x, double y)
        {
            double v;
            switch (_type)
            {
                case FractalType.FBM:
                    v = fB_get(x, y);
                    break;
                case FractalType.RIDGEDMULTI:
                    v = RidgedMulti_get(x, y);
                    break;
                case FractalType.BILLOW:
                    v = Billow_get(x, y);
                    break;
                case FractalType.MULTI:
                    v = Multi_get(x, y);
                    break;
                case FractalType.HYBRIDMULTI:
                    v = HybridMulti_get(x, y);
                    break;
                default:
                    v = fB_get(x, y);
                    break;
            }

            return Helper.Clamp(v, -1.0, 1.0);
        }

        #region Gets

        private double fB_get(double x, double y)
        {
            double value = 0.0;

            x *= _frequency;
            y *= _frequency;

            for (int i = 0; i < _numoctaves; i++)
            {
                double signal = _basis[i].Get(x, y)*_exparray[i];
                value += signal;
                x *= _lacunarity;
                y *= _lacunarity;
            }

            return value;
        }

        private double Multi_get(double x, double y)
        {
            double value = 1.0;
            x *= _frequency;
            y *= _frequency;

            for (int i = 0; i < _numoctaves; i++)
            {
                value *= _basis[i].Get(x, y)*_exparray[i] + 1.0;
                x *= _lacunarity;
                y *= _lacunarity;
            }

            return value*_correct[_numoctaves - 1, 0] + _correct[_numoctaves - 1, 1];
        }

        private double Billow_get(double x, double y)
        {
            double value = 0.0;
            x *= _frequency;
            y *= _frequency;

            for (uint i = 0; i < _numoctaves; ++i)
            {
                double signal = _basis[i].Get(x, y);
                signal = 2.0*Math.Abs(signal) - 1.0;
                value += signal*_exparray[i];

                x *= _lacunarity;
                y *= _lacunarity;
            }

            value += 0.5;
            return value*_correct[_numoctaves - 1, 0] + _correct[_numoctaves - 1, 1];
        }

        private double RidgedMulti_get(double x, double y)
        {
            double result = 0.0;
            x *= _frequency;
            y *= _frequency;

            for (uint i = 0; i < _numoctaves; ++i)
            {
                double signal = _basis[i].Get(x, y);
                signal = _offset - Math.Abs(signal);
                signal *= signal;
                result += signal*_exparray[i];

                x *= _lacunarity;
                y *= _lacunarity;

            }

            return result*_correct[_numoctaves - 1, 0] + _correct[_numoctaves - 1, 1];
        }

        private double HybridMulti_get(double x, double y)
        {
            x *= _frequency;
            y *= _frequency;

            double value = _basis[0].Get(x, y) + _offset;
            double weight = _gain*value;
            x *= _lacunarity;
            y *= _lacunarity;

            for (uint i = 1; i < _numoctaves; ++i)
            {
                if (weight > 1.0) weight = 1.0;
                double signal = (_basis[i].Get(x, y) + _offset)*_exparray[i];
                value += weight*signal;
                weight *= _gain*signal;
                x *= _lacunarity;
                y *= _lacunarity;
            }

            return value*_correct[_numoctaves - 1, 0] + _correct[_numoctaves - 1, 1];
        }

        #endregion
    }
}