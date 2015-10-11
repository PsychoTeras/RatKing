using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GMechanics.FlowchartControl.ItemControls
{
    internal class ItemLinkPoint : IItemControl
    {

#region Private members

        private int _linkPointSize = 5;
        private Color _linkPointColor = Color.FromArgb(255, 0, 0, 0);
        private Color _linkPointColorMouseOn = Color.FromArgb(255, 255, 255, 255);
        private Color _linkPointColorSelected = Color.FromArgb(255, 125, 125, 125);

        private Brush _linkPointBrush;
        private Brush _linkPointBrushMouseOn;
        private Brush _linkPointBrushSelected;

#endregion

#region Properties

        public Color Color
        {
            get { return _linkPointColor; }
            set
            {
                if (_linkPointColor != value)
                {
                    _linkPointColor = value;
                    RecreateGraphic();
                }
            }
        }

        public Color ColorMouseOn
        {
            get { return _linkPointColorMouseOn; }
            set
            {
                if (_linkPointColorMouseOn != value)
                {
                    _linkPointColorMouseOn = value;
                    RecreateGraphic();
                }
            }
        }

        public Color ColorSelected
        {
            get { return _linkPointColorSelected; }
            set
            {
                if (_linkPointColorSelected != value)
                {
                    _linkPointColorSelected = value;
                    RecreateGraphic();
                }
            }
        }

        public int Size
        {
            get { return _linkPointSize; }
            set
            {
                if (_linkPointSize != value)
                {
                    _linkPointSize = value;
                    RecreateGraphic();
                }
            }
        }

        public ItemControlType ControlType { get { return ItemControlType.LinkPoint; } }
        public ItemControlState State { get; set; }
        public bool Selected { get; set; }
        public Rectangle ClientRectangle { get; set; }
        public object UserObject { get; private set; }
        public OnItemPrePaint ItemPrePaint { get; set; }
        public OnItemPostPaint ItemPostPaint { get; set; }
        public bool Destroyed { get; set; }
        public Cursor Cursor { get; set; }
        public string Hint { get; set; }
        public bool IsStaticControl { get { return true; } }
        public bool Visible { get; set; }

#endregion

#region Class functions

        public ItemLinkPoint()
        {
            RecreateGraphic();
            UserObject = null;
        }

        private void RecreateGraphic()
        {
            if (_linkPointBrush != null)
            {
                _linkPointBrush.Dispose();
            }
            _linkPointBrush = new SolidBrush(_linkPointColor);

            if (_linkPointBrushMouseOn != null)
            {
                _linkPointBrushMouseOn.Dispose();
            }
            _linkPointBrushMouseOn = new SolidBrush(_linkPointColorMouseOn);

            if (_linkPointBrushSelected != null)
            {
                _linkPointBrushSelected.Dispose();
            }
            _linkPointBrushSelected = new SolidBrush(_linkPointColorSelected);
        }

        public void Draw(Graphics graphics, Color color, Color colorMouseOn, 
            Color colorSelected)
        {
            Color = color;
            ColorMouseOn = colorMouseOn;
            ColorSelected = colorSelected;
            Draw(graphics);
        }

        private void DrawNormal(Graphics graphics)
        {
            graphics.FillRectangle(_linkPointBrush, ClientRectangle);
        }

        private void DrawMouseOn(Graphics graphics)
        {
            Rectangle rectangle = Rectangle.Inflate(ClientRectangle, -1, -1);
            graphics.FillRectangle(_linkPointBrushMouseOn, rectangle);
        }

        private void DrawSelected(Graphics graphics)
        {
            Rectangle rectangle = Rectangle.Inflate(ClientRectangle, -1, -1);
            graphics.FillRectangle(_linkPointBrushSelected, rectangle);
        }

        public void Draw(Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.None;

            if (ItemPrePaint != null)
            {
                ItemPrePaint(graphics, this);
            }

            DrawNormal(graphics);

            switch (State)
            {
                case ItemControlState.MouseOn:
                case ItemControlState.ClickedLeft:
                case ItemControlState.ClickedRight:
                    DrawMouseOn(graphics);
                    break;
            }

            if (Selected && State != ItemControlState.MouseOn)
            {
                DrawSelected(graphics);
            }

            if (ItemPostPaint != null)
            {
                ItemPostPaint(graphics, this);
            }

            graphics.SmoothingMode = SmoothingMode.HighQuality;
        }

        public void Dispose()
        {
            _linkPointBrush.Dispose();
            _linkPointBrushMouseOn.Dispose();
        }

#endregion

    }
}
