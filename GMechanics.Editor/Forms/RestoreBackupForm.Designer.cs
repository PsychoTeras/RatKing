namespace GMechanics.Editor.Forms
{
    partial class RestoreBackupForm
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.lvBackups = new System.Windows.Forms.ListView();
            this.chFileDate = new System.Windows.Forms.ColumnHeader();
            this.chkLastTen = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(260, 294);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 35;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnRestore
            // 
            this.btnRestore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRestore.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnRestore.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnRestore.Location = new System.Drawing.Point(179, 294);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(75, 23);
            this.btnRestore.TabIndex = 34;
            this.btnRestore.Text = "Restore";
            this.btnRestore.UseVisualStyleBackColor = true;
            // 
            // lvBackups
            // 
            this.lvBackups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvBackups.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFileDate});
            this.lvBackups.FullRowSelect = true;
            this.lvBackups.Location = new System.Drawing.Point(12, 12);
            this.lvBackups.MultiSelect = false;
            this.lvBackups.Name = "lvBackups";
            this.lvBackups.Size = new System.Drawing.Size(323, 276);
            this.lvBackups.TabIndex = 36;
            this.lvBackups.UseCompatibleStateImageBehavior = false;
            this.lvBackups.View = System.Windows.Forms.View.Details;
            this.lvBackups.SelectedIndexChanged += new System.EventHandler(this.LvBackupsSelectedIndexChanged);
            this.lvBackups.DoubleClick += new System.EventHandler(this.LvBackupsDoubleClick);
            // 
            // chFileDate
            // 
            this.chFileDate.Text = "Backup date";
            this.chFileDate.Width = 300;
            // 
            // chkLastTen
            // 
            this.chkLastTen.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.chkLastTen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkLastTen.AutoSize = true;
            this.chkLastTen.Checked = true;
            this.chkLastTen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLastTen.Location = new System.Drawing.Point(12, 298);
            this.chkLastTen.Name = "chkLastTen";
            this.chkLastTen.Size = new System.Drawing.Size(153, 17);
            this.chkLastTen.TabIndex = 37;
            this.chkLastTen.Text = "Show only the last ten";
            this.chkLastTen.UseVisualStyleBackColor = true;
            this.chkLastTen.Click += new System.EventHandler(this.RestoreFormLoad);
            // 
            // RestoreBackupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 329);
            this.Controls.Add(this.chkLastTen);
            this.Controls.Add(this.lvBackups);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRestore);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RestoreBackupForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Restore backup";
            this.Load += new System.EventHandler(this.RestoreFormLoad);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RestoreBackupFormKeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.ListView lvBackups;
        private System.Windows.Forms.ColumnHeader chFileDate;
        private System.Windows.Forms.CheckBox chkLastTen;

    }
}