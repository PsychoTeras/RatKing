namespace GMechanics.Editor.UIControls.PropertyGridEditor
{
    partial class EntityPropertyGridEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // oDocCommentTitle
            // 
            this.oDocCommentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.oDocCommentTitle.Location = new System.Drawing.Point(3, 3);
            // 
            // oDocCommentDescription
            // 
            this.oDocCommentDescription.AccessibleName = "";
            this.oDocCommentDescription.Location = new System.Drawing.Point(3, 18);
            // 
            // EntityPropertyGridEditor
            // 
            this.AutoSizeProperties = true;
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DrawFlatToolbar = true;
            this.HelpVisible = false;
            this.Name = "EntityPropertiesGridEditor";
            this.Size = new System.Drawing.Size(150, 150);
            this.TabIndex = 4;
            this.ToolbarVisible = false;
            this.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.OnPropertyValueChanged);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
