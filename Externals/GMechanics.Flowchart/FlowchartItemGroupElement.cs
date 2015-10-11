using System;

namespace GMechanics.FlowchartDemo.FlowchartControl
{
    public class FlowchartItemGroupElement : IComparable<FlowchartItemGroupElement>
    {

#region Private members

        private readonly FlowchartItemGroup _parentGroup;

        private string _text;
        private bool _visible;

#endregion

#region Properties

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value ?? string.Empty;
                Repaint();
            }
        }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                ParentGroup.RecalculateVisibled();
                Repaint();
            }
        }
        
        private FlowchartItemGroup ParentGroup
        {
            get { return _parentGroup; }
        }

        public object UserObject { get; set; }

#endregion

#region Class functions

        internal FlowchartItemGroupElement(FlowchartItemGroup parentGroup,
            string text, object userObject)
        {
            _parentGroup = parentGroup;
            UserObject = userObject;
            _text = text;
        }

        public void Repaint()
        {
            ParentGroup.Repaint();
        }

        public int CompareTo(FlowchartItemGroupElement other)
        {
            return String.CompareOrdinal(Text, other.Text);
        }

#endregion

    }
}
