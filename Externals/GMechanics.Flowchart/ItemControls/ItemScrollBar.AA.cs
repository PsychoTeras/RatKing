using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using FlowchartControl.ItemPainters;

namespace FlowchartControl.ItemControls
{
    internal class ItemScrollBar : IItemControl
    {

#region Private members

        private int _width;
        private int _position;
        private readonly FlowchartItem _item;

        private Color _bodyColor;
        private Color _frontColor;
        private Color _marksColor;

        private Brush _bodyBrush;
        private Brush _frontBrush;
        private Brush _marksBrush;

        private readonly Font _marksFont = new Font("Webdings", 9.5f, FontStyle.Bold);
        private readonly StringFormat _marksFormat = new StringFormat(StringFormatFlags.NoWrap)
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
        };

#endregion

#region Properties

        public int AreaSize { get; set; }

        public int MinValue { get; set; }

        public int MaxValue { get; set; }

        public int Position
        {
            get { return _position; }
            set
            {
                int position = value >= _position
                                   ? Math.Min(value, MaxValue)
                                   : Math.Max(value, MinValue);
                if (position != _position)
                {
                    _position = position;
                    _item.Repaint();
                }
            }
        }

        public byte DotSize { get; set; }

        public ItemControlType ControlType { get { return ItemControlType.ScrollBar; } }

        public ItemControlState ControlState { get; set; }

        public Rectangle ClientRectangle { get; set; }

        public object UserObject { get; set; }

        public OnItemPrePaint ItemPrePaint { get; set; }

        public OnItemPostPaint ItemPostPaint { get; set; }

        public int Width
        {
            get { return !Visible ? 0 : _width; }
            set { _width = value; }
        }

        public Rectangle SliderRectangle { get; private set; }

        public bool Destroyed { get; set; }

        public bool Visible
        {
            get { return MaxValue - MinValue > 0; }
        }

        #endregion

#region Class functions

        public ItemScrollBar(FlowchartItem item)
        {
            _width = 16;
            MinValue = 0;
            MaxValue = 100;
            _item = item;
        }

        public void SetPositionSilent(int position)
        {
            _position = position;
        }

        private void DrawBody(Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.HighSpeed;
            graphics.FillRectangle (_bodyBrush, ClientRectangle);
//            ItemPainterHelper.DrawRoundedRectangle(graphics, ClientRectangle,
//                3, null, _bodyBrush);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
        }

        private void DrawButtons(Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.HighSpeed;

            Rectangle btnTop = new Rectangle(ClientRectangle.Left + 1,
                ClientRectangle.Top + 1, ClientRectangle.Width - 2,
                ClientRectangle.Width - 2);
            graphics.FillRectangle(_frontBrush, btnTop);

            Rectangle btnBottom = new Rectangle(ClientRectangle.Left + 1,
                ClientRectangle.Top + ClientRectangle.Height - 
                ClientRectangle.Width + 1, ClientRectangle.Width - 2, 
                ClientRectangle.Width - 2);
            graphics.FillRectangle(_frontBrush, btnBottom);

            graphics.SmoothingMode = SmoothingMode.HighQuality;

            btnBottom.Offset(1, -1);
            graphics.DrawString("6", _marksFont, _marksBrush, btnBottom,
                    _marksFormat);
            btnTop.Offset(1, -1);
            graphics.DrawString("5", _marksFont, _marksBrush, btnTop,
                    _marksFormat);
        }

        private void DrawSlider(Graphics graphics)
        {
            float buttonSize = ClientRectangle.Width - 2;
            float freeSpace = ClientRectangle.Height - buttonSize * 2 - 3;
            float absoluteDistance = MaxValue - MinValue;

            if (Visible)
            {
                float absolutePosition = Position - MinValue;
                float sliderSize = Math.Max(
                    freeSpace * ((AreaSize / (absoluteDistance + AreaSize))),
                    buttonSize);

                float przPosition = absolutePosition / absoluteDistance;
                float sliderPosition = ((freeSpace - sliderSize) * przPosition) +
                                       buttonSize + 2;

                SliderRectangle = new Rectangle(ClientRectangle.Left + 1,
                    ClientRectangle.Top + (int)sliderPosition, 
                    ClientRectangle.Width - 2, (int)sliderSize);

                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.FillRectangle(_frontBrush, SliderRectangle);
                graphics.SmoothingMode = SmoothingMode.HighQuality;
            }
        }

        public void Draw(Graphics graphics, Color bodyColor, Color frontColor, 
                         Color marksColor)
        {
            if (bodyColor != _bodyColor)
            {
                if (_bodyBrush != null)
                {
                    _bodyBrush.Dispose();
                }
                _bodyBrush = new SolidBrush(_bodyColor = bodyColor);
            }
            if (frontColor != _frontColor)
            {
                if (_frontBrush != null)
                {
                    _frontBrush.Dispose();
                }
                _frontBrush = new SolidBrush(_frontColor = frontColor);
            }
            if (marksColor != _marksColor)
            {
                if (_marksBrush != null)
                {
                    _marksBrush.Dispose();
                }
                _marksBrush = new SolidBrush(_marksColor = marksColor);
            }
            Draw(graphics);
        }

        public void Draw(Graphics graphics)
        {
            if (Visible)
            {
                if (ItemPrePaint != null)
                {
                    ItemPrePaint(graphics, this);
                }

                DrawBody(graphics);
                DrawButtons(graphics);
                DrawSlider(graphics);

                if (ItemPostPaint != null)
                {
                    ItemPostPaint(graphics, this);
                }
            }
        }

        public void Dispose()
        {
            if (_bodyBrush != null)
            {
                _bodyBrush.Dispose();
            }
            if (_frontBrush != null)
            {
                _frontBrush.Dispose();
            }
            if (_marksBrush != null)
            {
                _marksBrush.Dispose();
            }
            _marksFont.Dispose();
            _marksFormat.Dispose();
        }

#endregion

    }
}
