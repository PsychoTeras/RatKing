using System;
using System.Runtime.InteropServices;

namespace RK.Common.Win32
{
    public static unsafe class Memory
    {
        private const int HEAP_ZERO_MEMORY = 0x00000008;

        [DllImport("kernel32")]
        private static extern int GetProcessHeap();

        [DllImport("kernel32")]
        private static extern void* HeapAlloc(int hHeap, int flags, int size);

        [DllImport("kernel32")]
        private static extern bool HeapFree(int hHeap, int flags, void* block);

        [DllImport("kernel32")]
        private static extern void* HeapReAlloc(int hHeap, int flags, void* block, int size);

        [DllImport("kernel32")]
        private static extern int HeapSize(int hHeap, int flags, void* block);

        [DllImport("kernel32", EntryPoint = "RtlZeroMemory")]
        public static extern void ZeroMemory(void* dest, int size);

        private static int _ph = GetProcessHeap();

        public static void* Alloc(int size, bool zeroMem = true)
        {
            void* result = HeapAlloc(_ph, zeroMem ? HEAP_ZERO_MEMORY : 0, size);
            if (result == null)
            {
                throw new OutOfMemoryException();
            }
            return result;
        }

        public static void Free(void* block)
        {
            if (block != null && !HeapFree(_ph, 0, block))
            {
                throw new InvalidOperationException();
            }
        }

        public static void* ReAlloc(void* block, int size)
        {
            void* result = HeapReAlloc(_ph, HEAP_ZERO_MEMORY, block, size);
            if (result == null)
            {
                throw new OutOfMemoryException();
            }
            return result;
        }

        public static int SizeOf(void* block)
        {
            int result = HeapSize(_ph, 0, block);
            if (result == -1)
            {
                throw new InvalidOperationException();
            }
            return result;
        }

        public static void Copy(void* src, void* dst, int count)
        {
            byte* ps = (byte*)src;
            byte* pd = (byte*)dst;
            if (ps > pd)
            {
                for (; count != 0; count--) *pd++ = *ps++;
            }
            else if (ps < pd)
            {
                for (ps += count, pd += count; count != 0; count--) *--pd = *--ps;
            }
        }

    }
}
