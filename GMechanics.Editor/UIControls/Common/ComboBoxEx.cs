using System.ComponentModel;
using System.Windows.Forms;

namespace GMechanics.Editor.UIControls.Common
{
    public class ComboBoxEx : ComboBox
    {
        public class DroppingDownEventArgs
        {
            public bool Cancel;
        }

        public delegate void DroppingDownHandler(object s, DroppingDownEventArgs e);

        [Category("Action")]
        public event DroppingDownHandler DroppingDown;

        protected override void WndProc(ref  Message m)
        {
            if (m.Msg == 0x201 || m.Msg == 0x203)
            {
                if (DroppingDown != null)
                {
                    DroppingDownEventArgs e = new DroppingDownEventArgs();
                    DroppingDown(this, e);
                    if (e.Cancel)
                    {
                        return;
                    }
                }
            }
            base.WndProc(ref m);
        }
    }
}
