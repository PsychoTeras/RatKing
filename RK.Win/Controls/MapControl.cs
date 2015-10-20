using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using RK.Common.Algo;
using RK.Common.Classes.Map;
using RK.Common.Classes.Units;
using RK.Common.Common;
using RK.Common.Const;
using RK.Common.Host;
using RK.Common.Proto;
using RK.Common.Proto.Packets;
using RK.Common.Proto.Responses;
using RK.Win.Classes.Ex;
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
        private GameHost _host;

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

        private long _sessionToken;
        private Dictionary<int, Player> _players;
        private Dictionary<int, PlayerEx> _playersEx;

        private Player _myPlayer;

        private Thread _threadRenderer;
        private DateTime _lastFrameRenderTime;

        private byte[] _fpsCounterData;
        private int _fpsCounterArrIdx;
        private Brush _fpsFontBrush;

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
        public GameHost Host
        {
            get { return _host; }
            set
            {
                if (_host != null)
                {
                    _host.GameHostResponse -= GameHostResponse;
                }
                _host = value;
                ConnectToHost();
            }
        }

        [Browsable(false)]
        public GameMap Map
        {
            get { return _map; }
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
            ShowTileNumber = DEF_SHOW_TILE_NUMBER;

            if (!DesignMode)
            {
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

        private void ConnectToHost()
        {
            if (_host == null)
            {
                return;
            }

            _host.GameHostResponse += GameHostResponse;

            try
            {
                if (_host.World.FirstMap == null)
                {
                    _host.World.LoadMap();
                }

                if (_sessionToken != 0)
                {
                    _host.ProcessPacket(new PUserLogout
                    {
                        SessionToken = _sessionToken
                    });
                }

                RUserLogin userLogin = _host.ProcessPacket(new PUserLogin
                {
                    UserName = "PsychoTeras",
                    Password = "password"
                }).As<RUserLogin>();
                _sessionToken = userLogin.SessionToken;

                _players.Clear();
                _playersEx.Clear();
                RPlayerEnter playerEnter = _host.ProcessPacket(new PPlayerEnter
                {
                    SessionToken = _sessionToken
                }).As<RPlayerEnter>();
                foreach (Player p in playerEnter.PlayersOnLocation)
                {
                    _players.Add(p.Id, p);
                    _playersEx.Add(p.Id, new PlayerEx(p));
                }
                _myPlayer = _players[playerEnter.MyPlayerId];

                LoadMap(_host.World.FirstMap);

                CenterTo((int) (_myPlayer.Position.X * _scaleFactor),
                         (int) (_myPlayer.Position.Y * _scaleFactor),
                         true);
            }
            catch (Exception ex)
            {
                string msg = string.Format("ConnectToHost:\r\n{0}", ex);
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeEnvironment()
        {
            _syncScroll = new object();

            _fpsCounterData = new byte[10];
            _fpsFontBrush = new SolidBrush(Color.White);

            _players = new Dictionary<int, Player>();
            _playersEx = new Dictionary<int, PlayerEx>();
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
                foreach (IMapRenderer renderer in _renderers)
                {
                    renderer.ChangeMap(this);
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
                    OnMouseWheel(m.WParam.ToInt32() > 0 ? 1 : -1);
                    return true;
                }
            }
            return false;
        }

        private void CheckMyPlayerRotate()
        {
            if (_myPlayer != null)
            {
                float pSizeWh = _myPlayer.Size.Width * _scaleFactor / 2;
                float pSizeHh = _myPlayer.Size.Height * _scaleFactor / 2;

                Point mouseLocation = PointToClient(Cursor.Position);
                Point p1 = new Point((int)(_myPlayer.Position.X * _scaleFactor + pSizeWh),
                                     (int)(_myPlayer.Position.Y * _scaleFactor + pSizeHh));
                Point p2 = new Point(mouseLocation.X + _posX, mouseLocation.Y + _posY);
                float angle = Geometry.GetAngleOfLine(p1, p2) - 90;
                if (angle != _myPlayer.Angle)
                {
                    _host.ProcessPacket(new PPlayerRotate
                    {
                        SessionToken = _sessionToken,
                        Angle = angle
                    });
                }
            }
        }

        private void MapControlMouseMove(object sender, MouseEventArgs e)
        {
            CheckMyPlayerRotate();
        }

        private void MapControlMouseWheel(object sender, int delta)
        {
            CheckMyPlayerRotate();
        }

        public new void Dispose()
        {
            if (!DesignMode)
            {
                Application.RemoveMessageFilter(this);
                _threadRenderer.Abort();
                _threadRenderer.Join();
                DestroyGraphics();
            }
            base.Dispose();
        }

#endregion

#region Renderer

        private int CalculatePlayerTraveledDistance(Player player, PlayerEx playerEx)
        {
            double moveTimeElapsed = DateTime.Now.Subtract(playerEx.MovingStartTime).
                TotalMilliseconds;
            playerEx.MovingStartTime = DateTime.Now;
            return (int)Math.Ceiling(moveTimeElapsed / player.Speed);
        }

        private Point CalculateNewPlayerPos(Player player, PlayerEx playerEx, int traveled)
        {
            Point newPos;
            Point startPos = player.Position;
            switch (player.Direction)
            {
                case Direction.N:
                    newPos = new Point(startPos.X, startPos.Y - traveled);
                    break;
                case Direction.NW:
                    newPos = new Point(startPos.X - traveled, startPos.Y - traveled);
                    break;
                case Direction.NE:
                    newPos = new Point(startPos.X + traveled, startPos.Y - traveled);
                    break;
                case Direction.S:
                    newPos = new Point(startPos.X, startPos.Y + traveled);
                    break;
                case Direction.SW:
                    newPos = new Point(startPos.X - traveled, startPos.Y + traveled);
                    break;
                case Direction.SE:
                    newPos = new Point(startPos.X + traveled, startPos.Y + traveled);
                    break;
                case Direction.W:
                    newPos = new Point(startPos.X - traveled, startPos.Y);
                    break;
                case Direction.E:
                    newPos = new Point(startPos.X + traveled, startPos.Y);
                    break;
                default:
                    newPos = default(Point);
                    break;
            }
            return newPos;
        }

        private void UpdateScreenPosByPlayerPos(Player player)
        {
            int wPart = (int)(Width / 3f);
            int leftWinBorder = _posX + wPart;
            int rightWinBorder = _posX + Width - wPart;
            int hPart = (int)(Height / 3f);
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

        private void PlayerPosProcessCollision(Player player, ref Point newPos)
        {
            Point pos = player.Position;
            TinySize size = player.Size;
            bool stop = false, xStop = false, yStop = false;

            int mapWidth = _map.Width, mapHeight = _map.Height;

            int cellTs = (int)Math.Floor((float)pos.Y / ConstMap.PIXEL_SIZE);
            int cellTe = (int)Math.Floor((float)newPos.Y / ConstMap.PIXEL_SIZE);
            int cellLs = (int)Math.Floor((float)pos.X / ConstMap.PIXEL_SIZE);
            int cellLe = (int)Math.Floor((float)newPos.X / ConstMap.PIXEL_SIZE);

            int cellBs = (int)Math.Floor((float)(pos.Y + size.Height - 1) / ConstMap.PIXEL_SIZE);
            int cellBe = (int)Math.Floor((float)(newPos.Y + size.Height - 1) / ConstMap.PIXEL_SIZE);
            int cellRs = (int)Math.Floor((float)(pos.X + size.Width - 1) / ConstMap.PIXEL_SIZE);
            int cellRe = (int)Math.Floor((float)(newPos.X + size.Width - 1) / ConstMap.PIXEL_SIZE);

            int cellT = (int)Math.Floor((float)pos.Y / ConstMap.PIXEL_SIZE);
            int cellB = (int)Math.Floor((float)(pos.Y + size.Height - 1) / ConstMap.PIXEL_SIZE);

            int cellL = (int)Math.Floor((float)pos.X / ConstMap.PIXEL_SIZE);
            int cellR = (int)Math.Floor((float)(pos.X + size.Width - 1) / ConstMap.PIXEL_SIZE);

            switch (player.Direction)
            {
                case Direction.N:
                {
                    for (int y = cellTs; y >= cellTe; y--)
                    {
                        if (y < 0 || y >= mapHeight || stop) break;
                        for (int x = cellL; x <= cellR; x++)
                        {
                            if (x < 0 || x >= mapWidth) continue;
                            if ((*_map[(ushort) x, (ushort) y]).Type != TileType.Nothing)
                            {
                                newPos.Y = y*ConstMap.PIXEL_SIZE + ConstMap.PIXEL_SIZE;
                                stop = true;
                                break;
                            }
                        }
                    }
                    break;
                }
                case Direction.NW:
                {
                    int tTotalDist = cellTs - cellTe;
                    int lTotalDist = cellLs - cellLe;
                    int maxTotalDist = Math.Max(tTotalDist, lTotalDist);

                    for (int i = 1; i <= maxTotalDist; i++)
                    {
                        if (xStop && yStop) break;

                        cellT--; cellB--; cellL--; cellR--;

                        for (int y = cellT; y <= cellB; y++)
                        {
                            if (y < 0 || y >= mapHeight) continue;

                            //Y
                            if (y == cellT && i <= tTotalDist)
                            {
                                for (int x = cellL; x <= cellR; x++)
                                {
                                    if (x < 0 || x >= mapWidth) continue;
                                    if (!yStop && (*_map[(ushort) (x + 1), (ushort) cellT]).Type != TileType.Nothing)
                                    {
                                        newPos.Y = cellT*ConstMap.PIXEL_SIZE + ConstMap.PIXEL_SIZE;
                                        yStop = true;
                                        break;
                                    }
                                }
                            }

                            //X
                            if (!xStop && i <= lTotalDist &&
                                (*_map[(ushort) cellL, (ushort) (y + 1)]).Type != TileType.Nothing)
                            {
                                newPos.X = cellL*ConstMap.PIXEL_SIZE + ConstMap.PIXEL_SIZE;
                                xStop = true;
                                break;
                            }

                            if (xStop && yStop) break;
                        }
                    }
                    break;
                }
                case Direction.NE:
                {
                    int tTotalDist = cellTs - cellTe;
                    int rTotalDist = cellRe - cellRs;
                    int maxTotalDist = Math.Max(tTotalDist, rTotalDist);

                    for (int i = 1; i <= maxTotalDist; i++)
                    {
                        if (xStop && yStop) break;

                        cellT--; cellB--; cellL++; cellR++;

                        for (int y = cellT; y <= cellB; y++)
                        {
                            if (y < 0 || y >= mapHeight) continue;

                            //Y
                            if (y == cellT && i <= tTotalDist)
                            {
                                for (int x = cellL; x <= cellR; x++)
                                {
                                    if (x < 0 || x >= mapWidth) continue;
                                    if (!yStop && (*_map[(ushort) (x - 1), (ushort) cellT]).Type != TileType.Nothing)
                                    {
                                        newPos.Y = cellT*ConstMap.PIXEL_SIZE + ConstMap.PIXEL_SIZE;
                                        yStop = true;
                                        break;
                                    }
                                }
                            }

                            //X
                            if (!xStop && i <= rTotalDist &&
                                (*_map[(ushort) cellR, (ushort) (y + 1)]).Type != TileType.Nothing)
                            {
                                newPos.X = cellR*ConstMap.PIXEL_SIZE - size.Width;
                                xStop = true;
                                break;
                            }

                            if (xStop && yStop) break;
                        }
                    }
                    break;
                }
                case Direction.S:
                {
                    for (int y = cellBs; y <= cellBe; y++)
                    {
                        if (y < 0 || y >= mapHeight || stop) break;
                        for (int x = cellL; x <= cellR; x++)
                        {
                            if (x < 0 || x >= mapWidth) continue;
                            if ((*_map[(ushort) x, (ushort) y]).Type != TileType.Nothing)
                            {
                                newPos.Y = y*ConstMap.PIXEL_SIZE - size.Height;
                                stop = true;
                                break;
                            }
                        }
                    }
                    break;
                }
                case Direction.SW:
                {
                    int bTotalDist = cellBe - cellBs;
                    int lTotalDist = cellLs - cellLe;
                    int maxTotalDist = Math.Max(bTotalDist, lTotalDist);

                    for (int i = 1; i <= maxTotalDist; i++)
                    {
                        if (xStop && yStop) break;

                        cellT++; cellB++; cellL--; cellR--;

                        for (int y = cellT; y <= cellB; y++)
                        {
                            if (y < 0 || y >= mapHeight) continue;

                            //Y
                            if (y == cellT && i <= bTotalDist)
                            {
                                for (int x = cellL; x <= cellR; x++)
                                {
                                    if (x < 0 || x >= mapWidth) continue;
                                    if (!yStop && (*_map[(ushort) (x + 1), (ushort) cellB]).Type != TileType.Nothing)
                                    {
                                        newPos.Y = cellB*ConstMap.PIXEL_SIZE - size.Height;
                                        yStop = true;
                                        break;
                                    }
                                }
                            }

                            //X
                            if (!xStop && i <= lTotalDist &&
                                (*_map[(ushort) cellL, (ushort) (y - 1)]).Type != TileType.Nothing)
                            {
                                newPos.X = cellL*ConstMap.PIXEL_SIZE + ConstMap.PIXEL_SIZE;
                                xStop = true;
                                break;
                            }

                            if (xStop && yStop) break;
                        }
                    }
                    break;
                }
                case Direction.SE:
                {
                    int bTotalDist = cellBe - cellBs;
                    int rTotalDist = cellRe - cellRs;
                    int maxTotalDist = Math.Max(bTotalDist, rTotalDist);

                    for (int i = 1; i <= maxTotalDist; i++)
                    {
                        if (xStop && yStop) break;

                        cellT++; cellB++; cellL++; cellR++;

                        for (int y = cellT; y <= cellB; y++)
                        {
                            if (y < 0 || y >= mapHeight) continue;

                            //Y
                            if (y == cellT && i <= bTotalDist)
                            {
                                for (int x = cellL; x <= cellR; x++)
                                {
                                    if (x < 0 || x >= mapWidth) continue;
                                    if (!yStop && (*_map[(ushort) (x - 1), (ushort) cellB]).Type != TileType.Nothing)
                                    {
                                        newPos.Y = cellB*ConstMap.PIXEL_SIZE - size.Height;
                                        yStop = true;
                                        break;
                                    }
                                }
                            }

                            //X
                            if (!xStop && i <= rTotalDist &&
                                (*_map[(ushort) cellR, (ushort) (y - 1)]).Type != TileType.Nothing)
                            {
                                newPos.X = cellR*ConstMap.PIXEL_SIZE - size.Width;
                                xStop = true;
                                break;
                            }

                            if (xStop && yStop) break;
                        }
                    }
                    break;
                }
                case Direction.W:
                {
                    for (int x = cellLs; x >= cellLe; x--)
                    {
                        if (x < 0 || x >= mapWidth || stop) break;
                        for (int y = cellT; y <= cellB; y++)
                        {
                            if (y < 0 || y >= mapHeight) continue;
                            if ((*_map[(ushort) x, (ushort) y]).Type != TileType.Nothing)
                            {
                                newPos.X = x*ConstMap.PIXEL_SIZE + ConstMap.PIXEL_SIZE;
                                stop = true;
                                break;
                            }
                        }
                    }
                    break;
                }
                case Direction.E:
                {
                    for (int x = cellRs; x <= cellRe; x++)
                    {
                        if (x < 0 || x >= mapWidth || stop) break;
                        for (int y = cellT; y <= cellB; y++)
                        {
                            if (y < 0 || y >= mapHeight) continue;
                            if ((*_map[(ushort) x, (ushort) y]).Type != TileType.Nothing)
                            {
                                newPos.X = x*ConstMap.PIXEL_SIZE - size.Width;
                                stop = true;
                                break;
                            }
                        }
                    }
                    break;
                }
            }
        }

        private void UpdatePlayersPosition()
        {
            foreach (int playerId in _players.Keys)
            {
                Player player = _players[playerId];
                if (player.Direction == Direction.None)
                {
                    continue;
                }

                PlayerEx playerEx = _playersEx[playerId];
                
                int traveled = CalculatePlayerTraveledDistance(player, playerEx);
                
                Point newPos = CalculateNewPlayerPos(player, playerEx, traveled);
                
                if (player.Id == _myPlayer.Id)
                {
                    PlayerPosProcessCollision(player, ref newPos);
                }
                
                if (player.Position != newPos)
                {
                    player.Position = newPos;
                    if (player.Id == _myPlayer.Id)
                    {
                        UpdateScreenPosByPlayerPos(player);

                        Invoke(new Action(CheckMyPlayerRotate));
                    }
                }
            }
        }

        private void RendererProc()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                DateTime prewFrameRenderTime = _lastFrameRenderTime;

                UpdatePlayersPosition();

                Invoke(new Action(Repaint));

                _fpsCounterArrIdx = _fpsCounterArrIdx < _fpsCounterData.Length - 1
                    ? _fpsCounterArrIdx + 1
                    : 0;
                _fpsCounterData[_fpsCounterArrIdx] = (byte)_lastFrameRenderTime.
                    Subtract(prewFrameRenderTime).TotalMilliseconds;

                Thread.Sleep(1);
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

        private void DestroyGraphics()
        {
            if (_bufferBitmap != null)
            {
                _buffer.Dispose();
                _bufferBitmap.Dispose();
                _controlGraphics.Dispose();
                _playerBitmap.Dispose();
                _bufferBitmap = null;

                if (_playerRotatedBitmap != null)
                {
                    _playerRotatedBitmap.Dispose();
                    _playerRotated.Dispose();
                    _playerRotatedBitmap = null;
                }
            }
        }

        private void InitializeGraphics()
        {
            if (Width != 0 && Height != 0)
            {
                _playerBitmap = Image.FromFile(@"Resources\player_s.png");

                _playerRotatedBitmap = new Bitmap(_playerBitmap.Width, _playerBitmap.Height);
                _playerRotated = Graphics.FromImage(_playerRotatedBitmap);
                _playerRotated.InterpolationMode = InterpolationMode.Low;
                _playerRotated.SmoothingMode = SmoothingMode.HighSpeed;

                _bufferBitmap = new Bitmap(Width, Height);
                _buffer = Graphics.FromImage(_bufferBitmap);
                _buffer.InterpolationMode = InterpolationMode.Low;
                _buffer.SmoothingMode = SmoothingMode.HighSpeed;

                _controlGraphics = Graphics.FromHwnd(Handle);

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
            Rectangle area = new Rectangle(_posX, _posY, Width, Height);
            foreach (IMapRenderer renderer in _renderers)
            {
                renderer.Render(this, _buffer, area);
            }
        }

        private void PlayersPaint()
        {
            foreach (int playerId in _players.Keys)
            {
                Player player = _players[playerId];
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
                    new RectangleF((player.Position.X * _scaleFactor - _posX),
                                   (player.Position.Y * _scaleFactor - _posY),
                                   pSizeW, pSizeH),
                    new RectangleF(0, 0, _playerBitmap.Width, _playerBitmap.Height), 
                    GraphicsUnit.Pixel);
            }
        }

        private void DrawFps()
        {
            int sum = 0;
            foreach (byte rec in _fpsCounterData)
            {
                sum += rec;
            }
            int avg = sum/_fpsCounterData.Length;
            int curFps = (int) Math.Ceiling(1000f/avg);
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

        private void LoadMap(GameMap map)
        {
            _map = map;
            OnMapChanged();
            if (MapLoaded != null)
            {
                MapLoaded(this);
            }
        }

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
                Tile* tile = _map[(ushort)x, (ushort)y];
                (*tile).Type = type;
                foreach (IMapRenderer renderer in _renderers)
                {
                    renderer.UpdateTile(this, (ushort)x, (ushort)y, *tile);
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
                (*tile).Type = (*tile).Type == TileType.Wall ? TileType.Nothing : TileType.Wall;
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

        private void DoScrollToPos()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                lock (_syncScroll)
                {
                    if (_scrollToPos == null)
                    {
                        Invoke(new Action(CheckMyPlayerRotate));
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
            if (_myPlayer != null)
            {
                CenterTo((int)(_myPlayer.Position.X * _scaleFactor),
                         (int)(_myPlayer.Position.Y * _scaleFactor),
                         true);
            }
        }

#endregion

#region Players

        public void PlayerMove(Direction direction)
        {
            if (_myPlayer != null && _myPlayer.Direction != direction)
            {
                _host.ProcessPacket(new PPlayerMove
                {
                    SessionToken = _sessionToken,
                    X =  _myPlayer.Position.X,
                    Y =  _myPlayer.Position.Y,
                    D = direction
                });
            }
        }

#endregion

#region Game host events

        private void GameHostResponse(BaseResponse e)
        {
            switch (e.Type)
            {
                case PacketType.PlayerRotate:
                    RPlayerRotate rPlayerRotate = (RPlayerRotate) e;
                    _players[rPlayerRotate.PlayerId].Angle = rPlayerRotate.Angle;
                    break;

                case PacketType.PlayerMove:
                    RPlayerMove rPlayerMove = (RPlayerMove) e;
                    Player player = _players[rPlayerMove.PlayerId];
                    player.Position = new Point(rPlayerMove.X, rPlayerMove.Y);
                    player.Direction = rPlayerMove.D;

                    PlayerEx playerEx = _playersEx[rPlayerMove.PlayerId];
                    playerEx.MovingStartTime = DateTime.Now;
                    playerEx.MovingStartedPoint = player.Position;
                    break;
            }
        }

#endregion

    }
}
