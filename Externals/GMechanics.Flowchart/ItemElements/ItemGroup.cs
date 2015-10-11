using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GMechanics.FlowchartControl.ItemElements
{
    public class ItemGroup : IComparable<ItemGroup>, IItemElement
    {

#region Private members

        private readonly ItemGroupsList _parentGroupsList;

        private string _caption;
        private bool _collapsed;
        private bool _visible;
        private bool _iconVisible;

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

        public bool Collapsed
        {
            get { return _collapsed; }
            set
            {
                _collapsed = value;
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

        public bool IconVisible
        {
            get { return _iconVisible; }
            set
            {
                _iconVisible = value;
                Repaint();
            }
        }

        internal ItemGroupsList ParentGroupsList
        {
            get { return _parentGroupsList; }
        }

        public ItemElementType ElementType { get { return ItemElementType.Group; } }

        public object UserObject { get; set; }

        public ItemGroupElementsList Elements { get; private set; }

        public Image Icon { get; private set; }

#endregion

#region Class functions

        internal ItemGroup(ItemGroupsList parentGroupsList, string caption,
            ImageList imageList, int iconIndex)
        {
            _caption = caption;
            _iconVisible = _visible = true;
            _parentGroupsList = parentGroupsList;
            if (imageList != null && iconIndex >= 0 && 
                iconIndex < imageList.Images.Count)
            {
                Icon = new Bitmap(imageList.Images[iconIndex]);
            }
            Elements = new ItemGroupElementsList(this, null, 0);
        }

        ~ItemGroup()
        {
            if (Icon != null)
            {
                Icon.Dispose();
            }
        }

        public void Expand()
        {
            Collapsed = true;
        }

        public void Collapse()
        {
            Collapsed = false;
        }

        public void Repaint()
        {
            ParentGroupsList.ParentItem.Repaint();
        }

        public List<ItemGroupElement> GetVisibleElements()
        {
            List<ItemGroupElement> elements = new List<ItemGroupElement>();
            Elements.GetVisible(elements);
            return elements;
        }

        public int CompareTo(ItemGroup other)
        {
            return String.CompareOrdinal(Caption, other.Caption);
        }

#endregion

    }

}