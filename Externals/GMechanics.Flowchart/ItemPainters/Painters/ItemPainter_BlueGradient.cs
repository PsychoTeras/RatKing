using System.Drawing;

namespace GMechanics.FlowchartControl.ItemPainters.Painters
{
    internal sealed class ItemPainter_BlueGradient : ItemPainter_BaseGradient
    {

#region Overrided style members

        protected override Color FrameColor { get { return Color.FromArgb(117, 135, 149); } }
        protected override Color FrameColorSelected { get { return Color.White; } }

        protected override Color TopColor { get { return Color.FromArgb(218, 232, 241); } }
        protected override Color BottomColor { get { return Color.FromArgb(137, 177, 206); } }
        protected override Color HeaderTextColor { get { return Color.FromArgb(220, 0, 0, 0); } }

        protected override Color WorkplaceBrushColor { get { return Color.FromArgb(10, 0, 0, 0); } }
        protected override Color WorkplacePenColor { get { return Color.FromArgb(30, 0, 0, 0); } }

        protected override Color ScrollBodyColor { get { return Color.FromArgb(180, 135, 135, 135); } }
        protected override Color ScrollFrontColor { get { return TopColor; } }
        protected override Color ScrollMarksColor { get { return ScrollBodyColor; } }

        protected override Color ButtonColor { get { return Color.FromArgb(150, 255, 255, 255); } }
        protected override Color ButtonCaptionColor { get { return Color.FromArgb(220, 0, 0, 0); } }

        protected override Color GroupPlusMinusFrameColor { get { return ScrollBodyColor; } }
        protected override Color GroupPlusMinusFillColor { get { return Color.FromArgb(150, 150, 150); } }
        protected override Color GroupPlusMinusMarkColor { get { return Color.FromArgb(255, 255, 255); } }

        protected override Color LeftLinkPointColor { get { return Color.FromArgb(144, 152, 158); } }
        protected override Color LeftLinkPointColorMouseOn { get { return Color.FromArgb(220, 220, 220); } }
        protected override Color LeftLinkPointColorSelected { get { return Color.FromArgb(255, 255, 255); } }

        protected override Color RightLinkPointColor { get { return LeftLinkPointColor; } }
        protected override Color RightLinkPointColorMouseOn { get { return LeftLinkPointColorMouseOn; } }
        protected override Color RightLinkPointColorSelected { get { return LeftLinkPointColorSelected; } }

#endregion

#region Class functions

        public override string Name
        {
            get { return "Blue gradient"; }
        }

#endregion

    }
}
