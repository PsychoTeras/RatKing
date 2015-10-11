using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using GMechanics.FlowchartControl.ItemPainters.Painters;

namespace GMechanics.FlowchartControl.ItemPainters
{
    internal class ItemPaintersList : List<IItemPainter>
    {
        private static ItemPaintersList _instanceRef;
        private string[] _painterNames;
        private readonly IItemPainter _defaultPainter = new ItemPainter_BlueGradient();

        public static ItemPaintersList Instance
        {
            get 
            {
                if (_instanceRef == null)
                {
                    _instanceRef = new ItemPaintersList();

                    Assembly currentAssembly = Assembly.GetCallingAssembly();
                    foreach (Type type in currentAssembly.GetTypes())
                    {
                        Type[] interfaces = type.GetInterfaces();
                        if (interfaces.Contains(typeof(IItemPainter)))
                        {
                            IItemPainter comparer = (IItemPainter)Activator.CreateInstance(type);
                            if (!string.IsNullOrEmpty(comparer.Name))
                            {
                                _instanceRef.Add(comparer);
                            }
                        }
                    }

                    List<string> names = new List<string>();
                    foreach (IItemPainter painter in _instanceRef)
                    {
                        names.Add(painter.Name);
                    }
                    _instanceRef._painterNames = names.ToArray();
                }

                return _instanceRef;
            }
        }

        public IItemPainter this[string name]
        {
            get
            {
                return Find(painter => painter.Name.Equals(name,
                    StringComparison.InvariantCultureIgnoreCase)) 
                    ?? _defaultPainter;
            }
        }

        public static string[] PainterNames
        {
            get { return _instanceRef._painterNames; }
        }
    }
}
