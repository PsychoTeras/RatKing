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

        [DllImport("kernel32")]
        private static extern bool HeapSetInformation(void* handle, int informationClass,
            void* information, uint informationLength);

        [DllImport("kernel32", EntryPoint = "RtlZeroMemory")]
        public static extern void Zero(void* dest, int size);

        [DllImport("kernel32", EntryPoint = "CopyMemory")]
        public static extern void* Copy(void* dest, void* src, int count);

        [DllImport("msvcrt.dll", EntryPoint = "memmove", CallingConvention = CallingConvention.Cdecl)]
        public static extern void* Move(void* dest, void* src, int count);

        private static int _ph = GetProcessHeap();

        public static void* HeapAlloc(int size, bool zeroMem = true)
        {
            void* result = HeapAlloc(_ph, zeroMem ? HEAP_ZERO_MEMORY : 0, size);
            if (result == null)
            {
                throw new OutOfMemoryException();
            }
            return result;
        }

        public static void HeapFree(void* block)
        {
            if (block != null && !HeapFree(_ph, 0, block))
            {
                throw new InvalidOperationException();
            }
        }

        public static void* HeapReAlloc(void* block, int size)
        {
            void* result = HeapReAlloc(_ph, HEAP_ZERO_MEMORY, block, size);
            if (result == null)
            {
                throw new OutOfMemoryException();
            }
            return result;
        }

        public static int HeapSizeOf(void* block)
        {
            int result = HeapSize(_ph, 0, block);
            if (result == -1)
            {
                throw new InvalidOperationException();
            }
            return result;
        }

        public static void HeapCopy(void* src, void* dst, int count)
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
