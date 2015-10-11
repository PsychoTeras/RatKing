namespace GMechanics.Editor.Forms.InteractiveManage
{
    partial class InteractiveRecipientsManageForm
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
            this.btnOk = new System.Windows.Forms.Button();
            this.pValues = new System.Windows.Forms.Panel();
            this.lvRecipients = new System.Windows.Forms.ListView();
            this.headerFeature = new System.Windows.Forms.ColumnHeader();
            this.columnEvent = new System.Windows.Forms.ColumnHeader();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
            this.btnEdit = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.lblLoV = new System.Windows.Forms.Label();
            this.pValues.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(307, 303);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnOk.Location = new System.Drawing.Point(226, 303);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "Save";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOkClick);
            // 
            // pValues
            // 
            this.pValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pValues.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pValues.Controls.Add(this.lvRecipients);
            this.pValues.Controls.Add(this.toolStrip);
            this.pValues.Location = new System.Drawing.Point(15, 28);
            this.pValues.Name = "pValues";
            this.pValues.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.pValues.Size = new System.Drawing.Size(367, 263);
            this.pValues.TabIndex = 36;
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
            this.lvRecipients.Location = new System.Drawing.Point(-1, 24);
            this.lvRecipients.MultiSelect = false;
            this.lvRecipients.Name = "lvRecipients";
            this.lvRecipients.ShowItemToolTips = true;
            this.lvRecipients.Size = new System.Drawing.Size(367, 238);
            this.lvRecipients.TabIndex = 1;
            this.lvRecipients.UseCompatibleStateImageBehavior = false;
            this.lvRecipients.View = System.Windows.Forms.View.Details;
            this.lvRecipients.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LvRecipientsMouseDoubleClick);
            this.lvRecipients.SelectedIndexChanged += new System.EventHandler(this.LvRecipientsSelectedIndexChanged);
            this.lvRecipients.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.LvRecipientsColumnClick);
            // 
            // headerFeature
            // 
            this.headerFeature.Text = "Feature name";
            this.headerFeature.Width = 219;
            // 
            // columnEvent
            // 
            this.columnEvent.Text = "Event type";
            this.columnEvent.Width = 123;
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAdd,
            this.btnEdit,
            this.btnDelete});
            this.toolStrip.Location = new System.Drawing.Point(1, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(364, 25);
            this.toolStrip.TabIndex = 1;
            // 
            // btnAdd
            // 
            this.btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAdd.Image = global::GMechanics.Editor.Properties.Resources.flash_add;
            this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(23, 22);
            this.btnAdd.Text = "Add interactive recipient (F5)";
            this.btnAdd.Click += new System.EventHandler(this.BtnAddClick);
            // 
            // btnEdit
            // 
            this.btnEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEdit.Image = global::GMechanics.Editor.Properties.Resources.flash_edit;
            this.btnEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(23, 22);
            this.btnEdit.Text = "Edit selected interactive recipient (F6)";
            this.btnEdit.Click += new System.EventHandler(this.BtnEditClick);
            // 
            // btnDelete
            // 
            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDelete.Image = global::GMechanics.Editor.Properties.Resources.flash_delete;
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(23, 22);
            this.btnDelete.Text = "Delete selected interactive recipient (F8, Del)";
            this.btnDelete.Click += new System.EventHandler(this.BtnDeleteClick);
            // 
            // lblLoV
            // 
            this.lblLoV.AutoSize = true;
            this.lblLoV.Location = new System.Drawing.Point(12, 12);
            this.lblLoV.Name = "lblLoV";
            this.lblLoV.Size = new System.Drawing.Size(90, 13);
            this.lblLoV.TabIndex = 37;
            this.lblLoV.Text = "Recipients list:";
            // 
            // InteractiveRecipientsManageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 338);
            this.Controls.Add(this.lblLoV);
            this.Controls.Add(this.pValues);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "InteractiveRecipientsManageForm";
            this.Padding = new System.Windows.Forms.Padding(12, 12, 12, 0);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage interactive recipients";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.InteractiveRecipientsManageFormFormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InteractiveRecipientsManageFormKeyDown);
            this.pValues.ResumeLayout(false);
            this.pValues.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel pValues;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnAdd;
        private System.Windows.Forms.ToolStripButton btnEdit;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.ListView lvRecipients;
        private System.Windows.Forms.ColumnHeader headerFeature;
        private System.Windows.Forms.ColumnHeader columnEvent;
        private System.Windows.Forms.Label lblLoV;
    }
}