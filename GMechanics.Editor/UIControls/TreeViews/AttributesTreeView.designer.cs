using GMechanics.Editor.UIControls.Common;
using GMechanics.Editor.UIControls.TreeViewSearchBox;

namespace GMechanics.Editor.UIControls.TreeViews
{
    partial class AttributesTreeView
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Attributes");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttributesTreeView));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnAddAttribute = new System.Windows.Forms.ToolStripButton();
            this.btnEditAttribute = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnManageInteractiveRecipients = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnDeleteAttribute = new System.Windows.Forms.ToolStripButton();
            this.pTreeView = new System.Windows.Forms.Panel();
            this.tvAttributes = new TreeViewEx();
            imageListAttributesTv = new System.Windows.Forms.ImageList(this.components);
            this.pSplitter = new System.Windows.Forms.Panel();
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
            this.btnAddAttribute,
            this.btnEditAttribute,
            this.toolStripSeparator1,
            this.btnManageInteractiveRecipients,
            this.toolStripSeparator2,
            this.btnDeleteAttribute});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.toolStrip.Size = new System.Drawing.Size(215, 25);
            this.toolStrip.TabIndex = 5;
            this.toolStrip.Resize += new System.EventHandler(this.ToolStripResize);
            // 
            // btnAddAttribute
            // 
            this.btnAddAttribute.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddAttribute.Image = global::GMechanics.Editor.Properties.Resources.key1_add;
            this.btnAddAttribute.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddAttribute.Name = "btnAddAttribute";
            this.btnAddAttribute.Size = new System.Drawing.Size(23, 22);
            this.btnAddAttribute.Text = "Add new attribute";
            this.btnAddAttribute.ToolTipText = "Add new attribute (F5)";
            this.btnAddAttribute.Click += new System.EventHandler(this.BtnAddGameObjectAttributeClick);
            // 
            // btnEditAttribute
            // 
            this.btnEditAttribute.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEditAttribute.Image = global::GMechanics.Editor.Properties.Resources.key1_edit;
            this.btnEditAttribute.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditAttribute.Name = "btnEditAttribute";
            this.btnEditAttribute.Size = new System.Drawing.Size(23, 22);
            this.btnEditAttribute.Text = "Edit attribute";
            this.btnEditAttribute.ToolTipText = "Edit attribute (F6)";
            this.btnEditAttribute.Click += new System.EventHandler(this.BtnEditAttributeClick);
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
            this.btnManageInteractiveRecipients.Text = "Manage interactive recipients (F7)";
            this.btnManageInteractiveRecipients.Click += new System.EventHandler(this.BtnManageInteractiveRecipientsClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnDeleteAttribute
            // 
            this.btnDeleteAttribute.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDeleteAttribute.Image = global::GMechanics.Editor.Properties.Resources.delete;
            this.btnDeleteAttribute.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteAttribute.Name = "btnDeleteAttribute";
            this.btnDeleteAttribute.Size = new System.Drawing.Size(23, 22);
            this.btnDeleteAttribute.Text = "Delete selected attribute";
            this.btnDeleteAttribute.ToolTipText = "Delete selected attribute (F8, Delete)";
            this.btnDeleteAttribute.Click += new System.EventHandler(this.BtnDeleteAttributeClick);
            // 
            // pTreeView
            // 
            this.pTreeView.Controls.Add(this.tvAttributes);
            this.pTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pTreeView.Location = new System.Drawing.Point(0, 25);
            this.pTreeView.Name = "pTreeView";
            this.pTreeView.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.pTreeView.Size = new System.Drawing.Size(215, 317);
            this.pTreeView.TabIndex = 7;
            // 
            // tvAttributes
            // 
            this.tvAttributes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvAttributes.FullRowSelect = true;
            this.tvAttributes.HideSelection = false;
            this.tvAttributes.ImageIndex = 0;
            this.tvAttributes.ImageList = imageListAttributesTv;
            this.tvAttributes.Location = new System.Drawing.Point(0, 1);
            this.tvAttributes.Name = "tvAttributes";
            treeNode1.Name = "Node0";
            treeNode1.Text = "Attributes";
            this.tvAttributes.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.tvAttributes.SelectedImageIndex = 0;
            this.tvAttributes.ShowNodeToolTips = true;
            this.tvAttributes.Size = new System.Drawing.Size(215, 316);
            this.tvAttributes.TabIndex = 5;
            this.tvAttributes.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TvAttributesNodeMouseDoubleClick);
            this.tvAttributes.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TvAttributesAfterSelect);
            this.tvAttributes.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TvAttributesMouseDown);
            this.tvAttributes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TvAttributesKeyDown);
            // 
            // imageListAttributesTv
            // 
            imageListAttributesTv.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListAttributesTv.ImageStream")));
            imageListAttributesTv.TransparentColor = System.Drawing.Color.Transparent;
            imageListAttributesTv.Images.SetKeyName(0, "bullet_arrow_right.png");
            imageListAttributesTv.Images.SetKeyName(1, "key1.png");
            // 
            // pSplitter
            // 
            this.pSplitter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pSplitter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pSplitter.Location = new System.Drawing.Point(0, 24);
            this.pSplitter.Name = "pSplitter";
            this.pSplitter.Size = new System.Drawing.Size(215, 1);
            this.pSplitter.TabIndex = 8;
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
            this.sbMain.TabIndex = 9;
            this.sbMain.ToolTip = "Quick search (Ctrl+Q)";
            this.sbMain.TreeView = this.tvAttributes;
            this.sbMain.FilterNode += new TreeViewSearchBox.TreeViewSearchBox.OnFilterNodeEvent(this.SbMainFilterNode);
            // 
            // AttributesTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sbMain);
            this.Controls.Add(this.pSplitter);
            this.Controls.Add(this.pTreeView);
            this.Controls.Add(this.toolStrip);
            this.Name = "AttributesTreeView";
            this.Size = new System.Drawing.Size(215, 342);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.pTreeView.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnAddAttribute;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnDeleteAttribute;
        private System.Windows.Forms.ToolStripButton btnEditAttribute;
        private System.Windows.Forms.Panel pTreeView;
        private TreeViewEx tvAttributes;
        private System.Windows.Forms.ToolStripButton btnManageInteractiveRecipients;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Panel pSplitter;
        private TreeViewSearchBox.TreeViewSearchBox sbMain;
        public static System.Windows.Forms.ImageList imageListAttributesTv;
    }
}
