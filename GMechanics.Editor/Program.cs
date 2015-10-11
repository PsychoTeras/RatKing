using System;
using System.Runtime.Remoting;
using System.Windows.Forms;
using GMechanics.Editor.Forms;

namespace GMechanics.Editor
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            RemotingConfiguration.Configure("GMechanics.Editor.exe.config", false);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}