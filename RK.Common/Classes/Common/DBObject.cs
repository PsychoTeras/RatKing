using System.Threading;

namespace RK.Common.Classes.Common
{
    public abstract class DbObject
    {

#region Static fields

        public static long IdCounter;

#endregion

#region Public fields

        public long Id;

#endregion

#region Protected methods

        protected void SetNewId()
        {
            Id = Interlocked.Increment(ref IdCounter);
        }

#endregion

    }
}
