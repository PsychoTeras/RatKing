using System;
using System.Threading;

namespace RK.Common.Classes.Common
{
    internal sealed class HybridLock : IDisposable
    {
        private int _waiters;
        private AutoResetEvent _waiterLock = new AutoResetEvent(false);

        public void WaitOne(int timeout = Timeout.Infinite)
        {
            if (Interlocked.Increment(ref _waiters) == 1) return;
            _waiterLock.WaitOne(timeout);
        }

        public void Set()
        {
            if (_waiters == 0 || Interlocked.Decrement(ref _waiters) == 0) return;
            _waiterLock.Set();
        }

        public void Dispose()
        {
            _waiterLock.Dispose();
        }
    }
}
