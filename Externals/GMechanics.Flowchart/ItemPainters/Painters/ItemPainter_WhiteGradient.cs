using System.Drawing;

namespace GMechanics.FlowchartControl.ItemPainters.Painters
{
    internal sealed class ItemPainter_WhiteGradient : ItemPainter_BaseGradient
    {

#region Overrided style members

        protected override Color FrameColor { get { return Color.FromArgb(169, 173, 143); } }
        protected override Color FrameColorSelected { get { return Color.FromArgb(255, 255, 255); } }

        protected override Color TopColor { get { return Color.FromArgb(254, 255, 232); } }
        protected override Color BottomColor { get { return Color.FromArgb(189, 193, 163); } }
        protected override Color HeaderTextColor { get { return Color.FromArgb(220, 0, 0, 0); } }

        protected override Color WorkplaceBrushColor { get { return Color.FromArgb(10, 89, 93, 63); } }
        protected override Color WorkplacePenColor { get { return Color.FromArgb(50, WorkplaceBrushColor); } }

        protected override Color ScrollBodyColor { get { return Color.FromArgb(150, FrameColor); } }
        protected override Color ScrollFrontColor { get { return TopColor; } }
        protected override Color ScrollMarksColor { get { return Color.FromArgb(200, FrameColor); } }

        protected override Color ButtonColor { get { return Color.FromArgb(150, 255, 255, 255); } }
        protected override Color ButtonCaptionColor { get { return HeaderTextColor; } }

        protected override Color GroupPlusMinusFrameColor { get { return ScrollBodyColor; } }
        protected override Color GroupPlusMinusFillColor { get { return TopColor; } }
        protected override Color GroupPlusMinusMarkColor { get { return ScrollMarksColor; } }

        protected override Color LeftLinkPointColor { get { return Color.FromArgb(160, 128, 37); } }
        protected override Color LeftLinkPointColorMouseOn { get { return Color.FromArgb(220, 220, 220); } }
        protected override Color LeftLinkPointColorSelected { get { return Color.FromArgb(255, 255, 255); } }

        protected override Color RightLinkPointColor { get { return LeftLinkPointColor; } }
        protected override Color RightLinkPointColorMouseOn { get { return LeftLinkPointColorMouseOn; } }
        protected override Color RightLinkPointColorSelected { get { return LeftLinkPointColorSelected; } }

#endregion

#region Class functions

        public override string Name
        {
            get { return "White gradient"; }
        }

#endregion

    }
}
