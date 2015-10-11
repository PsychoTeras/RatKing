using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GMechanics.FlowchartControl.LinkingArrowControl
{
    internal class LinkingArrow : IDisposable
    {

#region Private members

        private const int LinkingArrowPen = 5;
        private readonly Pen _linkingArrowBorderPen;

#endregion

#region Properties

        public FlowchartItem SourceItem { get; set; }
        public FlowchartItem DestinationItem { get; set; }
        public Point EndPoint { get; set; }

        public LinkingArrowState State { get; private set; }

#endregion

#region Class functions

        public LinkingArrow(FlowchartItem sourceItem, FlowchartItem destinationItem,
            LinkingArrowState state)
        {
            SourceItem = sourceItem;
            DestinationItem = destinationItem;
            State = state;

            _linkingArrowBorderPen = new Pen(Color.White, LinkingArrowPen);
            _linkingArrowBorderPen.EndCap = LineCap.ArrowAnchor;
            _linkingArrowBorderPen.DashStyle = DashStyle.Dot;
            _linkingArrowBorderPen.DashCap = DashCap.Round;
            _linkingArrowBorderPen.DashOffset = 0.08f;
        }

        public void Link(FlowchartItem destinationItem)
        {
            DestinationItem = destinationItem;
            State = LinkingArrowState.Linked;
        }

        public void Unlink()
        {
            DestinationItem = null;
            State = LinkingArrowState.Linking;
        }

        public void Draw(Graphics graphics, Point startPoint, Point endPoint)
        {
            if (State != LinkingArrowState.None)
            {
                graphics.DrawLine(_linkingArrowBorderPen, startPoint, endPoint);
            }
        }

        public void Dispose()
        {
            _linkingArrowBorderPen.Dispose();
        }

#endregion

    }
}
