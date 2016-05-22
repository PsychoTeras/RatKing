using System.Collections.Generic;
using System.Threading.Tasks;
using RK.Common.Proto;

namespace RK.Common.Host.Validators
{
    internal sealed class ValidatorList : List<BaseValidator>, IValidator
    {
        public void RegisterSession(long sessionToken)
        {
            Parallel.ForEach(this, v =>
            {
                v.RegisterSession(sessionToken);
            });
        }

        public void UnregisterSession(long sessionToken)
        {
            Parallel.ForEach(this, v =>
            {
                v.UnregisterSession(sessionToken);
            });
        }

        public void Validate(BasePacket packet)
        {
            Parallel.ForEach(this, v =>
            {
                v.Validate(packet);
            });
        }
    }
}
