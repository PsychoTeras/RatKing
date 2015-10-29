using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace RK.Server.Controls
{
    namespace Graphing
    {
        public class PerfGraph : Control
        {
            public class LineHandle
            {
                private Line _line;
                private PerfGraph _owner;

                public LineHandle(ref Object line, PerfGraph owner)
                {
                    if (string.Compare(line.GetType().Name, "Line") != 0)
                    {
                        throw new ArithmeticException(
                            "LineHandle: First Parameter must be " +
                            "type of 'Line' cast to base 'Object'");
                    }

                    _line = (Line) line;
                    _owner = owner;
                }

                public void Clear()
                {
                    _line.MagnitudeList.Clear();
                    _owner.UpdateGraph();
                }

                public Color Color
                {
                    set
                    {
                        if (_line.Color != value)
                        {
                            _line.Color = value;
                            _owner.Invalidate(false);
                        }
                    }
                    get { return _line.Color; }

                }

                public uint Thickness
                {
                    set
                    {
                        if (_line.Thickness != value)
                        {
                            _line.Thickness = value;
                            _owner.Invalidate(false);
                        }
                    }
                    get { return _line.Thickness; }
                }

                public bool Visible
                {
                    set
                    {
                        if (_line.Visible != value)
                        {
                            _line.Visible = value;
                            _owner.Invalidate(false);
                        }
                    }
                    get { return _line.Visible; }
                }

                public bool ShowAsBar
                {
                    set
                    {
                        if (_line.ShowAsBar != value)
                        {
                            _line.ShowAsBar = value;
                            _owner.Invalidate(false);
                        }
                    }
                    get { return _line.ShowAsBar; }
                }
            }

            private class Line : IDisposable
            {
                public string TagText;
                public SizeF TagTextSize;

                public List<float> MagnitudeList = new List<float>();
                public int NumID = -1;
                public bool ShowAsBar;
                public bool Visible = true;

                public Pen Pen;
                public Pen PenThin;
                public Brush Brush;

                private Color _color = Color.Green;
                private uint _thickness = 1;

                public Color Color
                {
                    get { return _color; }
                    set
                    {
                        if (value != _color)
                        {
                            Pen.Dispose();
                            PenThin.Dispose();
                            Brush.Dispose();
                            Pen = new Pen(_color = value, _thickness);
                            PenThin = new Pen(_color, 1);
                            Brush = new SolidBrush(_color);
                        }
                    }
                }

                public uint Thickness
                {
                    get { return _thickness; }
                    set
                    {
                        if (value != _thickness)
                        {
                            Pen.Dispose();
                            Pen = new Pen(_color, _thickness = value);
                        }
                    }
                }

                public Line(int num)
                {
                    NumID = num;
                    Pen = new Pen(_color, _thickness);
                    PenThin = new Pen(_color, 1);
                    Brush = new SolidBrush(_color);
                }

                public void Dispose()
                {
                    Pen.Dispose();
                    Brush.Dispose();
                }
            }

            private SolidBrush _backBrush;

            private Color _textColor = Color.Yellow;
            private SolidBrush _textBrush;
            private Pen _borderPen;

            private Color _gridColor = Color.Green;
            private Pen _gridPen;

            private string _maxLabel;
            private string _minLabel;
            private bool _bHighQuality = true;
            private bool _bAutoScale;
            private bool _bMinLabelSet;
            private bool _bMaxLabelSet;
            private bool _bShowMinMax = true;
            private bool _bShowGrid = true;
            private int _maxCoords = -1;
            private int _lineInterval = 5;
            private float _maxPeek = 100;
            private float _minPeek;
            private int _gridSize = 15;
            private int _offsetX;

            private string _units = string.Empty;

            private List<Line> _lines = new List<Line>();
            private IContainer components;

            public PerfGraph()
            {
                InitializeComponent();
                InitializeStyles();
            }

            public PerfGraph(Form parent)
            {
                parent.Controls.Add(this);

                InitializeComponent();
                InitializeStyles();
            }

            public PerfGraph(Form parent, Rectangle rectPos)
            {
                parent.Controls.Add(this);

                Location = rectPos.Location;
                Height = rectPos.Height;
                Width = rectPos.Width;

                InitializeComponent();
                InitializeStyles();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _borderPen.Dispose();
                    _textBrush.Dispose();
                    _gridPen.Dispose();
                    foreach (Line line in _lines)
                    {
                        line.Dispose();
                    }
                }
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }

            private void InitializeComponent()
            {
                components = new Container();
            }

            private void InitializeStyles()
            {
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                SetStyle(ControlStyles.UserPaint, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.ResizeRedraw, true);

                _gridPen = new Pen(_gridColor, 1);
                _textBrush = new SolidBrush(_textColor);
                _borderPen = new Pen(_textColor, 1);
                _backBrush = new SolidBrush(BackColor);

                MouseLeave += C2DPushGraph_MouseLeave;
                MouseMove += C2DPushGraph_MouseMove;
            }

            private new bool DesignMode
            {
                get { return base.DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime; }
            }

            public override Color BackColor
            {
                get { return base.BackColor; }
                set
                {
                    _backBrush.Dispose();
                    _backBrush = new SolidBrush(base.BackColor = value);
                }
            }

            public string Units
            {
                get { return _units; }
                set
                {
                    if ((value = value ?? string.Empty) != _units)
                    {
                        _units = value;
                        Invalidate(false);
                    }
                }
            }

            public Color TextColor
            {
                set
                {
                    if (_textColor != value)
                    {
                        _textBrush.Dispose();
                        _borderPen.Dispose();
                        _textBrush = new SolidBrush(_textColor = value);
                        _borderPen = new Pen(_textColor, 1);
                        Invalidate(false);
                    }
                }
                get { return _textColor; }
            }

            public Color GridColor
            {
                set
                {
                    if (_gridColor != value)
                    {
                        _gridPen.Dispose();
                        _gridPen = new Pen(_gridColor = value, 1);
                        Invalidate(false);
                    }
                }
                get { return _gridColor; }
            }

            public ushort LineInterval
            {
                set
                {
                    if ((ushort) _lineInterval != value)
                    {
                        _lineInterval = value;
                        _maxCoords = -1;
                        Invalidate(false);
                    }
                }
                get { return (ushort) _lineInterval; }
            }

            public string MaxLabel
            {
                set
                {
                    if (DesignMode || (!string.IsNullOrEmpty(value) && value != "0"))
                    {
                        _bMaxLabelSet = true;
                        _maxLabel = value;
                        _maxCoords = -1;
                        Invalidate(false);
                    }
                }
                get { return _maxLabel; }
            }

            public string MinLabel
            {
                set
                {
                    if (DesignMode || (!string.IsNullOrEmpty(value) && value != "0"))
                    {
                        _bMinLabelSet = true;
                        _minLabel = value;
                        _maxCoords = -1;
                        Invalidate(false);
                    }
                }
                get { return _minLabel; }
            }

            public ushort GridSize
            {
                set
                {
                    if (_gridSize != value)
                    {
                        _gridSize = value;
                        Invalidate(false);
                    }
                }
                get { return (ushort) _gridSize; }
            }

            public float MaxPeekMagnitude
            {
                set
                {
                    _maxPeek = value;
                    RefreshLabels();
                }
                get { return _maxPeek; }
            }

            public float MinPeekMagnitude
            {
                set
                {
                    _minPeek = value;
                    RefreshLabels();
                }
                get { return _minPeek; }
            }

            public bool AutoAdjustPeek
            {
                set
                {
                    if (_bAutoScale != value)
                    {
                        _bAutoScale = value;
                        Invalidate(false);
                    }
                }
                get { return _bAutoScale; }
            }

            public bool HighQuality
            {
                set
                {
                    if (value != _bHighQuality)
                    {
                        _bHighQuality = value;
                        Invalidate(false);
                    }
                }
                get { return _bHighQuality; }
            }

            public bool ShowLabels
            {
                set
                {
                    if (_bShowMinMax != value)
                    {
                        _bShowMinMax = value;
                        _maxCoords = -1;
                        Invalidate(false);
                    }
                }
                get { return _bShowMinMax; }
            }

            public bool ShowGrid
            {
                set
                {
                    if (_bShowGrid != value)
                    {
                        _bShowGrid = value;
                        Invalidate(false);
                    }
                }
                get { return _bShowGrid; }
            }


            protected override void OnSizeChanged(EventArgs e)
            {
                _maxCoords = -1;
                Invalidate(false);
                base.OnSizeChanged(e);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = _bHighQuality
                    ? SmoothingMode.HighQuality
                    : SmoothingMode.Default;

                _offsetX = 0;

                if (_bShowMinMax)
                {
                    DrawLabels(ref g);
                }

                if (_bShowGrid)
                {
                    DrawGrid(ref g);
                }

                if (_offsetX != 0)
                {
                    g.Clip = new Region(new Rectangle(_offsetX, 0, Width - _offsetX, Height));
                }

                DrawLines(ref g);

                if (_mouseUnderPos != null)
                {
                    int lineIdx = (_mouseUnderPos.Value.X - _offsetX)/_lineInterval;
                    if (lineIdx >= 0)
                    {
                        DrawLineUnderCursorInfo(ref g, lineIdx);
                    }
                }

                DrawLinesInfo(ref g);

                g.ResetClip();
            }

            private void DrawLineUnderCursorInfo(ref Graphics g, int lineIdx)
            {
                List<Line> linesToDraw = new List<Line>
                    (
                    _lines.Where(l => lineIdx < l.MagnitudeList.Count)
                    );
                if (linesToDraw.Count > 0)
                {
                    int linePosX = _offsetX + (lineIdx*_lineInterval);
                    linePosX -= linesToDraw.Count/2;
                    for (int i = 0; i < linesToDraw.Count; i++)
                    {
                        Line line = linesToDraw[i];
                        float val = line.MagnitudeList[lineIdx];
                        float endY = Height - (val*Height/(_maxPeek - _minPeek)) - line.Thickness;
                        g.DrawLine(line.PenThin, linePosX + i, 0, linePosX, endY);

                        string sVal = string.Format("{0} {1}", val, _units).TrimEnd();
                        SizeF textSize = g.MeasureString(sVal, Font);
                        int textPosX = (int) (linePosX + textSize.Width >= Width
                            ? linePosX - textSize.Width
                            : linePosX + 1);
                        int textPosY = textPosX + textSize.Width > Width - _maxInfoRectWidth
                            ? _maxInfoRectHeight
                            : 0;
                        g.DrawString(sVal, Font, line.Brush, textPosX, textPosY + textSize.Height*i + i + 1);
                    }
                }
            }

            private int _maxInfoRectWidth;
            private int _maxInfoRectHeight;
            private void DrawLinesInfo(ref Graphics g)
            {
                Line[] lines = _lines.Where(l => l.MagnitudeList.Count > 0).ToArray();
                int cnt = lines.Length;
                if (cnt > 0)
                {
                    SmoothingMode oldSmoothing = g.SmoothingMode;
                    g.SmoothingMode = SmoothingMode.HighSpeed;

                    int maxStrLen = 0;
                    foreach (Line line in lines)
                    {
                        float avgVal = line.MagnitudeList.Average();
                        line.TagText = string.Format("Avg: {0} {1}", avgVal.ToString("F"), _units).TrimEnd();
                        line.TagTextSize = g.MeasureString(line.TagText, Font);
                        maxStrLen = (int) Math.Max(maxStrLen, line.TagTextSize.Width);
                    }

                    const int iconSize = 16;
                    _maxInfoRectWidth = Math.Max(_maxInfoRectWidth, maxStrLen + iconSize + 4);
                    int x = Width - _maxInfoRectWidth;
                    _maxInfoRectHeight = Math.Max(_maxInfoRectHeight, cnt * iconSize + cnt * 2 - (cnt-2));
                    Rectangle r = new Rectangle(x, -1, _maxInfoRectWidth, _maxInfoRectHeight);
                    g.FillRectangle(_backBrush, r);
                    g.DrawRectangle(_borderPen, r);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        Line line = lines[i];
                        r = new Rectangle(x + 2, iconSize*i + i + 1, iconSize, iconSize);
                        g.FillRectangle(line.Brush, r);
                        g.DrawString(line.TagText, Font, _textBrush, x + iconSize + 4,
                            iconSize*i + ((iconSize - line.TagTextSize.Height)/2) + i + 2);
                    }

                    g.SmoothingMode = oldSmoothing;
                }
            }

            public void UpdateGraph()
            {
                int greatestMCount = 0;
                foreach (Line line in _lines)
                {
                    if (greatestMCount < line.MagnitudeList.Count)
                    {
                        greatestMCount = line.MagnitudeList.Count;
                    }
                }
                CullAndEqualizeMagnitudeCounts();
                Invalidate(false);
            }

            protected void CalculateMaxPushPoints()
            {
                int areaWidth = Width - _offsetX;
                _maxCoords = (areaWidth/_lineInterval) + (areaWidth%_lineInterval != 0 ? _lineInterval / 2 : 0);

                if (_maxCoords <= 0)
                {
                    _maxCoords = 1;
                }
            }

            protected void DrawLabels(ref Graphics g)
            {
                SizeF maxSize = g.MeasureString(_maxLabel, Font);
                SizeF minSize = g.MeasureString(_minLabel, Font);

                int textWidth = (int) ((maxSize.Width > minSize.Width) ? maxSize.Width : minSize.Width) + 2;

                g.DrawString(_maxLabel, Font, _textBrush, (float) textWidth/2 - (maxSize.Width/2) + 1, 1);
                g.DrawString(_minLabel, Font, _textBrush, (float) textWidth/2 - (minSize.Width/2) + 1,
                    Height - minSize.Height + 1);

                g.DrawLine(_borderPen, textWidth, 0, textWidth, Height);

                _offsetX = textWidth + 1;
            }

            protected void RefreshLabels()
            {
                if (!_bMinLabelSet)
                {
                    _minLabel = _minPeek.ToString();
                }
                if (!_bMaxLabelSet)
                {
                    _maxLabel = _maxPeek.ToString();
                }
            }

            protected void DrawGrid(ref Graphics g)
            {
                for (int n = Height - _gridSize; n >= 0; n -= _gridSize)
                {
                    g.DrawLine(_gridPen, _offsetX, n, Width, n);
                }
            }

            private void CullAndEqualizeMagnitudeCounts()
            {
                if (_maxCoords == -1)
                {
                    CalculateMaxPushPoints();
                }

                int greatestMCount = 0;
                foreach (Line line in _lines)
                {
                    if (greatestMCount < line.MagnitudeList.Count)
                    {
                        greatestMCount = line.MagnitudeList.Count;
                    }
                }

                if (greatestMCount == 0)
                {
                    return;
                }

                foreach (Line line in _lines)
                {
                    if (line.MagnitudeList.Count == 0)
                    {
                        line.MagnitudeList.Add(_minPeek);
                    }

                    while (line.MagnitudeList.Count < greatestMCount)
                    {
                        line.MagnitudeList.Add(line.MagnitudeList[line.MagnitudeList.Count - 1]);
                    }

                    int cullsRequired = (line.MagnitudeList.Count - _maxCoords) + 1;
                    if (cullsRequired > 0)
                    {
                        line.MagnitudeList.RemoveRange(0, cullsRequired);
                    }
                }
            }

            protected void DrawLines(ref Graphics g)
            {
                foreach (Line line in _lines)
                {
                    if (line.MagnitudeList.Count == 0)
                    {
                        return;
                    }

                    if (!line.Visible)
                    {
                        continue;
                    }

                    PointF lastPoint = new PointF();
                    lastPoint.X = _offsetX;
                    lastPoint.Y = Height - (line.MagnitudeList[0]*Height/(_maxPeek - _minPeek));

                    for (int n = 0; n < line.MagnitudeList.Count; ++n)
                    {
                        if (line.ShowAsBar)
                        {
                            RectangleF barRect = new RectangleF();
                            PointF p = barRect.Location;
                            p.X = _offsetX + (n*_lineInterval) + 1;
                            p.Y = Height - ((line.MagnitudeList[n]*Height)/(_maxPeek - _minPeek));
                            barRect.Location = p;
                            barRect.Width = _lineInterval - 1;
                            barRect.Height = Height;
                            DrawBar(barRect, line, ref g);
                        }
                        else
                        {
                            float newX = _offsetX + (n*_lineInterval);
                            float newY = Height - ((line.MagnitudeList[n]*Height)/(_maxPeek - _minPeek)) - 1;
                            g.DrawLine(line.Pen, lastPoint.X, lastPoint.Y, newX, newY);
                            lastPoint = new PointF(newX, newY);
                        }
                    }
                }
            }

            private void DrawBar(RectangleF rect, Line line, ref Graphics g)
            {
                g.FillRectangle(line.Brush, rect);
            }

            public LineHandle GetLineHandle(int numID)
            {
                Object line = GetLine(numID);
                return (line != null ? new LineHandle(ref line, this) : null);
            }

            private Line GetLine(int numID)
            {
                foreach (Line line in _lines)
                {
                    if (numID == line.NumID)
                    {
                        return line;
                    }
                }
                return null;
            }

            public bool LineExists(int numID)
            {
                return GetLine(numID) != null;
            }

            public LineHandle AddLine(int numID, Color clr)
            {
                if (LineExists(numID))
                {
                    return null;
                }

                Line line = new Line(numID);
                line.Color = clr;

                _lines.Add(line);
                Object objLine = line;
                return (new LineHandle(ref objLine, this));
            }

            public bool RemoveLine(int numID)
            {
                Line line = GetLine(numID);
                if (line == null)
                {
                    return false;
                }

                return _lines.Remove(line);
            }


            public bool Push(float magnitude, int numID)
            {
                Line line = GetLine(numID);
                if (line == null)
                {
                    return false;
                }

                return PushDirect(magnitude, line);
            }

            private bool PushDirect(float magnitude, Line line)
            {
                if (!_bAutoScale && magnitude > _maxPeek)
                {
                    magnitude = _maxPeek;
                }
                else if (_bAutoScale && magnitude > _maxPeek)
                {
                    _maxPeek = magnitude;
                    RefreshLabels();
                }
                else if (!_bAutoScale && magnitude < _minPeek)
                {
                    magnitude = _minPeek;
                }
                else if (_bAutoScale && magnitude < _minPeek)
                {
                    _minPeek = magnitude;
                    RefreshLabels();
                }

                magnitude -= _minPeek;

                line.MagnitudeList.Add(magnitude);
                return true;
            }

            private Point? _mouseUnderPos;
            private void C2DPushGraph_MouseLeave(object sender, EventArgs e)
            {
                _mouseUnderPos = null;
                Invalidate(false);
            }

            private void C2DPushGraph_MouseMove(object sender, MouseEventArgs e)
            {
                _mouseUnderPos = e.Location;
                Invalidate(false);
            }
        }
    }
}