using System.Collections.Generic;

namespace GMechanics.Core.Classes.Entities.GameObjects
{
    public sealed class GameObjectParentsList
    {

#region Private fields

        private readonly ElementaryGameObject _owner;
        private readonly HashSet<ElementaryGameObject> _list; 

#endregion

#region Properties

        public int Count
        {
            get { return _list.Count; }
        }

        public HashSet<ElementaryGameObject> OwnParents
        {
            get { return _list; }
        }

#endregion

#region Ctor

        public GameObjectParentsList(ElementaryGameObject owner)
        {
            _owner = owner;
            _list = new HashSet<ElementaryGameObject>();
        }

#endregion

#region Class methods

        public void Add(ElementaryGameObject parent)
        {
            if (parent != null && !_list.Contains(parent))
            {
                _list.Add(parent);
            }
        }

        public void Remove(ElementaryGameObject parent)
        {
            if (parent != null && _list.Contains(parent))
            {
                _list.Remove(parent);
            }
        }

        private IEnumerable<ElementaryGameObject> Get(HashSet<ElementaryGameObject> parents)
        {
            foreach (ElementaryGameObject ego in parents)
            {
                yield return ego;
                foreach (var pego in Get(ego.Parents._list))
                {
                    yield return pego;
                }
            }
        }

        public IEnumerable<ElementaryGameObject> Get(bool findInParent = true)
        {
            if (_owner == null)
            {
                yield break;
            }

            yield return _owner;

            if (findInParent)
            {
                foreach (ElementaryGameObject ego in Get(_list))
                {
                    yield return ego;
                }
            }
        }

        public bool IsOwner(ElementaryGameObject item)
        {
            return item != null && item == _owner;
        }

        public bool HasOwnParent(ElementaryGameObject item)
        {
            return item != null && _list.Contains(item);
        }

#endregion

    }

}
