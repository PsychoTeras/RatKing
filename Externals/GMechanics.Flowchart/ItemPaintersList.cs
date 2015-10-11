using System.Collections.Generic;
using GMechanics.FlowchartDemo.FlowchartControl.ItemPainters;

namespace GMechanics.FlowchartDemo.FlowchartControl
{
    internal class ItemPaintersList : List<IItemPainter>
    {
        private static ItemPaintersList _instanceRef;
        private string[] _paintersNames;

        public static ItemPaintersList Instance
        {
            get 
            {
                if (_instanceRef == null)
                {
                    _instanceRef = new ItemPaintersList();

                    _instanceRef.Add(new ItemPainter_Test());
                    _instanceRef.Add(new ItemPainter_BlueGradient());
                    _instanceRef.Add(new ItemPainter_BlackGradient());
                    _instanceRef.Sort();

                    List<string> names = new List<string>();
                    foreach (IItemPainter painter in _instanceRef)
                    {
                        names.Add(painter.Name);
                    }
                    _instanceRef._paintersNames = names.ToArray();
                }
                return _instanceRef;
            }
        }

        public IItemPainter this[string name]
        {
            get
            {
                return Find(painter => painter.Name.Equals(name,
                    System.StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public string[] PaintersNames
        {
            get { return _paintersNames; }
        }
    }
}
