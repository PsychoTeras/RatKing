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
            this.pWorldResponsesPrc = new System.Windows.Forms.Panel();
            this.pNavigationControlsHeader = new System.Windows.Forms.Panel();
            this.lblNavigationControlsHeader = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pControls = new System.Windows.Forms.Panel();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.gWorldResponsesPrc = new RK.Server.Controls.Graphing.PerfGraph();
            this.pWorldResponsesPrc.SuspendLayout();
            this.pNavigationControlsHeader.SuspendLayout();
            this.pControls.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pWorldResponsesPrc
            // 
            this.pWorldResponsesPrc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pWorldResponsesPrc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pWorldResponsesPrc.Controls.Add(this.pNavigationControlsHeader);
            this.pWorldResponsesPrc.Controls.Add(this.gWorldResponsesPrc);
            this.pWorldResponsesPrc.Location = new System.Drawing.Point(12, 13);
            this.pWorldResponsesPrc.Name = "pWorldResponsesPrc";
            this.pWorldResponsesPrc.Size = new System.Drawing.Size(537, 179);
            this.pWorldResponsesPrc.TabIndex = 0;
            // 
            // pNavigationControlsHeader
            // 
            this.pNavigationControlsHeader.BackColor = System.Drawing.Color.Honeydew;
            this.pNavigationControlsHeader.Controls.Add(this.lblNavigationControlsHeader);
            this.pNavigationControlsHeader.Controls.Add(this.panel3);
            this.pNavigationControlsHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pNavigationControlsHeader.Location = new System.Drawing.Point(0, 0);
            this.pNavigationControlsHeader.Name = "pNavigationControlsHeader";
            this.pNavigationControlsHeader.Size = new System.Drawing.Size(535, 18);
            this.pNavigationControlsHeader.TabIndex = 3;
            // 
            // lblNavigationControlsHeader
            // 
            this.lblNavigationControlsHeader.BackColor = System.Drawing.Color.Khaki;
            this.lblNavigationControlsHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNavigationControlsHeader.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblNavigationControlsHeader.Location = new System.Drawing.Point(0, 0);
            this.lblNavigationControlsHeader.Name = "lblNavigationControlsHeader";
            this.lblNavigationControlsHeader.Size = new System.Drawing.Size(535, 17);
            this.lblNavigationControlsHeader.TabIndex = 1;
            this.lblNavigationControlsHeader.Text = "World Responses Processing";
            this.lblNavigationControlsHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 17);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(535, 1);
            this.panel3.TabIndex = 0;
            // 
            // pControls
            // 
            this.pControls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pControls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pControls.Controls.Add(this.tbOutput);
            this.pControls.Controls.Add(this.panel2);
            this.pControls.Location = new System.Drawing.Point(12, 202);
            this.pControls.Name = "pControls";
            this.pControls.Size = new System.Drawing.Size(538, 315);
            this.pControls.TabIndex = 1;
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
            this.tbOutput.Size = new System.Drawing.Size(536, 295);
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
            this.panel2.Size = new System.Drawing.Size(536, 18);
            this.panel2.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LightBlue;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(536, 17);
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
            this.panel4.Size = new System.Drawing.Size(536, 1);
            this.panel4.TabIndex = 0;
            // 
            // gWorldResponsesPrc
            // 
            this.gWorldResponsesPrc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gWorldResponsesPrc.AutoAdjustPeek = true;
            this.gWorldResponsesPrc.DisabledColor = System.Drawing.SystemColors.ButtonFace;
            this.gWorldResponsesPrc.ForeColor = System.Drawing.Color.SaddleBrown;
            this.gWorldResponsesPrc.GridColor = System.Drawing.Color.GhostWhite;
            this.gWorldResponsesPrc.GridSize = ((ushort)(10));
            this.gWorldResponsesPrc.HighQuality = true;
            this.gWorldResponsesPrc.LineInterval = ((ushort)(3));
            this.gWorldResponsesPrc.Location = new System.Drawing.Point(0, 18);
            this.gWorldResponsesPrc.MaxLabel = "";
            this.gWorldResponsesPrc.MaxPeekMagnitude = 0F;
            this.gWorldResponsesPrc.MinLabel = " ";
            this.gWorldResponsesPrc.MinPeekMagnitude = 0F;
            this.gWorldResponsesPrc.Name = "gWorldResponsesPrc";
            this.gWorldResponsesPrc.ShowGrid = true;
            this.gWorldResponsesPrc.ShowLabels = true;
            this.gWorldResponsesPrc.Size = new System.Drawing.Size(535, 159);
            this.gWorldResponsesPrc.TabIndex = 1;
            this.gWorldResponsesPrc.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.gWorldResponsesPrc.Units = "msec";
            this.gWorldResponsesPrc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GraphMouseDown);
            // 
            // frmDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 529);
            this.Controls.Add(this.pControls);
            this.Controls.Add(this.pWorldResponsesPrc);
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
            this.Name = "frmDashboard";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RK.Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDashboard_FormClosing);
            this.pWorldResponsesPrc.ResumeLayout(false);
            this.pNavigationControlsHeader.ResumeLayout(false);
            this.pControls.ResumeLayout(false);
            this.pControls.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pWorldResponsesPrc;
        private Controls.Graphing.PerfGraph gWorldResponsesPrc;
        private System.Windows.Forms.Panel pNavigationControlsHeader;
        private System.Windows.Forms.Label lblNavigationControlsHeader;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel pControls;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox tbOutput;
    }
}

