using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GMechanics.FlowchartDemo.FlowchartControl.ItemPainters
{
    internal class ItemPainter_Base : IItemPainter
    {
        private GraphicsPath _graphicsPath;
        private readonly Font _headerFont = new Font("Verdana", 8.25f, FontStyle.Bold);
        private readonly StringFormat _stringFormat = new StringFormat(StringFormatFlags.NoWrap)
                                                          {
                                                              Alignment = StringAlignment.Center,
                                                              LineAlignment = StringAlignment.Center,
                                                              Trimming = StringTrimming.EllipsisCharacter
                                                          };

        public virtual string Name { get { return "Base painter"; } }

        protected virtual Font HeaderFont { get { return _headerFont; } }

        protected virtual StringFormat HeaderFormat { get { return _stringFormat; } }

        protected virtual int HeaderHeight { get { return 24; } }

        protected virtual int CornerRadius { get { return 15; } }

        protected virtual int ShadowDistance { get { return 4; } }

        protected virtual Color ShadowColor { get { return Color.FromArgb(220, 0, 0, 0); } }

        public GraphicsPath GraphicsPath
        {
            get { return _graphicsPath; }
            set
            {
                if (_graphicsPath != null)
                {
                    _graphicsPath.Dispose();
                }
                _graphicsPath = value;
            }
        }

        public virtual void Paint(FlowchartItem item) { }

        public int CompareTo(IItemPainter other)
        {
            return String.CompareOrdinal(Name, other.Name);
        }

        public override string ToString()
        {
            return Name;
        }

        public virtual void Dispose()
        {
            GraphicsPath = null;
            _headerFont.Dispose();
            _stringFormat.Dispose();
        }
    }
}
