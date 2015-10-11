using System.Drawing;

namespace FlowchartControl.ItemControls
{
    internal class ItemPinPoint : IItemControl
    {

#region Properties

        public ItemControlType ControlType { get; private set; }
        public ItemControlState ControlState { get; set; }
        public Rectangle ClientRectangle { get; set; }
        public object UserObject { get; private set; }
        public OnItemPrePaint ItemPrePaint { get; set; }
        public OnItemPostPaint ItemPostPaint { get; set; }
        public bool Destroyed { get; set; }

#endregion

        public void Draw(Graphics graphics)
        {
            if (ItemPrePaint != null)
            {
                ItemPrePaint(graphics, this);
            }


            if (ItemPostPaint != null)
            {
                ItemPostPaint(graphics, this);
            }
        }

        public void Dispose()
        {
        }
    }
}
