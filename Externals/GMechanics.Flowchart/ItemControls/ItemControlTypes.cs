using System;
using System.Collections.Generic;

namespace GMechanics.FlowchartControl.ItemControls
{
    internal static class ItemControlTypes
    {
        public static Dictionary<Type, ItemControlType> TypesTable { get; private set; }

        static ItemControlTypes()
        {
            TypesTable = new Dictionary<Type, ItemControlType>
                    {
                        {typeof (ItemButton), ItemControlType.Button},
                        {typeof (ItemScrollBar), ItemControlType.ScrollBar},
                        {typeof (ItemLinkPoint), ItemControlType.LinkPoint},
                        {typeof (ItemImage), ItemControlType.Image}
                    };
        }
    }
}
