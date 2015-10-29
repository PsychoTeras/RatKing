using System;
using System.Windows.Forms;
using IPCLogger.Core.Loggers.LIPC;
using RK.Common.Classes;
using RK.Common.Host;
using RK.Server.Controls.Graphing;

namespace RK.Server
{
    public partial class frmDashboard : Form
    {

#region Private fields

        private LIPCView _logger;
        private GameHost _gameHost;

        private C2DPushGraph.LineHandle _lineWorldResponsesPrc;

#endregion

#region Ctor

        public frmDashboard()
        {
            InitializeComponent();

            _lineWorldResponsesPrc = gWorldResponsesPrc.AddLine(0,
                gWorldResponsesPrc.ForeColor);
            _lineWorldResponsesPrc.Thickness = 2;

            _logger = new LIPCView();
            _logger.StartView("RK.Server", OnEvent);
            _gameHost = new GameHost();
        }

#endregion

#region Class methods

        private void WriteLog(string logMessage)
        {
            WriteLog(logMessage, true);
        }

        private void WriteLog(string logMessage, bool writeLine)
        {
            WriteLog(logMessage, writeLine, true);
        }

        private void WriteLog(string logMessage, bool writeLine, bool writeTime)
        {
            tbOutput.Invoke(new Action(delegate
            {
                if (writeTime)
                {
                    logMessage = string.Format("[{0}] {1}", DateTime.Now, logMessage ?? string.Empty);
                }
                if (writeLine)
                {
                    tbOutput.Text += string.Format("{0}{1}", logMessage, Environment.NewLine);
                }
                else
                {
                    tbOutput.Text += logMessage;
                }
            }));
        }

        private void ProcessDashboardMsg(LogEventType eventType, string data)
        {
            tbOutput.Invoke(new Action(delegate
            {
                switch (eventType)
                {
                    case LogEventType.SendWorldResponses:
                    {
                        float fData = float.Parse(data);
                        gWorldResponsesPrc.Push(fData, 0);
                        gWorldResponsesPrc.UpdateGraph();
                        break;
                    }
                }
            }));
        }

        private void OnEvent(IPCEvent ev)
        {
            if (!string.IsNullOrEmpty(ev.Message))
            {
                string[] msgData = ev.Message.Split('\x4');
                if (msgData.Length == 2)
                {
                    LogEventType eventType;
                    if (Enum.TryParse(msgData[0], false, out eventType))
                        ProcessDashboardMsg(eventType, msgData[1]);
                    else
                        WriteLog(ev.Message);
                }
                else
                {
                    WriteLog(ev.Message);
                }
            }
        }

        private void frmDashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            _gameHost.Dispose();
        }

#endregion

    }
}
