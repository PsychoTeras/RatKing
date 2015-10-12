using RK.Common.Proto;

namespace RK.Common.Host
{
    public abstract class BaseValidator
    {

#region Protected fields

        protected GameHost Host;

#endregion

#region Ctor

        protected BaseValidator(GameHost host)
        {
            Host = host;
        }

#endregion

#region Abstract methods

        public abstract void RegisterSession(long sessionMark);
        public abstract void UnregisterSession(long sessionMark);
        public abstract bool Validate(BasePacket packet);

#endregion

    }
}
