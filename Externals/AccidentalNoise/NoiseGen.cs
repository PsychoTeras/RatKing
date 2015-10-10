namespace AccidentalNoise
{
    public static class NoiseGen
    {
        private const uint FNV_32_PRIME = 0x01000193;
        private const uint FNV_32_INIT = 2166136261;
        private const uint FNV_MASK_8 = (((uint) 1 << 8) - 1);

        public enum NoiseFunc
        {
            value_noise_2,
            grad_noise_2
        }

        private static double NoInterp(double t)
        {
            return 0;
        }

        private static double LinearInterp(double t)
        {
            return t;
        }

        private static double HermiteInterp(double t)
        {
            return (t*t*(3 - 2*t));
        }

        private static double QuinticInterp(double t)
        {
            return t*t*t*(t*(t*6 - 15) + 10);
        }

        private static double Interp(double t, InterpTypes type)
        {
            switch (type)
            {
                case InterpTypes.NONE:
                    return NoInterp(t);
                case InterpTypes.LINEAR:
                    return LinearInterp(t);
                case InterpTypes.QUINTIC:
                    return QuinticInterp(t);
                case InterpTypes.CUBIC:
                    return HermiteInterp(t);
            }
            return NoInterp(t);
        }

        private static int fast_floor(double t)
        {
            return (t > 0 ? (int) t : (int) t - 1);
        }

        // Edge/Face/Cube/Hypercube interpolation
        private static double Lerp(double s, double v1, double v2)
        {
            return v1 + s*(v2 - v1);
        }

        private static double Noisefunc(double x, double y, int ix, int iy, uint seed, NoiseFunc function)
        {
            switch (function)
            {
                case NoiseFunc.value_noise_2:
                    return value_noise_2(x, y, ix, iy, seed);
                case NoiseFunc.grad_noise_2:
                    return grad_noise_2(x, y, ix, iy, seed);
                default:
                    return value_noise_2(x, y, ix, iy, seed);
            }
        }

        public static double value_noise2D(double x, double y, uint seed, InterpTypes type)
        {
            int x0 = fast_floor(x);
            int y0 = fast_floor(y);

            int x1 = x0 + 1;
            int y1 = y0 + 1;

            double xs = Interp((x - x0), type);
            double ys = Interp((y - y0), type);

            return interp_XY_2(x, y, xs, ys, x0, x1, y0, y1, seed, NoiseFunc.value_noise_2);
        }

        private static double grad_noise_2(double x, double y, int ix, int iy, uint seed)
        {
            uint hash = hash_coords_2(ix, iy, seed);
            double vec1 = _gradient2DLut[hash][0];
            double vec2 = _gradient2DLut[hash][1];

            double dx = x - ix;
            double dy = y - iy;

            return (dx*vec1 + dy*vec2);
        }

        public static double gradient_noise2D(double x, double y, uint seed, InterpTypes type)
        {
            int x0 = fast_floor(x);
            int y0 = fast_floor(y);

            int x1 = x0 + 1;
            int y1 = y0 + 1;

            double xs = Interp((x - x0), type);
            double ys = Interp((y - y0), type);

            return interp_XY_2(x, y, xs, ys, x0, x1, y0, y1, seed, NoiseFunc.grad_noise_2);
        }

        public static double gradval_noise2D(double x, double y, uint seed, InterpTypes type)
        {
            return value_noise2D(x, y, seed, type) + gradient_noise2D(x, y, seed, type);
        }

        public static double white_noise2D(double x, double y, uint seed, InterpTypes interp)
        {
            byte hash = (byte) compute_hash_double_2(x, y, seed);
            return _whitenoiseLut[hash];
        }

        private static double interp_X_2(double x, double y, double xs, int x0, int x1, int iy, uint seed,
            NoiseFunc function)
        {
            double v1 = Noisefunc(x, y, x0, iy, seed, function);
            double v2 = Noisefunc(x, y, x1, iy, seed, function);
            return Lerp(xs, v1, v2);
        }

        private static double interp_XY_2(double x, double y, double xs, double ys, int x0, int x1, int y0, int y1,
            uint seed, NoiseFunc noisefunc)
        {
            double v1 = interp_X_2(x, y, xs, x0, x1, y0, seed, noisefunc);
            double v2 = interp_X_2(x, y, xs, x0, x1, y1, seed, noisefunc);
            return Lerp(ys, v1, v2);
        }

        private static double value_noise_2(double x, double y, int ix, int iy, uint seed)
        {
            uint n = (hash_coords_2(ix, iy, seed));
            double noise = n/255.0;
            return noise*2.0 - 1.0;
        }

        private static uint hash_coords_2(int x, int y, uint seed)
        {
            uint[] d =
            {
                (uint) x,
                (uint) y,
                seed
            };

            return xor_fold_hash(fnv_32_a_buf(d));
        }

        private static uint compute_hash_double_2(double x, double y, uint seed)
        {
            double[] d = {x, y, seed};
            return xor_fold_hash(fnv_32_a_buf(d));
        }

        private static uint fnv_32_a_buf(uint[] buf)
        {
            uint hval = FNV_32_INIT;
            byte[] bp = IntsToBytes(buf);
            int i = 0;

            while (i < bp.Length)
            {
                hval ^= bp[i++];
                hval *= FNV_32_PRIME;
            }

            return hval;
        }

        private static uint fnv_32_a_buf(double[] buf)
        {
            uint hval = FNV_32_INIT;
            byte[] bp = DoublesToBytes(buf);
            int i = 0;

            while (i < bp.Length)
            {
                hval ^= bp[i++];
                hval *= FNV_32_PRIME;
            }

            return hval;
        }

        private static byte[] DoublesToBytes(double[] doubles)
        {
            byte[] bytes = new byte[doubles.Length*8];
            int r = 0;
            foreach (double d in doubles)
            {
                byte[] one = System.BitConverter.GetBytes(d);
                foreach (byte b in one)
                {
                    bytes[r] = b;
                    r++;
                }
            }
            return bytes;
        }

        private static byte[] IntsToBytes(uint[] ints)
        {
            byte[] bytes = new byte[ints.Length*4];
            for (int i = 0, j = 0; i < ints.Length; i++)
            {
                bytes[j++] = (byte) (ints[i] & 0xFF);
                bytes[j++] = (byte) ((ints[i] >> 8) & 0xFF);
                bytes[j++] = (byte) ((ints[i] >> 16) & 0xFF);
                bytes[j++] = (byte) ((ints[i] >> 24) & 0xFF);
            }
            return bytes;
        }

        private static byte xor_fold_hash(uint hash)
        {
            // Implement XOR-folding to reduce from 32 to 8-bit hash
            return (byte) ((hash >> 8) ^ (hash & FNV_MASK_8));
        }

        #region Lookup Tables

        private static double[] _whitenoiseLut =
        {
            -0.714286,
            0.301587,
            0.333333,
            -1,
            0.396825,
            -0.0793651,
            -0.968254,
            -0.047619,
            0.301587,
            -0.111111,
            0.015873,
            0.968254,
            -0.428571,
            0.428571,
            0.047619,
            0.84127,
            -0.015873,
            -0.746032,
            -0.809524,
            -0.619048,
            -0.301587,
            -0.68254,
            0.777778,
            0.365079,
            -0.460317,
            0.714286,
            0.142857,
            0.047619,
            -0.0793651,
            -0.492063,
            -0.873016,
            -0.269841,
            -0.84127,
            -0.809524,
            -0.396825,
            -0.777778,
            -0.396825,
            -0.746032,
            0.301587,
            -0.52381,
            0.650794,
            0.301587,
            -0.015873,
            0.269841,
            0.492063,
            -0.936508,
            -0.777778,
            0.555556,
            0.68254,
            -0.650794,
            -0.968254,
            0.619048,
            0.777778,
            0.68254,
            0.206349,
            -0.555556,
            0.904762,
            0.587302,
            -0.174603,
            -0.047619,
            -0.206349,
            -0.68254,
            0.111111,
            -0.52381,
            0.174603,
            -0.968254,
            -0.111111,
            -0.238095,
            0.396825,
            -0.777778,
            -0.206349,
            0.142857,
            0.904762,
            -0.111111,
            -0.269841,
            0.777778,
            -0.015873,
            -0.047619,
            -0.333333,
            0.68254,
            -0.238095,
            0.904762,
            0.0793651,
            0.68254,
            -0.301587,
            -0.333333,
            0.206349,
            0.52381,
            0.904762,
            -0.015873,
            -0.555556,
            0.396825,
            0.460317,
            -0.142857,
            0.587302,
            1,
            -0.650794,
            -0.333333,
            -0.365079,
            0.015873,
            -0.873016,
            -1,
            -0.777778,
            0.174603,
            -0.84127,
            -0.428571,
            0.365079,
            -0.587302,
            -0.587302,
            0.650794,
            0.714286,
            0.84127,
            0.936508,
            0.746032,
            0.047619,
            -0.52381,
            -0.714286,
            -0.746032,
            -0.206349,
            -0.301587,
            -0.174603,
            0.460317,
            0.238095,
            0.968254,
            0.555556,
            -0.269841,
            0.206349,
            -0.0793651,
            0.777778,
            0.174603,
            0.111111,
            -0.714286,
            -0.84127,
            -0.68254,
            0.587302,
            0.746032,
            -0.68254,
            0.587302,
            0.365079,
            0.492063,
            -0.809524,
            0.809524,
            -0.873016,
            -0.142857,
            -0.142857,
            -0.619048,
            -0.873016,
            -0.587302,
            0.0793651,
            -0.269841,
            -0.460317,
            -0.904762,
            -0.174603,
            0.619048,
            0.936508,
            0.650794,
            0.238095,
            0.111111,
            0.873016,
            0.0793651,
            0.460317,
            -0.746032,
            -0.460317,
            0.428571,
            -0.714286,
            -0.365079,
            -0.428571,
            0.206349,
            0.746032,
            -0.492063,
            0.269841,
            0.269841,
            -0.365079,
            0.492063,
            0.873016,
            0.142857,
            0.714286,
            -0.936508,
            1,
            -0.142857,
            -0.904762,
            -0.301587,
            -0.968254,
            0.619048,
            0.269841,
            -0.809524,
            0.936508,
            0.714286,
            0.333333,
            0.428571,
            0.0793651,
            -0.650794,
            0.968254,
            0.809524,
            0.492063,
            0.555556,
            -0.396825,
            -1,
            -0.492063,
            -0.936508,
            -0.492063,
            -0.111111,
            0.809524,
            0.333333,
            0.238095,
            0.174603,
            0.333333,
            0.873016,
            0.809524,
            -0.047619,
            -0.619048,
            -0.174603,
            0.84127,
            0.111111,
            0.619048,
            -0.0793651,
            0.52381,
            1,
            0.015873,
            0.52381,
            -0.619048,
            -0.52381,
            1,
            0.650794,
            -0.428571,
            0.84127,
            -0.555556,
            0.015873,
            0.428571,
            0.746032,
            -0.238095,
            -0.238095,
            0.936508,
            -0.206349,
            -0.936508,
            0.873016,
            -0.555556,
            -0.650794,
            -0.904762,
            0.52381,
            0.968254,
            -0.333333,
            -0.904762,
            0.396825,
            0.047619,
            -0.84127,
            -0.365079,
            -0.587302,
            -1,
            -0.396825,
            0.365079,
            0.555556,
            0.460317,
            0.142857,
            -0.460317,
            0.238095
        };

        private static double[][] _gradient2DLut =
        {
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0},
            new double[] {0, 1},
            new double[] {0, -1},
            new double[] {1, 0},
            new double[] {-1, 0}
        };

        #endregion
    }
}