using System.Threading;

namespace RK.Common.Classes.Common
{
    public abstract class DbObject
    {
        public static long IdCounter;

        public long Id;

        protected void SetNewId()
        {
            Id = Interlocked.Increment(ref IdCounter);
        }
    }
}
