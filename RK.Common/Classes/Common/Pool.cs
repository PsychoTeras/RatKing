using System.Collections.Generic;

namespace RK.Common.Classes.Common
{
    internal sealed class Pool<T>
    {
        private readonly Stack<T> _pool;
        
        public Pool(int capacity)
        {
            _pool = new Stack<T>(capacity);
        }

        public int Count
        {
            get { return _pool.Count; }
        }

        public T Pop()
        {
            lock (_pool)
            {
                return _pool.Pop();
            }
        }

        public void Push(T item)
        {
            lock (_pool)
            {
                _pool.Push(item);
            }
        }
    }
}