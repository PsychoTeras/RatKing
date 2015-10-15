using System.Threading;

namespace RK.Common.Classes.Common
{
    public abstract class DbObject
    {
        public static int IdCounter;

        public int Id;

        protected void SetNewId()
        {
            Id = Interlocked.Increment(ref IdCounter);
        }
    }
}
