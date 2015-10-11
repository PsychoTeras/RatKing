using System.Collections.Generic;

namespace GMechanics.FlowchartDemo.FlowchartControl
{
    public class FlowchartItemGroups : Dictionary<string, FlowchartItemGroup>
    {

#region Private members

        private readonly FlowchartItem _parentItem;

#endregion

#region Properties

        internal FlowchartItem ParentItem
        {
            get { return _parentItem; }
        }

#endregion

#region Class functions

        public FlowchartItemGroups(FlowchartItem parentItem)
        {
            _parentItem = parentItem;
        }

#endregion

    }
}
