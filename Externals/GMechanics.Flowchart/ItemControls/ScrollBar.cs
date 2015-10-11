using System;
using System.Drawing;

namespace FlowchartControl.ItemControls
{
    internal class ScrollBar
    {

#region Private members

        private FlowchartItem _item;
        private int _minValue;
        private int _maxValue;
        private int _position;
        private byte _dotSize;
        private Rectangle _rectangle;
        private int _width;

#endregion

#region Properties

        public int MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        public int MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        public int Position
        {
            get { return _position; }
            set
            {
                int position = value >= _position
                                   ? Math.Min(value, _maxValue)
                                   : Math.Max(value, _minValue);
                if (position != _position)
                {
                    _position = position;
                    _item.Repaint();
                }
            }
        }

        public byte DotSize
        {
            get { return _dotSize; }
            set { _dotSize = value; }
        }

        public Rectangle ClientRectangle
        {
            get { return _rectangle; }
            set { _rectangle = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

#endregion

#region Class functions

        public ScrollBar(FlowchartItem item)
        {
            _width = 16;
            _minValue = 0;
            _maxValue = 100;
            _item = item;
        }

        public void SetMaxValueAndRecalculateValue(int maxValue)
        {
            _maxValue = maxValue;
        }

        private void DrawBody(Graphics graphics, Color bodyColor)
        {
            
        }

        private void DrawUpDownArrows(Graphics graphics, Color upDownArrowColor)
        {
            
        }

        private void DrawDot(Graphics graphics, Color dotColor)
        {
            
        }

        public void Draw(Graphics graphics, Color bodyColor, Color upDownArrowColor, 
                         Color dotColor)
        {
            DrawBody(graphics, bodyColor);
            DrawUpDownArrows(graphics, upDownArrowColor);
            DrawDot(graphics, dotColor);
        }

#endregion

    }
}
