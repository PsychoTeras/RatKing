using System;
using System.Threading;

namespace RK.Common.Proto
{
    public abstract class BasePacket
    {
        private static long _packetIdCounter;
        private static int _sessionIdCounter = Environment.TickCount;

        public long Id = Interlocked.Increment(ref _packetIdCounter);
        public long SessionToken;
        public long TimeStamp = DateTime.UtcNow.ToBinary();

        public abstract PacketType Type { get; }

        public void ResetTimeStamp()
        {
            TimeStamp = DateTime.UtcNow.ToBinary();
        }

        public static long NewSessionToken(int userId)
        {
            return ((long)userId << 32) | Interlocked.Increment(ref _sessionIdCounter);
        }
    }
}
