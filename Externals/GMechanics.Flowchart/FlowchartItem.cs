using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GMechanics.FlowchartControl.ItemControls;
using GMechanics.FlowchartControl.ItemElements;
using GMechanics.FlowchartControl.ItemPainters;
using GMechanics.FlowchartControl.LinkingArrowControl;

namespace GMechanics.FlowchartControl
{
    public partial class FlowchartItem : Control, IMessageFilter
    {
        
#region P/Invoke

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point pt);

#endregion

#region Static members

        public static string[] SkinNamesList
        {
            get { return ItemPaintersList.PainterNames; }
        }

#endregion

#region Private members

        private const int ScrollBarDelta = 30;
        private const int ResizeBorderWidth = 8;

        private static readonly Size MinimalSize = new Size(150, 150);
        private static readonly Size MaximalSize = new Size(300, 400);

        private Graphics _controlGraphics;
        private IItemPainter _activePainter;
        private GraphicsPath _itemRegion;
        private readonly ToolTip _toolTip;

        private bool _mouseDown;
        private Point _previousMousePos;
        private IItemControl _itemControlThatHandledMouse;
        private ItemResizePosition _resizePosition = ItemResizePosition.None;
        private IItemControl _controlUnderCursor;

        private readonly ItemScrollBar _scrollBar;
        private readonly ItemLinkPoint[] _linkPoints;
        private readonly bool[] _linkPointVisible;

        private string _caption;
        private Image _icon;

        private bool _repaintLocked;
        private bool _selected;

#endregion

#region Internal members

        internal Graphics Buffer;
        internal Bitmap BufferBitmap;
        internal GraphicsPath ItemRegion
        {
            get { return _itemRegion; }
            set
            {
                if (_itemRegion != null)
                {
                    _itemRegion.Dispose();
                }
                _itemRegion = value;
            }
        }

        internal Dictionary<object, IItemControl> ControlsOnScreen;
        internal WorkplaceBuffer WorkplaceBuffer;
        internal Size PreviousItemSize;
        internal Rectangle WorkplaceRectangle;
        internal readonly List<LinkingArrow> LinkingArrows;

        internal IItemControl ControlUnderCursor
        {
            get { return _controlUnderCursor; }
            set
            {
                if (_controlUnderCursor != value)
                {
                    _controlUnderCursor = value;
                    _toolTip.RemoveAll();
                    SetCursorAccordingToResizePosition(_resizePosition);
                    if (_controlUnderCursor != null)
                    {
                        if (!string.IsNullOrEmpty(_controlUnderCursor.Hint))
                        {
                            _toolTip.SetToolTip(this, _controlUnderCursor.Hint);
                        }
                    }
                }
            }
        }

        internal Flowchart.OnItemMouseEvent ItemMouseDown;
        internal Flowchart.OnItemMouseEvent ItemMouseUp;
        internal Flowchart.OnItemMouseClick ItemMouseClick;
        internal Flowchart.OnItemControlMouseEvent ItemControlMouseDown;
        internal Flowchart.OnItemControlMouseEvent ItemControlMouseUp;
        internal Flowchart.OnItemControlMouseClick ItemControlMouseClick;
        internal Flowchart.OnItemDragEnter ItemDragEnter;
        internal Flowchart.OnItemDragDrop ItemDragDrop;
        internal Flowchart.OnItemLocation ItemMoved;
        internal Flowchart.OnItemLocation ItemResized;

        internal ItemLinkPoint LeftLinkPoint
        {
            get { return _linkPoints[0]; }
        }
        internal ItemLinkPoint RightLinkPoint
        {
            get { return _linkPoints[1]; }
        }

        internal bool ReadOnly { get; set; }

#endregion

#region Properties

        public string SkinName
        {
            get { return _activePainter != null ? _activePainter.Name : String.Empty; }
            set
            {
                IItemPainter newPainter = ItemPaintersList.Instance[value];
                if (newPainter != _activePainter)
                {
                    _activePainter = ItemPaintersList.Instance[value];
                    if (ControlsOnScreen != null)
                    {
                        foreach (IItemControl control in ControlsOnScreen.Values)
                        {
                            control.Dispose();
                        }
                        ControlsOnScreen.Clear();
                    }
                    FullRepaint();
                }
            }
        }

        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                Repaint();
            }
        }

        public new Flowchart Parent
        {
            get { return (Flowchart) base.Parent; }
            internal set { base.Parent = value; }
        }

        public object UserObject { get; set; }

        public ItemGroupsList Groups { get; internal set; }

        public IItemElement SelectedItemElement
        {
            get
            {
                IItemControl control = ControlsOnScreen.Values.
                    FirstOrDefault(c => c.Selected);
                return (IItemElement) (control == null ? null : control.UserObject);
            }
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    if (!_selected)
                    {
                        foreach (IItemControl control in ControlsOnScreen.Values)
                        {
                            control.Selected = false;
                        }
                    }
                    FullRepaint();
                }
            }
        }

        public Image Icon
        {
            get
            {
                return _icon;
            }
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
                FullRepaint();
            }
        }

        public bool LeftLinkPointVisible
        {
            get { return _linkPointVisible[0]; }
            set
            {
                if (_linkPointVisible[0] != value)
                {
                    _linkPointVisible[0] = value;
                    FullRepaint();
                }
            }
        }

        public bool RightLinkPointVisible
        {
            get { return _linkPointVisible[1]; }
            set
            {
                if (_linkPointVisible[1] != value)
                {
                    _linkPointVisible[1] = value;
                    FullRepaint();
                }
            }
        }

#endregion

#region Class functions

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        public FlowchartItem(Flowchart parent, string skinName)
        {
            Application.AddMessageFilter(this);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            InitializeComponent();

            Parent = parent;
            SkinName = skinName;

            _scrollBar = new ItemScrollBar(this);
            _linkPoints = new[] { new ItemLinkPoint(), new ItemLinkPoint() };
            _linkPointVisible = new[] {true, true};

            _toolTip = new ToolTip();
            _toolTip.InitialDelay = 300;

            ControlsOnScreen = new Dictionary<object, IItemControl>();
            WorkplaceBuffer = new WorkplaceBuffer();
            LinkingArrows = new List<LinkingArrow>();
            Groups = new ItemGroupsList(this);
        }

        internal void UpdateLinkPoints()
        {
            _activePainter.UpdateLinkPoints(this, _linkPoints);
        }

        public void BeginUpdate()
        {
            _repaintLocked = true;
        }

        public void EndUpdate()
        {
            EndUpdate(true, false);
        }

        public void EndUpdate(bool repaint)
        {
            EndUpdate(repaint, false);
        }

        public void EndUpdate(bool repaint, bool fullRepaint)
        {
            _repaintLocked = false;
            if (repaint)
            {
                if (fullRepaint)
                {
                    FullRepaint();
                }
                else
                {
                    Repaint();
                }
            }
        }

        public void FullRepaint()
        {
            PreviousItemSize = new Size(-1, -1);
            Repaint(ClientRectangle, true);
        }

        public void RenitializeGraphics()
        {
            if ((BufferBitmap == null || BufferBitmap.Width != Width ||
                BufferBitmap.Height != Height) && _activePainter != null)
            {
                DestroyGraphics();
                InitializeGraphics();
            }
        }

        public void Repaint()
        {
            Repaint(ClientRectangle, true);
        }

        private void Repaint(Rectangle clipRectangle, bool repaintBody)
        {
            if (!_repaintLocked && BufferBitmap != null && _activePainter != null)
            {
                if (repaintBody)
                {
                    _activePainter.Paint(this, _scrollBar, _linkPoints);
                }
                _controlGraphics.DrawImage(BufferBitmap, clipRectangle,
                    clipRectangle, GraphicsUnit.Pixel);
            }
        }

        private void DestroyGraphics()
        {
            if (BufferBitmap != null)
            {
                BufferBitmap.Dispose();
                Buffer.Dispose();
                _controlGraphics.Dispose();
                ItemRegion.Dispose();
                foreach (IItemControl control in ControlsOnScreen.Values)
                {
                    control.Dispose();
                }
                ControlsOnScreen.Clear();
                ControlUnderCursor = null;
            }
        }

        private void InitializeGraphics()
        {
            ItemRegion = new GraphicsPath();
            BufferBitmap = new Bitmap(Width, Height);
            Buffer = Graphics.FromImage(BufferBitmap);
            Buffer.SmoothingMode = SmoothingMode.HighQuality;
            _controlGraphics = Graphics.FromHwnd(Handle);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            RenitializeGraphics();
            Repaint(e.ClipRectangle, true);
        }

        private void SetCursorAccordingToResizePosition(ItemResizePosition resizePosition)
        {
            switch (resizePosition)
            {
                case ItemResizePosition.Move:
                    Cursor = Cursors.SizeAll;
                    break;
                case ItemResizePosition.Bottom:
                    Cursor = Cursors.SizeNS;
                    break;
                case ItemResizePosition.Right:
                    Cursor = Cursors.SizeWE;
                    break;
                case ItemResizePosition.BottomRight:
                    Cursor = Cursors.SizeNWSE;
                    break;
                default:
                    Cursor = _controlUnderCursor != null
                                 ? _controlUnderCursor.Cursor
                                 : Cursors.Default;
                    break;
            }
        }

        private ItemResizePosition GetCurrentResizePosition(Point pos)
        {
            ItemResizePosition resizePosition = ItemResizePosition.None;

            if (ItemRegion != null)
            {
                RectangleF rect = ClientRectangle;
                rect.Size = ItemRegion.GetBounds().Size;

                //Right
                if (pos.X >= rect.Width - ResizeBorderWidth)
                {
                    resizePosition = pos.Y >= rect.Height - ResizeBorderWidth
                                         ? ItemResizePosition.BottomRight
                                         : ItemResizePosition.Right;
                } else 

                if (pos.Y >= rect.Height - ResizeBorderWidth)
                {
                    resizePosition = ItemResizePosition.Bottom;
                }
            }

            return resizePosition;
        }

        private void CheckOnResize(Point pos)
        {
            if (ControlUnderCursor == null)
            {
                ItemResizePosition resizePosition = GetCurrentResizePosition(pos);
                SetCursorAccordingToResizePosition(resizePosition);
            }
        }

        private void ResizeControl(Point pos)
        {
            Size newSize = Size;

            switch (_resizePosition)
            {
                case ItemResizePosition.Right:
                    newSize = new Size(Size.Width + (pos.X - _previousMousePos.X),
                                       Size.Height);
                    break;
                case ItemResizePosition.Bottom:
                    newSize = new Size(Size.Width, Size.Height + (pos.Y - _previousMousePos.Y));
                    break;
                case ItemResizePosition.BottomRight:
                    newSize = new Size(Size.Width + (pos.X - _previousMousePos.X), 
                                       Size.Height + (pos.Y - _previousMousePos.Y));
                    break;
            }

            Size = new Size(Math.Min(MaximalSize.Width, newSize.Width),
                            Math.Min(MaximalSize.Height, newSize.Height));
            Size = new Size(Math.Max(MinimalSize.Width, Size.Width),
                            Math.Max(MinimalSize.Height, Size.Height));
            
            _previousMousePos = pos;
        }

        private void MoveControl(Point pos)
        {
            Location = new Point(Location.X + (pos.X - _previousMousePos.X),
                                 Location.Y + (pos.Y - _previousMousePos.Y));
        }

        internal IItemControl GetItemControlUnderCursor(Point pos)
        {
            //Check on different item control
            foreach (IItemControl control in ControlsOnScreen.Values)
            {
                if (control.ClientRectangle.Contains(pos))
                {
                    return control;
                }
            }

            //Check on item scrollbar
            if (_scrollBar.ClientRectangle.Contains(pos))
            {
                return _scrollBar;
            }

            //Check on one of linkpoints
            foreach (ItemLinkPoint control in _linkPoints)
            {
                if (control.ClientRectangle.Contains(pos))
                {
                    return control;
                }
            }

            //No control under cursor
            return null;
        }

        private LinkingArrow GetLeftSideLinkedArrow()
        {
            return LinkingArrows.FirstOrDefault(a => a.DestinationItem == this &&
                a.State == LinkingArrowState.Linked);
        }

        private LinkingArrow GetRightSideLinkedArrow()
        {
            return LinkingArrows.FirstOrDefault(a => a.SourceItem == this &&
                a.State == LinkingArrowState.Linked);
        }

        private void OnItemControlClick(IItemControl control, MouseButtons mb)
        {
            switch (mb)
            {
                case MouseButtons.Left:
                    {
                        ItemGroup group = control.UserObject as ItemGroup;
                        if (group != null)
                        {
                            group.Collapsed = !group.Collapsed;
                        }
                        break;
                    }
            }
        }

        private void SetItemControlSelected(IItemControl control)
        {
            ItemControlType controlType = (ItemControlType)ItemControlTypes.
                TypesTable[control.GetType()];
            if (controlType == ItemControlType.Button)
            {
                foreach (IItemControl c in ControlsOnScreen.Values)
                {
                    c.Selected = c == control;
                }
            }
        }

        private bool CheckOnRepaintItemControls(Point pos, MouseButtons mb)
        {
            //If mouse inside workplace rectangle, get item control under cursor
            IItemControl control = GetItemControlUnderCursor(pos);
            if (!WorkplaceRectangle.Contains(pos) && !(control is ItemLinkPoint))
            {
                control = null;
            }

            //If control under cursor it`s not same as previous control under cursor
            //or control state was changed, process changes
            if (control != ControlUnderCursor || (control != null && 
                (ItemControlHelper.ControlStateToMouseButton(control) != mb)))
            {
                //Control state was changed
                if (control != null && control == ControlUnderCursor)
                {
                    ItemControlState controlState = control.State;
                    ItemControlHelper.SetControlStateAccordingToMouse(control, mb);

                    //Check on mouse down
                    if (control.State == ItemControlState.ClickedLeft ||
                        control.State == ItemControlState.ClickedRight)
                    {
                        //Update current item control selection
                        SetItemControlSelected(control);

                        //Call ItemControlMouseDown delegate
                        if (ItemControlMouseDown != null)
                        {
                            ItemControlMouseDown(this, (IItemElement) control.UserObject, 
                                new MouseEventArgs(mb, 1, pos.X, pos.Y, 0));
                        }
                    }

                    //Check on mouse click
                    else if ((controlState == ItemControlState.ClickedLeft || 
                              controlState == ItemControlState.ClickedRight) &&
                              control.State == ItemControlState.MouseOn)
                    {
                        //Get popped mouse button
                        mb = controlState == ItemControlState.ClickedLeft
                                 ? MouseButtons.Left
                                 : MouseButtons.Right;

                        //Call ItemControlMouseUp delegate
                        if (ItemControlMouseUp != null)
                        {
                            ItemControlMouseUp(this, control.UserObject as IItemElement, 
                                new MouseEventArgs(mb, 1, pos.X, pos.Y, 0));
                        }

                        //Internal ItemControlClick
                        OnItemControlClick(control, mb);

                        //Call ItemControlMouseClick delegate
                        if (ItemControlMouseClick != null)
                        {
                            ItemControlMouseClick(this, (IItemElement)control.UserObject);
                        }
                    }
                }
                else
                {
                    //If control under cursor it`s not same as previous control under cursor
                    if (ControlUnderCursor != null && ControlUnderCursor.State != ItemControlState.Disabled)
                    {
                        ControlUnderCursor.State = ItemControlState.Normal;
                    }
                    if (control != null)
                    {
                        control.State = ItemControlState.MouseOn;
                    }
                }

                //Set current item control as Control under cursor
                ControlUnderCursor = control;

                //Repaint item
                Repaint();

                //Drop resize position
                _resizePosition = ItemResizePosition.None;
                SetCursorAccordingToResizePosition(_resizePosition);

                //Done
                return true;
            }

            //No changes
            return false;
        }

        private void CheckOnSelectedItemControlMouseMove(Point pos)
        {
            //No control handled mouse
            if (_itemControlThatHandledMouse == null)
            {
                return;
            }

            //Get current control type
            ItemControlType controlType = (ItemControlType)ItemControlTypes.
                TypesTable[_itemControlThatHandledMouse.GetType()];

            switch (controlType)
            {
                //LinkPoint
                case ItemControlType.LinkPoint:
                    {
                        if (ReadOnly)
                        {
                            return;
                        }

                        LinkingArrow arrow = Parent.CurrentLinkingArrow;
                        FlowchartItem srcItem = arrow.SourceItem;

                        //Get destionation item under mouse cursor
                        FlowchartItem destItem = Parent.GetItemForLinkingUnderCursor(this, srcItem, pos);

                        //If destionation exists
                        if (destItem != null)
                        {
                            //If destItem not setted yet as destination item for the current linking arrow
                            if (destItem != arrow.DestinationItem)
                            {
                                //Set left link point selected
                                destItem.LeftLinkPoint.Selected = true;
                                arrow.DestinationItem = destItem;

                                //Repaint self and parent
                                Parent.Repaint();
                                destItem.Repaint();
                            }
                        }

                        //otherwise
                        else
                        {
                            destItem = arrow.DestinationItem;
                            if (destItem != null)
                            {
                                //Set left link point unselected
                                destItem.LeftLinkPoint.Selected = false;

                                //Repaint self
                                destItem.Repaint();

                                //Drop destination item references
                                destItem = arrow.DestinationItem = null;
                            }

                            //Update current linking arrow dest point according to mouse position
                            Point point = Parent.PointToClient(PointToScreen(pos));
                            Parent.UpdateLinkingArrowEndPoint(arrow, point);
                            Parent.Repaint();
                        }

                        //Temporary set destination item for the current linking arrow
                        arrow.DestinationItem = destItem;

                        break;
                    }
            }
        }

        private bool CheckOnSelectedItemControlHandleMouse(Point pos)
        {
            if (ControlUnderCursor != null)
            {
                //Get current control type
                ItemControlType controlType = (ItemControlType)ItemControlTypes.
                    TypesTable[ControlUnderCursor.GetType()];

                //Store current item control that handled mouse
                _itemControlThatHandledMouse = ControlUnderCursor;

                switch (controlType)
                {
                    //LinkPoint
                    case ItemControlType.LinkPoint:
                        {
                            if (ReadOnly)
                            {
                                return true;
                            }

                            //For right-side link point
                            if (ControlUnderCursor == RightLinkPoint)
                            {
                                //Add new linking arrow
                                Parent.CreateLinkingArrow(this);

                                //Set selected state for the current link point and repaint self
                                _itemControlThatHandledMouse.Selected = true;
                                Repaint();
                            }

                            //For left-side link point
                            else
                            {
                                LinkingArrow arrow = GetLeftSideLinkedArrow();
                                if (arrow != null)
                                {
                                    Parent.UnlinkArrrow(arrow, this, true);
                                    Parent.CurrentLinkingArrow = arrow;
                                    _itemControlThatHandledMouse.Selected = GetLeftSideLinkedArrow() != null;
                                }
                                else
                                {
                                    _itemControlThatHandledMouse = null;
                                }
                            }
                            break;
                        }

                    //Image
                    case ItemControlType.Image:
                        {
                            return false;
                        }
                }

                //Case mouse movement
                CheckOnSelectedItemControlMouseMove(pos);

                //Mouse handled
                return true;
            }

            //No control under cursor
            return false;
        }

        private bool CheckOnSelectedItemControlUnhandleMouse()
        {
            //No control handled mouse
            if (_itemControlThatHandledMouse == null)
            {
                return false;
            }

            //Get control type
            ItemControlType controlType = (ItemControlType) ItemControlTypes.
                TypesTable[_itemControlThatHandledMouse.GetType()];

            switch (controlType)
            {
                //LinkPoint
                case ItemControlType.LinkPoint:
                    {
                        if (ReadOnly)
                        {
                            return true;
                        }

                        LinkingArrow arrow = Parent.CurrentLinkingArrow;
                        FlowchartItem srcItem = arrow.SourceItem;
                        FlowchartItem destItem = arrow.DestinationItem;

                        //If no endpoint defined for the current linking arrow
                        if (destItem == null)
                        {
                            //Remove linking arrow from parent arrows list
                            Parent.RemoveLinkingArrow(arrow);

                            //Set unselected state for the current link point
                            srcItem.RightLinkPoint.Selected = srcItem.GetRightSideLinkedArrow() != null;
                        }

                        //otherwise connect current linking arrow
                        else
                        {
                            bool cancelled = !Parent.LinkArrow(arrow, destItem, true);

                            //Check if linking was cancelled
                            if (cancelled)
                            {
                                //Remove linking arrow from parent arrows list
                                Parent.RemoveLinkingArrow(arrow);

                                //Set unselected state for the link points
                                destItem.LeftLinkPoint.Selected = srcItem.GetLeftSideLinkedArrow() != null;
                                srcItem.RightLinkPoint.Selected = srcItem.GetRightSideLinkedArrow() != null;
                            }
                        }

                        //Repaint source, destination items and parent
                        if (this != srcItem)
                        {
                            srcItem.Repaint();
                        }
                        Repaint();
                        Parent.Repaint();

                        break;
                    }
            }

            //Mouse unhandled
            return true;
        }

        private void FlowchartItemMouseMove(object sender, MouseEventArgs e)
        {
            //If mouse button pressed
            if (_mouseDown)
            {
                //Check on mouse handled by item control
                if (_resizePosition == ItemResizePosition.None &&
                    CheckOnSelectedItemControlHandleMouse(e.Location))
                {
                    _resizePosition = ItemResizePosition.HandledByItemControl;
                }

                //If mouse handled by item control, process mouse by this control
                if (_resizePosition == ItemResizePosition.HandledByItemControl)
                {
                    CheckOnSelectedItemControlMouseMove(e.Location);
                }

                //Do resize control
                else if (_resizePosition > ItemResizePosition.Move)
                {
                    ResizeControl(e.Location);
                }

                //Do move control
                else if (!ReadOnly && e.Button == MouseButtons.Left)
                {   
                    //Select actual resize position
                    if (_resizePosition == ItemResizePosition.None)
                    {
                        _resizePosition = ItemResizePosition.Move;
                        SetCursorAccordingToResizePosition(_resizePosition);
                    }

                    //Move control
                    MoveControl(e.Location);

                    //Repaint parent for linking arrows
                    if (Parent.LinkingArrows.Count > 0)
                    {
                        Parent.Repaint();
                    }
                }
            }
            else
            {
                //If item control under cursor, check on repaint
                if (!CheckOnRepaintItemControls(e.Location, MouseButtons.None) && !ReadOnly)
                {
                    //otherwise check on item resize
                    CheckOnResize(e.Location);
                }
            }
        }

        private void FlowchartItemMouseLeave(object sender, EventArgs e)
        {
            //Fix false mouse leave event caused by hint showing
            Control thisItem = Parent.GetChildAtPoint(Parent.PointToClient(MousePosition));
            if (thisItem == this)
            {
                return;
            }

            //Turn current resize position state to None
            _resizePosition = ItemResizePosition.None;
            SetCursorAccordingToResizePosition(_resizePosition);

            //Reset MouseOn state for control under cursor
            if (ControlUnderCursor != null)
            {
                if (ControlUnderCursor.State != ItemControlState.Disabled)
                {
                    ControlUnderCursor.State = ItemControlState.Normal;
                }
                ControlUnderCursor = null;
                Repaint();
            }
        }

        private void FlowchartItemMouseDown(object sender, MouseEventArgs e)
        {
            if (Parent.SetItemSelected(this))
            {
                _mouseDown = true;
                _previousMousePos = e.Location;

                //Set item selected
                Selected = true;

                //If item control under cursor, check on repaint
                if (!CheckOnRepaintItemControls(e.Location, e.Button))
                {
                    //otherwise check on item resize
                    if (!ReadOnly && e.Button == MouseButtons.Left)
                    {
                        _resizePosition = GetCurrentResizePosition(e.Location);
                    }

                    //Call ItemMouseDown delegate
                    if (ItemMouseDown != null)
                    {
                        ItemMouseDown(this, e);
                    }
                }
            }
        }

        private void FlowchartItemMouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;

            //If some control was handled mouse
            if (CheckOnSelectedItemControlUnhandleMouse())
            {
                _itemControlThatHandledMouse = null;
            }
            else
            {
                if (LinkingArrows.Count > 0)
                {
                    Parent.Repaint();
                }
            }

            //If item control under cursor, check on repaint
            if (!CheckOnRepaintItemControls(e.Location, e.Button))
            {
                //otherwise check on item resize or move
                if (!ReadOnly && e.Button == MouseButtons.Left)
                {
                    ItemResizePosition resizePosition = _resizePosition;
                    _resizePosition = GetCurrentResizePosition(e.Location);
                    SetCursorAccordingToResizePosition(_resizePosition);

                    //Fire actual delegate
                    switch (resizePosition)
                    {
                        case ItemResizePosition.Move:
                            {
                                if (ItemMoved != null)
                                {
                                    ItemMoved(this);
                                }
                                break;
                            }
                        default:
                            {
                                if (_resizePosition <= ItemResizePosition.BottomRight &&
                                    _resizePosition > ItemResizePosition.None &&
                                    ItemResized != null)
                                {
                                    ItemResized(this);
                                }
                                break;
                            }
                    }
                }

                //Call ItemMouseUp delegate
                if (ItemMouseUp != null)
                {
                    ItemMouseUp(this, e);
                }

                //Call ItemMouseClick delegate
                if (ItemMouseClick != null)
                {
                    ItemMouseClick(this);
                }
            }
        }

        private void FlowchartItemDragEnter(object sender, DragEventArgs e)
        {
            if (!ReadOnly && ItemDragEnter != null)
            {
                ItemDragEnter(this, e);
            }
        }

        private void FlowchartItemDragDrop(object sender, DragEventArgs e)
        {
            if (!ReadOnly && ItemDragDrop != null)
            {
                ItemDragDrop(this, e);
            }
        }

        public new void Dispose()
        {
            base.Dispose();
            DestroyGraphics();
            WorkplaceBuffer.Dispose();
            _scrollBar.Dispose();
            _linkPoints[0].Dispose();
            _linkPoints[1].Dispose();
            if (_toolTip != null)
            {
                _toolTip.Dispose();
            }
            if (_icon != null)
            {
                _icon.Dispose();
            }
            Application.RemoveMessageFilter(this);
        }

#endregion

#region IMessageFilter

        protected override void WndProc(ref Message m)
        {
            if (m.Msg != 0x7)
            {
                base.WndProc(ref m);
            }
        } 

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x20a)
            {
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                if (WindowFromPoint(pos) == Handle)
                {
                    _scrollBar.Position += m.WParam.ToInt32() < 0
                                               ? ScrollBarDelta
                                               : -ScrollBarDelta;
                    CheckOnRepaintItemControls(PointToClient(pos), MouseButtons.None);
                    return true;
                }
            }
            return false;
        }

#endregion

    }
}
