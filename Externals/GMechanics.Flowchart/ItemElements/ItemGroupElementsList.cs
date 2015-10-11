using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GMechanics.FlowchartControl.ItemElements
{
    public class ItemGroupElementsList : IEnumerable<ItemGroupElement>
    {

#region Private members

        private readonly ItemGroup _parentGroup;
        private readonly ItemGroupElement _parentItemGroupElement;
        private readonly List<ItemGroupElement> _elements = new List<ItemGroupElement>();

#endregion

#region Properties

        internal ItemGroupElement ParentItemGroupElement
        {
            get { return _parentItemGroupElement; }
        }

        public ItemGroupElement this[string name]
        {
            get
            {
                return _elements.FirstOrDefault(e => e.Text.Equals(name,
                  StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public ItemGroupElement this[int index]
        {
            get
            {
                return index >= 0 && index < _elements.Count
                           ? _elements[index]
                           : null;
            }
        }

        public int Count
        {
            get { return _elements.Count; }
        }

        public int NestingLevel { get; private set; }

#endregion

#region Class functions

        internal ItemGroupElementsList(ItemGroup parentGroup,
            ItemGroupElement parentItemGroupElement, int nestingLevel)
        {
            _parentGroup = parentGroup;
            _parentItemGroupElement = parentItemGroupElement;
            NestingLevel = nestingLevel;
        }

        public void Repaint()
        {
            _parentGroup.Repaint();
        }

#endregion

#region Elements control functions

        public void Sort()
        {
            _elements.Sort();
            Repaint();
        }

        public void Clear()
        {
            _elements.Clear();
            Repaint();
        }

        public ItemGroupElement Add(string text, object userObject)
        {
            return Add(text, userObject, true);
        }

        public ItemGroupElement Add(string text, object userObject, 
            bool sort)
        {
            ItemGroupElement element = new ItemGroupElement(_parentGroup,
                text, userObject, NestingLevel);
            _elements.Add(element);
            if (sort)
            {
                Sort();
            }
            else
            {
                Repaint();
            }
            return element;
        }

        public List<ItemGroupElement> GetAll(string text)
        {
            return _elements.FindAll(e => e.Text.Equals(text, StringComparison.
                InvariantCultureIgnoreCase));
        }

        public void GetVisible(List<ItemGroupElement> elements)
        {
            _elements.FindAll(e => e.Visible).ForEach(e =>
                {
                    elements.Add(e);
                    e.Elements.GetVisible(elements);
                });
        }

        public void Remove(string text)
        {
            _elements.Remove(this[text]);
            Repaint();
        }

        public void Remove(ItemGroupElement element)
        {
            _elements.Remove(element);
            Repaint();
        }

        public void RemoveAll(string text)
        {
            _elements.RemoveAll(e => e.Text.Equals(text, StringComparison.
                InvariantCultureIgnoreCase));
            Repaint();
        }

#endregion

#region IEnumerable

        public IEnumerator<ItemGroupElement> GetEnumerator()
        {
            return new ItemGroupElementEnumerator(_elements);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

#endregion

    }

#region ItemGroupElementEnumerator

    internal class ItemGroupElementEnumerator : IEnumerator<ItemGroupElement>
    {
        private int _index;
        private readonly List<ItemGroupElement> _elements = new List<ItemGroupElement>();

        public ItemGroupElementEnumerator(List<ItemGroupElement> elements)
        {
            Reset();
            _elements = elements;
        }

        public bool MoveNext()
        {
            _index++;
            return _index < _elements.Count();
        }

        public void Reset()
        {
            _index = -1;
        }

        ItemGroupElement IEnumerator<ItemGroupElement>.Current
        {
            get { return _elements[_index]; }
        }

        public object Current
        {
            get { return _elements[_index]; }
        }

        public void Dispose() {}
    }

#endregion

}
