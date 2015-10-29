// ===================================================================
// C2DPushGraph Control
// -------------------------------------------------------------------
// Author: Stuart D. Konen
// E-mail: skonen _|a.t|_ gmail.com
// Date of Release: December 2nd, 2006 
// ===================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RK.Server.Controls
{
    namespace Graphing
    {
        public class C2DPushGraph : Control
        {
            // ===================================================================
            // PUBLIC LINEHANDLE CLASS 
            // (Provides public method access to line data members)
            // ===================================================================

            public class LineHandle
            {
                private Line _line;
                private C2DPushGraph _owner;

                // ===================================================================

                public LineHandle(ref Object line, C2DPushGraph owner)
                {
                    /* A small hack to get around the compiler error CS0051: */
                    if (string.Compare(line.GetType().Name, "Line") != 0)
                    {
                        throw new ArithmeticException(
                            "LineHandle: First Parameter must be " +
                            "type of 'Line' cast to base 'Object'");
                    }

                    _line = (Line)line;
                    _owner = owner;
                }

                // ===================================================================

                /// <summary> 
                /// Clears any currently displayed magnitudes.
                /// </summary>

                public void Clear()
                {
                    _line.MagnitudeList.Clear();
                    _owner.UpdateGraph();                    
                }
                
                // ===================================================================

                /// <summary> 
                /// Sets or gets the line's current color.
                /// </summary>

                public Color Color
                {
                    set
                    {
                        if (_line.Color != value)
                        {
                            _line.Color = value;
                            _owner.Refresh();
                        }
                    }
                    get { return _line.Color; }

                }

                // ===================================================================

                /// <summary> 
                /// Sets or gets the line's thickness in pixels. NOTE: It is advisable
                /// to set HighQuality to false if using a thickness greater than
                /// 2 pixels as the antialiasing creates imperfections.
                /// </summary>

                public uint Thickness
                {
                    set
                    {
                        if (_line.Thickness != value)
                        {
                            _line.Thickness = value;
                            _owner.Refresh();
                        }
                    }
                    get { return _line.Thickness; }
                }

                // ===================================================================

                /// <summary> 
                /// Gets or sets a value indicating whether the line is visible.
                /// </summary>

                public bool Visible
                {
                    set
                    {
                        if (_line.Visible != value)
                        {
                            _line.Visible = value;
                            _owner.Refresh();
                        }
                    }
                    get { return _line.Visible; }
                }

                // ===================================================================

                /// <summary> 
                /// Gets or sets a value indicating whether this line's magnitudes are
                /// displayed in a bar graph style.
                /// </summary>

                public bool ShowAsBar
                {
                    set
                    {
                        if (_line.ShowAsBar != value)
                        {
                            _line.ShowAsBar = value;
                            _owner.Refresh();
                        }
                    }
                    get { return _line.ShowAsBar; }
                }
            }

            // ===================================================================
            // PRIVATE LINE CLASS (Contains Line Data Members)
            // ===================================================================

            private class Line
            {
                public List<int> MagnitudeList = new List<int>();
                public Color  Color = Color.Green;
                public string NameID = "";
                public int    NumID = -1;
                public uint   Thickness = 1;
                public bool   ShowAsBar;
                public bool   Visible = true;

                // ===================================================================

                public Line(string name)
                {
                    NameID = name;
                }

                // ===================================================================

                public Line(int num)
                {
                    NumID = num;
                }
            }


            // ===================================================================
            // MAIN CONTROL CLASS
            // ===================================================================

            private Color  _textColor = Color.Yellow;
            private Color  _gridColor = Color.Green;
            private string _maxLabel = "Max";
            private string _minLabel = "Minimum";
            private bool   _bHighQuality = true;
            private bool   _bAutoScale;
            private bool   _bMinLabelSet;
            private bool   _bMaxLabelSet;
            private bool   _bShowMinMax = true;
            private bool   _bShowGrid = true;
            private int    _moveOffset;
            private int    _maxCoords = -1;
            private int    _lineInterval = 5;
            private int    _maxPeek = 100;
            private int    _minPeek;
            private int    _gridSize = 15;
            private int    _offsetX;

            private List<Line> _lines = new List<Line>();
            private System.ComponentModel.IContainer components;
            
            // ===================================================================

            public C2DPushGraph()
            {                
                InitializeComponent();
                InitializeStyles();
            }

            // ===================================================================

            public C2DPushGraph(Form parent)
            {
                parent.Controls.Add(this);

                InitializeComponent();
                InitializeStyles();
            }

            // ===================================================================

            public C2DPushGraph(Form parent, Rectangle rectPos)
            {
                parent.Controls.Add(this);

                Location = rectPos.Location;
                Height = rectPos.Height;
                Width = rectPos.Width;

                InitializeComponent();
                InitializeStyles();
            }

            // ===================================================================
                   
            /// <summary> 
            /// Clean up any resources being used.
            /// </summary>
            /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>

            protected override void Dispose(bool disposing)
            {                
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }

            #region Component Designer generated code

            // ===================================================================

            /// <summary> 
            /// Required method for Designer support - do not modify 
            /// the contents of this method with the code editor.
            /// </summary>

            private void InitializeComponent()
            {
                components = new System.ComponentModel.Container();
            }

            #endregion

            // ===================================================================

            private void InitializeStyles()
            {
                BackColor = Color.Black;

                /* Enable double buffering and similiar techniques to 
                 * eliminate flicker */

                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                SetStyle(ControlStyles.UserPaint, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                DoubleBuffered = true;

                SetStyle(ControlStyles.ResizeRedraw, true);
            }

            // ===================================================================

            /// <summary> 
            /// Gets or sets the color of any text displayed in the graph (labels).
            /// </summary>

            public Color TextColor
            {
                set
                {
                    if (_textColor != value)
                    {
                        _textColor = value;
                        Refresh();
                    }
                }
                get { return _textColor; }
            }

            // ===================================================================

            /// <summary> 
            /// Gets or sets the graph's grid color.
            /// </summary>

            public Color GridColor
            {
                set
                {
                    if (_gridColor != value)
                    {
                        _gridColor = value;
                        Refresh();
                    }
                }
                get { return _gridColor; }
            }

            // ===================================================================

            /// <summary> 
            /// Gets or sets the number of pixels between each displayed magnitude.        
            /// </summary>

            public ushort LineInterval
            {
                set
                {
                    if ((ushort)_lineInterval != value)
                    {
                        _lineInterval = value;
                        _maxCoords = -1; // Recalculate
                        Refresh();
                    }
                }
                get { return (ushort)_lineInterval; }
            }

            // ===================================================================

            /// <summary> 
            /// Gets or sets the string to display as the graph's 'maximum label'.
            /// </summary>

            public string MaxLabel
            {
                set
                {
                    _bMaxLabelSet = true;

                    if (string.Compare(_maxLabel, value) != 0)
                    {
                        _maxLabel = value;
                        _maxCoords = -1; // Recalculate
                        Refresh();
                    }
                }
                get { return _maxLabel; }
            }

            // ===================================================================

            /// <summary> 
            /// Gets or sets the string to display as the graph's 'minimum label'.
            /// </summary>

            public string MinLabel
            {
                set
                {
                    _bMinLabelSet = true;

                    if (string.Compare(_minLabel, value) != 0)
                    {
                        _minLabel = value;
                        _maxCoords = -1; // Recalculate
                        Refresh();
                    }
                }
                get { return _minLabel; }
            }

            // ===================================================================

            /// <summary> 
            /// Gets or sets the width/height (in pixels) of each square in
            /// the graph's grid.
            /// </summary>

            public ushort GridSize
            {
                set
                {
                    if (_gridSize != value)
                    {
                        _gridSize = value;
                        Refresh();

                    }
                }
                get { return (ushort)_gridSize; }
            }

            // ===================================================================

            /// <summary> 
            /// Gets or sets the maximum peek magnitude of the graph, which should be
            /// the largest value you could potentially push to the graph. NOTE: If you
            /// have set AutoScale to true, this value will automatically adjust to
            /// the highest magnitude pushed to the graph.
            /// </summary>

            public int MaxPeekMagnitude
            {
                set 
                { 
                    _maxPeek = value;
                    RefreshLabels(); 
                }
                get { return _maxPeek; }
            }

            // ===================================================================

            /// <summary> 
            /// Gets or sets the minimum magnitude of the graph, which should be
            /// the smallest value you could potentially push to the graph.
            /// NOTE: If you have set AutoScale to true, this value will 
            /// automatically adjust to the lowest magnitude pushed to the graph.
            /// </summary>

            public int MinPeekMagnitude
            {
                set 
                { 
                    _minPeek = value; 
                    RefreshLabels(); 
                }
                get { return _minPeek; }
            }


            // ===================================================================

            /// <summary> 
            /// Gets or sets the value indicating whether the graph automatically
            /// adjusts MinPeekMagnitude and MaxPeekMagnitude to the lowest and highest
            /// values pushed to the graph.
            /// </summary>

            public bool AutoAdjustPeek
            {
                set
                {
                    if (_bAutoScale != value)
                    {
                        _bAutoScale = value;
                        Refresh();
                    }
                }
                get { return _bAutoScale; }
            }

            // ===================================================================

            /// <summary> 
            /// Gets or sets the value indicating whether the graph is rendered in
            /// 'high quality' mode (with antialiasing). It is suggested that this property
            /// be set to false if you intend to display your graph using bar graph 
            /// styles, line thickness greater than two, or if maximum performance 
            /// is absolutely crucial.
            /// </summary>

            public bool HighQuality
            {
                set
                {
                    if (value != _bHighQuality)
                    {
                        _bHighQuality = value;
                        Refresh(); // Force redraw
                    }
                }
                get { return _bHighQuality; }
            }

            // ===================================================================

            /// <summary> 
            /// Gets or sets the value indicating whether the mimimum and maximum labels
            /// are displayed.
            /// </summary>

            public bool ShowLabels
            {
                set
                {
                    if (_bShowMinMax != value)
                    {
                        _bShowMinMax = value;

                        /* We're going to need to recalculate our maximum 
                         * coordinates since our graphable width changed */
                        _maxCoords = -1;

                        Refresh();
                    }
                }
                get { return _bShowMinMax; }
            }

            // ===================================================================

            /// <summary> 
            /// Gets or sets the value indicating whether the graph's grid is 
            /// displayed.
            /// </summary>

            public bool ShowGrid
            {
                set
                {
                    if (_bShowGrid != value)
                    {
                        _bShowGrid = value;
                        Refresh();
                    }
                }
                get { return _bShowGrid; }
            }

            // ===================================================================

            protected override void OnSizeChanged(EventArgs e)
            {
                /* We're going to need to recalculate our maximum 
                 * coordinates since our graphable width changed */
                _maxCoords = -1;

                Refresh();

                base.OnSizeChanged(e); 
            }

            // ===================================================================

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;// CreateGraphics();

                SmoothingMode prevSmoothingMode = g.SmoothingMode;
                g.SmoothingMode = (_bHighQuality ? SmoothingMode.HighQuality
                                                  : SmoothingMode.Default);

                /* Reset our offset so we don't continually shift to the right: */

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
                    /* This is to avoid crossing the left grid boundary when 
                     * working with lines with great thickness */

                    g.Clip = new Region(
                        new Rectangle(_offsetX, 0, Width - _offsetX, Height));
                }

                DrawLines(ref g);
                g.ResetClip();

                g.SmoothingMode = prevSmoothingMode;
            }

            // ===================================================================

            /// <summary> 
            /// This function is to be called after you have pushed new magnitude(s)
            /// to the graph's lines and want the control re-rendered to take the
            /// changes into account.
            /// </summary>

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

                if (greatestMCount >= _maxCoords)
                {
                    _moveOffset =
                        (_moveOffset - (((greatestMCount - _maxCoords) + 1) * _lineInterval))
                        % _gridSize;
                }

                CullAndEqualizeMagnitudeCounts();
                Refresh();
            }

            // ===================================================================
            
            protected void CalculateMaxPushPoints()
            {
                /* Calculate the maximum of push points (magnitudes) that can be 
                 * drawn for the graphable display width: */

                _maxCoords = ((Width - _offsetX) / _lineInterval) + 2
                                   + (((Width - _offsetX) % _lineInterval) != 0 ? 1 : 0);

                if (_maxCoords <= 0)
                {
                    _maxCoords = 1;
                }
            }

            // ===================================================================

            protected void DrawLabels(ref Graphics g)
            {
                SizeF maxSize = g.MeasureString(_maxLabel, Font);
                SizeF minSize = g.MeasureString(_minLabel, Font);

                int textWidth = (int)((maxSize.Width > minSize.Width)
                                ? maxSize.Width
                                : minSize.Width) + 6;

                SolidBrush textBrush = new SolidBrush(_textColor);


                /* Draw the labels (max: Top) (min: Bottom) */

                g.DrawString(_maxLabel, Font, textBrush,
                              textWidth / 2 - (maxSize.Width / 2),
                              2);

                g.DrawString(_minLabel, Font, textBrush,
                              textWidth / 2 - (minSize.Width / 2),
                              Height - minSize.Height - 2);

                textBrush.Dispose();


                /* Draw the bordering line */

                Pen borderPen = new Pen(_gridColor, 1);
                g.DrawLine(borderPen, textWidth + 6, 0, textWidth + 6, Height);

                borderPen.Dispose();

                /* Update the offset so we don't draw the graph over the labels */
                _offsetX = textWidth + 6;
            }

            // ===================================================================

            protected void RefreshLabels()
            {
                /* Within this function we ensure our labels are up to date
                 * if the user isn't using custom labels. It is called whenever
                 * the graph's range changes. */

                if (!_bMinLabelSet)
                {
                    /* Use the minimum magnitude as the label since the
                     * user has not yet assigned a custom label: */

                    _minLabel = _minPeek.ToString();
                }

                if (!_bMaxLabelSet)
                {
                    /* Use the maximum magnitude as the label since the
                     * user has not yet assigned a custom label: */

                    _maxLabel = _maxPeek.ToString();
                }
            }

            // ===================================================================

            protected void DrawGrid(ref Graphics g)
            {
                Pen gridPen = new Pen(_gridColor, 1);
                for (int n = Height - 1; n >= 0; n -= _gridSize)
                {
                    g.DrawLine(gridPen, _offsetX, n, Width, n);
                }

                for (int n = _offsetX + _moveOffset; n < Width; n += _gridSize)
                {
                    if (n < _offsetX)
                    {
                        continue;
                    }

                    g.DrawLine(gridPen, n, 0, n, Height);
                }

                gridPen.Dispose();
            }

            // ===================================================================

            private void CullAndEqualizeMagnitudeCounts()
            {
                if (_maxCoords == -1)
                {
                    /* Maximum push points not yet calculated */
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
                    return; // No magnitudes
                }

                foreach (Line line in _lines)
                {
                    /* If the line has less push points than the line with the greatest
                    number of push points, new push points are appended with
                    the same magnitude as the previous push point. If no push points
                    exist for the line, one is added with the least magnitude possible. */

                    if (line.MagnitudeList.Count == 0)
                    {
                        line.MagnitudeList.Add(_minPeek);
                    }

                    while (line.MagnitudeList.Count < greatestMCount)
                    {
                        line.MagnitudeList.Add(
                            line.MagnitudeList[line.MagnitudeList.Count - 1]);
                    }

                    int cullsRequired = (line.MagnitudeList.Count - _maxCoords) + 1;
                    if (cullsRequired > 0)
                    {
                        line.MagnitudeList.RemoveRange(0, cullsRequired);
                    }
                }
            }

            // ===================================================================

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
                    
                    /* Now prepare to draw the line or bar */

                    Pen linePen = new Pen(line.Color, line.Thickness);

                    Point lastPoint = new Point();
                    lastPoint.X = _offsetX;
                    lastPoint.Y = Height - ((line.MagnitudeList[0] *
                       Height) / (_maxPeek - _minPeek));

                    for (int n = 0; n < line.MagnitudeList.Count; ++n)
                    {
                        if (line.ShowAsBar)
                        {
                            /* The line is set to be shown as a bar graph, so
                            first we get the bars rectangle, then draw the bar */

                            Rectangle barRect = new Rectangle();

                            // Weird hack because BarRect.Location.* causes error
                            Point p = barRect.Location;
                            p.X = _offsetX + (n * _lineInterval) + 1;
                            p.Y = Height - ((line.MagnitudeList[n] * Height) /
                                                (_maxPeek - _minPeek));
                            barRect.Location = p;

                            barRect.Width = _lineInterval - 1;
                            barRect.Height = Height;

                            DrawBar(barRect, line, ref g);
                        }
                        else
                        {
                            /* Draw a line */

                            int newX = _offsetX + (n * _lineInterval);
                            int newY = Height - ((line.MagnitudeList[n] * Height) /
                                (_maxPeek - _minPeek));

                            g.DrawLine(linePen, lastPoint.X, lastPoint.Y, newX, newY);

                            lastPoint.X = newX;
                            lastPoint.Y = newY;
                        }
                    }

                    linePen.Dispose();
                }
            }
            
            // ===================================================================

            private void DrawBar(Rectangle rect, Line line, ref Graphics g)
            {
                SolidBrush barBrush = new SolidBrush(line.Color);
                g.FillRectangle(barBrush, rect);
                barBrush.Dispose();
            }

            // ===================================================================

            /// <summary> 
            /// Returns a new line handle (LineHandle object) to the line 
            /// with the matching numerical ID. Returns NULL if a line with a 
            /// matching ID is not found.
            /// </summary>
            /// <param name="numID">
            /// The numerical ID of the line you wish to get a handle to.
            /// </param>

            public LineHandle GetLineHandle(int numID)
            {
                Object line = GetLine(numID);
                return (line != null ? new LineHandle(ref line, this) : null);
            }

            // ===================================================================

            /// <summary> 
            /// Returns a new line handle (LineHandle object) to the line 
            /// with the matching name (case insensitive). Returns NULL if a 
            /// line with a matching name is not found.
            /// </summary>
            /// <param name="nameID">
            /// The case insensitive name of the line you wish to get a handle to.
            /// </param>

            public LineHandle GetLineHandle(string nameID)
            {
                Object line = GetLine(nameID);
                return (line != null ? new LineHandle(ref line, this) : null);
            }

            // ===================================================================

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

            // ===================================================================

            private Line GetLine(string nameID)
            {
                foreach (Line line in _lines)
                {
                    if (string.Compare(nameID, line.NameID, true) == 0)
                    {
                        return line;
                    }
                }
                return null;
            }

            // ===================================================================

            /// <summary> 
            /// Returns true if a line exists with an identification number mathing 
            /// the passed value. Returns false if no match is found.
            /// </summary>
            /// <param name="numID">
            /// The case numerical ID of the line you wish to check the existence
            /// of.
            /// </param>
            
            public bool LineExists(int numID)
            {
                return GetLine(numID) != null;
            }

            // ===================================================================

            /// <summary> 
            /// Returns true if a line exists with a name that case insensitively
            /// matches the passes name. Returns false if no match is found.
            /// </summary>
            /// <param name="nameID">
            /// The case insensitive name of the line you wish to check the existence
            /// of.
            /// </param>

            public bool LineExists(string nameID)
            {
                return GetLine(nameID) != null;
            }

            // ===================================================================

            /// <summary> 
            /// Adds a new line using the passed name as an identifier and sets
            /// the line's initial color to the passed color. If successful, returns
            /// a handle to the new line.
            /// </summary>
            /// <param name="nameID">
            /// A case insensitive name for the line you wish to create.
            /// </param>
            /// <param name="clr">
            /// The line's initial color.
            /// </param>

            public LineHandle AddLine(string nameID, Color clr)
            {
                if (LineExists(nameID))
                {
                    return null;
                }

                Line line = new Line(nameID);
                line.Color = clr;

                _lines.Add(line);

                Object objLine = line;
                return (new LineHandle(ref objLine, this));
            }

            // ===================================================================
            // I strongly suggest that you use this method when performance is critical

            /// <summary> 
            /// Adds a new line using the passed numeric ID as an identifier and sets
            /// the line's initial color to the passed color. If successful, returns
            /// a handle to the new line.
            /// </summary>
            /// <param name="numID">
            /// A unique numerical for the line you wish to create.
            /// </param>
            /// <param name="clr">
            /// The line's initial color.
            /// </param>

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

            // ===================================================================
            
            /// <summary> 
            /// Removes a line by its name.
            /// </summary>
            /// <param name="nameID">
            /// The line's case-insensitive name.
            /// </param>  

            public bool RemoveLine(string nameID)
            {
                Line line = GetLine(nameID);
                if (line == null)
                {
                    return false;
                }

                return _lines.Remove(line);
            }

            // ===================================================================
            
            /// <summary> 
            /// Removes a line by its numerical ID.
            /// </summary>
            /// <param name="numID">
            /// The line's numerical ID.
            /// </param>  

            public bool RemoveLine(int numID)
            {
                Line line = GetLine(numID);
                if (line == null)
                {
                    return false;
                }

                return _lines.Remove(line);
            }

            // ===================================================================       

            /// <summary> 
            /// Pushes a new magnitude (point) to the line with the passed name.
            /// </summary>
            /// <param name="magnitude">
            /// The magnitude of the new point.
            /// </param>  
            /// <param name="nameID">
            /// The line's case-insensitive name.
            /// </param>  

            public bool Push(int magnitude, string nameID)
            {
                Line line = GetLine(nameID);
                if (line == null)
                {
                    return false;
                }

                return PushDirect(magnitude, line); 
            }

            // ===================================================================       

            /// <summary> 
            /// Pushes a new magnitude (point) to the line with the passed 
            /// numerical ID.
            /// </summary>
            /// <param name="magnitude">
            /// The magnitude of the new point.
            /// </param>  
            /// <param name="numID">
            /// The line's numerical ID.
            /// </param>  

            public bool Push(int magnitude, int numID)
            {
                Line line = GetLine(numID);
                if (line == null)
                {
                    return false;
                }

                return PushDirect(magnitude, line);
            }
            
            // ===================================================================       

            private bool PushDirect(int magnitude, Line line)
            {
                /* Now add the magnitude (push point) to the array of push points, but
                   first restrict it to the peek bounds */

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

        }
    }
}
