using System;

namespace AccidentalNoise
{
    public enum BasisTypes
    {
        VALUE,
        GRADIENT,
        GRADVAL,
        SIMPLEX,
        WHITE
    };

    public enum InterpTypes
    {
        NONE,
        LINEAR,
        CUBIC,
        QUINTIC
    };

    public class BasisFunction
    {
        private BasisTypes _type;
        private InterpTypes _interp;
        private double[] _scale = new double[4];
        private double[] _offset = new double[4];

        private uint _seed;
        private double _cos2D, _sin2D;

        public BasisFunction(BasisTypes type, InterpTypes interp, uint seed, double angle)
        {
            _type = type;
            _interp = interp;
            _seed = seed;
            SetAngle(angle);
            SetMagicNumbers(type);
        }

        private double GetBasis(double x, double y, uint seed)
        {
            switch (_type)
            {
                case BasisTypes.VALUE:
                    return NoiseGen.value_noise2D(x, y, seed, _interp);
                case BasisTypes.GRADIENT:
                    return NoiseGen.gradient_noise2D(x, y, seed, _interp);
                case BasisTypes.GRADVAL:
                    return NoiseGen.gradval_noise2D(x, y, seed, _interp);
                case BasisTypes.WHITE:
                    return NoiseGen.white_noise2D(x, y, seed, _interp);
                case BasisTypes.SIMPLEX:
                    return 0;
                default:
                    return NoiseGen.gradient_noise2D(x, y, seed, _interp);
            }
        }

        private void SetMagicNumbers(BasisTypes type)
        {
            // This function is a damned hack.
            // The underlying noise functions don't return values in the range [-1,1] cleanly, and the ranges vary depending
            // on basis type and dimensionality. There's probably a better way to correct the ranges, but for now I'm just
            // setting he magic numbers _scale and _offset manually to empirically determined magic numbers.
            switch (type)
            {
                case BasisTypes.VALUE:
                {
                    _scale[0] = 1.0;
                    _offset[0] = 0.0;
                    _scale[1] = 1.0;
                    _offset[1] = 0.0;
                    _scale[2] = 1.0;
                    _offset[2] = 0.0;
                    _scale[3] = 1.0;
                    _offset[3] = 0.0;
                }
                    break;

                case BasisTypes.GRADIENT:
                {
                    _scale[0] = 1.86848;
                    _offset[0] = -0.000118;
                    _scale[1] = 1.85148;
                    _offset[1] = -0.008272;
                    _scale[2] = 1.64127;
                    _offset[2] = -0.01527;
                    _scale[3] = 1.92517;
                    _offset[3] = 0.03393;
                }
                    break;

                case BasisTypes.GRADVAL:
                {
                    _scale[0] = 0.6769;
                    _offset[0] = -0.00151;
                    _scale[1] = 0.6957;
                    _offset[1] = -0.133;
                    _scale[2] = 0.74622;
                    _offset[2] = 0.01916;
                    _scale[3] = 0.7961;
                    _offset[3] = -0.0352;
                }
                    break;

                case BasisTypes.WHITE:
                {
                    _scale[0] = 1.0;
                    _offset[0] = 0.0;
                    _scale[1] = 1.0;
                    _offset[1] = 0.0;
                    _scale[2] = 1.0;
                    _offset[2] = 0.0;
                    _scale[3] = 1.0;
                    _offset[3] = 0.0;
                }
                    break;

                default:
                {
                    _scale[0] = 1.0;
                    _offset[0] = 0.0;
                    _scale[1] = 1.0;
                    _offset[1] = 0.0;
                    _scale[2] = 1.0;
                    _offset[2] = 0.0;
                    _scale[3] = 1.0;
                    _offset[3] = 0.0;
                }
                    break;
            }
        }

        public void SetAngle(double angle)
        {
            _cos2D = Math.Cos(angle);
            _sin2D = Math.Sin(angle);
        }

        public double Get(double x, double y)
        {
            double nx = x*_cos2D - y*_sin2D;
            double ny = y*_cos2D + x*_sin2D;
            return GetBasis(nx, ny, _seed);
        }
    }
}