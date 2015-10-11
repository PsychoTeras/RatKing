using System.Drawing;

namespace FlowchartControl.ItemScrollBar
{
    internal class ScrollBar
    {

#region Private members

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
            set { _position = value; }
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

        public ScrollBar()
        {
            _width = 16;
            _minValue = 0;
            _maxValue = 100;
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
