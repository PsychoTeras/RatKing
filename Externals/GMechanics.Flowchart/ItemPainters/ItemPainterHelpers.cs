using System.Drawing;
using System.Drawing.Drawing2D;

namespace FlowchartControl.ItemPainters
{
    internal static class ItemPainterHelpers
    {
        public static void DrawRoundedRectangleShadow(Graphics graphics,
            ref Rectangle rectangle, int cornerRadius, int shadowDistance, 
            Color shadowColor)
        {
            Rectangle shadowRectangle = new Rectangle(rectangle.Location,
                rectangle.Size);
            shadowRectangle.Inflate(-shadowDistance, -shadowDistance);
            shadowRectangle.Offset(shadowDistance - 4, shadowDistance - 3);
            GraphicsPath shadowPath = CalculateRoundedRectangleGraphicsPath(
                shadowRectangle, cornerRadius + shadowDistance);
            DrawShadow(graphics, shadowPath, shadowColor);

            rectangle.Inflate(-shadowDistance, -shadowDistance);
            rectangle.Offset(-shadowDistance, -shadowDistance);
        }

        private static void DrawShadow(Graphics graphics, GraphicsPath graphicsPath, 
            Color shadowColor)
        {
            using (PathGradientBrush brush = new PathGradientBrush(graphicsPath))
            {
                ColorBlend colorBlend = new ColorBlend(3);
                colorBlend.Colors = new[] { Color.Transparent,shadowColor,
                                            Color.Transparent };
                colorBlend.Positions = new[] { 0f, .08f, 1f };
                brush.InterpolationColors = colorBlend;
                brush.RotateTransform(-0.15f);
                graphics.FillPath(brush, graphicsPath);
            }
        }

        public static GraphicsPath CalculateRoundedRectangleGraphicsPath(Rectangle bounds,
            int cornerRadius)
        {
            GraphicsPath gfxPath = new GraphicsPath();
            gfxPath.AddArc(bounds.X, bounds.Y, cornerRadius, cornerRadius, 180, 90);
            gfxPath.AddArc(bounds.X + bounds.Width - cornerRadius, bounds.Y, cornerRadius,
                           cornerRadius, 270, 90);
            gfxPath.AddArc(bounds.X + bounds.Width - cornerRadius, bounds.Y + bounds.Height -
                           cornerRadius, cornerRadius, cornerRadius, 0, 90);
            gfxPath.AddArc(bounds.X, bounds.Y + bounds.Height - cornerRadius, cornerRadius,
                           cornerRadius, 90, 90);
            gfxPath.CloseAllFigures();
            return gfxPath;
        }

        public static void DrawRoundedRectangle(Graphics gfx, Rectangle bounds, int cornerRadius, 
                                                Pen pen, Brush fillBrush)
        {
            int strokeOffset = pen == null ? 0 : (int)pen.Width;
            bounds = Rectangle.Inflate(bounds, -strokeOffset, -strokeOffset);

            GraphicsPath gfxPath = CalculateRoundedRectangleGraphicsPath(bounds, cornerRadius);

            if (fillBrush != null)
            {
                gfx.FillPath(fillBrush, gfxPath);
            }

            if (pen != null)
            {
                pen.EndCap = pen.StartCap = LineCap.Round;
                gfx.DrawPath(pen, gfxPath);
            }
        }
    }
}
