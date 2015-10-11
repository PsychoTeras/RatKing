using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using RK.Common.Classes.Common;
using RK.Common.Classes.Map;
using RK.Win.Classes.Map.Renderers;

namespace RK.Win.Controls
{
    public delegate void MapEvent(object sender);
    public delegate void MouseWheel(object sender, int delta);

    public unsafe partial class MapControl : Control, IMessageFilter
    {

#region P/Invoke

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point pt);

#endregion

#region Constants

        private const float DEF_SCALE_FACTOR = 1f;
        private const bool DEF_SHOW_TILE_NUMBER = true;

#endregion

#region Private fields

        private GameMap _map;

        private Graphics _buffer;
        private Bitmap _bufferBitmap;
        private Graphics _controlGraphics;
        private bool _repaintLocked;

        private List<IMapRenderer> _renderers;

        private int _posX;
        private int _posY;
        private float _scaleFactor;

        private bool _showTileNumber;

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

        [Browsable(false)]
        public int PosX
        {
            get { return _posX; }
            set
            {
                if (_posX != value)
                {
                    _posX = value;
                    Repaint();
                    if (PositionChanged != null)
                    {
                        PositionChanged(this);
                    }
                }
            }
        }

        [Browsable(false)]
        public int PosY
        {
            get { return _posY; }
            set
            {
                if (_posY != value)
                {
                    _posY = value;
                    Repaint();
                    if (PositionChanged != null)
                    {
                        PositionChanged(this);
                    }
                }
            }
        }

        [Browsable(true), DefaultValue(DEF_SCALE_FACTOR)]
        public float ScaleFactor
        {
            get { return _scaleFactor; }
            set
            {
                value = Math.Min(Math.Max(value, 0.1f), 1f);
                if (_scaleFactor != value)
                {
                    float scaleDiff = value/_scaleFactor;
                    _posX = (int) (_posX*scaleDiff);
                    _posY = (int) (_posY*scaleDiff);

                    _scaleFactor = value;
                    OnScaleFactorChanged();
                }
            }
        }

        [Browsable(true), DefaultValue(DEF_SHOW_TILE_NUMBER)]
        public bool ShowTileNumber
        {
            get { return _showTileNumber; }
            set
            {
                _showTileNumber = value;
                Repaint();
            }
        }

        [Browsable(false)]
        public GameMap Map
        {
            get { return _map; }
            set
            {
                _map = value;
                MapChanged();
                Repaint();
            }
        }

        [Browsable(false)]
        public bool IsMapLoaded
        {
            get { return _map != null; }
        }

        [Browsable(true)]
        public new event MouseWheel MouseWheel;

        [Browsable(true)]
        public event MapEvent ScaleFactorChanged;

        [Browsable(true)]
        public event MapEvent MapLoaded;

        [Browsable(true)]
        public event MapEvent PositionChanged;

        [Browsable(true)]
        public event MapEvent TilesChanged;

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

        public MapControl()
        {
            _scaleFactor = DEF_SCALE_FACTOR;
            _showTileNumber = DEF_SHOW_TILE_NUMBER;

            if (!DesignMode)
            {
                SetStyle(ControlStyles.UserPaint, true);
                SetStyle(ControlStyles.ResizeRedraw, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.Selectable, false);
                Application.AddMessageFilter(this);
                InitializeRenderers();
            }

            InitializeComponent();
        }

        public MapControl(Control parent)
            : this()
        {
            Parent = parent;
        }

#endregion

#region Class methods

        public void LoadMap(GameMap map)
        {
            Map = map;
            if (MapLoaded != null)
            {
                MapLoaded(this);
            }
        }

        public void BeginUpdate()
        {
            _repaintLocked = true;
        }

        public void EndUpdate()
        {
            _repaintLocked = false;
            Repaint();
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
            if (Width != 0 && Height != 0)
            {
                _bufferBitmap = new Bitmap(Width, Height);
                _buffer = Graphics.FromImage(_bufferBitmap);
                _buffer.InterpolationMode = InterpolationMode.Low;
                _buffer.SmoothingMode = SmoothingMode.HighSpeed;
                _controlGraphics = Graphics.FromHwnd(Handle);
            }
        }

        public void Repaint()
        {
            Repaint(ClientRectangle);
        }

        private void Repaint(Rectangle clipRectangle)
        {
            if (!_repaintLocked && !DesignMode)
            {
                if (_bufferBitmap == null || _bufferBitmap.Width != Width ||
                    _bufferBitmap.Height != Height)
                {
                    DestroyGraphics();
                    InitializeGraphics();
                }

                if (_bufferBitmap != null)
                {
                    GeneralPaint();
                    Rectangle area = new Rectangle(0, 0, _bufferBitmap.Width,
                        _bufferBitmap.Height);
                    _controlGraphics.DrawImage(_bufferBitmap, clipRectangle, area, 
                                               GraphicsUnit.Pixel);
                }
            }
        }

        private void GeneralPaint()
        {
            foreach (IMapRenderer renderer in _renderers)
            {
                Rectangle area = new Rectangle(_posX, _posY, _bufferBitmap.Width,
                    _bufferBitmap.Height);
                renderer.Render(this, _buffer, area);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!DesignMode)
            {
                if (!_repaintLocked)
                {
                    Repaint(e.ClipRectangle);
                }
            }
            else
            {
                base.OnPaint(e);
            }
        }

        private void InitializeRenderers()
        {
            _renderers = new List<IMapRenderer>
            {
                new RendererBG(_scaleFactor),
                new RendererWalls(),
                new RendererBorders()
            };
        }

        private void OnScaleFactorChanged()
        {
            if (!DesignMode)
            {
                foreach (IMapRenderer renderer in _renderers)
                {
                    renderer.ChangeScaleFactor(this, _scaleFactor);
                }
                Repaint();
                if (ScaleFactorChanged != null)
                {
                    ScaleFactorChanged(this);
                }
            }
        }

        private void MapChanged()
        {
            if (!DesignMode)
            {
                foreach (IMapRenderer renderer in _renderers)
                {
                    renderer.ChangeMap(this);
                }
                Repaint();
            }
        }

        public void SetPosition(int x, int y)
        {
            _posX = x;
            _posY = y;
            Repaint();
            if (PositionChanged != null)
            {
                PositionChanged(this);
            }
        }

        public void ShiftPosition(int shiftX, int shiftY)
        {
            _posX += shiftX;
            _posY += shiftY;
            Repaint();
            if (PositionChanged != null)
            {
                PositionChanged(this);
            }
        }

        protected virtual void OnMouseWheel(int delta)
        {
            MouseWheel handler = MouseWheel;
            if (handler != null)
            {
                handler(this, delta);
            }
        }
        
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x20a)
            {
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                if (WindowFromPoint(pos) == Handle)
                {
                    OnMouseWheel(m.WParam.ToInt32() > 0 ? 1 : -1);
                    return true;
                }
            }
            return false;
        }

        public ShortPoint? CursorToTilePos(Point location)
        {
            if (_map != null)
            {
                float pixelSize = RendererWalls.PIXEL_SIZE * _scaleFactor;
                int x = (int)Math.Floor((_posX + location.X) / pixelSize);
                int y = (int)Math.Floor((_posY + location.Y) / pixelSize);
                if (x >= 0 && y >= 0 && x < _map.Width && y < _map.Height)
                {
                    return new ShortPoint(x, y);
                }
            }
            return null;
        }

        private void SetTileType(int x, int y, TileType type, bool set)
        {
            if (!set)
            {
                return;
            }

            if (x >= 0 && y >= 0 && x < _map.Width && y < _map.Height)
            {
                Tile* tile = _map[(ushort)x, (ushort)y];
                (*tile).Type = type;
                foreach (IMapRenderer renderer in _renderers)
                {
                    renderer.UpdateTile(this, (ushort) x, (ushort) y, *tile);
                }
            }
        }

        public void ExplodeWallsUnderCursor(Point location, int radius)
        {
            ShortPoint? tilePos = CursorToTilePos(location);
            if (tilePos != null)
            {
                int x = tilePos.Value.X;
                int y = tilePos.Value.Y;
                float sinus = 0.70710678118F;
                int range = (int) (radius/(2*sinus));
                Random rnd = new Random(Environment.TickCount);
                for (int i = radius; i >= range; --i)
                {
                    int j = (int) Math.Sqrt(radius*radius - i*i);
                    for (int k = -j; k <= j; k++)
                    {
                        bool set = i != radius - 1 || rnd.Next(0, 2) == 1;
                        SetTileType(x - k, y + i, TileType.Nothing, set);
                        SetTileType(x - k, y - i, TileType.Nothing, set);
                        SetTileType(x + i, y + k, TileType.Nothing, set);
                        SetTileType(x - i, y - k, TileType.Nothing, set);
                    }
                }
                range = (int) (radius*sinus);
                for (int i = x - range + 1; i < x + range; i++)
                {
                    for (int j = y - range + 1; j < y + range; j++)
                    {
                        SetTileType(i, j, TileType.Nothing, true);
                    }
                }
                Repaint();
                if (TilesChanged != null)
                {
                    TilesChanged(this);
                }
            }
        }

        public void ToggleWallUnderCursor(Point location)
        {
            ShortPoint? tilePos = CursorToTilePos(location);
            if (tilePos != null)
            {
                Tile* tile = _map[tilePos.Value.X, tilePos.Value.Y];
                (*tile).Type = (*tile).Type == TileType.Wall ? TileType.Nothing : TileType.Wall;
                foreach (IMapRenderer renderer in _renderers)
                {
                    renderer.UpdateTile(this, tilePos.Value.X, tilePos.Value.Y, *tile);
                }
                Repaint();
                if (TilesChanged != null)
                {
                    TilesChanged(this);
                }
            }
        }

        public new void Dispose()
        {
            if (!DesignMode)
            {
                Application.RemoveMessageFilter(this);
                DestroyGraphics();
            }
            base.Dispose();
        }

#endregion

    }
}
