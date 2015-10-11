using GMechanics.Editor.UIControls.Common;
using GMechanics.Editor.UIControls.TreeViewSearchBox;

namespace GMechanics.Editor.UIControls.TreeViews
{
    partial class GameObjectsTreeView
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Elementary game objects", 0, 0);
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Game objects", 0, 0);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameObjectsTreeView));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnAddGameObjectGroup = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAddGameObject = new System.Windows.Forms.ToolStripButton();
            this.btnEditGameObject = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.tvGameObjects = new TreeViewEx();
            this.imageListGameObjectsTv = new System.Windows.Forms.ImageList(this.components);
            this.sbMain = new TreeViewSearchBox.TreeViewSearchBox();
            this.pSplitter = new System.Windows.Forms.Panel();
            this.pTreeView = new System.Windows.Forms.Panel();
            this.toolStrip.SuspendLayout();
            this.pTreeView.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.AutoSize = false;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddGameObjectGroup,
            this.toolStripSeparator2,
            this.btnAddGameObject,
            this.btnEditGameObject,
            this.toolStripSeparator1,
            this.btnDelete});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.toolStrip.Size = new System.Drawing.Size(206, 25);
            this.toolStrip.TabIndex = 6;
            this.toolStrip.Resize += new System.EventHandler(this.ToolStripResize);
            // 
            // btnAddGameObjectGroup
            // 
            this.btnAddGameObjectGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddGameObjectGroup.Image = global::GMechanics.Editor.Properties.Resources.folder_add;
            this.btnAddGameObjectGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddGameObjectGroup.Name = "btnAddGameObjectGroup";
            this.btnAddGameObjectGroup.Size = new System.Drawing.Size(23, 22);
            this.btnAddGameObjectGroup.Click += new System.EventHandler(this.BtnAddGameObjectGroupClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnAddGameObject
            // 
            this.btnAddGameObject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddGameObject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddGameObject.Name = "btnAddGameObject";
            this.btnAddGameObject.Size = new System.Drawing.Size(23, 22);
            this.btnAddGameObject.Click += new System.EventHandler(this.BtnAddGameObjectClick);
            // 
            // btnEditGameObject
            // 
            this.btnEditGameObject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEditGameObject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditGameObject.Name = "btnEditGameObject";
            this.btnEditGameObject.Size = new System.Drawing.Size(23, 22);
            this.btnEditGameObject.Click += new System.EventHandler(this.BtnEditGameObjectClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnDelete
            // 
            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDelete.Image = global::GMechanics.Editor.Properties.Resources.delete;
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(23, 22);
            this.btnDelete.ToolTipText = "Delete selected group or object (F8, Delete)";
            this.btnDelete.Click += new System.EventHandler(this.BtnDeleteClick);
            // 
            // tvGameObjects
            // 
            this.tvGameObjects.BackColor = System.Drawing.SystemColors.Window;
            this.tvGameObjects.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvGameObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvGameObjects.FullRowSelect = true;
            this.tvGameObjects.HideSelection = false;
            this.tvGameObjects.ImageIndex = 0;
            this.tvGameObjects.ImageList = this.imageListGameObjectsTv;
            this.tvGameObjects.Location = new System.Drawing.Point(0, 1);
            this.tvGameObjects.Name = "tvGameObjects";
            treeNode1.ImageIndex = 0;
            treeNode1.Name = "Node0";
            treeNode1.SelectedImageIndex = 0;
            treeNode1.Text = "Elementary game objects";
            treeNode2.ImageIndex = 0;
            treeNode2.Name = "Node0";
            treeNode2.SelectedImageIndex = 0;
            treeNode2.Text = "Game objects";
            this.tvGameObjects.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            this.tvGameObjects.SelectedImageIndex = 0;
            this.tvGameObjects.ShowNodeToolTips = true;
            this.tvGameObjects.Size = new System.Drawing.Size(206, 202);
            this.tvGameObjects.TabIndex = 14;
            this.tvGameObjects.DoubleClick += new System.EventHandler(this.TvGameObjectsDoubleClick);
            this.tvGameObjects.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TvGameObjectsAfterSelect);
            this.tvGameObjects.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TvGameObjectsMouseDown);
            this.tvGameObjects.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TvGameObjectsKeyDown);
            this.tvGameObjects.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.TvGameObjectsItemDrag);
            // 
            // imageListGameObjectsTv
            // 
            this.imageListGameObjectsTv.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListGameObjectsTv.ImageStream")));
            this.imageListGameObjectsTv.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListGameObjectsTv.Images.SetKeyName(0, "bullet_arrow_right.png");
            this.imageListGameObjectsTv.Images.SetKeyName(1, "folder2 copy.png");
            this.imageListGameObjectsTv.Images.SetKeyName(2, "cube_molecule.png");
            this.imageListGameObjectsTv.Images.SetKeyName(3, "cube_molecule_add.png");
            this.imageListGameObjectsTv.Images.SetKeyName(4, "cube_molecule_edit.png");
            this.imageListGameObjectsTv.Images.SetKeyName(5, "folder_cube_yellow.png");
            this.imageListGameObjectsTv.Images.SetKeyName(6, "cube_yellow.png");
            this.imageListGameObjectsTv.Images.SetKeyName(7, "cube_yellow_add.png");
            this.imageListGameObjectsTv.Images.SetKeyName(8, "cube_yellow_edit.png");
            this.imageListGameObjectsTv.Images.SetKeyName(9, "check.png");
            // 
            // sbMain
            // 
            this.sbMain.ActiveColor = System.Drawing.Color.Gray;
            this.sbMain.ClearButtonToolTipToolTip = "Cancel search (Escape)";
            this.sbMain.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.sbMain.InactiveColor = System.Drawing.Color.LightGray;
            this.sbMain.Location = new System.Drawing.Point(113, 2);
            this.sbMain.MaximumSize = new System.Drawing.Size(200, 20);
            this.sbMain.MinimumSize = new System.Drawing.Size(100, 20);
            this.sbMain.MouseOnControl = System.Drawing.Color.Silver;
            this.sbMain.Name = "sbMain";
            this.sbMain.Padding = new System.Windows.Forms.Padding(1);
            this.sbMain.SearchLevel = SearchLevel.FirstChild;
            this.sbMain.Size = new System.Drawing.Size(100, 20);
            this.sbMain.TabIndex = 12;
            this.sbMain.ToolTip = "Quick search (Ctrl+Q)";
            this.sbMain.TreeView = this.tvGameObjects;
            this.sbMain.FilterNode += new TreeViewSearchBox.TreeViewSearchBox.OnFilterNodeEvent(this.SbMainFilterNode);
            // 
            // pSplitter
            // 
            this.pSplitter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pSplitter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pSplitter.Location = new System.Drawing.Point(0, 24);
            this.pSplitter.Name = "pSplitter";
            this.pSplitter.Size = new System.Drawing.Size(206, 1);
            this.pSplitter.TabIndex = 14;
            // 
            // pTreeView
            // 
            this.pTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pTreeView.Controls.Add(this.tvGameObjects);
            this.pTreeView.ImeMode = System.Windows.Forms.ImeMode.On;
            this.pTreeView.Location = new System.Drawing.Point(0, 25);
            this.pTreeView.Name = "pTreeView";
            this.pTreeView.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.pTreeView.Size = new System.Drawing.Size(206, 203);
            this.pTreeView.TabIndex = 15;
            // 
            // GameObjectsTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pSplitter);
            this.Controls.Add(this.pTreeView);
            this.Controls.Add(this.sbMain);
            this.Controls.Add(this.toolStrip);
            this.Name = "GameObjectsTreeView";
            this.Size = new System.Drawing.Size(206, 228);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.pTreeView.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private TreeViewSearchBox.TreeViewSearchBox sbMain;
        private System.Windows.Forms.Panel pSplitter;
        private System.Windows.Forms.ImageList imageListGameObjectsTv;
        private System.Windows.Forms.ToolStripButton btnAddGameObjectGroup;
        private System.Windows.Forms.ToolStripButton btnAddGameObject;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnEditGameObject;
        private System.Windows.Forms.Panel pTreeView;
        private TreeViewEx tvGameObjects;
    }
}
