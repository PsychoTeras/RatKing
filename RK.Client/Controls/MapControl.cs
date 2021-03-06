﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using IPCLogger.Core.Loggers.LFactory;
using RK.Client.Classes;
using RK.Client.Classes.Map.Renderers;
using RK.Common.Algo;
using RK.Common.Classes.Common;
using RK.Common.Classes.Units;
using RK.Common.Const;
using RK.Common.Map;
using RK.Common.Net.Client;
using RK.Common.Proto;
using RK.Common.Proto.Packets;
using RK.Common.Proto.Responses;

namespace RK.Client.Controls
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

        private const float BORDER_AREA_SPACE_PART = 3f;

#endregion

#region Private fields

        private TCPClient _tcpClient;
        private volatile bool _connecting;
        private volatile bool _reconnecting;
        
        private ClientMap _map;

        private Graphics _buffer;
        private Bitmap _bufferBitmap;
        private Graphics _controlGraphics;

        private Image _playerBitmap;
        private Bitmap _playerRotatedBitmap;
        private Graphics _playerRotated;

        private List<IMapRenderer> _renderers;

        private int _posX;
        private int _posY;
        private float _scaleFactor;

        private Point? _scrollToPos;
        private object _syncScroll;

        private int _sessionToken;
        private Dictionary<int, PlayerDataEx> _playersData;
        private ReadOnlyCollection<PlayerDataEx> _playersRo;

        private PlayerDataEx _myPlayerData;

        private Thread _threadWorld;
        private Thread _threadRenderer;
        private Thread _threadObjectChanged;

        private DateTime _lastFrameRenderTime;
        private volatile bool _somethingChanged;

        private byte[] _fpsCounterData;
        private int _fpsCounterArrIdx;
        private Brush _fpsFontBrush;

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

        [Browsable(false)]
        public int PosX
        {
            get { return _posX; }
            set
            {
                if (_posX != value)
                {
                    lock (_syncScroll)
                    {
                        _scrollToPos = null;
                        _posX = value;
                        if (PositionChanged != null)
                        {
                            PositionChanged(this);
                        }
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
                    lock (_syncScroll)
                    {
                        _scrollToPos = null;
                        _posY = value;
                        if (PositionChanged != null)
                        {
                            PositionChanged(this);
                        }
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
        public bool ShowTileNumber { get; set; }

        [Browsable(false)]
        public ReadOnlyCollection<PlayerDataEx> Players
        {
            get { return _sessionToken != 0 ? _playersRo : null; }
        }

        [Browsable(false)]
        public ClientMap Map
        {
            get { return _map; }
        }

        [Browsable(false)]
        public bool IsMapLoaded { get; private set; }

        [Browsable(false)]
        public PlayerDataEx PlayerData
        {
            get { return _myPlayerData; }
        }

#endregion

#region Events

        [Browsable(true)]
        public new event MouseWheel MouseWheel;

        [Browsable(true)]
        public event MapEvent ScaleFactorChanged;

        [Browsable(true)]
        public event MapEvent MapChanged;

        [Browsable(true)]
        public event MapEvent PositionChanged;

        [Browsable(true)]
        public event MapEvent TilesChanged;

        [Browsable(true)]
        public event MapEvent ObjectChanged;

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
            ShowTileNumber = DEF_SHOW_TILE_NUMBER;

            if (!DesignMode)
            {
                LFactory.Instance.Initialize("RK.Client.config");

                SetStyle(ControlStyles.Opaque, true);
                SetStyle(ControlStyles.Selectable, false);

                MouseMove += MapControlMouseMove;
                MouseWheel += MapControlMouseWheel;

                Application.AddMessageFilter(this);

                InitializeEnvironment();
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

        private void WriteLog(string msg)
        {
            LFactory.Instance.WriteLine(msg + Environment.NewLine);
        }

        public void DisconnectFromHost()
        {
            if (_tcpClient.IsConnected)
            {
                WriteLog("DisconnectFromHost");
                TCPClientDataSend(new PUserLogout
                {
                    SessionToken = _sessionToken
                });
                _sessionToken = 0;
                IsMapLoaded = false;
                _myPlayerData = null;
                if (_tcpClient != null)
                {
                    _tcpClient.Disconnect();
                }
            }
            else
            {
                WriteLog("!DisconnectFromHost");
            }
        }

        public void ConnectToHost()
        {
            if (_connecting)
            {
                WriteLog("!ConnectToHost");
                return;
            }
            _connecting = true;

            WriteLog("ConnectToHost");

            try
            {
                lock (_playersData)
                {
                    _playersData.Clear();
                    _playersRo = new ReadOnlyCollection<PlayerDataEx>(new PlayerDataEx[] {});
                }

                DisconnectFromHost();
                _tcpClient.Connect();
            }
            catch (Exception ex)
            {
                string msg = string.Format("ConnectToHost:\r\n{0}", ex);
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _connecting = false;
            }
        }

        public void ReconnectToHost()
        {
            if (_connecting)
            {
                WriteLog("!ReconnectToHost");
                return;
            }

            if (_tcpClient.IsConnected)
            {
                WriteLog("ReconnectToHost");
                _reconnecting = true;
                DisconnectFromHost();
            }
            else
            {
                WriteLog("ReconnectToHost:ConnectToHost");
                ConnectToHost();
            }
        }

        private void InitializeEnvironment()
        {
            _map = new ClientMap();

            _syncScroll = new object();

            _fpsCounterData = new byte[10];
            _fpsFontBrush = new SolidBrush(Color.White);

            _playersData = new Dictionary<int, PlayerDataEx>();

            _threadWorld = new Thread(WorldProcessingProc);
            _threadWorld.IsBackground = true;
            _threadWorld.Start();

            _threadObjectChanged = new Thread(ObjectChangedProc);
            _threadObjectChanged.IsBackground = true;
            _threadObjectChanged.Start();

            TCPClientSettings settings = new TCPClientSettings
                (
                ushort.MaxValue, "127.0.0.1", 15051, true
                );
            _tcpClient = new TCPClient(settings);
            _tcpClient.Connected += TCPConnected;
            _tcpClient.DataReceived += TCPDataReceived;
            _tcpClient.Disconnected += TCPDisconnected;
        }

        private void OnScaleFactorChanged()
        {
            if (!DesignMode)
            {
                foreach (IMapRenderer renderer in _renderers)
                {
                    renderer.ChangeScaleFactor(this, _scaleFactor);
                }
                if (ScaleFactorChanged != null)
                {
                    ScaleFactorChanged(this);
                }
            }
        }

        private void OnMapChanged()
        {
            if (!DesignMode)
            {
                IsMapLoaded = true;
                foreach (IMapRenderer renderer in _renderers)
                {
                    renderer.ChangeMap(this);
                }
                if (MapChanged != null)
                {
                    MapChanged(this);
                }
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
                    OnMouseWheel(m.WParam.ToInt64() > 0 ? 1 : -1);
                    return true;
                }
            }
            return false;
        }

        private void MapControlMouseMove(object sender, MouseEventArgs e)
        {
            CheckMyPlayerRotation();
        }

        private void MapControlMouseWheel(object sender, int delta)
        {
            CheckMyPlayerRotation();
        }

        public new void Dispose()
        {
            if (!DesignMode && !IsDisposed)
            {
                _terminating = true;

                DisconnectFromHost();

                _threadObjectChanged.Join(100);
                _threadWorld.Join(100);
                _threadRenderer.Abort();
                _threadRenderer.Join(100);

                DestroyGraphics();

                _map.Dispose();

                Application.RemoveMessageFilter(this);

                LFactory.Instance.Deinitialize();
            }
            base.Dispose();
        }

#endregion

#region World

        private void UpdateScreenPosByPlayerPos(Player player)
        {
            int wPart = (int)(Width / BORDER_AREA_SPACE_PART);
            int leftWinBorder = _posX + wPart;
            int rightWinBorder = _posX + Width - wPart;
            int hPart = (int)(Height / BORDER_AREA_SPACE_PART);
            int topWinBorder = _posY + hPart;
            int bottomWinBorder = _posY + Height - hPart;

            float pSizeW = player.Size.Width * _scaleFactor;
            float pSizeH = player.Size.Height * _scaleFactor;
            float nPosX = player.Position.X * _scaleFactor;
            float nPosY = player.Position.Y * _scaleFactor;

            if (nPosX + pSizeW > rightWinBorder)
            {
                PosX = (int)(nPosX - (wPart * 2) + pSizeW);
            }
            else if (nPosX < leftWinBorder)
            {
                PosX = (int)(nPosX - wPart);
            }
            if (nPosY + pSizeH > bottomWinBorder)
            {
                PosY = (int)(nPosY - (hPart * 2) + pSizeH);
            }
            else if (nPosY < topWinBorder)
            {
                PosY = (int)(nPosY - hPart);
            }
        }

        private void UpdatePlayersPosition()
        {
            if (PlayerData == null) return;

            lock (_playersData)
            {
                foreach (int playerId in _playersData.Keys)
                {
                    PlayerDataEx playerData = _playersData[playerId];
                    if (Engine.ValidateNewPlayerPosition(playerData, _map))
                    {
                        if (_myPlayerData.Player.Id == playerId && !_myPlayerData.GettingMapWindow &&
                            _myPlayerData.Player.Direction != Direction.None &&
                            _map.NeedsToLoadMapWindow(_myPlayerData, BORDER_AREA_SPACE_PART))
                        {
                            _myPlayerData.GettingMapWindow = true;
                            TCPClientDataSend(new PMapData
                            {
                                SessionToken = _sessionToken,
                            });
                        }
                        _somethingChanged = playerData.NeedUpdatePosition = true;
                    }
                }
            }
        }

        private void CheckMyPlayerRotation()
        {
            if (_myPlayerData != null)
            {
                float pSizeWh = _myPlayerData.Player.Size.Width*_scaleFactor/2;
                float pSizeHh = _myPlayerData.Player.Size.Height*_scaleFactor/2;

                Point mouseLocation = PointToClient(Cursor.Position);
                Point p1 = new Point((int) (_myPlayerData.Player.Position.X*_scaleFactor + pSizeWh),
                                     (int) (_myPlayerData.Player.Position.Y*_scaleFactor + pSizeHh));
                Point p2 = new Point(mouseLocation.X + _posX, mouseLocation.Y + _posY);
                float angle = (float) Math.Round(Geometry.GetAngleOfLine(p1, p2) - 90);
                if (angle != _myPlayerData.Player.Angle)
                {
                    TCPClientDataSend(new PPlayerRotate
                    {
                        SessionToken = _sessionToken,
                        Angle = angle
                    });
                }
            }
        }

        private void WorldProcessingProc()
        {
            while (!_terminating && !IsDisposed)
            {
                if (Enabled)
                {
                    UpdatePlayersPosition();
                }
                Thread.Sleep(1);
            }
        }

        private void ObjectChangedProc()
        {
            while (!_terminating && !IsDisposed)
            {
                if (Enabled)
                {
                    if (_somethingChanged && ObjectChanged != null)
                    {
                        ObjectChanged(this);
                    }
                    _somethingChanged = false;
                }
                Thread.Sleep(1);
            }
        }

#endregion

#region Renderer

        private void RendererProc()
        {
            while (!_terminating && !IsDisposed)
            {
                if (Enabled)
                {
                    DateTime prewFrameRenderTime = _lastFrameRenderTime;

                    try
                    {
                        Invoke(new Action(() =>
                        {
                            CheckMyPlayerPosition();
                            Repaint();
                        }));
                    }
                    catch
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    _fpsCounterArrIdx = _fpsCounterArrIdx < _fpsCounterData.Length - 1
                        ? _fpsCounterArrIdx + 1
                        : 0;
                    _fpsCounterData[_fpsCounterArrIdx] = (byte) _lastFrameRenderTime.
                        Subtract(prewFrameRenderTime).TotalMilliseconds;
                }
                Thread.Sleep(5);
            }
        }

        private void InitializeRenderers()
        {
            _renderers = new List<IMapRenderer>
            {
                new RendererBG(_scaleFactor),
                new RendererWalls(),
                new RendererBorders(),
//                new RendererFOV()
            };
        }

        private void DestroyGraphics()
        {
            if (_bufferBitmap != null)
            {
                _buffer.Dispose();
                _bufferBitmap.Dispose();
                _controlGraphics.Dispose();
                _playerBitmap.Dispose();
                _playerRotatedBitmap.Dispose();
                _playerRotated.Dispose();
                _bufferBitmap = null;
            }
        }

        private void InitializeGraphics()
        {
            if (Width != 0 && Height != 0)
            {
                _playerBitmap = Image.FromFile(@"Resources\player_s.png");

                _playerRotatedBitmap = new Bitmap(_playerBitmap.Width, _playerBitmap.Height);
                
                _playerRotated = Graphics.FromImage(_playerRotatedBitmap);
                _playerRotated.CompositingQuality = CompositingQuality.HighSpeed;
                _playerRotated.InterpolationMode = InterpolationMode.Low;
                _playerRotated.SmoothingMode = SmoothingMode.HighSpeed;

                _bufferBitmap = new Bitmap(Width, Height);
                
                _buffer = Graphics.FromImage(_bufferBitmap);
                _buffer.CompositingQuality = CompositingQuality.HighSpeed;
                _buffer.InterpolationMode = InterpolationMode.Low;
                _buffer.SmoothingMode = SmoothingMode.HighSpeed;

                _controlGraphics = Graphics.FromHwnd(Handle);
                _controlGraphics.CompositingQuality = CompositingQuality.HighSpeed;
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
                GeneralPaint();
                PlayersPaint();
                DrawFps();
                _controlGraphics.DrawImage(_bufferBitmap, ClientRectangle,
                    ClientRectangle, GraphicsUnit.Pixel);
                _lastFrameRenderTime = DateTime.Now;
            }
        }

        private void GeneralPaint()
        {
            _buffer.SetClip(ClientRectangle);
            Rectangle area = new Rectangle(_posX, _posY, Width, Height);
            foreach (IMapRenderer renderer in _renderers)
            {
                renderer.Render(this, _buffer, area);
            }
        }

        private void PlayersPaint()
        {
            lock (_playersData)
            {
                foreach (int playerId in _playersData.Keys)
                {
                    Player player = _playersData[playerId];
                    float pSizeW = player.Size.Width*_scaleFactor,
                        pSizeHw = (float) player.Size.Width/2;
                    float pSizeH = player.Size.Height*_scaleFactor,
                        pSizeHh = (float) player.Size.Height/2;

                    _playerRotated.ResetTransform();
//                _playerRotated.Clear(Color.FromArgb(255, 255, 157, 0));
                    _playerRotated.Clear(Color.FromArgb(0, 0, 0, 0));

                    _playerRotated.TranslateTransform(pSizeHw, pSizeHh);
                    _playerRotated.RotateTransform(player.Angle);
                    _playerRotated.TranslateTransform(-pSizeHw, -pSizeHh);
                    _playerRotated.DrawImage(_playerBitmap, 0, 0);

                    _buffer.DrawImage(_playerRotatedBitmap,
                        new RectangleF((player.Position.X*_scaleFactor - _posX),
                            (player.Position.Y*_scaleFactor - _posY),
                            pSizeW, pSizeH),
                        new RectangleF(0, 0, _playerBitmap.Width, _playerBitmap.Height),
                        GraphicsUnit.Pixel);
                }
            }
        }

        private void CheckMyPlayerPosition()
        {
            if (_myPlayerData != null && _myPlayerData.NeedUpdatePosition)
            {
                _myPlayerData.NeedUpdatePosition = false;
                CheckMyPlayerRotation();
                UpdateScreenPosByPlayerPos(_myPlayerData);
            }
        }

        private void DrawFps()
        {
            int sum = 0;
            foreach (byte rec in _fpsCounterData)
            {
                sum += rec;
            }
            int avg = sum / _fpsCounterData.Length;
            int curFps = (int)Math.Ceiling(1000f / avg);
            string fps = string.Format("{0} FPS", curFps);
            _buffer.DrawString(fps, Font, _fpsFontBrush, 10, 10);
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

#endregion

#region Map
        
        public void SetPosition(int x, int y)
        {
            lock (_syncScroll)
            {
                _scrollToPos = null;
                _posX = x;
                _posY = y;
                if (PositionChanged != null)
                {
                    PositionChanged(this);
                }
            }
        }

        public void ShiftPosition(int shiftX, int shiftY)
        {
            ShiftPosition(shiftX, shiftY, false);
        }

        private void ShiftPosition(int shiftX, int shiftY, bool scrollToPos)
        {
            lock (_syncScroll)
            {
                if (!scrollToPos)
                {
                    _scrollToPos = null;
                }
                _posX += shiftX;
                _posY += shiftY;
                if (PositionChanged != null)
                {
                    PositionChanged(this);
                }
            }
        }

        public ShortPoint? CursorToTilePos(Point location)
        {
            if (_map != null)
            {
                float pixelSize = ConstMap.PIXEL_SIZE * _scaleFactor;
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
                Tile* tile = _map.SetTileType((ushort) x, (ushort) y, type);
                if (tile != null)
                {
                    foreach (IMapRenderer renderer in _renderers)
                    {
                        renderer.UpdateTile(this, (ushort) x, (ushort) y, *tile);
                    }
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
                int range = (int)(radius / (2 * sinus));
                Random rnd = new Random(Environment.TickCount);
                for (int i = radius; i >= range; --i)
                {
                    int j = (int)Math.Sqrt(radius * radius - i * i);
                    for (int k = -j; k <= j; k++)
                    {
                        bool set = i != radius - 1 || rnd.Next(0, 2) == 1;
                        SetTileType(x - k, y + i, TileType.Nothing, set);
                        SetTileType(x - k, y - i, TileType.Nothing, set);
                        SetTileType(x + i, y + k, TileType.Nothing, set);
                        SetTileType(x - i, y - k, TileType.Nothing, set);
                    }
                }
                range = (int)(radius * sinus);
                for (int i = x - range + 1; i < x + range; i++)
                {
                    for (int j = y - range + 1; j < y + range; j++)
                    {
                        SetTileType(i, j, TileType.Nothing, true);
                    }
                }
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
                TileType newTileType = (*tile).Type == TileType.Wall ? TileType.Nothing : TileType.Wall;
                tile = _map.SetTileType(tilePos.Value.X, tilePos.Value.Y, newTileType);
                if (tile != null)
                {
                    foreach (IMapRenderer renderer in _renderers)
                    {
                        renderer.UpdateTile(this, tilePos.Value.X, tilePos.Value.Y, *tile);
                    }
                    if (TilesChanged != null)
                    {
                        TilesChanged(this);
                    }
                }
            }
        }

        private void DoScrollToPos()
        {
            while (!_terminating && !IsDisposed)
            {
                lock (_syncScroll)
                {
                    if (_scrollToPos == null)
                    {
                        return;
                    }
                    int xShift = (_scrollToPos.Value.X - _posX) / 2;
                    int yShift = (_scrollToPos.Value.Y - _posY) / 2;
                    if (Math.Abs(xShift) <= 35 && Math.Abs(yShift) <= 35)
                    {
                        SetPosition(_scrollToPos.Value.X, _scrollToPos.Value.Y);
                    }
                    else
                    {
                        ShiftPosition(xShift, yShift, true);
                    }
                }
                Thread.Sleep(25);
            }
        }

        public void CenterTo(Point pos, bool smothScroll = false)
        {
            CenterTo(pos.X, pos.Y, smothScroll);
        }

        public void CenterTo(int x, int y, bool smothScroll = false)
        {
            lock (_syncScroll)
            {
                x -= Width / 2;
                y -= Height / 2;
                if (!smothScroll)
                {
                    SetPosition(x, y);
                }
                else
                {
                    if (_scrollToPos != null)
                    {
                        _scrollToPos = new Point(x, y);
                    }
                    else
                    {
                        _scrollToPos = new Point(x, y);
                        ThreadPool.QueueUserWorkItem(o => DoScrollToPos());
                    }
                }
            }
        }

        public void CenterPlayer()
        {
            if (_myPlayerData != null)
            {
                CenterTo((int)(_myPlayerData.Player.Position.X * _scaleFactor),
                         (int)(_myPlayerData.Player.Position.Y * _scaleFactor),
                         true);
            }
        }

#endregion

#region Players

        public void PlayerMove(Direction direction)
        {
            if (_myPlayerData != null && _myPlayerData.Player.Direction != direction)
            {
                TCPClientDataSend(new PPlayerMove
                {
                    SessionToken = _sessionToken,
                    Position = _myPlayerData.Player.Position,
                    Direction = direction
                });
            }
        }

#endregion

#region Game host events

        private void GameHostResponse(BaseResponse e)
        {
            if (e.HasError)
            {
                ReconnectToHost();
                return;
            }

            switch (e.Type)
            {
                case PacketType.UserLogin:
                    RUserLogin userLogin = (RUserLogin) e;
                    _sessionToken = userLogin.SessionToken;
                    ShortSize screenRes = new ShortSize
                        (
                        Screen.PrimaryScreen.Bounds.Width,
                        Screen.PrimaryScreen.Bounds.Height
                        );
                    TCPClientDataSend(new PUserEnter
                    {
                        SessionToken = _sessionToken,
                        ScreenRes = screenRes
                    });
                    break;

                case PacketType.UserEnter:
                    lock (_playersData)
                    {
                        RUserEnter userEnter = (RUserEnter) e; //!!!
                        _map.Setup(userEnter.MapSize);
                        _map.AppendMapData(userEnter.MapData, userEnter.MapWindow);
                        _map.AppendMiniMapData(userEnter.MiniMapData, userEnter.MiniMapSize);
                        OnMapChanged();

                        foreach (Player p in userEnter.PlayersOnLocation)
                        {
                            if (!_playersData.ContainsKey(p.Id))
                            {
                                _playersData.Add(p.Id, new PlayerDataEx(p));
                            }
                        }
                        _myPlayerData = _playersData[userEnter.MyPlayerId];
                        _playersRo = new ReadOnlyCollection<PlayerDataEx>(_playersData.Values.ToArray());
                    }
                    CenterTo((int) (_myPlayerData.Player.Position.X*_scaleFactor), //!!!
                             (int) (_myPlayerData.Player.Position.Y*_scaleFactor),
                             true);
                    break;

                case PacketType.PlayerEnter:
                    lock (_playersData)
                    {
                        RPlayerEnter playerEnter = (RPlayerEnter) e;
                        if (!_playersData.ContainsKey(playerEnter.Player.Id) && _myPlayerData != null &&
                            playerEnter.Player.Id != _myPlayerData.Player.Id)
                        {
                            _playersData.Add(playerEnter.Player.Id, new PlayerDataEx(playerEnter.Player));
                            _playersRo = new ReadOnlyCollection<PlayerDataEx>(_playersData.Values.ToArray());
                            _somethingChanged = true; //!!!
                        }
                    }
                    break;

                case PacketType.PlayerExit:
                    lock (_playersData)
                    {
                        RPlayerExit playerExit = (RPlayerExit) e;
                        if (_playersData.ContainsKey(playerExit.PlayerId) && _myPlayerData != null &&
                            playerExit.PlayerId != _myPlayerData.Player.Id)
                        {
                            _playersData.Remove(playerExit.PlayerId);
                            _playersRo = new ReadOnlyCollection<PlayerDataEx>(_playersData.Values.ToArray());
                            _somethingChanged = true; //!!!
                        }
                    }
                    break;

                case PacketType.PlayerRotate:
                    lock (_playersData)
                    {
                        PlayerDataEx playerData;
                        RPlayerRotate playerRotate = (RPlayerRotate) e;
                        if (_playersData.TryGetValue(playerRotate.PlayerId, out playerData))
                            playerData.Player.Angle = playerRotate.Angle;
                    }
                    break;

                case PacketType.PlayerMove:
                    lock (_playersData)
                    {
                        PlayerDataEx playerData;
                        RPlayerMove playerMove = (RPlayerMove) e;
                        if (_playersData.TryGetValue(playerMove.PlayerId, out playerData))
                        {
                            if (playerMove.Direction == Direction.None)
                                playerData.StopMoving(playerMove.Position);
                            else
                                playerData.StartMoving(playerMove.Position, playerMove.Direction);
                        }
                    }
                    break;

                case PacketType.MapData:
                    RMapData mapData = (RMapData) e;
                    _map.AppendMapData(mapData.MapData, mapData.MapWindow); //!!!
                    _myPlayerData.GettingMapWindow = false;
                    break;
            }
        }

#endregion

#region TCP

        private static string _longPassword = new string('P', short.MaxValue - 100);
        private void TCPConnected(TCPClient client)
        {
            WriteLog("TCPConnected" + Environment.NewLine);
            _connecting = false;
            TCPClientDataSend(new PUserLogin
            {
                UserName = "PsychoTeras",
                Password = "password"
//                Password =  _longPassword
            });
        }

        private void TCPClientDataSend(BasePacket packet)
        {
            if (_sessionToken != 0 || packet.Type == PacketType.UserLogin)
            {
                _tcpClient.Send(packet);
            }
        }

        private void TCPDataReceived(TCPClient client, IList<BaseResponse> packets)
        {
            foreach (BaseResponse packet in packets)
            {
                GameHostResponse(packet);
            }
        }

        private void TCPDisconnected(TCPClient client)
        {
            if (_reconnecting)
            {
                WriteLog("TCPDisconnected");
                _reconnecting = false;
                ConnectToHost();
            }
            else
            {
                WriteLog("!TCPDisconnected");
            }
        }

#endregion

    }
}
