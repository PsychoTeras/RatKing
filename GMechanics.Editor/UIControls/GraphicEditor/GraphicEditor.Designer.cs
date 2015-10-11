namespace GMechanics.Editor.UIControls.GraphicEditor
{
    partial class GraphicEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphicEditor));
            this.ilEntities = new System.Windows.Forms.ImageList(this.components);
            this.lblNoGameObject = new System.Windows.Forms.Label();
            this.menuFlowchartItemElement = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemRemoveElement = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemRemoveItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ilMenu = new System.Windows.Forms.ImageList(this.components);
            this.menuFlowchartItemElement.SuspendLayout();
            this.SuspendLayout();
            // 
            // ilEntities
            // 
            this.ilEntities.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilEntities.ImageStream")));
            this.ilEntities.TransparentColor = System.Drawing.Color.Transparent;
            this.ilEntities.Images.SetKeyName(0, "key1.png");
            this.ilEntities.Images.SetKeyName(1, "component_yellow.png");
            this.ilEntities.Images.SetKeyName(2, "star_yellow.png");
            this.ilEntities.Images.SetKeyName(3, "cube_molecule.png");
            this.ilEntities.Images.SetKeyName(4, "cube_yellow.png");
            this.ilEntities.Images.SetKeyName(5, "branch_element.png");
            this.ilEntities.Images.SetKeyName(6, "graph_edge_directed.png");
            // 
            // lblNoGameObject
            // 
            this.lblNoGameObject.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblNoGameObject.AutoSize = true;
            this.lblNoGameObject.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblNoGameObject.ForeColor = System.Drawing.Color.DarkGray;
            this.lblNoGameObject.Location = new System.Drawing.Point(58, 145);
            this.lblNoGameObject.Name = "lblNoGameObject";
            this.lblNoGameObject.Size = new System.Drawing.Size(210, 18);
            this.lblNoGameObject.TabIndex = 0;
            this.lblNoGameObject.Text = "No game object selected";
            // 
            // menuFlowchartItemElement
            // 
            this.menuFlowchartItemElement.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemRemoveElement,
            this.toolStripMenuItem1,
            this.menuItemRemoveItem});
            this.menuFlowchartItemElement.Name = "menuFlowchartItemElement";
            this.menuFlowchartItemElement.Size = new System.Drawing.Size(164, 54);
            this.menuFlowchartItemElement.Opening += new System.ComponentModel.CancelEventHandler(this.MenuFlowchartItemElementOpening);
            // 
            // menuItemRemoveElement
            // 
            this.menuItemRemoveElement.Name = "menuItemRemoveElement";
            this.menuItemRemoveElement.Size = new System.Drawing.Size(163, 22);
            this.menuItemRemoveElement.Text = "Remove element";
            this.menuItemRemoveElement.Click += new System.EventHandler(this.MenuItemRemoveElementClick);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(160, 6);
            // 
            // menuItemRemoveItem
            // 
            this.menuItemRemoveItem.Name = "menuItemRemoveItem";
            this.menuItemRemoveItem.Size = new System.Drawing.Size(163, 22);
            this.menuItemRemoveItem.Text = "Remove item";
            this.menuItemRemoveItem.Click += new System.EventHandler(this.MenuItemRemoveItemClick);
            // 
            // ilMenu
            // 
            this.ilMenu.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilMenu.ImageStream")));
            this.ilMenu.TransparentColor = System.Drawing.Color.Transparent;
            this.ilMenu.Images.SetKeyName(0, "key1_delete.png");
            this.ilMenu.Images.SetKeyName(1, "component_yellow_delete.png");
            this.ilMenu.Images.SetKeyName(2, "star_yellow_delete.png");
            this.ilMenu.Images.SetKeyName(3, "cube_molecule_delete.png");
            this.ilMenu.Images.SetKeyName(4, "cube_yellow_delete.png");
            // 
            // GraphicEditor
            // 
            this.BackColor = System.Drawing.Color.DimGray;
            this.Controls.Add(this.lblNoGameObject);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "GraphicEditor";
            this.Size = new System.Drawing.Size(327, 308);
            this.Load += new System.EventHandler(this.GraphicEditorLoad);
            this.menuFlowchartItemElement.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList ilEntities;
        private System.Windows.Forms.Label lblNoGameObject;
        private System.Windows.Forms.ContextMenuStrip menuFlowchartItemElement;
        private System.Windows.Forms.ToolStripMenuItem menuItemRemoveElement;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem menuItemRemoveItem;
        private System.Windows.Forms.ImageList ilMenu;
    }
}
