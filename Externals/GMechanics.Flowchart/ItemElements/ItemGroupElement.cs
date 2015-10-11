using System;
using System.Drawing;
using System.Windows.Forms;

namespace GMechanics.FlowchartControl.ItemElements
{
    public class ItemGroupElement : IComparable<ItemGroupElement>, IItemElement
    {

#region Private members

        private readonly ItemGroup _parentGroup;

        private string _text;
        private bool _visible;
        private Image _icon;
        private Cursor _iconCursor;
        private bool _iconVisible;

#endregion

#region Properties

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value ?? string.Empty;
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

        public Image Icon
        {
            get { return _icon; }
            set
            {
                if (_icon != null)
                {
                    _icon.Dispose();
                    _icon = null;
                }
                if (value != null)
                {
                    _icon = new Bitmap(value);
                }
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

        public Cursor IconCursor
        {
            get { return _iconCursor; }
            set
            {
                _iconCursor = value;
                Repaint();
            }
        }

        public string IconHint { get; set; }
      
        private ItemGroup ParentGroup
        {
            get { return _parentGroup; }
        }

        public ItemElementType ElementType { get { return ItemElementType.GroupElement; } }

        public object UserObject { get; set; }

        public ItemGroupElementsList Elements { get; private set; }

        public int NestingLevel { get; private set; }

        public object Tag { get; set; }

#endregion

#region Class functions

        internal ItemGroupElement(ItemGroup parentGroup, string text,
            object userObject, int nestingLevel)
        {
            _text = text;
            _parentGroup = parentGroup;
            _iconVisible = _visible = true;
            UserObject = userObject;
            NestingLevel = nestingLevel;
            IconCursor = Cursors.Default;
            Elements = new ItemGroupElementsList(ParentGroup, this, NestingLevel + 1);
        }

        ~ItemGroupElement()
        {
            if (_icon != null)
            {
                _icon.Dispose();
            }
        }

        public void Repaint()
        {
            ParentGroup.Repaint();
        }

        public int CompareTo(ItemGroupElement other)
        {
            return String.CompareOrdinal(Text, other.Text);
        }

#endregion

    }
}
