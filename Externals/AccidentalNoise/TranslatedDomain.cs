namespace AccidentalNoise
{
    public class TranslatedDomain : ModuleBase
    {
        private ModuleBase _tx, _ty;

        public TranslatedDomain(ModuleBase source, ModuleBase tx, ModuleBase ty)
        {
            Source = source;
            _tx = tx;
            _ty = ty;
        }

        public override double Get(double x, double y)
        {
            double ax = _tx == null ? 0 : _tx.Get(x, y);
            double ay = _ty == null ? 0 : _ty.Get(x, y);
            return Source.Get(x + ax, y + ay);
        }
    }
}