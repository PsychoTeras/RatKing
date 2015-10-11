using System;
using System.Threading;

namespace RK.Common.Proto
{
    public abstract class BasePacket
    {
        private static int _sessionIdCounter = Environment.TickCount;

        public int UserId;
        public int SessionId;

        public long SessionMark
        {
            get { return UserId << 32 | SessionId; }
        }

        public abstract PacketType Type { get; }

        public void NewSessionId()
        {
            SessionId = Interlocked.Increment(ref _sessionIdCounter);
        }
    }
}
