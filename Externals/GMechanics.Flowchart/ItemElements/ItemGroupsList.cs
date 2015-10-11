using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GMechanics.FlowchartControl.ItemElements
{
    public class ItemGroupsList : IEnumerable<ItemGroup>
    {

#region Private members

        private readonly FlowchartItem _parentItem;
        private readonly List<ItemGroup> _groups = new List<ItemGroup>();

#endregion

#region Properties

        internal FlowchartItem ParentItem
        {
            get { return _parentItem; }
        }

        public ItemGroup this[string name]
        {
            get
            {
                return _groups.FirstOrDefault(g => g.Caption.Equals(name,
                  StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public ItemGroup this[int index]
        {
            get
            {
                return index >= 0 && index < _groups.Count
                           ? _groups[index]
                           : null;
            }
        }

        public int Count
        {
            get { return _groups.Count; }
        }

#endregion

#region Class functions

        public ItemGroupsList(FlowchartItem parentItem)
        {
            _parentItem = parentItem;
        }

        public void Repaint()
        {
            ParentItem.Repaint();
        }

#endregion

#region Elements control functions

        public void SortGroups()
        {
            _groups.Sort();
            Repaint();
        }

        public void ClearGroups()
        {
            _groups.Clear();
            Repaint();
        }

        public ItemGroup AddGroup(string caption)
        {
            return AddGroup(caption, null, -1, true);
        }

        public ItemGroup AddGroup(string caption, ImageList imageList, 
            int iconIndex)
        {
            return AddGroup(caption, imageList, iconIndex, true);
        }

        public ItemGroup AddGroup(string caption, ImageList imageList, 
            int iconIndex, bool sort)
        {
            ItemGroup group = new ItemGroup(this, caption, imageList, iconIndex);
            _groups.Add(group);
            if (sort)
            {
                SortGroups();
            }
            else
            {
                Repaint();
            }
            return group;
        }

        public ItemGroup GetGroup(string caption)
        {
            return this[caption];
        }

        public ItemGroup GetGroup(int index)
        {
            return this[index];
        }

        public List<ItemGroup> GetGroups(string caption)
        {
            return _groups.FindAll(g => g.Caption.Equals(caption, StringComparison.
                InvariantCultureIgnoreCase));
        }

        public List<ItemGroup> GetVisibleGroups()
        {
            return _groups.FindAll(g => g.Visible);
        }

        public void RemoveGroup(string caption)
        {
            _groups.Remove(this[caption]);
            Repaint();
        }

        public void RemoveGroup(ItemGroup group)
        {
            _groups.Remove(group);
            Repaint();
        }

        public void RemoveGroups(string caption)
        {
            _groups.RemoveAll(g => g.Caption.Equals(caption, StringComparison.
                InvariantCultureIgnoreCase));
            Repaint();
        }

#endregion

#region IEnumerable

        public IEnumerator<ItemGroup> GetEnumerator()
        {
            return new ItemGroupEnumerator(_groups);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

#endregion

    }

#region ItemGroupEnumerator

    internal class ItemGroupEnumerator : IEnumerator<ItemGroup>
    {
        private int _index;
        private readonly List<ItemGroup> _groups = new List<ItemGroup>();

        public ItemGroupEnumerator(List<ItemGroup> groups)
        {
            Reset();
            _groups = groups;
        }

        public bool MoveNext()
        {
            _index++;
            return _index < _groups.Count();
        }

        public void Reset()
        {
            _index = -1;
        }

        ItemGroup IEnumerator<ItemGroup>.Current
        {
            get { return _groups[_index]; }
        }

        public object Current
        {
            get { return _groups[_index]; }
        }

        public void Dispose() { }
    }

#endregion

}
