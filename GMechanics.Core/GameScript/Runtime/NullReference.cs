namespace GMechanics.Core.GameScript.Runtime
{
    internal class NullReference
    {
        #region Private Static Variables

        private static NullReference _nullReference;

        #endregion

        #region Private methods

        private NullReference()
        {
        }

        #endregion

        #region Public methods

        public static NullReference Instance
        {
            get { return _nullReference ?? (_nullReference = new NullReference()); }
        }

        public override string ToString()
        {
            return "NULL";
        }

        #endregion
    }
}
