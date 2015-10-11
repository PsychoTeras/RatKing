using GMechanics.Editor.UIControls.Common;
using GMechanics.Editor.UIControls.TreeViewSearchBox;

namespace GMechanics.Editor.UIControls.TreeViews
{
    partial class FeaturesTreeView
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Features");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FeaturesTreeView));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnAddFeatureClass = new System.Windows.Forms.ToolStripButton();
            this.btnAddFeature = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnManageInteractiveRecipients = new System.Windows.Forms.ToolStripButton();
            this.btnEditScript = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnDeleteFeature = new System.Windows.Forms.ToolStripButton();
            this.pSplitter = new System.Windows.Forms.Panel();
            this.pTreeView = new System.Windows.Forms.Panel();
            this.tvFeatures = new TreeViewEx();
            imageListFeaturesTv = new System.Windows.Forms.ImageList(this.components);
            this.sbMain = new TreeViewSearchBox.TreeViewSearchBox();
            this.toolStrip.SuspendLayout();
            this.pTreeView.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.AutoSize = false;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddFeatureClass,
            this.btnAddFeature,
            this.toolStripSeparator1,
            this.btnManageInteractiveRecipients,
            this.btnEditScript,
            this.toolStripSeparator2,
            this.btnDeleteFeature});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.toolStrip.Size = new System.Drawing.Size(215, 25);
            this.toolStrip.TabIndex = 5;
            this.toolStrip.Resize += new System.EventHandler(this.ToolStripResize);
            // 
            // btnAddFeatureClass
            // 
            this.btnAddFeatureClass.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.btnAddFeatureClass.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddFeatureClass.Image = global::GMechanics.Editor.Properties.Resources.folder_add;
            this.btnAddFeatureClass.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddFeatureClass.Name = "btnAddFeatureClass";
            this.btnAddFeatureClass.Size = new System.Drawing.Size(23, 22);
            this.btnAddFeatureClass.Text = "Add new feature class";
            this.btnAddFeatureClass.ToolTipText = "Add new feature class (F4)";
            this.btnAddFeatureClass.Click += new System.EventHandler(this.BtnAddGameObjectFeatureClassClick);
            // 
            // btnAddFeature
            // 
            this.btnAddFeature.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.btnAddFeature.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddFeature.Image = global::GMechanics.Editor.Properties.Resources.star_yellow_add;
            this.btnAddFeature.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddFeature.Name = "btnAddFeature";
            this.btnAddFeature.Size = new System.Drawing.Size(23, 22);
            this.btnAddFeature.Text = "Add new feature";
            this.btnAddFeature.ToolTipText = "Add new feature (F5)";
            this.btnAddFeature.Click += new System.EventHandler(this.BtnAddGameObjectFeatureClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnManageInteractiveRecipients
            // 
            this.btnManageInteractiveRecipients.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnManageInteractiveRecipients.Image = global::GMechanics.Editor.Properties.Resources.flash;
            this.btnManageInteractiveRecipients.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnManageInteractiveRecipients.Name = "btnManageInteractiveRecipients";
            this.btnManageInteractiveRecipients.Size = new System.Drawing.Size(23, 22);
            this.btnManageInteractiveRecipients.Text = "Manage interactive recipients";
            this.btnManageInteractiveRecipients.ToolTipText = "Manage interactive recipients (F7)";
            this.btnManageInteractiveRecipients.Click += new System.EventHandler(this.BtnManageInteractiveRecipientsClick);
            // 
            // btnEditScript
            // 
            this.btnEditScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEditScript.Image = global::GMechanics.Editor.Properties.Resources.cog;
            this.btnEditScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditScript.Name = "btnEditScript";
            this.btnEditScript.Size = new System.Drawing.Size(23, 22);
            this.btnEditScript.Text = "Edit feature action script";
            this.btnEditScript.ToolTipText = "Edit feature action script (F6)";
            this.btnEditScript.Click += new System.EventHandler(this.BtnEditScriptClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnDeleteFeature
            // 
            this.btnDeleteFeature.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDeleteFeature.Image = global::GMechanics.Editor.Properties.Resources.delete;
            this.btnDeleteFeature.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteFeature.Name = "btnDeleteFeature";
            this.btnDeleteFeature.Size = new System.Drawing.Size(23, 22);
            this.btnDeleteFeature.Text = "Delete selected feature or class";
            this.btnDeleteFeature.ToolTipText = "Delete selected feature or class (F8, Delete)";
            this.btnDeleteFeature.Click += new System.EventHandler(this.BtnDeleteFeatureClick);
            // 
            // pSplitter
            // 
            this.pSplitter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pSplitter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pSplitter.Location = new System.Drawing.Point(0, 24);
            this.pSplitter.Name = "pSplitter";
            this.pSplitter.Size = new System.Drawing.Size(215, 1);
            this.pSplitter.TabIndex = 6;
            // 
            // pTreeView
            // 
            this.pTreeView.Controls.Add(this.tvFeatures);
            this.pTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pTreeView.Location = new System.Drawing.Point(0, 25);
            this.pTreeView.Name = "pTreeView";
            this.pTreeView.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.pTreeView.Size = new System.Drawing.Size(215, 317);
            this.pTreeView.TabIndex = 7;
            // 
            // tvFeatures
            // 
            this.tvFeatures.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvFeatures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFeatures.FullRowSelect = true;
            this.tvFeatures.HideSelection = false;
            this.tvFeatures.ImageIndex = 0;
            this.tvFeatures.ImageList = imageListFeaturesTv;
            this.tvFeatures.Location = new System.Drawing.Point(0, 1);
            this.tvFeatures.Name = "tvFeatures";
            treeNode1.Name = "Node0";
            treeNode1.Text = "Features";
            this.tvFeatures.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.tvFeatures.SelectedImageIndex = 0;
            this.tvFeatures.ShowNodeToolTips = true;
            this.tvFeatures.Size = new System.Drawing.Size(215, 316);
            this.tvFeatures.TabIndex = 5;
            this.tvFeatures.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TvFeaturesNodeMouseDoubleClick);
            this.tvFeatures.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TvFeaturesAfterSelect);
            this.tvFeatures.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TvFeaturesKeyDown);
            // 
            // imageListFeaturesTv
            // 
            imageListFeaturesTv.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListFeaturesTv.ImageStream")));
            imageListFeaturesTv.TransparentColor = System.Drawing.Color.Transparent;
            imageListFeaturesTv.Images.SetKeyName(0, "bullet_arrow_right.png");
            imageListFeaturesTv.Images.SetKeyName(1, "folder.png");
            imageListFeaturesTv.Images.SetKeyName(2, "star_grey.png");
            imageListFeaturesTv.Images.SetKeyName(3, "star_yellow.png");
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
            this.sbMain.TabIndex = 11;
            this.sbMain.ToolTip = "Quick search (Ctrl+Q)";
            this.sbMain.TreeView = this.tvFeatures;
            this.sbMain.FilterNode += new TreeViewSearchBox.TreeViewSearchBox.OnFilterNodeEvent(this.SbMainFilterNode);
            // 
            // FeaturesTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sbMain);
            this.Controls.Add(this.pSplitter);
            this.Controls.Add(this.pTreeView);
            this.Controls.Add(this.toolStrip);
            this.Name = "FeaturesTreeView";
            this.Size = new System.Drawing.Size(215, 342);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.pTreeView.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnAddFeatureClass;
        private System.Windows.Forms.ToolStripButton btnAddFeature;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnDeleteFeature;
        private System.Windows.Forms.Panel pSplitter;
        private System.Windows.Forms.Panel pTreeView;
        private TreeViewEx tvFeatures;
        private System.Windows.Forms.ToolStripButton btnEditScript;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnManageInteractiveRecipients;
        private TreeViewSearchBox.TreeViewSearchBox sbMain;
        public static System.Windows.Forms.ImageList imageListFeaturesTv;
    }
}
