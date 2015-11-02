using System.Collections.Generic;
using System.Net.Sockets;

namespace RK.Common.Net.TCP2
{
    internal class BufferManager
    {
        private int _bufferSize;
        private int _numberOfSocketEvents;
        private int _totalBytesInBuffer;

        private byte[] _bufferBlock;
        private Stack<int> _freeIndexPool;
        private int _currentIndex;

        public BufferManager(int bufferSize, int numberOfSocketEvents)
        {
            _bufferSize = bufferSize;
            _numberOfSocketEvents = numberOfSocketEvents;
            _totalBytesInBuffer = _bufferSize*_numberOfSocketEvents;

            _freeIndexPool = new Stack<int>();
            _bufferBlock = new byte[_totalBytesInBuffer];
        }

        public void Reset()
        {
            _freeIndexPool.Clear();
            _currentIndex = 0;
        }

        public bool SetBuffer(SocketAsyncEventArgs e)
        {
            if (_freeIndexPool.Count > 0)
            {
                e.SetBuffer(_bufferBlock, _freeIndexPool.Pop(), _bufferSize);
            }
            else
            {
                if (_totalBytesInBuffer - _bufferSize < _currentIndex)
                {
                    return false;
                }
                e.SetBuffer(_bufferBlock, _currentIndex, _bufferSize);
                _currentIndex += _bufferSize;
            }
            return true;
        }

        public void FreeBuffer(SocketAsyncEventArgs e)
        {
            _freeIndexPool.Push(e.Offset);
            e.SetBuffer(null, 0, 0);
        }
    }
}