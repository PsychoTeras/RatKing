using GMechanics.Core.Classes.GameObjects;
using GMechanics.Core.Helpers;

namespace GMechanics.Core.Classes.Storages
{
    public class CommonObjects : BaseContainer<CommonObject, CommonObjects>
    {
        public bool Add(string name, string transcription, string description,
                        ElementaryObject parent)
        {
            return Add(new CommonObject(name, transcription, description, parent));
        }

        public override void RemoveDestroyedItems()
        {
            base.RemoveDestroyedItems();
            foreach (string key in Keys)
            {
                CommonHelper.RemoveDestroyedItemsFromListsOwner(this[key]);
            }
        }
    }
}