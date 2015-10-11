using System.Drawing;
using System.Drawing.Drawing2D;

namespace GMechanics.FlowchartDemo.FlowchartControl.ItemPainters
{
    internal class ItemPainter_BlueGradient : ItemPainter_Base
    {

#region Private members

        private readonly Color _frameColor = Color.FromArgb(117, 135, 149);
        private readonly Color _topColor = Color.FromArgb(218, 232, 241);
        private readonly Color _bottomColor = Color.FromArgb(137, 177, 206);
        private readonly Color _headerTextColor = Color.FromArgb(220, 0, 0, 0);

        private readonly Brush _solidBrush;
        private readonly Pen _outerFramePen;
        private readonly Pen _innerFramePen;
        private readonly Pen _headerLinePen;
        private readonly Brush _headerTextBrush;

#endregion

#region Class functions

        public ItemPainter_BlueGradient()
        {
            _headerTextBrush = new SolidBrush(_headerTextColor);
            _headerLinePen = new Pen(Color.FromArgb(150, _frameColor), 1);
            _innerFramePen = new Pen(Color.FromArgb(100, _topColor), 1);
            _outerFramePen = new Pen(_frameColor, 1);
            _solidBrush = new SolidBrush(_topColor);
        }

        public override string Name
        {
            get { return "Blue gradient"; }
        }

        public override void Dispose()
        {
            base.Dispose();
            _solidBrush.Dispose();
            _outerFramePen.Dispose();
            _innerFramePen.Dispose();
            _headerLinePen.Dispose();
            _headerTextBrush.Dispose();
        }

#endregion

#region Paint functions

        public override void Paint(FlowchartItem item)
        {
            DrawBody(item);
            DrawHeader(item);
        }

        private void DrawBody(FlowchartItem item)
        {
            Rectangle rectangle = item.ClientRectangle;
            ItemPainterHelpers.DrawRoundedRectangleShadow(item.BufferGraphics,
                ref rectangle, CornerRadius, ShadowDistance, ShadowColor);

            GraphicsPath = ItemPainterHelpers.CalculateRoundedRectangleGraphicsPath(
                rectangle, CornerRadius);

            ItemPainterHelpers.DrawRoundedRectangle(item.BufferGraphics, rectangle,
                CornerRadius, _outerFramePen, _solidBrush);

            rectangle.Inflate(-1, -1);
            using (Brush gradientBrush = new LinearGradientBrush(rectangle, _topColor,
                _bottomColor, 90, false))
            {
                ItemPainterHelpers.DrawRoundedRectangle(item.BufferGraphics, rectangle,
                    CornerRadius, null, gradientBrush);
            }

            ItemPainterHelpers.DrawRoundedRectangle(item.BufferGraphics, rectangle,
                CornerRadius, _innerFramePen, null);
        }

        private void DrawHeader(FlowchartItem item)
        {
            RectangleF bounds = GraphicsPath.GetBounds();
            RectangleF headerRectangle = new RectangleF(bounds.Left + 2, 
                bounds.Top, bounds.Left + bounds.Width - 3, bounds.Top + 
                HeaderHeight);

            item.BufferGraphics.DrawLine(_headerLinePen, headerRectangle.X, 
                headerRectangle.Y + headerRectangle.Height - 1, headerRectangle.X +
                headerRectangle.Width, headerRectangle.Y + headerRectangle.Height - 1);

            if (!string.IsNullOrEmpty(item.Caption))
            {
                item.BufferGraphics.DrawString(item.Caption, HeaderFont, 
                    _headerTextBrush, headerRectangle, HeaderFormat);
            }
        }

//        private void DrawGroup(FlowchartItem item, )

#endregion

    }
}
