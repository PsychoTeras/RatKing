using System.Threading;

namespace RK.Common.Classes.Common
{
    public sealed class LightLock
    {
        private int _waiter;

        public void WaitOne()
        {
            while (Interlocked.CompareExchange(ref _waiter, 1, 0) != 0)
            {
                Thread.Sleep(0);
            }
        }

        public void Set()
        {
            Interlocked.CompareExchange(ref _waiter, 0, 1);
        }
    }
}
