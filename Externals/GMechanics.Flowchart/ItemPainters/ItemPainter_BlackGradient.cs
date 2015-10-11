using System.Drawing;
using System.Drawing.Drawing2D;

namespace GMechanics.FlowchartDemo.FlowchartControl.ItemPainters
{
    internal class ItemPainter_BlackGradient : ItemPainter_Base
    {
        private readonly Color _frameColor = Color.FromArgb(0, 0, 0);
        private readonly Color _topColor = Color.FromArgb(124, 125, 124);
        private readonly Color _bottomColor = Color.FromArgb(16, 16, 16);

        private readonly Pen _outerFramePen;

        public ItemPainter_BlackGradient()
        {
            _outerFramePen = new Pen(_frameColor, 1);
        }

        protected override Color ShadowColor { get { return Color.FromArgb(255, 0, 0, 0); } }

        public override void Paint(FlowchartItem item)
        {
            Rectangle rectangle = item.ClientRectangle;
            ItemPainterHelpers.DrawRoundedRectangleShadow(item.BufferGraphics,
                ref rectangle, CornerRadius, ShadowDistance, ShadowColor);

            GraphicsPath = ItemPainterHelpers.CalculateRoundedRectangleGraphicsPath(
                rectangle, CornerRadius);

            using (Brush gradientBrush = new LinearGradientBrush(rectangle, _topColor,
                   _bottomColor, 90, false))
            {
                ItemPainterHelpers.DrawRoundedRectangle(item.BufferGraphics, rectangle,
                    CornerRadius, _outerFramePen, gradientBrush);
            }
        }

        public override string Name
        {
            get { return "Black gradient"; }
        }

        public override void Dispose()
        {
            base.Dispose();
            _outerFramePen.Dispose();
        }
    }
}
