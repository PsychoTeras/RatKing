using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GMechanics.FlowchartControl.ItemControls
{
    internal class ItemButton : IItemControl
    {

#region Private members

        private int _textMargin;
        private Color _captionColor;
        private Rectangle _clientRectangle;
        private Rectangle _textRectangle;
        private LinearGradientBrush _fillBrush;
        private readonly Pen _framePen = new Pen(Color.FromArgb(70, 0, 0, 0));

        private Font _captionFont;
        private Brush _captionBrush;
        private readonly StringFormat _captionFormat = new StringFormat(StringFormatFlags.NoWrap)
        {
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };

#endregion

#region Properties

        public ItemControlType ControlType { get { return ItemControlType.Button; } }

        public ItemControlState State { get; set; }

        public bool Selected { get; set; }

        public Rectangle ClientRectangle
        {
            get { return _clientRectangle; }
            set
            {
                _clientRectangle = value;
                if (_fillBrush != null)
                {
                    _fillBrush.Dispose();
                }
                _fillBrush = new LinearGradientBrush(_clientRectangle, Color.Transparent,
                                                     Color.Transparent, 90, false);
                TextMargin = TextMargin;
            }
        }

        public object UserObject { get; private set; }

        public OnItemPrePaint ItemPrePaint { get; set; }

        public OnItemPostPaint ItemPostPaint { get; set; }

        public bool Destroyed { get; set; }

        public Cursor Cursor { get; set; }

        public string Hint { get; set; }

        public Color Color { get; set; }

        public Color CaptionColor
        {
            get { return _captionColor; }
            set
            {
                if (_captionColor != value)
                {
                    _captionColor = value;
                    if (_captionBrush != null)
                    {
                        _captionBrush.Dispose();
                    }
                    _captionBrush = new SolidBrush(_captionColor);
                }
            }
        }

        public string Caption { get; set; }

        public int TextMargin
        {
            get { return _textMargin; }
            set
            {
                _textMargin = value;
                _textRectangle = new Rectangle(_clientRectangle.Left + TextMargin,
                    _clientRectangle.Top, _clientRectangle.Width - (int)(TextMargin * 1.7f),
                    _clientRectangle.Height);
            }
        }

        public Font Font
        {
            get { return _captionFont; }
            set
            {
                if (_captionFont != null)
                {
                    _captionFont.Dispose();
                }
                _captionFont = (Font) value.Clone();
            }
        }

        public Image Icon { get; set; }

        public bool Visible { get; set; }

#endregion

#region Class functions

        public ItemButton(Rectangle rectangle, object userObject)
        {
            Color = Color.White;
            CaptionColor = Color.FromArgb(220, 0, 0, 0);
            _captionFont = new Font("Verdana", 8.25f, FontStyle.Regular);
            ClientRectangle = rectangle;
            UserObject = userObject;
        }

        private void DrawMouseOn(Graphics graphics)
        {
            //Draw fill
            ColorBlend colorBlend = new ColorBlend(3);
            colorBlend.Colors = new[] { Color.FromArgb(Color.A / 2, Color), 
                                        Color.FromArgb(Color.A, Color),
                                        Color.Transparent };
            colorBlend.Positions = new[] { 0f, .5f, 1f };
            _fillBrush.InterpolationColors = colorBlend;
            graphics.FillRectangle(_fillBrush, _clientRectangle);

            //Draw frame
            graphics.DrawRectangle(_framePen, ClientRectangle);
        }

        private void DrawMouseClick(Graphics graphics)
        {
            //Draw fill
            ColorBlend colorBlend = new ColorBlend(3);
            colorBlend.Colors = new[] { Color.FromArgb((int) (Color.A / 2.5f), Color), 
                                        Color.FromArgb((int) (Color.A / 1.3f), Color),
                                        Color.Transparent };
            colorBlend.Positions = new[] { 0f, .5f, 1f };
            _fillBrush.InterpolationColors = colorBlend;
            graphics.FillRectangle(_fillBrush, _clientRectangle);

            //Draw frame
            graphics.DrawRectangle(_framePen, ClientRectangle);
        }

        private void DrawCaption(Graphics graphics)
        {
            if (!string.IsNullOrEmpty(Caption))
            {
                graphics.DrawString(Caption, Font, _captionBrush, _textRectangle, 
                    _captionFormat);
            }
        }

        private void DrawIcon(Graphics graphics)
        {
            if (Icon != null)
            {
                int margin = (ClientRectangle.Height / 2 - Icon.Height / 2);
                graphics.DrawImage(Icon, ClientRectangle.Left + margin,
                                   ClientRectangle.Top + margin);
            }
        }

        public void Draw(Graphics graphics)
        {
            if (ItemPrePaint != null)
            {
                ItemPrePaint(graphics, this);
            }

            DrawIcon(graphics);

            switch (State)
            {
                case ItemControlState.MouseOn:
                        DrawMouseOn(graphics);
                        break;
                case ItemControlState.ClickedLeft:
                case ItemControlState.ClickedRight:
                        DrawMouseClick(graphics);
                        break;
                default:
                        if (Selected)
                        {
                            DrawMouseOn(graphics);
                        }
                        break;
            }

            DrawCaption(graphics);

            if (ItemPostPaint != null)
            {
                ItemPostPaint(graphics, this);
            }
        }

        public void Dispose()
        {
            _captionFont.Dispose();
            _framePen.Dispose();
            _captionBrush.Dispose();
            _captionFormat.Dispose();
        }

#endregion

    }
}
