using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace GMechanics.FlowchartDemo.FlowchartControl
{
    public class FlowchartItemGroup : IEnumerable<FlowchartItemGroupElement>
    {

#region Private members

        private readonly FlowchartItemGroups _parentGroupsList;

        private string _caption;
        private bool _expanded;
        private bool _visible;

        private readonly List<FlowchartItemGroupElement> _elements = 
            new List<FlowchartItemGroupElement>();

#endregion

#region Properties

        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value ?? string.Empty;
                Repaint();
            }
        }

        public bool Expanded
        {
            get { return _expanded; }
            set
            {
                if (!(_expanded = value))
                {
                    Collapse();
                }
                else
                {
                    Expand();
                }
                Repaint();
            }
        }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                Repaint();
            }
        }

        public FlowchartItemGroupElement this[string name]
        {
            get
            {
                return _elements.FirstOrDefault(e => e.Text.Equals(name,
                  StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public FlowchartItemGroupElement this[int index]
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
            get
            {
                return Count;
            }
        }

        internal int VisibledCount { get; private set; }

        internal FlowchartItemGroups ParentGroupsList
        {
            get { return _parentGroupsList; }
        }

#endregion

#region Class functions

        public FlowchartItemGroup(FlowchartItemGroups parentGroupsList)
        {
            _parentGroupsList = parentGroupsList;
        }

        public void Expand()
        {
            Expanded = true;
        }

        public void Collapse()
        {
            Expanded = false;
        }

        internal void Repaint()
        {
            Repaint(true);
        }

        internal void Repaint(bool repaint)
        {
            if (repaint)
            {
                ParentGroupsList.ParentItem.Repaint();
            }
        }

        internal void RecalculateVisibled()
        {
            VisibledCount = _elements.FindAll(e => e.Visible).Count;
        }

#endregion

#region Elements control functions

        public void SortElements()
        {
            SortElements(true);
        }

        public void SortElements(bool repaint)
        {
            _elements.Sort();
            Repaint(repaint);
        }

        public void ClearElements()
        {
            ClearElements(true);
        }

        public void ClearElements(bool repaint)
        {
            _elements.Clear();
            RecalculateVisibled();
            Repaint(repaint);
        }

        public FlowchartItemGroupElement AddElement(string text, object userObject)
        {
            return AddElement(text, userObject, true, true);
        }

        public FlowchartItemGroupElement AddElement(string text, object userObject,
            bool sort)
        {
            return AddElement(text, userObject, sort, true);
        }

        public FlowchartItemGroupElement AddElement(string text, object userObject, 
                                                    bool sort, bool repaint)
        {
            FlowchartItemGroupElement element = new FlowchartItemGroupElement(this, 
                text, userObject);
            _elements.Add(element);
            if (sort)
            {
                SortElements(false);
            }
            RecalculateVisibled();
            Repaint(repaint);
            return element;
        }

        public FlowchartItemGroupElement GetElement(string text)
        {
            return this[text];
        }

        public FlowchartItemGroupElement[] GetElements(string text)
        {
            return _elements.FindAll(e => e.Text.Equals(text, StringComparison.
                InvariantCultureIgnoreCase)).ToArray();
        }

        public void RemoveElement(string text)
        {
            RemoveElement(text, true);
        }

        public void RemoveElement(string text, bool repaint)
        {
            _elements.Remove(this[text]);
            RecalculateVisibled();
            Repaint(repaint);
        }

        public void RemoveElement(FlowchartItemGroupElement element)
        {
            RemoveElement(element, true);
        }

        public void RemoveElement(FlowchartItemGroupElement element, bool repaint)
        {
            _elements.Remove(element);
            RecalculateVisibled();
            Repaint(repaint);
        }

        public void RemoveElements(string text)
        {
            RemoveElements(text, true);
        }

        public void RemoveElements(string text, bool repaint)
        {
            _elements.RemoveAll(e => e.Text.Equals(text, StringComparison.
                InvariantCultureIgnoreCase));
            RecalculateVisibled();
            Repaint(repaint);
        }

#endregion

#region IEnumerable

        public IEnumerator<FlowchartItemGroupElement> GetEnumerator()
        {
            return new FlowchartItemGroupElementEnumerator(_elements);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

#endregion

    }

#region FlowchartItemGroupElementEnumerator

    public class FlowchartItemGroupElementEnumerator : IEnumerator<FlowchartItemGroupElement>
    {
        private int _index;
        private readonly List<FlowchartItemGroupElement> _elements =
            new List<FlowchartItemGroupElement>();

        public FlowchartItemGroupElementEnumerator(List<FlowchartItemGroupElement> elements)
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

        FlowchartItemGroupElement IEnumerator<FlowchartItemGroupElement>.Current
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