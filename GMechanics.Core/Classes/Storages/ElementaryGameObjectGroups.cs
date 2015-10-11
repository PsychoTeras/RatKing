using System;
using System.Runtime.Serialization;
using GMechanics.Core.Classes.Storages.BaseStorages;

namespace GMechanics.Core.Classes.Storages
{
    [Serializable]
    public sealed class ElementaryGameObjectGroups : BaseGameObjectGroups<ElementaryGameObjectGroups>
    {
        public ElementaryGameObjectGroups() { }

        public ElementaryGameObjectGroups(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

#region Properties

        protected override string GroupTableName
        {
            get { return "T_ELEMENTARY_GAME_OBJECT_GROUP"; }
        }

        protected override string GroupElementsTableName
        {
            get { return "T_ELEMENTARY_GAME_OBJECT_GROUP_ELEMENTS"; }
        }

#endregion

    }
}
