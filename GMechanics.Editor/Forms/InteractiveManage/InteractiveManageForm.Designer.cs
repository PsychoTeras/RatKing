namespace GMechanics.Editor.Forms.InteractiveManage
{
    partial class InteractiveManageForm
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
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.lvRecipients = new System.Windows.Forms.ListView();
            this.headerFeature = new System.Windows.Forms.ColumnHeader();
            this.columnEvent = new System.Windows.Forms.ColumnHeader();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(407, 25);
            this.toolStrip.TabIndex = 0;
            // 
            // lvRecipients
            // 
            this.lvRecipients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvRecipients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.headerFeature,
            this.columnEvent});
            this.lvRecipients.FullRowSelect = true;
            this.lvRecipients.HideSelection = false;
            this.lvRecipients.Location = new System.Drawing.Point(12, 28);
            this.lvRecipients.MultiSelect = false;
            this.lvRecipients.Name = "lvRecipients";
            this.lvRecipients.ShowItemToolTips = true;
            this.lvRecipients.Size = new System.Drawing.Size(383, 239);
            this.lvRecipients.TabIndex = 1;
            this.lvRecipients.UseCompatibleStateImageBehavior = false;
            this.lvRecipients.View = System.Windows.Forms.View.Details;
            // 
            // headerFeature
            // 
            this.headerFeature.Text = "Feature name";
            this.headerFeature.Width = 173;
            // 
            // columnEvent
            // 
            this.columnEvent.Text = "Event type";
            this.columnEvent.Width = 177;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(320, 273);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 35;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnOk.Location = new System.Drawing.Point(239, 273);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 34;
            this.btnOk.Text = "Save";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // InteractiveManageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 308);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lvRecipients);
            this.Controls.Add(this.toolStrip);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InteractiveManageForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Interactive management";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ListView lvRecipients;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.ColumnHeader headerFeature;
        private System.Windows.Forms.ColumnHeader columnEvent;
    }
}