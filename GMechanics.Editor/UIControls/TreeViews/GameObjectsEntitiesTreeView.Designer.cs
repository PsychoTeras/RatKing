using GMechanics.Editor.UIControls.Common;

namespace GMechanics.Editor.UIControls.TreeViews
{
    partial class GameObjectsEntitiesTreeView
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Attributes", 0, 0);
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Properties", 0, 0);
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Features", 0, 0);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameObjectsEntitiesTreeView));
            this.tvObjectsEntities = new TreeViewEx();
            this.ImageList = new System.Windows.Forms.ImageList(this.components);
            this.pSplitter = new System.Windows.Forms.Panel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.pTreeView = new System.Windows.Forms.Panel();
            this.sbMain = new TreeViewSearchBox.TreeViewSearchBox();
            this.toolStrip.SuspendLayout();
            this.pTreeView.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvObjectsEntities
            // 
            this.tvObjectsEntities.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvObjectsEntities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvObjectsEntities.FullRowSelect = true;
            this.tvObjectsEntities.HideSelection = false;
            this.tvObjectsEntities.ImageIndex = 0;
            this.tvObjectsEntities.ImageList = this.ImageList;
            this.tvObjectsEntities.Location = new System.Drawing.Point(0, 1);
            this.tvObjectsEntities.Name = "tvObjectsEntities";
            treeNode1.ImageIndex = 0;
            treeNode1.Name = "Node0";
            treeNode1.SelectedImageIndex = 0;
            treeNode1.Text = "Attributes";
            treeNode2.ImageIndex = 0;
            treeNode2.Name = "Node1";
            treeNode2.SelectedImageIndex = 0;
            treeNode2.Text = "Properties";
            treeNode3.ImageIndex = 0;
            treeNode3.Name = "Node2";
            treeNode3.SelectedImageIndex = 0;
            treeNode3.Text = "Features";
            this.tvObjectsEntities.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            this.tvObjectsEntities.SelectedImageIndex = 0;
            this.tvObjectsEntities.ShowNodeToolTips = true;
            this.tvObjectsEntities.Size = new System.Drawing.Size(206, 202);
            this.tvObjectsEntities.TabIndex = 5;
            this.tvObjectsEntities.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TvObjectsAfterSelect);
            this.tvObjectsEntities.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TvObjectsMouseDown);
            this.tvObjectsEntities.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TvObjectsKeyDown);
            this.tvObjectsEntities.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.TvObjectsItemDrag);
            // 
            // ImageList
            // 
            this.ImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImageList.ImageStream")));
            this.ImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ImageList.Images.SetKeyName(0, "bullet_arrow_right.png");
            // 
            // pSplitter
            // 
            this.pSplitter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pSplitter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pSplitter.Location = new System.Drawing.Point(0, 24);
            this.pSplitter.Name = "pSplitter";
            this.pSplitter.Size = new System.Drawing.Size(206, 1);
            this.pSplitter.TabIndex = 9;
            // 
            // toolStrip
            // 
            this.toolStrip.AutoSize = false;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRefresh});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.toolStrip.Size = new System.Drawing.Size(206, 25);
            this.toolStrip.TabIndex = 8;
            this.toolStrip.Resize += new System.EventHandler(this.ToolStripResize);
            // 
            // btnRefresh
            // 
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRefresh.Image = global::GMechanics.Editor.Properties.Resources.refresh;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(23, 22);
            this.btnRefresh.Text = "Refresh (F3)";
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefreshClick);
            // 
            // pTreeView
            // 
            this.pTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pTreeView.Controls.Add(this.tvObjectsEntities);
            this.pTreeView.Location = new System.Drawing.Point(0, 25);
            this.pTreeView.Name = "pTreeView";
            this.pTreeView.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.pTreeView.Size = new System.Drawing.Size(206, 203);
            this.pTreeView.TabIndex = 10;
            // 
            // sbMain
            // 
            this.sbMain.ActiveColor = System.Drawing.Color.Gray;
            this.sbMain.ClearButtonToolTipToolTip = "Cancel search (Escape)";
            this.sbMain.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.sbMain.InactiveColor = System.Drawing.Color.LightGray;
            this.sbMain.Location = new System.Drawing.Point(103, 2);
            this.sbMain.MaximumSize = new System.Drawing.Size(200, 20);
            this.sbMain.MinimumSize = new System.Drawing.Size(100, 20);
            this.sbMain.MouseOnControl = System.Drawing.Color.Silver;
            this.sbMain.Name = "sbMain";
            this.sbMain.Padding = new System.Windows.Forms.Padding(1);
            this.sbMain.Size = new System.Drawing.Size(100, 20);
            this.sbMain.TabIndex = 11;
            this.sbMain.ToolTip = "Quick search (Ctrl+Q)";
            this.sbMain.TreeView = this.tvObjectsEntities;
            this.sbMain.FilterNode += new TreeViewSearchBox.TreeViewSearchBox.OnFilterNodeEvent(this.SbMainFilterNode);
            // 
            // GameObjectsEntitiesTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pSplitter);
            this.Controls.Add(this.sbMain);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.pTreeView);
            this.Name = "GameObjectsEntitiesTreeView";
            this.Size = new System.Drawing.Size(206, 228);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.pTreeView.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TreeViewEx tvObjectsEntities;
        private System.Windows.Forms.Panel pSplitter;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.Panel pTreeView;
        private TreeViewSearchBox.TreeViewSearchBox sbMain;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        public System.Windows.Forms.ImageList ImageList;

    }
}
