using System;
using System.Runtime.Serialization;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Storages.BaseStorages;
using GMechanics.Core.Helpers;

namespace GMechanics.Core.Classes.Storages
{
    [Serializable]
    public sealed class ElementaryGameObjects : BaseGameObjects<ElementaryGameObject, 
        ElementaryGameObjects>
    {
        public ElementaryGameObjects() { }

        public ElementaryGameObjects(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public bool Add(string name, string transcription, string description,
                ElementaryGameObject parent)
        {
            return Add(new ElementaryGameObject(name, transcription, description, parent));
        }

        public override void RemoveDestroyedItems()
        {
            base.RemoveDestroyedItems();
            foreach (string key in Keys)
            {
                Helper.RemoveDestroyedItemsFromListsOwner(this[key]);
            }
        }
    }
}
