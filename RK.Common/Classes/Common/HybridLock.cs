using System;
using System.Threading;

namespace RK.Common.Classes.Common
{
    internal sealed class HybridLock : IDisposable
    {
        private Int32 _waiters;
        private AutoResetEvent _waiterLock = new AutoResetEvent(false);

        public void WaitOne()
        {
            if (Interlocked.Increment(ref _waiters) == 1) return;
            _waiterLock.WaitOne();
        }

        public void Set()
        {
            if (Interlocked.Decrement(ref _waiters) == 0) return;
            _waiterLock.Set();
        }

        public void Dispose()
        {
            _waiterLock.Dispose();
        }
    }
}
