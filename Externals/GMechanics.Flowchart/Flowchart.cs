using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using GMechanics.FlowchartControl.ItemElements;
using GMechanics.FlowchartControl.LinkingArrowControl;

namespace GMechanics.FlowchartControl
{
    public partial class Flowchart : Control
    {

#region Delegates

        public delegate void OnItemMouseEvent(FlowchartItem item, 
            MouseEventArgs e);
        public delegate void OnItemMouseClick(FlowchartItem item);
        public delegate void OnItemControlMouseEvent(FlowchartItem item,
            IItemElement itemElement, MouseEventArgs e);
        public delegate void OnItemControlMouseClick(FlowchartItem item,
            IItemElement itemElement);
        public delegate void OnItemLinking(FlowchartItem srcItem,
            FlowchartItem destItem, out bool cancelled);
        public delegate void OnItemLink(FlowchartItem srcItem,
            FlowchartItem destItem, bool manual);
        public delegate void OnItemDragEnter(FlowchartItem item,
            DragEventArgs e);
        public delegate void OnItemDragDrop(FlowchartItem item,
            DragEventArgs e);
        public delegate void OnItemLocation(FlowchartItem item);
        public delegate void OnItemSelected(FlowchartItem item);

#endregion

#region Events

        [Category("Action")]
        public event OnItemMouseEvent ItemMouseDown;

        [Category("Action")]
        public event OnItemMouseEvent ItemMouseUp;

        [Category("Action")]
        public event OnItemMouseClick ItemMouseClick;

        [Category("Action")]
        public event OnItemControlMouseEvent ItemElementMouseDown;

        [Category("Action")]
        public event OnItemControlMouseEvent ItemElementMouseUp;

        [Category("Action")]
        public event OnItemControlMouseClick ItemElementMouseClick;

        [Category("Action")]
        public event OnItemLinking ItemLinking;

        [Category("Action")]
        public event OnItemLink ItemLinked;

        [Category("Action")]
        public event OnItemLink ItemUnlinked;

        [Category("Action")]
        public event OnItemDragEnter ItemDragEnter;

        [Category("Action")]
        public event OnItemDragDrop ItemDragDrop;

        [Category("Action")]
        public event OnItemLocation ItemMoved;

        [Category("Action")]
        public event OnItemLocation ItemResized;

        [Category("Action")]
        public event OnItemSelected ItemSelected;

#endregion

#region Constants

        private static readonly Rectangle DefaultItemClientRectangle = new
            Rectangle(30, 30, 250, 250);

#endregion

#region Private members

        private Graphics _buffer;
        private Bitmap _bufferBitmap;
        private Graphics _controlGraphics;

        private bool _readOnly;
        private bool _repaintLocked;
        private readonly Bitmap _editModeIcon;

#endregion

#region Internal members

        internal LinkingArrow CurrentLinkingArrow;
        internal List<FlowchartItem> Items = new List<FlowchartItem>();
        internal List<LinkingArrow> LinkingArrows = new List<LinkingArrow>();

#endregion

#region Properties

        #region Hidden

        [DefaultValue(null), Browsable(false)]
        public new string Text { get; set; }

        [Browsable(false)]
        public new Color ForeColor { get; set; }

        [Browsable(false)]
        public new bool TabStop { get; set; }

        [Browsable(false)]
        public new int TabIndex { get; set; }

        [Browsable(false)]
        public new Padding Padding
        {
            get { return base.Padding; }
            set
            {
                base.Padding = value;
            }
        }

        [Browsable(false)]
        public new Image BackgroundImage { get; set; }

        [Browsable(false)]
        public new ImageLayout BackgroundImageLayout { get; set; }

        [Browsable(false)]
        public new bool AutoSize { get; set; }

        #endregion

        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
                Repaint();
            }
        }

        public FlowchartItem SelectedItem
        {
            get { return Items.FirstOrDefault(i => i.Selected); }
            set { SetItemSelected(value); }
        }

        [Browsable(true), DefaultValue(false)]
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                if (_readOnly != value)
                {
                    _readOnly = value;
                    foreach (FlowchartItem item in Items)
                    {
                        item.ReadOnly = _readOnly;
                        item.FullRepaint();
                    }
                    Repaint();
                }
            }
        }

#endregion

#region Class functions

        protected override void WndProc(ref Message m)
        {
            if ((m.Msg == 0x114 || m.Msg == 0x115) && 
                ((int) m.WParam & 0xFFFF) == 5)
            {
                m.WParam = (IntPtr) (((int) m.WParam & ~0xFFFF) | 4);
            }
            base.WndProc(ref m);
        }

        public Flowchart()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.Selectable, false);

            InitializeComponent();

            base.TabStop = false;

            using (Stream stream = Assembly.GetExecutingAssembly().
                GetManifestResourceStream("GMechanics.FlowchartControl.Resources.EditMode.png"))
            {
                if (stream != null)
                {
                    _editModeIcon = new Bitmap(stream);
                }
            }
        }

        public Flowchart(Control parent) : this()
        {
            Parent = parent;
        }

        public void BeginUpdate()
        {
            _repaintLocked = true;
        }

        public void EndUpdate()
        {
            EndUpdate(true, true);
        }

        public void EndUpdate(bool repaintWithItems)
        {
            EndUpdate(repaintWithItems, true);
        }

        public void EndUpdate(bool repaintWithItems, bool fullRepaint)
        {
            _repaintLocked = false;
            if (repaintWithItems)
            {
                RepaintWithItems(fullRepaint);
            }
            else
            {
                Repaint();
            }
        }

        private void DestroyGraphics()
        {
            if (_buffer != null)
            {
                _buffer.Dispose();
                _bufferBitmap.Dispose();
                _controlGraphics.Dispose();
                _buffer = null;
            }
        }

        private void InitializeGraphics()
        {
            _bufferBitmap = new Bitmap(Width, Height);
            _buffer = Graphics.FromImage(_bufferBitmap);
            _buffer.SmoothingMode = SmoothingMode.HighQuality;
            _controlGraphics = Graphics.FromHwnd(Handle);
        }

        private Point GetLinkArrowStartPoint(LinkingArrow arrow)
        {
            Rectangle rectangle = arrow.SourceItem.RightLinkPoint.ClientRectangle;
            rectangle.Offset(arrow.SourceItem.Left, arrow.SourceItem.Top);
            int x = rectangle.Left + rectangle.Width - 2;
            int y = rectangle.Top + (rectangle.Height/2) - 1;
            Point startPoint = arrow.SourceItem.PointToClient(
                arrow.SourceItem.PointToScreen(new Point(x, y)));
            return startPoint;
        }

        private Point GetLinkArrowEndPoint(LinkingArrow arrow)
        {
            if (arrow.DestinationItem != null)
            {
                Rectangle rectangle = arrow.DestinationItem.LeftLinkPoint.ClientRectangle;
                rectangle.Offset(arrow.DestinationItem.Left, arrow.DestinationItem.Top);
                int x = rectangle.Left;
                int y = rectangle.Top + (rectangle.Height / 2) - 1;
                Point endPoint = arrow.DestinationItem.PointToClient(
                    arrow.DestinationItem.PointToScreen(new Point(x, y)));
                return endPoint;
            }
            return arrow.EndPoint;
        }

        private void PaintLinkArrows()
        {
            if (_bufferBitmap != null)
            {
                _buffer.Clear(BackColor);
                foreach (LinkingArrow arrow in LinkingArrows)
                {
                    Point startPoint = GetLinkArrowStartPoint(arrow);
                    Point endPoint = GetLinkArrowEndPoint(arrow);
                    arrow.Draw(_buffer, startPoint, endPoint);
                }
            }
        }

        private void PaintReadOnlyIcon()
        {
            if (!_readOnly && Items.Count > 0 && _bufferBitmap != null && 
                _editModeIcon != null)
            {
                const int margin = 1;
                Point point = new Point(ClientRectangle.Width -
                    _editModeIcon.Width - margin - 1, margin);
                _buffer.DrawImageUnscaled(_editModeIcon, point);
            }
        }

        public void Repaint()
        {
            if (!_repaintLocked)
            {
                PaintLinkArrows();
                PaintReadOnlyIcon();
                Repaint(ClientRectangle);
            }
        }

        public void RepaintWithItems(bool fullRepaint)
        {
            if (!_repaintLocked)
            {
                foreach (FlowchartItem item in Items)
                {
                    item.EndUpdate(false);
                    item.RenitializeGraphics();
                    item.UpdateLinkPoints();
                    if (fullRepaint)
                    {
                        item.FullRepaint();
                    }
                    else
                    {
                        item.Repaint();
                    }
                }
                Repaint();
            }
        }

        private void Repaint(Rectangle clipRectangle)
        {
            if (!_repaintLocked)
            {
                if (_bufferBitmap == null || _bufferBitmap.Width != Width ||
                    _bufferBitmap.Height != Height)
                {
                    DestroyGraphics();
                    InitializeGraphics();
                    PaintLinkArrows();
                    PaintReadOnlyIcon();
                }

                if (_bufferBitmap != null)
                {
                    _controlGraphics.DrawImage(_bufferBitmap, clipRectangle,
                        clipRectangle, GraphicsUnit.Pixel);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!_repaintLocked)
            {
                Repaint(e.ClipRectangle);
            }
        }

        internal LinkingArrow CreateLinkingArrow(FlowchartItem sourceItem)
        {
            LinkingArrow arrow = new LinkingArrow(sourceItem, null, LinkingArrowState.Linking);
            LinkingArrows.Add(CurrentLinkingArrow = arrow);
            sourceItem.LinkingArrows.Add(arrow);
            return arrow;
        }

        internal void UpdateLinkingArrowEndPoint(LinkingArrow arrow, Point endPoint)
        {
            arrow.EndPoint = endPoint;
        }

        internal void RemoveLinkingArrow(LinkingArrow arrow)
        {
            if (arrow.SourceItem != null)
            {
                arrow.SourceItem.LinkingArrows.Remove(arrow);
            }
            if (arrow.DestinationItem != null)
            {
                arrow.DestinationItem.LinkingArrows.Remove(arrow);
            }
            LinkingArrows.Remove(arrow);
            if (CurrentLinkingArrow == arrow)
            {
                CurrentLinkingArrow = null;
            }
            arrow.Dispose();
        }

        internal bool LinkArrow(LinkingArrow arrow, FlowchartItem destinationItem,
            bool manual)
        {
            bool cancelled = false;
            if (ItemLinking != null)
            {
                ItemLinking(arrow.SourceItem, destinationItem, out cancelled);
            }

            if (!cancelled)
            {
                arrow.Link(destinationItem);
                destinationItem.LinkingArrows.Add(arrow);
                arrow.SourceItem.RightLinkPoint.Selected = 
                    destinationItem.LeftLinkPoint.Selected = true;
                if (CurrentLinkingArrow == arrow)
                {
                    CurrentLinkingArrow = null;
                }

                if (ItemLinked != null)
                {
                    ItemLinked(arrow.SourceItem, destinationItem, manual);
                }
            }

            return !cancelled;
        }

        internal void UnlinkArrrow(LinkingArrow arrow, FlowchartItem destinationItem,
            bool manual)
        {
            destinationItem.LinkingArrows.Remove(arrow);
            if (ItemUnlinked != null && arrow.SourceItem != null &&
                arrow.DestinationItem != null)
            {
                ItemUnlinked(arrow.SourceItem, arrow.DestinationItem, manual);
            }
            arrow.Unlink();
        }

        internal FlowchartItem GetItemForLinkingUnderCursor(FlowchartItem callerItem, 
            FlowchartItem srcItem, Point mousePos)
        {
            int cnt = Items.Count - 1;
            for (int i = cnt; i >= 0; i--)
            {
                FlowchartItem item = Items[i];
                if (item != srcItem)
                {
                    Point point = item.PointToClient(callerItem.PointToScreen(mousePos));
                    if (item.LeftLinkPointVisible && item.LeftLinkPoint.ClientRectangle.Contains(point))
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        public void LinkItems(FlowchartItem srcItem, FlowchartItem destItem)
        {
            if (srcItem != null && destItem != null)
            {
                LinkingArrow arrow = CreateLinkingArrow(srcItem);
                LinkArrow(arrow, destItem, false);
            }
        }

        internal bool SetItemSelected(FlowchartItem item)
        {
            /*bool shiftPressed = (ModifierKeys & Keys.Shift) == Keys.Shift;
            if (shiftPressed)
            {
                item.Selected = !item.Selected;
            }
            else */
            foreach (FlowchartItem i in Items)
            {
                i.Selected = i == item;
            }
            ItemSelected(item);
            return true;
        }

        private void FlowchartMouseDown(object sender, MouseEventArgs e)
        {
            foreach (FlowchartItem i in Items)
            {
                i.Selected = false;
            }
        }

        public bool ContainsItemWithUserObject(object userObject)
        {
            return GetItemWithUserObject(userObject) != null;
        }

        public FlowchartItem GetItemWithUserObject(object userObject)
        {
            foreach (FlowchartItem item in Items)
            {
                if (item.UserObject == userObject)
                {
                    return item;
                }
            }
            return null;
        }

        public new void Dispose()
        {
            DestroyGraphics();
            foreach (LinkingArrow arrow in LinkingArrows)
            {
                arrow.Dispose();
            }
            base.Dispose();
        }

#endregion

#region Item functions

        public void ClearItems()
        {
            foreach (FlowchartItem item in Items)
            {
                item.Dispose();
            }
            Items.Clear();
            foreach (LinkingArrow arrow in LinkingArrows)
            {
                arrow.Dispose();
            }
            LinkingArrows.Clear();
            CurrentLinkingArrow = null;
            Repaint();
        }

        public FlowchartItem AddItem(string caption)
        {
            return AddItem(caption, string.Empty, DefaultItemClientRectangle, null);
        }

        public FlowchartItem AddItem(string caption, string skinName)
        {
            return AddItem(caption, skinName, DefaultItemClientRectangle, null);
        }

        public FlowchartItem AddItem(string caption, string skinName,
            Rectangle clientRectangle)
        {
            return AddItem(caption, skinName, clientRectangle, null);
        }

        public FlowchartItem AddItem(string caption, string skinName,
            Rectangle clientRectangle, object userObject)
        {
            FlowchartItem item = new FlowchartItem(this, skinName)
                                     {
                                         Caption = caption,
                                         Size = clientRectangle.Size,
                                         Location = clientRectangle.Location,
                                         UserObject = userObject
                                     };

            item.ItemMouseDown += ItemMouseDown;
            item.ItemMouseUp += ItemMouseUp;
            item.ItemMouseClick += ItemMouseClick;
            item.ItemControlMouseDown += ItemElementMouseDown;
            item.ItemControlMouseUp += ItemElementMouseUp;
            item.ItemControlMouseClick += ItemElementMouseClick;
            item.ItemDragEnter += ItemDragEnter;
            item.ItemDragDrop += ItemDragDrop;
            item.ItemMoved += ItemMoved;
            item.ItemResized += ItemResized;
            item.ReadOnly = _readOnly;

            item.BringToFront();
            Items.Add(item);

            Repaint();

            return item;
        }

#endregion

    }
}