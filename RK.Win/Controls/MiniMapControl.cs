using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using RK.Common.Classes.Map;
using RK.Win.Classes.Map.Renderers;

namespace RK.Win.Controls
{
    public unsafe partial class MiniMapControl : Control
    {

#region Private fields

        private MapControl _map;

        private Graphics _buffer;
        private Bitmap _bufferBitmap;
        private Graphics _controlGraphics;

        private Image _bgBitmap;
        private Image _miniMapBitmap;
        private Graphics _miniMapBitmapBuffer;
        
        private Pen _mapWindowPen;
        private Brush _mapWallsBrush;

        private bool _inScroll;

#endregion

#region Properties

        #region Hidden

        [DefaultValue(null), Browsable(false)]
        public new string Text { get; set; }

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

        private new bool DesignMode
        {
            get { return base.DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime; }
        }

        [Browsable(true)]
        public MapControl MapControl
        {
            get { return _map; }
            set
            {
                if (_map != null)
                {
                    _map.MapLoaded -= MapLoaded;
                    _map.ScaleFactorChanged -= MapScaleFactorChanged;
                    _map.PositionChanged -= MapPositionChanged;
                    _map.TilesChanged -= MapTilesChanged;
                }
                if ((_map = value) != null)
                {
                    _map.MapLoaded += MapLoaded;
                    _map.ScaleFactorChanged += MapScaleFactorChanged;
                    _map.PositionChanged += MapTilesChanged;
                    _map.TilesChanged += MapTilesChanged;
                }
                Repaint();
            }
        }

#endregion

#region Ctor

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle &= 0x02000000;
                return cp;
            }
        }

        public MiniMapControl()
        {
            if (!DesignMode)
            {
                SetStyle(ControlStyles.UserPaint, true);
                SetStyle(ControlStyles.ResizeRedraw, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.Selectable, false);

                _mapWallsBrush = new SolidBrush(Color.Black);
                _mapWindowPen = new Pen(Color.LightSteelBlue);

                MouseDown += MiniMapControlMouseDown;
                MouseMove += MiniMapControlMouseMove;
                MouseUp += MiniMapControlMouseUp;
            }

            InitializeComponent();
        }

        public MiniMapControl(Control parent)
            : this()
        {
            Parent = parent;
        }

#endregion

#region Class methods

        private void MapTilesChanged(object sender)
        {
            Repaint();
        }

        private void MapPositionChanged(object sender)
        {
            Repaint(ClientRectangle, true);
        }

        private void MapScaleFactorChanged(object sender)
        {
            Repaint(ClientRectangle, true);
        }

        private void MapLoaded(object sender)
        {
            if (_bgBitmap != null)
            {
                _bgBitmap.Dispose();
                _bgBitmap = null;
            }
            if (!DesignMode && _map != null && _map.IsMapLoaded)
            {
                _bgBitmap = Image.FromFile("Resources\\bg_minimap.png");
            }
            Repaint();
        }

        private void DestroyGraphics()
        {
            if (_buffer != null)
            {
                _miniMapBitmap.Dispose();
                _miniMapBitmapBuffer.Dispose();
                _buffer.Dispose();
                _bufferBitmap.Dispose();
                _controlGraphics.Dispose();
                _buffer = null;
            }
        }

        private void InitializeGraphics()
        {
            if (Width != 0 && Height != 0)
            {
                _miniMapBitmap = new Bitmap(Width, Height);
                _miniMapBitmapBuffer = Graphics.FromImage(_miniMapBitmap);
                _bufferBitmap = new Bitmap(Width, Height);
                _buffer = Graphics.FromImage(_bufferBitmap);
                _buffer.InterpolationMode = InterpolationMode.Low;
                _buffer.SmoothingMode = SmoothingMode.HighSpeed;
                _controlGraphics = Graphics.FromHwnd(Handle);
            }
        }

        public void Repaint()
        {
            Repaint(ClientRectangle, false);
        }

        private void PaintMiniMap()
        {
            if (_map != null && _map.IsMapLoaded && _bgBitmap != null)
            {
                _miniMapBitmapBuffer.DrawImage(_bgBitmap, ClientRectangle);

                int w = _map.Map.Width - 1, h = _map.Map.Height - 1;
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        ushort tX = (ushort) ((float) (x + 1)/Width*w);
                        ushort tY = (ushort) ((float) (y + 1)/Height*h);
                        if ((*_map.Map[tX, tY]).Type == TileType.Wall)
                        {
                            _miniMapBitmapBuffer.FillRectangle(_mapWallsBrush, x, y, 1, 1);
                        }
                    }
                }
            }
        }

        private void PaintMapObjects()
        {
            if (_map != null && _map.IsMapLoaded)
            {
                float w = _map.Map.Width*RendererWalls.PIXEL_SIZE*_map.ScaleFactor;
                float h = _map.Map.Height*RendererWalls.PIXEL_SIZE*_map.ScaleFactor;

                int w1 = (int) (Width*(_map.Width/w));
                int h1 = (int) (Height*(_map.Height/h));

                int x1 = (int) (_map.PosX / w * Width);
                int y1 = (int) (_map.PosY / h * Height);

                _buffer.DrawImage(_miniMapBitmap, 0, 0);
                _buffer.DrawRectangle(_mapWindowPen, x1, y1, w1, h1);
            }
        }

        private void Repaint(Rectangle clipRectangle, bool paintMapObjectsOnly)
        {
            if (!DesignMode)
            {
                if (_bufferBitmap == null || _bufferBitmap.Width != Width ||
                    _bufferBitmap.Height != Height)
                {
                    DestroyGraphics();
                    InitializeGraphics();
                }

                if (_bufferBitmap != null)
                {
                    if (!paintMapObjectsOnly)
                    {
                        PaintMiniMap();
                    }
                    PaintMapObjects();
                    Rectangle area = new Rectangle(0, 0, Width, Height);
                    _controlGraphics.DrawImage(_bufferBitmap, clipRectangle, area, GraphicsUnit.Pixel);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!DesignMode)
            {
                Repaint(e.ClipRectangle, false);
            }
            else
            {
                base.OnPaint(e);
            }
        }

        private void SetMapPosition(Point location)
        {
            float w = _map.Map.Width * RendererWalls.PIXEL_SIZE * _map.ScaleFactor;
            float h = _map.Map.Height * RendererWalls.PIXEL_SIZE * _map.ScaleFactor;
            float x = (float)location.X / Width * w - ((float)_map.Width / 2);
            float y = (float)location.Y / Height * h - ((float)_map.Height / 2);
            _map.SetPosition((int)x, (int)y);
        }

        private void MiniMapControlMouseDown(object sender, MouseEventArgs e)
        {
            if (_map != null && _map.IsMapLoaded)
            {
                _inScroll = true;
                SetMapPosition(e.Location);
            }
        }

        private void MiniMapControlMouseMove(object sender, MouseEventArgs e)
        {
            if (_inScroll)
            {
                SetMapPosition(e.Location);
            }
        }

        private void MiniMapControlMouseUp(object sender, MouseEventArgs e)
        {
            _inScroll = false;
        }

#endregion

    }
}
