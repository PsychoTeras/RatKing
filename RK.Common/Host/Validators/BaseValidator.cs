﻿using RK.Common.Proto;

namespace RK.Common.Host.Validators
{
    internal abstract class BaseValidator : IValidator
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

        public abstract void RegisterSession(long sessionToken);
        public abstract void UnregisterSession(long sessionToken);
        public abstract void Validate(BasePacket packet);

#endregion

    }
}
