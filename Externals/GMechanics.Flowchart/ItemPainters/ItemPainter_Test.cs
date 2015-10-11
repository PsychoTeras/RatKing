using System.Drawing;
using System.Drawing.Drawing2D;

namespace GMechanics.FlowchartDemo.FlowchartControl.ItemPainters
{
    internal class ItemPainter_Test : ItemPainter_Base
    {
        public override void Paint(FlowchartItem item)
        {
            Rectangle rectangle = item.ClientRectangle;
            ItemPainterHelpers.DrawRoundedRectangleShadow(item.BufferGraphics,
                ref rectangle, CornerRadius, ShadowDistance, ShadowColor);

            Pen pen = new Pen(Color.FromArgb(255, Color.Green), 5);
            rectangle.Inflate(-(int)pen.Width, -(int)pen.Width);
            GraphicsPath = ItemPainterHelpers.CalculateRoundedRectangleGraphicsPath(
                rectangle, CornerRadius);

            pen.EndCap = pen.StartCap = LineCap.Flat;
            item.BufferGraphics.DrawPath(pen, GraphicsPath);
        }

        public override string Name
        {
            get { return "Test"; }
        }
    }
}
