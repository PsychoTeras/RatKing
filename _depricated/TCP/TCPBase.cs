using System;

namespace RK.Common.Net.TCP
{
    public abstract class TCPBase : IDisposable
    {

#region Delegates

        public delegate void OnLogMessage(string message);

#endregion

#region Events

        public OnLogMessage LogMessage { get; set; }

#endregion

#region Class methods

        protected virtual void OutLogMessage(string message)
        {
            if (LogMessage != null)
            {
                LogMessage(message);
            }
        }

#endregion

#region IDisposable

        public virtual void Dispose() {}

#endregion

    }
}
