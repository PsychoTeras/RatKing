namespace RK.Server
{
    partial class frmDashboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pMain = new System.Windows.Forms.TableLayoutPanel();
            this.pTCPConnections = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.hTCPConnections = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.pTCPResponsesProc = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.hTCPResponsesProc = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.pOutput = new System.Windows.Forms.Panel();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.pTCPResponsesSend = new System.Windows.Forms.Panel();
            this.pNavigationControlsHeader = new System.Windows.Forms.Panel();
            this.hTCPResponsesSend = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.gTCPConnections = new RK.Server.Controls.Graphing.PerfGraph();
            this.gTCPResponsesProc = new RK.Server.Controls.Graphing.PerfGraph();
            this.gTCPResponsesSend = new RK.Server.Controls.Graphing.PerfGraph();
            this.pMain.SuspendLayout();
            this.pTCPConnections.SuspendLayout();
            this.panel7.SuspendLayout();
            this.pTCPResponsesProc.SuspendLayout();
            this.panel5.SuspendLayout();
            this.pOutput.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pTCPResponsesSend.SuspendLayout();
            this.pNavigationControlsHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pMain
            // 
            this.pMain.ColumnCount = 3;
            this.pMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.62159F));
            this.pMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.04508F));
            this.pMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.pMain.Controls.Add(this.pTCPConnections, 0, 0);
            this.pMain.Controls.Add(this.pTCPResponsesProc, 1, 0);
            this.pMain.Controls.Add(this.pOutput, 0, 1);
            this.pMain.Controls.Add(this.pTCPResponsesSend, 2, 0);
            this.pMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pMain.Location = new System.Drawing.Point(0, 0);
            this.pMain.Name = "pMain";
            this.pMain.RowCount = 2;
            this.pMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 32.1361F));
            this.pMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 67.8639F));
            this.pMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.pMain.Size = new System.Drawing.Size(562, 529);
            this.pMain.TabIndex = 2;
            // 
            // pTCPConnections
            // 
            this.pTCPConnections.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pTCPConnections.Controls.Add(this.panel7);
            this.pTCPConnections.Controls.Add(this.gTCPConnections);
            this.pTCPConnections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pTCPConnections.Location = new System.Drawing.Point(3, 3);
            this.pTCPConnections.Name = "pTCPConnections";
            this.pTCPConnections.Size = new System.Drawing.Size(177, 163);
            this.pTCPConnections.TabIndex = 6;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.Honeydew;
            this.panel7.Controls.Add(this.hTCPConnections);
            this.panel7.Controls.Add(this.panel8);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel7.Location = new System.Drawing.Point(0, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(175, 18);
            this.panel7.TabIndex = 3;
            // 
            // hTCPConnections
            // 
            this.hTCPConnections.BackColor = System.Drawing.Color.Khaki;
            this.hTCPConnections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hTCPConnections.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hTCPConnections.Location = new System.Drawing.Point(0, 0);
            this.hTCPConnections.Name = "hTCPConnections";
            this.hTCPConnections.Size = new System.Drawing.Size(175, 17);
            this.hTCPConnections.TabIndex = 1;
            this.hTCPConnections.Text = "TCP Connections";
            this.hTCPConnections.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel8
            // 
            this.panel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel8.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel8.Location = new System.Drawing.Point(0, 17);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(175, 1);
            this.panel8.TabIndex = 0;
            // 
            // pTCPResponsesProc
            // 
            this.pTCPResponsesProc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pTCPResponsesProc.Controls.Add(this.panel5);
            this.pTCPResponsesProc.Controls.Add(this.gTCPResponsesProc);
            this.pTCPResponsesProc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pTCPResponsesProc.Location = new System.Drawing.Point(186, 3);
            this.pTCPResponsesProc.Name = "pTCPResponsesProc";
            this.pTCPResponsesProc.Size = new System.Drawing.Size(185, 163);
            this.pTCPResponsesProc.TabIndex = 4;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.Honeydew;
            this.panel5.Controls.Add(this.hTCPResponsesProc);
            this.panel5.Controls.Add(this.panel6);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(183, 18);
            this.panel5.TabIndex = 3;
            // 
            // hTCPResponsesProc
            // 
            this.hTCPResponsesProc.BackColor = System.Drawing.Color.Khaki;
            this.hTCPResponsesProc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hTCPResponsesProc.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hTCPResponsesProc.Location = new System.Drawing.Point(0, 0);
            this.hTCPResponsesProc.Name = "hTCPResponsesProc";
            this.hTCPResponsesProc.Size = new System.Drawing.Size(183, 17);
            this.hTCPResponsesProc.TabIndex = 1;
            this.hTCPResponsesProc.Text = "TCP Responses Proc";
            this.hTCPResponsesProc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel6
            // 
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 17);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(183, 1);
            this.panel6.TabIndex = 0;
            // 
            // pOutput
            // 
            this.pOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pMain.SetColumnSpan(this.pOutput, 3);
            this.pOutput.Controls.Add(this.tbOutput);
            this.pOutput.Controls.Add(this.panel2);
            this.pOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pOutput.Location = new System.Drawing.Point(3, 172);
            this.pOutput.Name = "pOutput";
            this.pOutput.Size = new System.Drawing.Size(556, 354);
            this.pOutput.TabIndex = 3;
            // 
            // tbOutput
            // 
            this.tbOutput.BackColor = System.Drawing.SystemColors.Window;
            this.tbOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbOutput.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbOutput.Location = new System.Drawing.Point(0, 18);
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ReadOnly = true;
            this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbOutput.Size = new System.Drawing.Size(554, 334);
            this.tbOutput.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Honeydew;
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(554, 18);
            this.panel2.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LightBlue;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(554, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Output";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 17);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(554, 1);
            this.panel4.TabIndex = 0;
            // 
            // pTCPResponsesSend
            // 
            this.pTCPResponsesSend.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pTCPResponsesSend.Controls.Add(this.pNavigationControlsHeader);
            this.pTCPResponsesSend.Controls.Add(this.gTCPResponsesSend);
            this.pTCPResponsesSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pTCPResponsesSend.Location = new System.Drawing.Point(377, 3);
            this.pTCPResponsesSend.Name = "pTCPResponsesSend";
            this.pTCPResponsesSend.Size = new System.Drawing.Size(182, 163);
            this.pTCPResponsesSend.TabIndex = 2;
            // 
            // pNavigationControlsHeader
            // 
            this.pNavigationControlsHeader.BackColor = System.Drawing.Color.Honeydew;
            this.pNavigationControlsHeader.Controls.Add(this.hTCPResponsesSend);
            this.pNavigationControlsHeader.Controls.Add(this.panel3);
            this.pNavigationControlsHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pNavigationControlsHeader.Location = new System.Drawing.Point(0, 0);
            this.pNavigationControlsHeader.Name = "pNavigationControlsHeader";
            this.pNavigationControlsHeader.Size = new System.Drawing.Size(180, 18);
            this.pNavigationControlsHeader.TabIndex = 3;
            // 
            // hTCPResponsesSend
            // 
            this.hTCPResponsesSend.BackColor = System.Drawing.Color.Khaki;
            this.hTCPResponsesSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hTCPResponsesSend.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hTCPResponsesSend.Location = new System.Drawing.Point(0, 0);
            this.hTCPResponsesSend.Name = "hTCPResponsesSend";
            this.hTCPResponsesSend.Size = new System.Drawing.Size(180, 17);
            this.hTCPResponsesSend.TabIndex = 1;
            this.hTCPResponsesSend.Text = "TCP Responses Send";
            this.hTCPResponsesSend.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 17);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(180, 1);
            this.panel3.TabIndex = 0;
            // 
            // gTCPConnections
            // 
            this.gTCPConnections.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gTCPConnections.AutoAdjustPeek = true;
            this.gTCPConnections.DisabledColor = System.Drawing.SystemColors.ButtonFace;
            this.gTCPConnections.ForeColor = System.Drawing.Color.SaddleBrown;
            this.gTCPConnections.GridColor = System.Drawing.Color.GhostWhite;
            this.gTCPConnections.GridSize = ((ushort)(10));
            this.gTCPConnections.HighQuality = true;
            this.gTCPConnections.LineInterval = ((ushort)(3));
            this.gTCPConnections.Location = new System.Drawing.Point(0, 18);
            this.gTCPConnections.MaxLabel = "";
            this.gTCPConnections.MaxPeekMagnitude = 0F;
            this.gTCPConnections.MinLabel = " ";
            this.gTCPConnections.MinPeekMagnitude = 0F;
            this.gTCPConnections.Name = "gTCPConnections";
            this.gTCPConnections.ShowGrid = true;
            this.gTCPConnections.ShowInfoAvg = false;
            this.gTCPConnections.ShowLabels = true;
            this.gTCPConnections.Size = new System.Drawing.Size(175, 143);
            this.gTCPConnections.TabIndex = 1;
            this.gTCPConnections.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.gTCPConnections.Units = "";
            // 
            // gTCPResponsesProc
            // 
            this.gTCPResponsesProc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gTCPResponsesProc.AutoAdjustPeek = true;
            this.gTCPResponsesProc.DisabledColor = System.Drawing.SystemColors.ButtonFace;
            this.gTCPResponsesProc.ForeColor = System.Drawing.Color.SaddleBrown;
            this.gTCPResponsesProc.GridColor = System.Drawing.Color.GhostWhite;
            this.gTCPResponsesProc.GridSize = ((ushort)(10));
            this.gTCPResponsesProc.HighQuality = true;
            this.gTCPResponsesProc.LineInterval = ((ushort)(3));
            this.gTCPResponsesProc.Location = new System.Drawing.Point(0, 18);
            this.gTCPResponsesProc.MaxLabel = "";
            this.gTCPResponsesProc.MaxPeekMagnitude = 50F;
            this.gTCPResponsesProc.MinLabel = " ";
            this.gTCPResponsesProc.MinPeekMagnitude = 0F;
            this.gTCPResponsesProc.Name = "gTCPResponsesProc";
            this.gTCPResponsesProc.ShowGrid = true;
            this.gTCPResponsesProc.ShowInfoAvg = true;
            this.gTCPResponsesProc.ShowLabels = true;
            this.gTCPResponsesProc.Size = new System.Drawing.Size(183, 143);
            this.gTCPResponsesProc.TabIndex = 1;
            this.gTCPResponsesProc.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.gTCPResponsesProc.Units = "msec";
            this.gTCPResponsesProc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GraphMouseDown);
            // 
            // gTCPResponsesSend
            // 
            this.gTCPResponsesSend.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gTCPResponsesSend.AutoAdjustPeek = true;
            this.gTCPResponsesSend.DisabledColor = System.Drawing.SystemColors.ButtonFace;
            this.gTCPResponsesSend.ForeColor = System.Drawing.Color.SaddleBrown;
            this.gTCPResponsesSend.GridColor = System.Drawing.Color.GhostWhite;
            this.gTCPResponsesSend.GridSize = ((ushort)(10));
            this.gTCPResponsesSend.HighQuality = true;
            this.gTCPResponsesSend.LineInterval = ((ushort)(3));
            this.gTCPResponsesSend.Location = new System.Drawing.Point(0, 18);
            this.gTCPResponsesSend.MaxLabel = "";
            this.gTCPResponsesSend.MaxPeekMagnitude = 0F;
            this.gTCPResponsesSend.MinLabel = " ";
            this.gTCPResponsesSend.MinPeekMagnitude = 0F;
            this.gTCPResponsesSend.Name = "gTCPResponsesSend";
            this.gTCPResponsesSend.ShowGrid = true;
            this.gTCPResponsesSend.ShowInfoAvg = true;
            this.gTCPResponsesSend.ShowLabels = true;
            this.gTCPResponsesSend.Size = new System.Drawing.Size(180, 143);
            this.gTCPResponsesSend.TabIndex = 1;
            this.gTCPResponsesSend.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.gTCPResponsesSend.Units = "msec";
            this.gTCPResponsesSend.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GraphMouseDown);
            // 
            // frmDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 529);
            this.Controls.Add(this.pMain);
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
            this.Name = "frmDashboard";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RK.Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmDashboardFormClosing);
            this.pMain.ResumeLayout(false);
            this.pTCPConnections.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.pTCPResponsesProc.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.pOutput.ResumeLayout(false);
            this.pOutput.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.pTCPResponsesSend.ResumeLayout(false);
            this.pNavigationControlsHeader.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel pMain;
        private System.Windows.Forms.Panel pTCPResponsesSend;
        private System.Windows.Forms.Panel pNavigationControlsHeader;
        private System.Windows.Forms.Label hTCPResponsesSend;
        private System.Windows.Forms.Panel panel3;
        private Controls.Graphing.PerfGraph gTCPResponsesSend;
        private System.Windows.Forms.Panel pOutput;
        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel pTCPResponsesProc;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label hTCPResponsesProc;
        private System.Windows.Forms.Panel panel6;
        private Controls.Graphing.PerfGraph gTCPResponsesProc;
        private System.Windows.Forms.Panel pTCPConnections;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label hTCPConnections;
        private System.Windows.Forms.Panel panel8;
        private Controls.Graphing.PerfGraph gTCPConnections;
    }
}

