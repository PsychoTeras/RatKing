using System.Drawing;

namespace GMechanics.FlowchartControl.ItemPainters.Painters
{
    internal sealed class ItemPainter_BlackGradient : ItemPainter_BaseGradient
    {

#region Overrided style members

        protected override Color FrameColor { get { return Color.Black; } }
        protected override Color FrameColorSelected { get { return Color.White; } }

        protected override Color TopColor { get { return Color.FromArgb(124, 125, 124); } }
        protected override Color BottomColor { get { return Color.FromArgb(16, 16, 16); } }
        protected override Color HeaderTextColor { get { return Color.FromArgb(230, 255, 255, 255); } }

        protected override Color WorkplaceBrushColor { get { return Color.FromArgb(5, 255, 255, 255); } }
        protected override Color WorkplacePenColor { get { return Color.FromArgb(150, 0, 0, 0); } }

        protected override Color ScrollBodyColor { get { return Color.FromArgb(180, 135, 135, 135); } }
        protected override Color ScrollFrontColor { get { return Color.FromArgb(255, 201, 15); } }
        protected override Color ScrollMarksColor { get { return Color.FromArgb(70, 70, 70); } }

        protected override Color ButtonColor { get { return Color.FromArgb(230, ScrollFrontColor); } }
        protected override Color ButtonCaptionColor { get { return Color.FromArgb(230, 255, 255, 255); } }

        protected override Color GroupPlusMinusFrameColor { get { return ScrollBodyColor; } }
        protected override Color GroupPlusMinusFillColor { get { return ScrollFrontColor; } }
        protected override Color GroupPlusMinusMarkColor { get { return Color.FromArgb(50, 50, 50); } }

        protected override Color LeftLinkPointColor { get { return Color.FromArgb(0, 0, 0); } }
        protected override Color LeftLinkPointColorMouseOn { get { return Color.FromArgb(220, 220, 220); } }
        protected override Color LeftLinkPointColorSelected { get { return Color.FromArgb(255, 255, 255); } }

        protected override Color RightLinkPointColor { get { return LeftLinkPointColor; } }
        protected override Color RightLinkPointColorMouseOn { get { return LeftLinkPointColorMouseOn; } }
        protected override Color RightLinkPointColorSelected { get { return LeftLinkPointColorSelected; } }

#endregion

#region Class functions

        public override string Name
        {
            get { return "Black gradient"; }
        }

#endregion

    }
}
