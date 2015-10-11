using System;
using System.Drawing;
using System.Windows.Forms;

namespace GMechanics.FlowchartControl.ItemControls
{
    internal class ItemImage : IItemControl
    {

#region Properties

        public ItemControlType ControlType { get { return ItemControlType.Image; } }

        public ItemControlState State { get; set; }

        public bool Selected { get; set; }

        public Rectangle ClientRectangle { get; set; }

        public object UserObject { get; private set; }

        public OnItemPrePaint ItemPrePaint { get; set; }

        public OnItemPostPaint ItemPostPaint { get; set; }

        public bool Destroyed { get; set; }

        public Cursor Cursor { get; set; }

        public string Hint { get; set; }

        public Image Image { get; set; }

#endregion

#region Class functions

        public ItemImage(Rectangle rectangle, object userObject)
        {
            ClientRectangle = rectangle;
            UserObject = userObject;
        }

        public void Draw(Graphics graphics)
        {
            if (ItemPrePaint != null)
            {
                ItemPrePaint(graphics, this);
            }

            if (Image != null)
            {
                graphics.DrawImageUnscaledAndClipped(Image, ClientRectangle);
            }

            if (ItemPostPaint != null)
            {
                ItemPostPaint(graphics, this);
            }
        }

        public bool Visible { get; set; }

        public void Dispose() { }

#endregion

    }
}
