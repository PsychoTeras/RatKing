using System.Collections.Generic;

namespace RK.Common.Classes.Common
{
    internal sealed class Pool<T> where T : new()
    {
        private int _maxItemsCount;
        private const float GROW_FACTOR = 1.5f;

        private readonly Queue<T> _pool;

        public Pool(int capacity, bool fillItemsPool)
        {
            _pool = new Queue<T>(capacity);
            if (fillItemsPool)
            {
                for (int i = 0; i < capacity; i++)
                {
                    _pool.Enqueue(new T());
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
            lock (this)
            {
                return _pool.Dequeue();
            }
        }

        public T PopExpand()
        {
            lock (this)
            {
                if (_pool.Count == 0) Expand();
                return _pool.Dequeue();
            }
        }

        public void Push(T item)
        {
            lock (this)
            {
                _pool.Enqueue(item);
            }
        }

        public T PopAsync()
        {
            return _pool.Dequeue();
        }

        public T PopExpandAsync()
        {
            if (_pool.Count == 0) Expand();
            return _pool.Dequeue();
        }

        public void PushAsync(T item)
        {
            _pool.Enqueue(item);
        }

        private void Expand()
        {
            int expandOnCount = (int) (_maxItemsCount*GROW_FACTOR) - _maxItemsCount;
            for (int i = 0; i < expandOnCount; i++)
            {
                _pool.Enqueue(new T());
            }
            _maxItemsCount += expandOnCount;
        }
    }
}