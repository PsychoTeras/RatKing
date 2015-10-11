using System;
using System.Drawing;
using System.Windows.Forms;

namespace GMechanics.FlowchartControl.ItemControls
{
    internal delegate void OnItemPrePaint(Graphics graphics, IItemControl control);
    internal delegate void OnItemPostPaint(Graphics graphics, IItemControl control);

    internal interface IItemControl : IDisposable
    {
        ItemControlType ControlType { get; }
        ItemControlState State { get; set; }
        bool Selected { get; set; }
        Rectangle ClientRectangle { get; set; }
        object UserObject { get; }
        OnItemPrePaint ItemPrePaint { get; set; }
        OnItemPostPaint ItemPostPaint { get; set; }
        bool Destroyed { get; set; }
        Cursor Cursor { get; set; }
        string Hint { get; set; }
        void Draw(Graphics graphics);
        bool Visible { get; set; }
    }
}
