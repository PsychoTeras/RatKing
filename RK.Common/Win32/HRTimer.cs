using System;
using System.Runtime.InteropServices;

namespace RK.Common.Win32
{
    public sealed class HRTimer
    {
        [DllImport("kernel32.dll")]
        private static extern int QueryPerformanceCounter(ref Int64 count);

        [DllImport("kernel32.dll")]
        private static extern int QueryPerformanceFrequency(ref Int64 frequency);

        private Int64 _start;
        private double _result;
        private readonly Int64 _frequency;

        public HRTimer()
        {
            _start = 0;
            QueryPerformanceFrequency(ref _frequency);
        }

        public double Result
        {
            get { return _result; }
        }

        public static HRTimer CreateAndStart()
        {
            HRTimer hr = new HRTimer();
            hr.StartWatch();
            return hr;
        }

        public void StartWatch()
        {
            _result = 0;
            QueryPerformanceCounter(ref _start);
        }

        public double StopWatch()
        {
            Int64 stop = 0;
            QueryPerformanceCounter(ref stop);
            _result = ((double)(stop - _start))/_frequency * 1000;
            return _result;
        }
    }
}
