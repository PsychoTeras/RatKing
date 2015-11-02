using System.Collections.Generic;

namespace RK.Common.Classes.Common
{
    internal sealed class Pool<T> where T: new()
    {
        private int _maxItemsCount;
        private const float GROW_FACTOR = 1.5f;

        private readonly Stack<T> _pool;
        
        public Pool(int capacity, bool fillItemsPool)
        {
            _pool = new Stack<T>(capacity);
            if (fillItemsPool)
            {
                for (int i = 0; i < capacity; i++)
                {
                    _pool.Push(new T());
                }
            }
            _maxItemsCount = _pool.Count;
        }

        public int Count
        {
            get { return _pool.Count; }
        }

        public T Pop()
        {
//            lock (_pool)
            {
                return _pool.Pop();
            }
        }

        public T PopExpand()
        {
//            lock (_pool)
            {
                if (_pool.Count == 0) Expand();
                return _pool.Pop();
            }
        }

        public void Push(T item)
        {
//            lock (_pool)
            {
                _pool.Push(item);
            }
        }

        private void Expand()
        {
            int count = (int) (_maxItemsCount*GROW_FACTOR) - _maxItemsCount;
            for (int i = 0; i < count; i++)
            {
                _pool.Push(new T());
            }
            _maxItemsCount += count;
        }
    }
}