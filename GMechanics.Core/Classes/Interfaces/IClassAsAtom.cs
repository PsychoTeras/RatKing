using GMechanics.Core.Classes.Entities;

namespace GMechanics.Core.Classes.Interfaces
{
    public interface IClassAsAtom
    {
        void Destroy();
        Atom ClassAsAtom { get; }
    }
}