﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;
using RK.Common.Classes.Units;
using RK.Common.Const;

namespace RK.Client.Controls
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

        private Thread _threadRenderer;
        private volatile bool _needRepaint;
        private volatile bool _tilesChanged;

        private Pen _mapWindowPen;
        private Brush _mapWallsBrush;
        private Brush _mapPlayersBrush;
        private Brush _mapMyPlayerBrush;

        private Point? _mousePos;
        private bool _dragScroll;

        private volatile bool _terminating;
        
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
                    _map.MapChanged -= MapChanged;
                    _map.TilesChanged -= MapTilesChanged;
                    _map.ScaleFactorChanged -= MapObjectChanged;
                    _map.PositionChanged -= MapObjectChanged;
                    _map.ObjectChanged -= MapObjectChanged;
                }
                _map = value;
                if (!DesignMode)
                {
                    if (_map != null)
                    {
                        _map.MapChanged += MapChanged;
                        _map.TilesChanged += MapTilesChanged;
                        _map.ScaleFactorChanged += MapObjectChanged;
                        _map.PositionChanged += MapObjectChanged;
                        _map.ObjectChanged += MapObjectChanged;
                    }
                    NeedRepaint(true);
                }
            }
        }

        private bool IsMapLoaded
        {
            get { return _map != null && _map.IsMapLoaded; }
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
                SetStyle(ControlStyles.Selectable, false);

                _mapWallsBrush = new SolidBrush(Color.Black);
                _mapMyPlayerBrush = new SolidBrush(Color.Orange);
                _mapPlayersBrush = new SolidBrush(Color.LimeGreen);
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

        private void NeedRepaint(bool tilesChanged)
        {
            _needRepaint = true;
            _tilesChanged |= tilesChanged;
        }

        private void MapTilesChanged(object sender)
        {
            NeedRepaint(true);
        }

        private void MapObjectChanged(object sender)
        {
            NeedRepaint(false);
        }

        private void MapChanged(object sender)
        {
            if (_bgBitmap != null)
            {
                _bgBitmap.Dispose();
                _bgBitmap = null;
            }
            if (!DesignMode && IsMapLoaded && _bgBitmap == null)
            {
                _bgBitmap = Image.FromFile("Resources\\bg_minimap.png");
            }
            NeedRepaint(true);
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
                _miniMapBitmapBuffer.InterpolationMode = InterpolationMode.Low;
                _miniMapBitmapBuffer.SmoothingMode = SmoothingMode.HighSpeed;

                _bufferBitmap = new Bitmap(Width, Height);
                
                _buffer = Graphics.FromImage(_bufferBitmap);
                _buffer.InterpolationMode = InterpolationMode.Low;
                _buffer.SmoothingMode = SmoothingMode.HighSpeed;
                
                _controlGraphics = Graphics.FromHwnd(Handle);
                _controlGraphics.InterpolationMode = InterpolationMode.Low;
                _controlGraphics.SmoothingMode = SmoothingMode.HighSpeed;

                if (_threadRenderer == null)
                {
                    _threadRenderer = new Thread(RendererProc);
                    _threadRenderer.IsBackground = true;
                    _threadRenderer.Start();
                }
            }
        }

        private void RendererProc()
        {
            while (!_terminating && !IsDisposed)
            {
                if (_needRepaint)
                {
                    _needRepaint = false;
                    Invoke(new Action(Repaint));
                }
                Thread.Sleep(10);
            }
        }

        private void PaintMiniMap()
        {
            if (IsMapLoaded && _bgBitmap != null)
            {
                _miniMapBitmapBuffer.DrawImage(_bgBitmap, ClientRectangle);

                ushort width = _map.Map.MiniMapSize.Width;
                ushort height = _map.Map.MiniMapSize.Height;
                for (ushort y = 0; y < height; y++)
                {
                    for (ushort x = 0; x < width; x++)
                    {
                        if (_map.Map.GetMiniMapPixel(x, y) == 1)
                        {
                            _miniMapBitmapBuffer.FillRectangle(_mapWallsBrush, x, y, 1, 1);
                        }
                    }
                }
            }
        }

        private void GeneralPaint()
        {
            if (IsMapLoaded && _map.PlayerData != null)
            {
                float w = _map.Map.Width*ConstMap.PIXEL_SIZE*_map.ScaleFactor;
                float h = _map.Map.Height*ConstMap.PIXEL_SIZE*_map.ScaleFactor;

                int w1 = (int) (Width*(_map.Width/w));
                int h1 = (int) (Height*(_map.Height/h));

                int x1 = (int) (_map.PosX/w*Width);
                int y1 = (int) (_map.PosY/h*Height);

                _buffer.DrawImage(_miniMapBitmap, 0, 0);
                _buffer.DrawRectangle(_mapWindowPen, x1, y1, w1, h1);

                int myPlayerId = _map.PlayerData.Player.Id;
                if (_map.Width > 0 && _map.Height > 0 && _map.Players != null)
                {
                    for (int i = 0; i < _map.Players.Count; i++)
                    {
                        Player player = _map.Players[i];
                        int px = (int) (Width*((float) player.Position.X/(_map.Map.Width*ConstMap.PIXEL_SIZE)));
                        int py = (int) (Height*((float) player.Position.Y/(_map.Map.Height*ConstMap.PIXEL_SIZE)));
                        Brush brush = player.Id == myPlayerId ? _mapMyPlayerBrush : _mapPlayersBrush;
                        _buffer.FillRectangle(brush, px, py, 1, 1);
                    }
                }
            }
        }

        private void Repaint()
        {
            if (_bufferBitmap == null || _bufferBitmap.Width != Width ||
                _bufferBitmap.Height != Height)
            {
                DestroyGraphics();
                InitializeGraphics();
            }

            if (_bufferBitmap != null)
            {
                if (_tilesChanged)
                {
                    PaintMiniMap();
                    _tilesChanged = false;
                }
                GeneralPaint();
                _controlGraphics.DrawImage(_bufferBitmap, ClientRectangle,
                    ClientRectangle, GraphicsUnit.Pixel);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!DesignMode)
            {
                Repaint();
            }
            else
            {
                base.OnPaint(e);
            }
        }

        private void SetMapPosition(Point location, bool smothScroll)
        {
            float w = _map.Map.Width*ConstMap.PIXEL_SIZE*_map.ScaleFactor;
            float h = _map.Map.Height*ConstMap.PIXEL_SIZE*_map.ScaleFactor;
            float x = (float) location.X/Width*w;
            float y = (float) location.Y/Height*h;
            _map.CenterTo((int) x, (int) y, smothScroll);
        }

        private void MiniMapControlMouseDown(object sender, MouseEventArgs e)
        {
            if (IsMapLoaded)
            {
                _mousePos = e.Location;
                SetMapPosition(e.Location, true);
            }
        }

        private void MiniMapControlMouseMove(object sender, MouseEventArgs e)
        {
            if (_mousePos != null && (_dragScroll || 
                Math.Abs(e.X - _mousePos.Value.X) > 3 ||
                Math.Abs(e.Y - _mousePos.Value.Y) > 3))
            {
                _dragScroll = true;
                SetMapPosition(e.Location, false);
            }
        }

        private void MiniMapControlMouseUp(object sender, MouseEventArgs e)
        {
            _mousePos = null;
            _dragScroll = false;
        }

        public new void Dispose()
        {
            if (!DesignMode && !IsDisposed)
            {
                _terminating = true;
                _threadRenderer.Abort();
                _threadRenderer.Join();
                DestroyGraphics();
            }
            base.Dispose();
        }

#endregion

    }
}
