using System.ComponentModel;

namespace GMechanics.FlowchartControl
{
    partial class FlowchartItem
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            // FlowchartItem
            // 
            this.AllowDrop = true;
            this.Location = new System.Drawing.Point(30, 30);
            this.Name = "FlowchartItem";
            this.Size = new System.Drawing.Size(238, 58);
            this.MouseLeave += new System.EventHandler(this.FlowchartItemMouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FlowchartItemMouseMove);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FlowchartItemDragDrop);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FlowchartItemMouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FlowchartItemMouseUp);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FlowchartItemDragEnter);
            this.ResumeLayout(false);

        }

        #endregion

    }
}
