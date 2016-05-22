using RK.Common.Proto;

namespace RK.Common.Host.Validators
{
    internal interface IValidator
    {
        void RegisterSession(long sessionToken);
        void UnregisterSession(long sessionToken);
        void Validate(BasePacket packet);
    }
}
