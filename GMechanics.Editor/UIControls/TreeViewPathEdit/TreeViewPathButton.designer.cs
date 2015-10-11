using GMechanics.Editor.UIControls.Common;

namespace GMechanics.Editor.UIControls.TreeViewPathEdit
{
    sealed partial class TreeViewPathButton
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TreeViewPathButton));
            this.lblText = new System.Windows.Forms.Label();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.pbArrow = new System.Windows.Forms.PictureBox();
            this.ilArrows = new System.Windows.Forms.ImageList(this.components);
            this.pForeground = new TransparentPanel();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbArrow)).BeginInit();
            this.SuspendLayout();
            // 
            // lblText
            // 
            this.lblText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblText.BackColor = System.Drawing.Color.Transparent;
            this.lblText.ForeColor = System.Drawing.Color.Black;
            this.lblText.Location = new System.Drawing.Point(21, 0);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(61, 21);
            this.lblText.TabIndex = 1;
            this.lblText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pbIcon
            // 
            this.pbIcon.BackColor = System.Drawing.Color.Transparent;
            this.pbIcon.Location = new System.Drawing.Point(4, 4);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(16, 16);
            this.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbIcon.TabIndex = 0;
            this.pbIcon.TabStop = false;
            this.pbIcon.VisibleChanged += new System.EventHandler(this.PbIconVisibleChanged);
            this.pbIcon.Paint += new System.Windows.Forms.PaintEventHandler(this.PbIconPaint);
            // 
            // pbArrow
            // 
            this.pbArrow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbArrow.Location = new System.Drawing.Point(83, 0);
            this.pbArrow.Name = "pbArrow";
            this.pbArrow.Size = new System.Drawing.Size(17, 22);
            this.pbArrow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbArrow.TabIndex = 3;
            this.pbArrow.TabStop = false;
            this.pbArrow.VisibleChanged += new System.EventHandler(this.PbArrowVisibleChanged);
            // 
            // ilArrows
            // 
            this.ilArrows.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilArrows.ImageStream")));
            this.ilArrows.TransparentColor = System.Drawing.Color.Transparent;
            this.ilArrows.Images.SetKeyName(0, "1.png");
            this.ilArrows.Images.SetKeyName(1, "4.png");
            // 
            // pForeground
            // 
            this.pForeground.BackColor = System.Drawing.Color.Transparent;
            this.pForeground.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pForeground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pForeground.Location = new System.Drawing.Point(0, 0);
            this.pForeground.Name = "pForeground";
            this.pForeground.Size = new System.Drawing.Size(100, 22);
            this.pForeground.TabIndex = 2;
            this.pForeground.MouseLeave += new System.EventHandler(this.ImageButtonMouseLeave);
            this.pForeground.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PForegroundMouseMove);
            this.pForeground.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PForegroundMouseDown);
            this.pForeground.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PForegroundMouseUp);
            this.pForeground.MouseEnter += new System.EventHandler(this.ImageButtonMouseEnter);
            // 
            // TreeViewPathButton
            // 
            this.Controls.Add(this.pForeground);
            this.Controls.Add(this.pbIcon);
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.pbArrow);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "TreeViewPathButton";
            this.Size = new System.Drawing.Size(100, 22);
            this.MouseLeave += new System.EventHandler(this.ImageButtonMouseLeave);
            this.MouseEnter += new System.EventHandler(this.ImageButtonMouseEnter);
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbArrow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbIcon;
        private System.Windows.Forms.Label lblText;
        private TransparentPanel pForeground;
        private System.Windows.Forms.PictureBox pbArrow;
        private System.Windows.Forms.ImageList ilArrows;
    }
}
