namespace AccidentalNoise
{
    public class Gradient : ModuleBase
    {
        private double _gx1;
        private double _gy1;
        private double _x;
        private double _y;
        private double _vlen;

        public Gradient(double x1, double x2, double y1, double y2)
        {
            SetGradient(x1, x2, y1, y2);
        }

        private void SetGradient(double x1, double x2, double y1, double y2)
        {
            _gx1 = x1;
            _gy1 = y1;
            _x = x1;
            _y = y2 - y1;
            _vlen = (_x*_x + _y*_y);
        }

        public override double Get(double x, double y)
        {
            double dx = x - _gx1;
            double dy = y - _gy1;
            double dp = dx*_x + dy*_y;
            dp /= _vlen;
            return dp;
        }
    }
}