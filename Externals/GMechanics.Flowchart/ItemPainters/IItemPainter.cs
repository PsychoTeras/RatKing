using System;
using GMechanics.FlowchartControl.ItemControls;

namespace GMechanics.FlowchartControl.ItemPainters
{
    internal interface IItemPainter : IComparable<IItemPainter>, IDisposable
    {
        string Name { get; }
        void UpdateLinkPoints(FlowchartItem item, ItemLinkPoint[] linkPoints);
        void Paint(FlowchartItem item, ItemScrollBar scrollBar, 
            ItemLinkPoint[] linkPoints);
    }
}
