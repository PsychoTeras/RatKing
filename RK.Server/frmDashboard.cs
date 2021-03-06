﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using IPCLogger.Core.Loggers.LIPC;
using IPCLogger.Core.Proto;
using RK.Common.Host;
using RK.Server.Controls.Graphing;

namespace RK.Server
{
    public partial class frmDashboard : Form
    {

#region Private fields

        private LIPCView _logger;
        private GameHost _gameHost;

#endregion

#region Ctor

        public frmDashboard()
        {
            InitializeComponent();

            gTCPResponsesProc.AddLine().Thickness = 2;
            gTCPResponsesSend.AddLine().Thickness = 2;
            gTCPConnections.AddLine().Thickness = 2;

            _logger = new LIPCView();
            _logger.QueryIntervalMsec = 30;
            _logger.StartView("RK.Server", OnEvent);
            _gameHost = new GameHost();
        }

#endregion

#region Class methods

        private void WriteLog(string logMessage, bool writeLine = true, bool writeTime = true)
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
                tbOutput.SelectionStart = tbOutput.Text.Length;
            }));
        }
        
        private void OnEvent(LogRecord ev)
        {
            //return;

            if (string.IsNullOrEmpty(ev.Message))
            {
                return;
            }

            tbOutput.Invoke(new Action(delegate
            {
                switch ((LogEventType) ev.Type)
                {
                    case LogEventType.TCPConnections:
                        {
                            int iData = int.Parse(ev.Message);
                            gTCPConnections.Push(iData, 0);
                            gTCPConnections.UpdateGraph();
                            hTCPConnections.Text = string.Format("TCP Connections - {0}", iData);
                            break;
                        }
                    case LogEventType.TCPResponsesProc:
                        {
                            if (gTCPResponsesProc.Enabled)
                            {
                                float fData = float.Parse(ev.Message);
                                gTCPResponsesProc.Push(fData, 0);
                                gTCPResponsesProc.UpdateGraph();
                            }
                            break;
                        }
                    case LogEventType.TCPResponsesSend:
                        {
                            if (gTCPResponsesSend.Enabled)
                            {
                                float fData = float.Parse(ev.Message);
                                gTCPResponsesSend.Push(fData, 0);
                                gTCPResponsesSend.UpdateGraph();
                            }
                            break;
                        }
                    default:
                    {
                            //WriteLog(ev.Message);
                            break;
                    }
                }
            }));
        }

        private void FrmDashboardFormClosing(object sender, FormClosingEventArgs e)
        {
            _logger.Dispose();
            _gameHost.Dispose();
            Process client = Process.GetProcessesByName("RK.Client").FirstOrDefault();
            if (client != null && !client.HasExited)
            {
                client.Kill();
            }
        }

        private void GraphMouseDown(object sender, MouseEventArgs e)
        {
            PerfGraph g = sender as PerfGraph;
            if (g != null && e.Button == MouseButtons.Right)
            {
                g.Enabled = !g.Enabled;
            }
        }

#endregion

    }
}
