using System;
using System.Threading;

namespace RK.Common.Proto
{
    public abstract class BasePacket
    {
        private static int _sessionIdCounter = Environment.TickCount;

        public long SessionToken;

        public abstract PacketType Type { get; }

        public static long NewSessionToken(int userId)
        {
            return ((long)userId << 32) | Interlocked.Increment(ref _sessionIdCounter);
        }
    }
}
