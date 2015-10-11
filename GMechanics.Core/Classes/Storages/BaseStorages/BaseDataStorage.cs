using GMechanics.Core.Classes.Entities;

namespace GMechanics.Core.Classes.Storages.BaseStorages
{
    public class BaseDataStorage<TObjType> : BaseContainer<TObjType, BaseDataStorage<TObjType>>
        where TObjType : Atom 
    {
    }
}
