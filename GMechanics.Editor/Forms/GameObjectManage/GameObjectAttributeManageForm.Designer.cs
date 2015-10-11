namespace GMechanics.Editor.Forms.GameObjectManage
{
    sealed partial class GameObjectAttributeManageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameObjectAttributeManageForm));
            this.pValues = new System.Windows.Forms.Panel();
            this.dgValues = new System.Windows.Forms.DataGridView();
            this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTranscription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.comMinValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMaxValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tsButtons = new System.Windows.Forms.ToolStrip();
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
            this.btnEdit = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.lblLoV = new System.Windows.Forms.Label();
            this.pAttributeSettings = new System.Windows.Forms.Panel();
            this.pDivider = new System.Windows.Forms.Panel();
            this.tbTranscription = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbAttributeName = new System.Windows.Forms.TextBox();
            this.lblPropertyName = new System.Windows.Forms.Label();
            this.pValues.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgValues)).BeginInit();
            this.tsButtons.SuspendLayout();
            this.pAttributeSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // pValues
            // 
            this.pValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pValues.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pValues.Controls.Add(this.dgValues);
            this.pValues.Controls.Add(this.tsButtons);
            this.pValues.Location = new System.Drawing.Point(15, 98);
            this.pValues.Name = "pValues";
            this.pValues.Size = new System.Drawing.Size(330, 210);
            this.pValues.TabIndex = 31;
            // 
            // dgValues
            // 
            this.dgValues.AllowUserToAddRows = false;
            this.dgValues.AllowUserToDeleteRows = false;
            this.dgValues.AllowUserToResizeRows = false;
            this.dgValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgValues.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.dgValues.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgValues.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgValues.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dgValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgValues.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colValue,
            this.colTranscription,
            this.comMinValue,
            this.colMaxValue});
            this.dgValues.EnableHeadersVisualStyles = false;
            this.dgValues.Location = new System.Drawing.Point(-1, 24);
            this.dgValues.MultiSelect = false;
            this.dgValues.Name = "dgValues";
            this.dgValues.RowHeadersVisible = false;
            this.dgValues.RowHeadersWidth = 16;
            this.dgValues.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgValues.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgValues.Size = new System.Drawing.Size(330, 183);
            this.dgValues.TabIndex = 32;
            this.dgValues.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgValuesCellValueChanged);
            this.dgValues.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.DgValuesCellValidating);
            this.dgValues.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgValuesCellEndEdit);
            this.dgValues.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DgValuesKeyDown);
            this.dgValues.SelectionChanged += new System.EventHandler(this.DgValuesSelectionChanged);
            // 
            // colValue
            // 
            this.colValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colValue.DataPropertyName = "Name";
            this.colValue.HeaderText = "Name";
            this.colValue.MaxInputLength = 50;
            this.colValue.Name = "colValue";
            // 
            // colTranscription
            // 
            this.colTranscription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colTranscription.DataPropertyName = "Transcription";
            this.colTranscription.HeaderText = "Transcription";
            this.colTranscription.MaxInputLength = 50;
            this.colTranscription.Name = "colTranscription";
            // 
            // comMinValue
            // 
            this.comMinValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.comMinValue.DataPropertyName = "MinValue";
            this.comMinValue.FillWeight = 50F;
            this.comMinValue.HeaderText = "Min. value";
            this.comMinValue.MaxInputLength = 20;
            this.comMinValue.Name = "comMinValue";
            // 
            // colMaxValue
            // 
            this.colMaxValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colMaxValue.DataPropertyName = "MaxValue";
            this.colMaxValue.FillWeight = 50F;
            this.colMaxValue.HeaderText = "Max. value";
            this.colMaxValue.MaxInputLength = 20;
            this.colMaxValue.Name = "colMaxValue";
            // 
            // tsButtons
            // 
            this.tsButtons.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsButtons.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAdd,
            this.btnEdit,
            this.btnDelete});
            this.tsButtons.Location = new System.Drawing.Point(0, 0);
            this.tsButtons.Name = "tsButtons";
            this.tsButtons.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.tsButtons.Size = new System.Drawing.Size(328, 25);
            this.tsButtons.TabIndex = 33;
            // 
            // btnAdd
            // 
            this.btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(23, 22);
            this.btnAdd.Text = "Add new record";
            this.btnAdd.ToolTipText = "Add new record (Insert)";
            this.btnAdd.Click += new System.EventHandler(this.BtnAddClick);
            // 
            // btnEdit
            // 
            this.btnEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEdit.Image = ((System.Drawing.Image)(resources.GetObject("btnEdit.Image")));
            this.btnEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(23, 22);
            this.btnEdit.Text = "Edit selected cell";
            this.btnEdit.ToolTipText = "Edit selected cell (F2)";
            this.btnEdit.Click += new System.EventHandler(this.BtnEditClick);
            // 
            // btnDelete
            // 
            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(23, 22);
            this.btnDelete.Text = "Delete selected record";
            this.btnDelete.ToolTipText = "Delete selected record (Delete)";
            this.btnDelete.Click += new System.EventHandler(this.BtnDeleteClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(270, 319);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 33;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancelClick);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnOk.Location = new System.Drawing.Point(189, 319);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 32;
            this.btnOk.Text = "Save";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOkClick);
            // 
            // lblLoV
            // 
            this.lblLoV.AutoSize = true;
            this.lblLoV.Location = new System.Drawing.Point(12, 82);
            this.lblLoV.Name = "lblLoV";
            this.lblLoV.Size = new System.Drawing.Size(119, 13);
            this.lblLoV.TabIndex = 34;
            this.lblLoV.Text = "Possible values list:";
            // 
            // pAttributeSettings
            // 
            this.pAttributeSettings.Controls.Add(this.pDivider);
            this.pAttributeSettings.Controls.Add(this.tbTranscription);
            this.pAttributeSettings.Controls.Add(this.label1);
            this.pAttributeSettings.Controls.Add(this.tbAttributeName);
            this.pAttributeSettings.Controls.Add(this.lblPropertyName);
            this.pAttributeSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.pAttributeSettings.Location = new System.Drawing.Point(0, 0);
            this.pAttributeSettings.Name = "pAttributeSettings";
            this.pAttributeSettings.Size = new System.Drawing.Size(357, 73);
            this.pAttributeSettings.TabIndex = 35;
            // 
            // pDivider
            // 
            this.pDivider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pDivider.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pDivider.Location = new System.Drawing.Point(15, 72);
            this.pDivider.Name = "pDivider";
            this.pDivider.Size = new System.Drawing.Size(329, 1);
            this.pDivider.TabIndex = 32;
            // 
            // tbTranscription
            // 
            this.tbTranscription.AcceptsTab = true;
            this.tbTranscription.AllowDrop = true;
            this.tbTranscription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTranscription.Location = new System.Drawing.Point(115, 39);
            this.tbTranscription.MaxLength = 100;
            this.tbTranscription.Name = "tbTranscription";
            this.tbTranscription.Size = new System.Drawing.Size(230, 21);
            this.tbTranscription.TabIndex = 29;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "Transcription:";
            // 
            // tbAttributeName
            // 
            this.tbAttributeName.AcceptsTab = true;
            this.tbAttributeName.AllowDrop = true;
            this.tbAttributeName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAttributeName.Location = new System.Drawing.Point(115, 12);
            this.tbAttributeName.MaxLength = 50;
            this.tbAttributeName.Name = "tbAttributeName";
            this.tbAttributeName.Size = new System.Drawing.Size(230, 21);
            this.tbAttributeName.TabIndex = 28;
            this.tbAttributeName.TextChanged += new System.EventHandler(this.TbAttributeNameTextChanged);
            // 
            // lblPropertyName
            // 
            this.lblPropertyName.AutoSize = true;
            this.lblPropertyName.Location = new System.Drawing.Point(12, 15);
            this.lblPropertyName.Name = "lblPropertyName";
            this.lblPropertyName.Size = new System.Drawing.Size(97, 13);
            this.lblPropertyName.TabIndex = 30;
            this.lblPropertyName.Text = "Attribute name:";
            // 
            // GameObjectAttributeManageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 354);
            this.Controls.Add(this.pAttributeSettings);
            this.Controls.Add(this.lblLoV);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.pValues);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 293);
            this.Name = "GameObjectAttributeManageForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Attribute manage";
            this.Load += new System.EventHandler(this.GameObjectAttributeManageFormLoad);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GameObjectAttributeManageFormFormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GameObjectAttributeManageFormKeyDown);
            this.pValues.ResumeLayout(false);
            this.pValues.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgValues)).EndInit();
            this.tsButtons.ResumeLayout(false);
            this.tsButtons.PerformLayout();
            this.pAttributeSettings.ResumeLayout(false);
            this.pAttributeSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pValues;
        private System.Windows.Forms.ToolStrip tsButtons;
        private System.Windows.Forms.DataGridView dgValues;
        private System.Windows.Forms.ToolStripButton btnAdd;
        private System.Windows.Forms.ToolStripButton btnEdit;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label lblLoV;
        private System.Windows.Forms.Panel pAttributeSettings;
        private System.Windows.Forms.Panel pDivider;
        public System.Windows.Forms.TextBox tbTranscription;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox tbAttributeName;
        private System.Windows.Forms.Label lblPropertyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTranscription;
        private System.Windows.Forms.DataGridViewTextBoxColumn comMinValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMaxValue;
    }
}