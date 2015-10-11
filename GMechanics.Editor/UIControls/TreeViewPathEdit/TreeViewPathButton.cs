using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace GMechanics.Editor.UIControls.TreeViewPathEdit
{
    sealed partial class TreeViewPathButton : UserControl
    {

#region Delegates

        public delegate void ArrowButtonClickHandler(object sender, EventArgs e);

#endregion

        private readonly Color _bottomArrowGradientColor = 
            Color.FromKnownColor(KnownColor.Highlight);
        private bool _arrowVisible = true;
        private Color _bottomGradientColor = Color.Silver;

        private bool _clicked;
        private bool _holdUpdate;
        private Image _icon;
        private bool _mouseOnArrow;
        private Color _topGradientColor = Color.Transparent;

        public TreeViewPathButton()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;

            InitializeComponent();

            lblText.Text = Name;
            SetActualArrowImage();
        }

        [Browsable(false)]
        public override Color ForeColor { get; set; }

        [Browsable(false)]
        public override Color BackColor { get; set; }

        [Browsable(false)]
        public override Image BackgroundImage { get; set; }

        [Browsable(false)]
        public override ImageLayout BackgroundImageLayout { get; set; }

        [DefaultValue(null), Browsable(true)]
        public Color TopGradientColor
        {
            get { return _topGradientColor; }
            set
            {
                _topGradientColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        public Color BottomGradientColor
        {
            get { return _bottomGradientColor; }
            set
            {
                _bottomGradientColor = value;
                Invalidate();
            }
        }

        [DefaultValue(null), Browsable(true)]
        public Image Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                pbIcon.Invalidate();
            }
        }

        [DefaultValue(null), Browsable(true)]
        public bool IconVisible
        {
            get { return pbIcon.Visible; }
            set { pbIcon.Visible = value; }
        }

        [DefaultValue(null), Browsable(true)]
        public bool ArrowButtonVisible
        {
            get { return pbArrow.Visible; }
            set { pbArrow.Visible = value; }
        }

        [DefaultValue(""), Browsable(true),
         Editor(@"System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, 
                Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof (UITypeEditor))]
        public string ButtonText
        {
            get { return lblText.Text; }
            set
            {
                lblText.Text = value;
                Width = (int) (pbIcon.Left + pbIcon.Width + CreateGraphics().
                                                                MeasureString(lblText.Text, lblText.Font).Width +
                               pbArrow.Width + 8);
                PbIconVisibleChanged(this, null);
            }
        }

        [DefaultValue(null), Browsable(true)]
        public Color TextColor
        {
            get { return lblText.ForeColor; }
            set { lblText.ForeColor = value; }
        }

        [Category("Action")]
        public event ArrowButtonClickHandler ArrowButtonClick;

        private static void DrawButtonFrame(Graphics g, Rectangle rect)
        {
            rect.Width--;
            rect.Height--;
            Pen pen = new Pen(Color.FromKnownColor(KnownColor.ControlDark));
            g.DrawRectangle(pen, rect);
            pen = new Pen(Color.White);
            rect.Inflate(-1, -1);
            g.DrawRectangle(pen, rect);
        }

        private void ImageButtonPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(ClientRectangle.Location, ClientRectangle.Size);
            if (_arrowVisible)
            {
                rect.Width -= pbArrow.Width;
            }

            Brush brush = new LinearGradientBrush(ClientRectangle, _clicked ? _bottomGradientColor : _topGradientColor,
                                                  _clicked ? _topGradientColor : _bottomGradientColor, 90, false);
            g.FillRectangle(brush, rect);
            DrawButtonFrame(g, rect);

            if (_arrowVisible)
            {
                if (_mouseOnArrow)
                {
                    if (_clicked)
                    {
                        brush = new SolidBrush(_bottomArrowGradientColor);
                    }
                    else
                    {
                        brush = new LinearGradientBrush(ClientRectangle, _topGradientColor, _bottomArrowGradientColor,
                                                        90, false);
                    }
                }
                rect = new Rectangle(new Point(pbArrow.Left - 1, pbArrow.Top), pbArrow.ClientRectangle.Size);
                g.FillRectangle(brush, rect);
                DrawButtonFrame(g, rect);
            }
        }

        private void PForegroundMouseMove(object sender, MouseEventArgs e)
        {
            Point mp = MousePosition;
            Point clientMp = PointToClient(mp);
            Rectangle rect = new Rectangle(new Point(pbArrow.Left - 1, pbArrow.Top), pbArrow.ClientRectangle.Size);
            bool mouseOnArrow = rect.Contains(clientMp);
            if (mouseOnArrow != _mouseOnArrow)
            {
                _mouseOnArrow = mouseOnArrow;
                Invalidate();
            }
        }

        private void ImageButtonMouseEnter(object sender, EventArgs e)
        {
            if (!_holdUpdate)
            {
                Paint += ImageButtonPaint;
                Invalidate();
            }
        }

        private void ImageButtonMouseLeave(object sender, EventArgs e)
        {
            if (!_holdUpdate)
            {
                _mouseOnArrow = _clicked = false;
                Paint -= ImageButtonPaint;
                Invalidate();
            }
        }

        private void PForegroundMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !_holdUpdate)
            {
                PForegroundMouseMove(sender, e);
                _clicked = true;
                SetActualArrowImage();
                Invalidate();

                if (_mouseOnArrow && ArrowButtonClick != null)
                {
                    ArrowButtonClick(this, new EventArgs());
                }
            }
        }

        private void PForegroundMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !_holdUpdate)
            {
                _clicked = false;
                SetActualArrowImage();
                Invalidate();

                if (!_mouseOnArrow)
                {
                    Point point = PointToClient(Cursor.Position);
                    if (ClientRectangle.Contains(point))
                    {
                        OnClick(e);
                    }
                }
            }
        }

        private void PbIconPaint(object sender, PaintEventArgs e)
        {
            if (_icon != null)
            {
                Graphics g = e.Graphics;
                ColorMatrix matrix = new ColorMatrix {Matrix33 = Enabled ? 1f : 0.5f};
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                g.DrawImage(_icon, pbIcon.ClientRectangle, 0, 0, _icon.Width, _icon.Height,
                            GraphicsUnit.Pixel, attributes);
            }
        }

        private void PbIconVisibleChanged(object sender, EventArgs e)
        {
            const int sp = 2;
            int shift = pbIcon.Left + pbIcon.Width + 1;
            if (pbIcon.Visible && lblText.Left == pbIcon.Left - sp)
            {
                lblText.Left += shift + sp;
                lblText.Width -= shift;
                Width += shift;
            }
            else if (!pbIcon.Visible && lblText.Left == shift)
            {
                lblText.Left = pbIcon.Left - sp;
                lblText.Width += shift;
                Width -= shift;
            }
        }

        private void PbArrowVisibleChanged(object sender, EventArgs e)
        {
            if (pbArrow.Visible && !_arrowVisible)
            {
                Width += pbArrow.Width;
                lblText.Width -= pbArrow.Width;
                _arrowVisible = true;
            }
            else if (!pbArrow.Visible && _arrowVisible)
            {
                Width -= pbArrow.Width;
                lblText.Width += pbArrow.Width;
                _arrowVisible = false;
            }
        }

        private void SetActualArrowImage()
        {
            pbArrow.Image = ilArrows.Images[_clicked && _mouseOnArrow ? 1 : 0];
        }

        public void HoldUpdate()
        {
            _holdUpdate = true;
        }

        public void UnholdUpdate()
        {
            try
            {
                _holdUpdate = false;
                PForegroundMouseMove(this, null);
                PForegroundMouseUp(this, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                Point point = PointToClient(Cursor.Position);
                if (!ClientRectangle.Contains(point))
                {
                    ImageButtonMouseLeave(this, null);
                }
                Invalidate();
            } catch {}
        }
    }

    public class ButtonClickEventArgs : EventArgs
    {
        public TreeNode TreeNode;
    }
}