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
            this.panel1 = new System.Windows.Forms.Panel();
            this.hNetworkProcess = new RK.Server.Controls.Header();
            this.gNetworkProcess = new RK.Server.Controls.Graphing.C2DPushGraph();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.gNetworkProcess);
            this.panel1.Controls.Add(this.hNetworkProcess);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(285, 208);
            this.panel1.TabIndex = 0;
            // 
            // hNetworkProcess
            // 
            this.hNetworkProcess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hNetworkProcess.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.hNetworkProcess.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.hNetworkProcess.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hNetworkProcess.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hNetworkProcess.Caption = "Network Process";
            this.hNetworkProcess.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hNetworkProcess.ForeColor = System.Drawing.Color.Black;
            this.hNetworkProcess.Location = new System.Drawing.Point(-1, -1);
            this.hNetworkProcess.Name = "hNetworkProcess";
            this.hNetworkProcess.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.hNetworkProcess.Size = new System.Drawing.Size(285, 21);
            this.hNetworkProcess.TabIndex = 0;
            this.hNetworkProcess.TabStop = false;
            this.hNetworkProcess.TopGradientColor = System.Drawing.Color.AntiqueWhite;
            // 
            // gNetworkProcess
            // 
            this.gNetworkProcess.AutoAdjustPeek = false;
            this.gNetworkProcess.BackColor = System.Drawing.Color.White;
            this.gNetworkProcess.GridColor = System.Drawing.Color.Green;
            this.gNetworkProcess.GridSize = ((ushort)(15));
            this.gNetworkProcess.HighQuality = true;
            this.gNetworkProcess.LineInterval = ((ushort)(1));
            this.gNetworkProcess.Location = new System.Drawing.Point(-1, 20);
            this.gNetworkProcess.MaxLabel = "1 sec";
            this.gNetworkProcess.MaxPeekMagnitude = 100;
            this.gNetworkProcess.MinLabel = "";
            this.gNetworkProcess.MinPeekMagnitude = 0;
            this.gNetworkProcess.Name = "gNetworkProcess";
            this.gNetworkProcess.ShowGrid = false;
            this.gNetworkProcess.ShowLabels = true;
            this.gNetworkProcess.Size = new System.Drawing.Size(284, 186);
            this.gNetworkProcess.TabIndex = 1;
            this.gNetworkProcess.TextColor = System.Drawing.Color.Black;
            // 
            // frmDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 420);
            this.Controls.Add(this.panel1);
            this.Name = "frmDashboard";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RK.Server";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private Controls.Header hNetworkProcess;
        private Controls.Graphing.C2DPushGraph gNetworkProcess;
    }
}

