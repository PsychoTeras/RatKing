using GMechanics.Editor.UIControls.Common;
using GMechanics.Editor.UIControls.TreeViewSearchBox;

namespace GMechanics.Editor.UIControls.TreeViews
{
    partial class PropertiesTreeView
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Properties");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesTreeView));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnAddPropertyClass = new System.Windows.Forms.ToolStripButton();
            this.btnAddProperty = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnManageInteractiveRecipients = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnDeleteFeature = new System.Windows.Forms.ToolStripButton();
            this.pSplitter = new System.Windows.Forms.Panel();
            this.pTreeView = new System.Windows.Forms.Panel();
            this.tvProperties = new TreeViewEx();
            imageListPropertiesTv = new System.Windows.Forms.ImageList(this.components);
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
            this.btnAddPropertyClass,
            this.btnAddProperty,
            this.toolStripSeparator1,
            this.btnManageInteractiveRecipients,
            this.toolStripSeparator2,
            this.btnDeleteFeature});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.toolStrip.Size = new System.Drawing.Size(215, 25);
            this.toolStrip.TabIndex = 5;
            this.toolStrip.Resize += new System.EventHandler(this.ToolStripResize);
            // 
            // btnAddPropertyClass
            // 
            this.btnAddPropertyClass.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddPropertyClass.Image = global::GMechanics.Editor.Properties.Resources.folder_add;
            this.btnAddPropertyClass.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddPropertyClass.Name = "btnAddPropertyClass";
            this.btnAddPropertyClass.Size = new System.Drawing.Size(23, 22);
            this.btnAddPropertyClass.Text = "Add new property class";
            this.btnAddPropertyClass.ToolTipText = "Add new property class (F4)";
            this.btnAddPropertyClass.Click += new System.EventHandler(this.BtnAddGameObjectPropertyClassClick);
            // 
            // btnAddProperty
            // 
            this.btnAddProperty.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.btnAddProperty.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddProperty.Image = global::GMechanics.Editor.Properties.Resources.component_yellow_add;
            this.btnAddProperty.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddProperty.Name = "btnAddProperty";
            this.btnAddProperty.Size = new System.Drawing.Size(23, 22);
            this.btnAddProperty.Text = "Add new property";
            this.btnAddProperty.ToolTipText = "Add new property (F5)";
            this.btnAddProperty.Click += new System.EventHandler(this.BtnAddGameObjectPropertyClick);
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
            this.btnDeleteFeature.Text = "Delete selected property or class";
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
            this.pTreeView.Controls.Add(this.tvProperties);
            this.pTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pTreeView.Location = new System.Drawing.Point(0, 25);
            this.pTreeView.Name = "pTreeView";
            this.pTreeView.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.pTreeView.Size = new System.Drawing.Size(215, 317);
            this.pTreeView.TabIndex = 7;
            // 
            // tvProperties
            // 
            this.tvProperties.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvProperties.FullRowSelect = true;
            this.tvProperties.HideSelection = false;
            this.tvProperties.ImageIndex = 0;
            this.tvProperties.ImageList = imageListPropertiesTv;
            this.tvProperties.Location = new System.Drawing.Point(0, 1);
            this.tvProperties.Name = "tvProperties";
            treeNode1.Name = "Node0";
            treeNode1.Text = "Properties";
            this.tvProperties.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.tvProperties.SelectedImageIndex = 0;
            this.tvProperties.ShowNodeToolTips = true;
            this.tvProperties.Size = new System.Drawing.Size(215, 316);
            this.tvProperties.TabIndex = 5;
            this.tvProperties.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TvPropertiesAfterSelect);
            this.tvProperties.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TvPropertiesKeyDown);
            // 
            // imageListPropertiesTv
            // 
            imageListPropertiesTv.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListPropertiesTv.ImageStream")));
            imageListPropertiesTv.TransparentColor = System.Drawing.Color.Transparent;
            imageListPropertiesTv.Images.SetKeyName(0, "bullet_arrow_right.png");
            imageListPropertiesTv.Images.SetKeyName(1, "folder.png");
            imageListPropertiesTv.Images.SetKeyName(2, "cube_yellow.png");
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
            this.sbMain.TabIndex = 10;
            this.sbMain.ToolTip = "Quick search (Ctrl+Q)";
            this.sbMain.TreeView = this.tvProperties;
            this.sbMain.FilterNode += new TreeViewSearchBox.TreeViewSearchBox.OnFilterNodeEvent(this.SbMainFilterNode);
            // 
            // PropertiesTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sbMain);
            this.Controls.Add(this.pSplitter);
            this.Controls.Add(this.pTreeView);
            this.Controls.Add(this.toolStrip);
            this.Name = "PropertiesTreeView";
            this.Size = new System.Drawing.Size(215, 342);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.pTreeView.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnAddPropertyClass;
        private System.Windows.Forms.ToolStripButton btnAddProperty;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnDeleteFeature;
        private System.Windows.Forms.Panel pSplitter;
        private System.Windows.Forms.Panel pTreeView;
        private TreeViewEx tvProperties;
        private System.Windows.Forms.ToolStripButton btnManageInteractiveRecipients;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private TreeViewSearchBox.TreeViewSearchBox sbMain;
        public static System.Windows.Forms.ImageList imageListPropertiesTv;
    }
}
