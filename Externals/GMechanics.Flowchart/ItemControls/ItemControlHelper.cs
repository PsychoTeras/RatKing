using System.Windows.Forms;

namespace GMechanics.FlowchartControl.ItemControls
{
    internal class ItemControlHelper
    {
        public static MouseButtons ControlStateToMouseButton(IItemControl control)
        {
            if (control != null)
            {
                switch (control.State)
                {
                    case ItemControlState.ClickedLeft:
                        return MouseButtons.Left;
                    case ItemControlState.ClickedRight:
                        return MouseButtons.Right;
                    default:
                        return MouseButtons.None;
                }
            }
            return MouseButtons.None;
        }

        public static void SetControlStateAccordingToMouse(IItemControl control, MouseButtons mb)
        {
            if (control.State != ItemControlState.Disabled)
            {
                switch (mb)
                {
                    case MouseButtons.Left:
                        control.State = ItemControlState.ClickedLeft;
                        break;
                    case MouseButtons.Right:
                        control.State = ItemControlState.ClickedRight;
                        break;
                    case MouseButtons.None:
                        control.State = ItemControlState.MouseOn;
                        break;
                }
            }
        }
    }
}
