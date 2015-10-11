using GMechanics.Editor.UIControls.Common;
using GMechanics.Editor.UIControls.GraphicEditor;
using GMechanics.Editor.UIControls.PropertyGridEditor;
using GMechanics.Editor.UIControls.TreeViewPathEdit;
using GMechanics.Editor.UIControls.TreeViews;

namespace GMechanics.Editor.Forms
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpAttributes = new System.Windows.Forms.TabPage();
            this.scAttributes = new System.Windows.Forms.SplitContainer();
            this.tpProperties = new System.Windows.Forms.TabPage();
            this.scProperties = new System.Windows.Forms.SplitContainer();
            this.tpFeatures = new System.Windows.Forms.TabPage();
            this.scFeatures = new System.Windows.Forms.SplitContainer();
            this.tpGameObjects = new System.Windows.Forms.TabPage();
            this.scGameObjects = new System.Windows.Forms.SplitContainer();
            this.scGameObjectsAndEntities = new System.Windows.Forms.SplitContainer();
            this.scGameObjectsEditor = new System.Windows.Forms.SplitContainer();
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMain = new System.Windows.Forms.ToolStrip();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnReload = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnBackup = new System.Windows.Forms.ToolStripButton();
            this.btnRestore = new System.Windows.Forms.ToolStripButton();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.hAttributesList = new Header();
            this.tvAttributes = new AttributesTreeView();
            this.hAttributeSettings = new Header();
            this.pgeAttributes = new EntityPropertyGridEditor();
            this.tvpeAttributes = new TreeViewPathEdit();
            this.hPropertiesList = new Header();
            this.tvProperties = new PropertiesTreeView();
            this.hPropertySettings = new Header();
            this.pgeProperties = new EntityPropertyGridEditor();
            this.tvpeProperties = new TreeViewPathEdit();
            this.hFeaturesList = new Header();
            this.tvFeatures = new FeaturesTreeView();
            this.hFeatureSettings = new Header();
            this.pgeFeatures = new EntityPropertyGridEditor();
            this.tvpeFeatures = new TreeViewPathEdit();
            this.tvpeGameObjects = new TreeViewPathEdit();
            this.tvGameObjects = new GameObjectsTreeView();
            this.hGameObjectsList = new Header();
            this.hGameObjectsEntitiesList = new Header();
            this.tvGameObjectsEntities = new GameObjectsEntitiesTreeView();
            this.graphicEditor = new GraphicEditor();
            this.hGraphicEditor = new Header();
            this.pgeGameObject = new GameObjectPropertyGridEditor();
            this.hGameObjectProperties = new Header();
            this.tcMain.SuspendLayout();
            this.tpAttributes.SuspendLayout();
            this.scAttributes.Panel1.SuspendLayout();
            this.scAttributes.Panel2.SuspendLayout();
            this.scAttributes.SuspendLayout();
            this.tpProperties.SuspendLayout();
            this.scProperties.Panel1.SuspendLayout();
            this.scProperties.Panel2.SuspendLayout();
            this.scProperties.SuspendLayout();
            this.tpFeatures.SuspendLayout();
            this.scFeatures.Panel1.SuspendLayout();
            this.scFeatures.Panel2.SuspendLayout();
            this.scFeatures.SuspendLayout();
            this.tpGameObjects.SuspendLayout();
            this.scGameObjects.Panel1.SuspendLayout();
            this.scGameObjects.Panel2.SuspendLayout();
            this.scGameObjects.SuspendLayout();
            this.scGameObjectsAndEntities.Panel1.SuspendLayout();
            this.scGameObjectsAndEntities.Panel2.SuspendLayout();
            this.scGameObjectsAndEntities.SuspendLayout();
            this.scGameObjectsEditor.Panel1.SuspendLayout();
            this.scGameObjectsEditor.Panel2.SuspendLayout();
            this.scGameObjectsEditor.SuspendLayout();
            this.menuMain.SuspendLayout();
            this.tsMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcMain
            // 
            this.tcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcMain.Controls.Add(this.tpAttributes);
            this.tcMain.Controls.Add(this.tpProperties);
            this.tcMain.Controls.Add(this.tpFeatures);
            this.tcMain.Controls.Add(this.tpGameObjects);
            this.tcMain.Location = new System.Drawing.Point(12, 65);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(772, 520);
            this.tcMain.TabIndex = 0;
            this.tcMain.TabStop = false;
            // 
            // tpAttributes
            // 
            this.tpAttributes.BackColor = System.Drawing.SystemColors.Window;
            this.tpAttributes.Controls.Add(this.scAttributes);
            this.tpAttributes.Controls.Add(this.tvpeAttributes);
            this.tpAttributes.Location = new System.Drawing.Point(4, 22);
            this.tpAttributes.Name = "tpAttributes";
            this.tpAttributes.Padding = new System.Windows.Forms.Padding(3);
            this.tpAttributes.Size = new System.Drawing.Size(764, 494);
            this.tpAttributes.TabIndex = 2;
            this.tpAttributes.Text = "Attributes";
            // 
            // scAttributes
            // 
            this.scAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scAttributes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.scAttributes.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scAttributes.Location = new System.Drawing.Point(6, 36);
            this.scAttributes.Name = "scAttributes";
            // 
            // scAttributes.Panel1
            // 
            this.scAttributes.Panel1.Controls.Add(this.hAttributesList);
            this.scAttributes.Panel1.Controls.Add(this.tvAttributes);
            this.scAttributes.Panel1MinSize = 250;
            // 
            // scAttributes.Panel2
            // 
            this.scAttributes.Panel2.Controls.Add(this.hAttributeSettings);
            this.scAttributes.Panel2.Controls.Add(this.pgeAttributes);
            this.scAttributes.Size = new System.Drawing.Size(752, 452);
            this.scAttributes.SplitterDistance = 300;
            this.scAttributes.SplitterWidth = 6;
            this.scAttributes.TabIndex = 4;
            this.scAttributes.TabStop = false;
            this.scAttributes.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SplitContainerSplitterMoved);
            // 
            // tpProperties
            // 
            this.tpProperties.BackColor = System.Drawing.SystemColors.Window;
            this.tpProperties.Controls.Add(this.scProperties);
            this.tpProperties.Controls.Add(this.tvpeProperties);
            this.tpProperties.Location = new System.Drawing.Point(4, 22);
            this.tpProperties.Name = "tpProperties";
            this.tpProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tpProperties.Size = new System.Drawing.Size(764, 494);
            this.tpProperties.TabIndex = 0;
            this.tpProperties.Text = "Properties";
            // 
            // scProperties
            // 
            this.scProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.scProperties.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scProperties.Location = new System.Drawing.Point(6, 36);
            this.scProperties.Name = "scProperties";
            // 
            // scProperties.Panel1
            // 
            this.scProperties.Panel1.Controls.Add(this.hPropertiesList);
            this.scProperties.Panel1.Controls.Add(this.tvProperties);
            this.scProperties.Panel1MinSize = 250;
            // 
            // scProperties.Panel2
            // 
            this.scProperties.Panel2.Controls.Add(this.hPropertySettings);
            this.scProperties.Panel2.Controls.Add(this.pgeProperties);
            this.scProperties.Size = new System.Drawing.Size(752, 452);
            this.scProperties.SplitterDistance = 300;
            this.scProperties.SplitterWidth = 6;
            this.scProperties.TabIndex = 2;
            this.scProperties.TabStop = false;
            this.scProperties.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SplitContainerSplitterMoved);
            // 
            // tpFeatures
            // 
            this.tpFeatures.BackColor = System.Drawing.SystemColors.Window;
            this.tpFeatures.Controls.Add(this.scFeatures);
            this.tpFeatures.Controls.Add(this.tvpeFeatures);
            this.tpFeatures.Location = new System.Drawing.Point(4, 22);
            this.tpFeatures.Name = "tpFeatures";
            this.tpFeatures.Size = new System.Drawing.Size(764, 494);
            this.tpFeatures.TabIndex = 1;
            this.tpFeatures.Text = "Features";
            // 
            // scFeatures
            // 
            this.scFeatures.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scFeatures.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.scFeatures.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scFeatures.Location = new System.Drawing.Point(6, 36);
            this.scFeatures.Name = "scFeatures";
            // 
            // scFeatures.Panel1
            // 
            this.scFeatures.Panel1.Controls.Add(this.hFeaturesList);
            this.scFeatures.Panel1.Controls.Add(this.tvFeatures);
            this.scFeatures.Panel1MinSize = 250;
            // 
            // scFeatures.Panel2
            // 
            this.scFeatures.Panel2.Controls.Add(this.hFeatureSettings);
            this.scFeatures.Panel2.Controls.Add(this.pgeFeatures);
            this.scFeatures.Size = new System.Drawing.Size(752, 452);
            this.scFeatures.SplitterDistance = 300;
            this.scFeatures.SplitterWidth = 6;
            this.scFeatures.TabIndex = 3;
            this.scFeatures.TabStop = false;
            this.scFeatures.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SplitContainerSplitterMoved);
            // 
            // tpGameObjects
            // 
            this.tpGameObjects.BackColor = System.Drawing.SystemColors.Window;
            this.tpGameObjects.Controls.Add(this.tvpeGameObjects);
            this.tpGameObjects.Controls.Add(this.scGameObjects);
            this.tpGameObjects.Location = new System.Drawing.Point(4, 22);
            this.tpGameObjects.Name = "tpGameObjects";
            this.tpGameObjects.Padding = new System.Windows.Forms.Padding(3);
            this.tpGameObjects.Size = new System.Drawing.Size(764, 494);
            this.tpGameObjects.TabIndex = 3;
            this.tpGameObjects.Text = "Game objects";
            // 
            // scGameObjects
            // 
            this.scGameObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scGameObjects.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.scGameObjects.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scGameObjects.Location = new System.Drawing.Point(6, 36);
            this.scGameObjects.Name = "scGameObjects";
            // 
            // scGameObjects.Panel1
            // 
            this.scGameObjects.Panel1.Controls.Add(this.scGameObjectsAndEntities);
            this.scGameObjects.Panel1MinSize = 250;
            // 
            // scGameObjects.Panel2
            // 
            this.scGameObjects.Panel2.Controls.Add(this.scGameObjectsEditor);
            this.scGameObjects.Size = new System.Drawing.Size(752, 452);
            this.scGameObjects.SplitterDistance = 250;
            this.scGameObjects.SplitterWidth = 6;
            this.scGameObjects.TabIndex = 4;
            this.scGameObjects.TabStop = false;
            this.scGameObjects.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SplitContainerSplitterMoved);
            // 
            // scGameObjectsAndEntities
            // 
            this.scGameObjectsAndEntities.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.scGameObjectsAndEntities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scGameObjectsAndEntities.Location = new System.Drawing.Point(0, 0);
            this.scGameObjectsAndEntities.Name = "scGameObjectsAndEntities";
            this.scGameObjectsAndEntities.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scGameObjectsAndEntities.Panel1
            // 
            this.scGameObjectsAndEntities.Panel1.Controls.Add(this.tvGameObjects);
            this.scGameObjectsAndEntities.Panel1.Controls.Add(this.hGameObjectsList);
            // 
            // scGameObjectsAndEntities.Panel2
            // 
            this.scGameObjectsAndEntities.Panel2.Controls.Add(this.hGameObjectsEntitiesList);
            this.scGameObjectsAndEntities.Panel2.Controls.Add(this.tvGameObjectsEntities);
            this.scGameObjectsAndEntities.Size = new System.Drawing.Size(250, 452);
            this.scGameObjectsAndEntities.SplitterDistance = 207;
            this.scGameObjectsAndEntities.SplitterWidth = 6;
            this.scGameObjectsAndEntities.TabIndex = 0;
            // 
            // scGameObjectsEditor
            // 
            this.scGameObjectsEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.scGameObjectsEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scGameObjectsEditor.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.scGameObjectsEditor.Location = new System.Drawing.Point(0, 0);
            this.scGameObjectsEditor.Name = "scGameObjectsEditor";
            // 
            // scGameObjectsEditor.Panel1
            // 
            this.scGameObjectsEditor.Panel1.Controls.Add(this.graphicEditor);
            this.scGameObjectsEditor.Panel1.Controls.Add(this.hGraphicEditor);
            // 
            // scGameObjectsEditor.Panel2
            // 
            this.scGameObjectsEditor.Panel2.Controls.Add(this.pgeGameObject);
            this.scGameObjectsEditor.Panel2.Controls.Add(this.hGameObjectProperties);
            this.scGameObjectsEditor.Panel2MinSize = 0;
            this.scGameObjectsEditor.Size = new System.Drawing.Size(496, 452);
            this.scGameObjectsEditor.SplitterDistance = 276;
            this.scGameObjectsEditor.SplitterWidth = 6;
            this.scGameObjectsEditor.TabIndex = 0;
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuMain.Size = new System.Drawing.Size(796, 24);
            this.menuMain.TabIndex = 1;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.reloadToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::GMechanics.Editor.Properties.Resources.database_save;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.BtnSaveClick);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Image = global::GMechanics.Editor.Properties.Resources.database_refresh;
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.reloadToolStripMenuItem.Text = "Reload";
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.BtnReloadClick);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(148, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.aboutToolStripMenuItem.Image = global::GMechanics.Editor.Properties.Resources.help;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(28, 20);
            this.aboutToolStripMenuItem.ToolTipText = "About (F1)";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItemClick);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backupToolStripMenuItem,
            this.restoreToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // backupToolStripMenuItem
            // 
            this.backupToolStripMenuItem.Image = global::GMechanics.Editor.Properties.Resources.drive_disk;
            this.backupToolStripMenuItem.Name = "backupToolStripMenuItem";
            this.backupToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.backupToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.backupToolStripMenuItem.Text = "Backup";
            this.backupToolStripMenuItem.Click += new System.EventHandler(this.BtnBackupClick);
            // 
            // restoreToolStripMenuItem
            // 
            this.restoreToolStripMenuItem.Image = global::GMechanics.Editor.Properties.Resources.drive_go;
            this.restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
            this.restoreToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F12;
            this.restoreToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.restoreToolStripMenuItem.Text = "Restore";
            this.restoreToolStripMenuItem.Click += new System.EventHandler(this.BtnRestoreClick);
            // 
            // tsMain
            // 
            this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave,
            this.btnReload,
            this.toolStripSeparator2,
            this.btnBackup,
            this.btnRestore});
            this.tsMain.Location = new System.Drawing.Point(0, 24);
            this.tsMain.Name = "tsMain";
            this.tsMain.Size = new System.Drawing.Size(796, 38);
            this.tsMain.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.Image = global::GMechanics.Editor.Properties.Resources.database_save;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(35, 35);
            this.btnSave.Text = "Save";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSave.Click += new System.EventHandler(this.BtnSaveClick);
            // 
            // btnReload
            // 
            this.btnReload.Image = global::GMechanics.Editor.Properties.Resources.database_refresh;
            this.btnReload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(47, 35);
            this.btnReload.Text = "Reload";
            this.btnReload.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnReload.Click += new System.EventHandler(this.BtnReloadClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 38);
            // 
            // btnBackup
            // 
            this.btnBackup.Image = global::GMechanics.Editor.Properties.Resources.drive_disk;
            this.btnBackup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(50, 35);
            this.btnBackup.Text = "Backup";
            this.btnBackup.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnBackup.Click += new System.EventHandler(this.BtnBackupClick);
            // 
            // btnRestore
            // 
            this.btnRestore.Image = global::GMechanics.Editor.Properties.Resources.drive_go;
            this.btnRestore.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(50, 35);
            this.btnRestore.Text = "Restore";
            this.btnRestore.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnRestore.Click += new System.EventHandler(this.BtnRestoreClick);
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Visible = true;
            // 
            // hAttributesList
            // 
            this.hAttributesList.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.hAttributesList.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.hAttributesList.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hAttributesList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hAttributesList.Caption = "Attributes list";
            this.hAttributesList.Dock = System.Windows.Forms.DockStyle.Top;
            this.hAttributesList.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hAttributesList.ForeColor = System.Drawing.Color.Black;
            this.hAttributesList.Location = new System.Drawing.Point(0, 0);
            this.hAttributesList.Name = "hAttributesList";
            this.hAttributesList.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.hAttributesList.Size = new System.Drawing.Size(298, 22);
            this.hAttributesList.TabIndex = 4;
            this.hAttributesList.TabStop = false;
            this.hAttributesList.TopGradientColor = System.Drawing.Color.LightGoldenrodYellow;
            // 
            // tvAttributes
            // 
            this.tvAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvAttributes.Location = new System.Drawing.Point(0, 22);
            this.tvAttributes.Name = "tvAttributes";
            this.tvAttributes.SelectedObject = null;
            this.tvAttributes.Size = new System.Drawing.Size(298, 428);
            this.tvAttributes.TabIndex = 0;
            this.tvAttributes.NodeSelect += new BaseTreeView.NodeSelectHandler(this.TvAttributesNodeSelect);
            // 
            // hAttributeSettings
            // 
            this.hAttributeSettings.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.hAttributeSettings.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.hAttributeSettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hAttributeSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hAttributeSettings.Caption = "Attribute settings";
            this.hAttributeSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.hAttributeSettings.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hAttributeSettings.ForeColor = System.Drawing.Color.Black;
            this.hAttributeSettings.Location = new System.Drawing.Point(0, 0);
            this.hAttributeSettings.Name = "hAttributeSettings";
            this.hAttributeSettings.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.hAttributeSettings.Size = new System.Drawing.Size(444, 22);
            this.hAttributeSettings.TabIndex = 4;
            this.hAttributeSettings.TabStop = false;
            this.hAttributeSettings.TopGradientColor = System.Drawing.Color.LightGoldenrodYellow;
            // 
            // pgeAttributes
            // 
            this.pgeAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgeAttributes.AutoSizeProperties = true;
            // 
            // 
            // 
            this.pgeAttributes.DocCommentDescription.AccessibleName = "";
            this.pgeAttributes.DocCommentDescription.AutoEllipsis = true;
            this.pgeAttributes.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this.pgeAttributes.DocCommentDescription.Location = new System.Drawing.Point(3, 18);
            this.pgeAttributes.DocCommentDescription.Name = "";
            this.pgeAttributes.DocCommentDescription.Size = new System.Drawing.Size(0, 52);
            this.pgeAttributes.DocCommentDescription.TabIndex = 1;
            this.pgeAttributes.DocCommentImage = null;
            // 
            // 
            // 
            this.pgeAttributes.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.pgeAttributes.DocCommentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.pgeAttributes.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.pgeAttributes.DocCommentTitle.Name = "";
            this.pgeAttributes.DocCommentTitle.Size = new System.Drawing.Size(0, 0);
            this.pgeAttributes.DocCommentTitle.TabIndex = 0;
            this.pgeAttributes.DocCommentTitle.UseMnemonic = false;
            this.pgeAttributes.DrawFlatToolbar = true;
            this.pgeAttributes.HelpVisible = false;
            this.pgeAttributes.Location = new System.Drawing.Point(-1, 21);
            this.pgeAttributes.Name = "pgeAttributes";
            this.pgeAttributes.ShowCustomPropertiesSet = true;
            this.pgeAttributes.Size = new System.Drawing.Size(446, 430);
            this.pgeAttributes.TabIndex = 1;
            this.pgeAttributes.ToolbarVisible = false;
            // 
            // 
            // 
            this.pgeAttributes.ToolStrip.AccessibleName = "ToolBar";
            this.pgeAttributes.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this.pgeAttributes.ToolStrip.AllowMerge = false;
            this.pgeAttributes.ToolStrip.AutoSize = false;
            this.pgeAttributes.ToolStrip.CanOverflow = false;
            this.pgeAttributes.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.pgeAttributes.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.pgeAttributes.ToolStrip.Location = new System.Drawing.Point(0, 1);
            this.pgeAttributes.ToolStrip.Name = "";
            this.pgeAttributes.ToolStrip.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.pgeAttributes.ToolStrip.Size = new System.Drawing.Size(0, 25);
            this.pgeAttributes.ToolStrip.TabIndex = 1;
            this.pgeAttributes.ToolStrip.TabStop = true;
            this.pgeAttributes.ToolStrip.Text = "PropertyGridToolBar";
            this.pgeAttributes.ToolStrip.Visible = false;
            this.pgeAttributes.TreeView = this.tvAttributes;
            this.pgeAttributes.PropertyNameExistsCheck += new EntityPropertyGridEditor.PropertyNameExistsCheckHandler(this.PgeAttributesPropertyNameExistsCheck);
            // 
            // tvpeAttributes
            // 
            this.tvpeAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvpeAttributes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvpeAttributes.Location = new System.Drawing.Point(6, 6);
            this.tvpeAttributes.Name = "tvpeAttributes";
            this.tvpeAttributes.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.tvpeAttributes.ShowNodesIcons = false;
            this.tvpeAttributes.Size = new System.Drawing.Size(752, 24);
            this.tvpeAttributes.TabIndex = 3;
            this.tvpeAttributes.TabStop = false;
            this.tvpeAttributes.TreeView = this.tvAttributes;
            this.tvpeAttributes.ButtonClick += new TreeViewPathEdit.ButtonClickHandler(this.TreeViewPathEditButtonClick);
            // 
            // hPropertiesList
            // 
            this.hPropertiesList.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.hPropertiesList.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.hPropertiesList.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hPropertiesList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hPropertiesList.Caption = "Properties list";
            this.hPropertiesList.Dock = System.Windows.Forms.DockStyle.Top;
            this.hPropertiesList.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hPropertiesList.ForeColor = System.Drawing.Color.Black;
            this.hPropertiesList.Location = new System.Drawing.Point(0, 0);
            this.hPropertiesList.Name = "hPropertiesList";
            this.hPropertiesList.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.hPropertiesList.Size = new System.Drawing.Size(298, 22);
            this.hPropertiesList.TabIndex = 4;
            this.hPropertiesList.TabStop = false;
            this.hPropertiesList.TopGradientColor = System.Drawing.Color.LightGoldenrodYellow;
            // 
            // tvProperties
            // 
            this.tvProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvProperties.Location = new System.Drawing.Point(0, 22);
            this.tvProperties.Name = "tvProperties";
            this.tvProperties.SelectedObject = null;
            this.tvProperties.Size = new System.Drawing.Size(298, 428);
            this.tvProperties.TabIndex = 0;
            this.tvProperties.NodeSelect += new BaseTreeView.NodeSelectHandler(this.TvPropertiesNodeSelect);
            // 
            // hPropertySettings
            // 
            this.hPropertySettings.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.hPropertySettings.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.hPropertySettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hPropertySettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hPropertySettings.Caption = "Property settings";
            this.hPropertySettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.hPropertySettings.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hPropertySettings.ForeColor = System.Drawing.Color.Black;
            this.hPropertySettings.Location = new System.Drawing.Point(0, 0);
            this.hPropertySettings.Name = "hPropertySettings";
            this.hPropertySettings.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.hPropertySettings.Size = new System.Drawing.Size(444, 22);
            this.hPropertySettings.TabIndex = 4;
            this.hPropertySettings.TabStop = false;
            this.hPropertySettings.TopGradientColor = System.Drawing.Color.LightGoldenrodYellow;
            // 
            // pgeProperties
            // 
            this.pgeProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgeProperties.AutoSizeProperties = true;
            // 
            // 
            // 
            this.pgeProperties.DocCommentDescription.AccessibleName = "";
            this.pgeProperties.DocCommentDescription.AutoEllipsis = true;
            this.pgeProperties.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this.pgeProperties.DocCommentDescription.Location = new System.Drawing.Point(3, 18);
            this.pgeProperties.DocCommentDescription.Name = "";
            this.pgeProperties.DocCommentDescription.Size = new System.Drawing.Size(0, 52);
            this.pgeProperties.DocCommentDescription.TabIndex = 1;
            this.pgeProperties.DocCommentImage = null;
            // 
            // 
            // 
            this.pgeProperties.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.pgeProperties.DocCommentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.pgeProperties.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.pgeProperties.DocCommentTitle.Name = "";
            this.pgeProperties.DocCommentTitle.Size = new System.Drawing.Size(0, 0);
            this.pgeProperties.DocCommentTitle.TabIndex = 0;
            this.pgeProperties.DocCommentTitle.UseMnemonic = false;
            this.pgeProperties.DrawFlatToolbar = true;
            this.pgeProperties.HelpVisible = false;
            this.pgeProperties.Location = new System.Drawing.Point(-1, 21);
            this.pgeProperties.Name = "pgeProperties";
            this.pgeProperties.ShowCustomPropertiesSet = true;
            this.pgeProperties.Size = new System.Drawing.Size(446, 430);
            this.pgeProperties.TabIndex = 0;
            this.pgeProperties.ToolbarVisible = false;
            // 
            // 
            // 
            this.pgeProperties.ToolStrip.AccessibleName = "ToolBar";
            this.pgeProperties.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this.pgeProperties.ToolStrip.AllowMerge = false;
            this.pgeProperties.ToolStrip.AutoSize = false;
            this.pgeProperties.ToolStrip.CanOverflow = false;
            this.pgeProperties.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.pgeProperties.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.pgeProperties.ToolStrip.Location = new System.Drawing.Point(0, 1);
            this.pgeProperties.ToolStrip.Name = "";
            this.pgeProperties.ToolStrip.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.pgeProperties.ToolStrip.Size = new System.Drawing.Size(0, 25);
            this.pgeProperties.ToolStrip.TabIndex = 1;
            this.pgeProperties.ToolStrip.TabStop = true;
            this.pgeProperties.ToolStrip.Text = "PropertyGridToolBar";
            this.pgeProperties.ToolStrip.Visible = false;
            this.pgeProperties.TreeView = this.tvProperties;
            this.pgeProperties.PropertyNameExistsCheck += new EntityPropertyGridEditor.PropertyNameExistsCheckHandler(this.GePropertiesPropertyNameExistsCheck);
            // 
            // tvpeProperties
            // 
            this.tvpeProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvpeProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvpeProperties.Location = new System.Drawing.Point(6, 6);
            this.tvpeProperties.Name = "tvpeProperties";
            this.tvpeProperties.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.tvpeProperties.ShowNodesIcons = false;
            this.tvpeProperties.Size = new System.Drawing.Size(752, 24);
            this.tvpeProperties.TabIndex = 1;
            this.tvpeProperties.TabStop = false;
            this.tvpeProperties.TreeView = this.tvProperties;
            this.tvpeProperties.ButtonClick += new TreeViewPathEdit.ButtonClickHandler(this.TreeViewPathEditButtonClick);
            // 
            // hFeaturesList
            // 
            this.hFeaturesList.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.hFeaturesList.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.hFeaturesList.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hFeaturesList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hFeaturesList.Caption = "Features list";
            this.hFeaturesList.Dock = System.Windows.Forms.DockStyle.Top;
            this.hFeaturesList.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hFeaturesList.ForeColor = System.Drawing.Color.Black;
            this.hFeaturesList.Location = new System.Drawing.Point(0, 0);
            this.hFeaturesList.Name = "hFeaturesList";
            this.hFeaturesList.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.hFeaturesList.Size = new System.Drawing.Size(298, 22);
            this.hFeaturesList.TabIndex = 4;
            this.hFeaturesList.TabStop = false;
            this.hFeaturesList.TopGradientColor = System.Drawing.Color.LightGoldenrodYellow;
            // 
            // tvFeatures
            // 
            this.tvFeatures.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvFeatures.Location = new System.Drawing.Point(0, 22);
            this.tvFeatures.Name = "tvFeatures";
            this.tvFeatures.SelectedObject = null;
            this.tvFeatures.Size = new System.Drawing.Size(298, 428);
            this.tvFeatures.TabIndex = 0;
            this.tvFeatures.NodeSelect += new BaseTreeView.NodeSelectHandler(this.TvFeaturesNodeSelect);
            // 
            // hFeatureSettings
            // 
            this.hFeatureSettings.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.hFeatureSettings.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.hFeatureSettings.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.hFeatureSettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hFeatureSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hFeatureSettings.Caption = "Feature settings";
            this.hFeatureSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.hFeatureSettings.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hFeatureSettings.ForeColor = System.Drawing.Color.Black;
            this.hFeatureSettings.Location = new System.Drawing.Point(0, 0);
            this.hFeatureSettings.Name = "hFeatureSettings";
            this.hFeatureSettings.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.hFeatureSettings.Size = new System.Drawing.Size(444, 22);
            this.hFeatureSettings.TabIndex = 4;
            this.hFeatureSettings.TabStop = false;
            this.hFeatureSettings.TopGradientColor = System.Drawing.Color.LightGoldenrodYellow;
            // 
            // pgeFeatures
            // 
            this.pgeFeatures.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgeFeatures.AutoSizeProperties = true;
            // 
            // 
            // 
            this.pgeFeatures.DocCommentDescription.AccessibleName = "";
            this.pgeFeatures.DocCommentDescription.AutoEllipsis = true;
            this.pgeFeatures.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this.pgeFeatures.DocCommentDescription.Location = new System.Drawing.Point(3, 18);
            this.pgeFeatures.DocCommentDescription.Name = "";
            this.pgeFeatures.DocCommentDescription.Size = new System.Drawing.Size(0, 52);
            this.pgeFeatures.DocCommentDescription.TabIndex = 1;
            this.pgeFeatures.DocCommentImage = null;
            // 
            // 
            // 
            this.pgeFeatures.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.pgeFeatures.DocCommentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.pgeFeatures.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.pgeFeatures.DocCommentTitle.Name = "";
            this.pgeFeatures.DocCommentTitle.Size = new System.Drawing.Size(0, 0);
            this.pgeFeatures.DocCommentTitle.TabIndex = 0;
            this.pgeFeatures.DocCommentTitle.UseMnemonic = false;
            this.pgeFeatures.DrawFlatToolbar = true;
            this.pgeFeatures.HelpVisible = false;
            this.pgeFeatures.Location = new System.Drawing.Point(-1, 21);
            this.pgeFeatures.Name = "pgeFeatures";
            this.pgeFeatures.ShowCustomPropertiesSet = true;
            this.pgeFeatures.Size = new System.Drawing.Size(446, 430);
            this.pgeFeatures.TabIndex = 0;
            this.pgeFeatures.ToolbarVisible = false;
            // 
            // 
            // 
            this.pgeFeatures.ToolStrip.AccessibleName = "ToolBar";
            this.pgeFeatures.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this.pgeFeatures.ToolStrip.AllowMerge = false;
            this.pgeFeatures.ToolStrip.AutoSize = false;
            this.pgeFeatures.ToolStrip.CanOverflow = false;
            this.pgeFeatures.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.pgeFeatures.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.pgeFeatures.ToolStrip.Location = new System.Drawing.Point(0, 1);
            this.pgeFeatures.ToolStrip.Name = "";
            this.pgeFeatures.ToolStrip.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.pgeFeatures.ToolStrip.Size = new System.Drawing.Size(0, 25);
            this.pgeFeatures.ToolStrip.TabIndex = 1;
            this.pgeFeatures.ToolStrip.TabStop = true;
            this.pgeFeatures.ToolStrip.Text = "PropertyGridToolBar";
            this.pgeFeatures.ToolStrip.Visible = false;
            this.pgeFeatures.TreeView = this.tvFeatures;
            this.pgeFeatures.PropertyNameExistsCheck += new EntityPropertyGridEditor.PropertyNameExistsCheckHandler(this.PgeFeaturesPropertyNameExistsCheck);
            // 
            // tvpeFeatures
            // 
            this.tvpeFeatures.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvpeFeatures.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvpeFeatures.Location = new System.Drawing.Point(6, 6);
            this.tvpeFeatures.Name = "tvpeFeatures";
            this.tvpeFeatures.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.tvpeFeatures.ShowNodesIcons = false;
            this.tvpeFeatures.Size = new System.Drawing.Size(752, 24);
            this.tvpeFeatures.TabIndex = 2;
            this.tvpeFeatures.TabStop = false;
            this.tvpeFeatures.TreeView = this.tvFeatures;
            this.tvpeFeatures.ButtonClick += new TreeViewPathEdit.ButtonClickHandler(this.TreeViewPathEditButtonClick);
            // 
            // tvpeGameObjects
            // 
            this.tvpeGameObjects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvpeGameObjects.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvpeGameObjects.Location = new System.Drawing.Point(6, 6);
            this.tvpeGameObjects.Name = "tvpeGameObjects";
            this.tvpeGameObjects.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.tvpeGameObjects.ShowNodesIcons = false;
            this.tvpeGameObjects.Size = new System.Drawing.Size(752, 24);
            this.tvpeGameObjects.TabIndex = 5;
            this.tvpeGameObjects.TabStop = false;
            this.tvpeGameObjects.TreeView = this.tvGameObjects;
            this.tvpeGameObjects.ButtonClick += new TreeViewPathEdit.ButtonClickHandler(this.TreeViewPathEditButtonClick);
            // 
            // tvGameObjects
            // 
            this.tvGameObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvGameObjects.BackColor = System.Drawing.Color.White;
            this.tvGameObjects.EditMode = false;
            this.tvGameObjects.Location = new System.Drawing.Point(0, 22);
            this.tvGameObjects.Name = "tvGameObjects";
            this.tvGameObjects.SelectedObject = null;
            this.tvGameObjects.Size = new System.Drawing.Size(248, 183);
            this.tvGameObjects.TabIndex = 3;
            this.tvGameObjects.EditObject += new GameObjectsTreeView.EditObjectHandler(this.TvGameObjectsEditObject);
            this.tvGameObjects.NodeSelect += new BaseTreeView.NodeSelectHandler(this.TvGameObjectsNodeSelect);
            // 
            // hGameObjectsList
            // 
            this.hGameObjectsList.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.hGameObjectsList.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.hGameObjectsList.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hGameObjectsList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hGameObjectsList.Caption = "Game objects list";
            this.hGameObjectsList.Dock = System.Windows.Forms.DockStyle.Top;
            this.hGameObjectsList.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hGameObjectsList.ForeColor = System.Drawing.Color.Black;
            this.hGameObjectsList.Location = new System.Drawing.Point(0, 0);
            this.hGameObjectsList.Name = "hGameObjectsList";
            this.hGameObjectsList.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.hGameObjectsList.Size = new System.Drawing.Size(248, 22);
            this.hGameObjectsList.TabIndex = 4;
            this.hGameObjectsList.TabStop = false;
            this.hGameObjectsList.TopGradientColor = System.Drawing.Color.LightGoldenrodYellow;
            // 
            // hGameObjectsEntitiesList
            // 
            this.hGameObjectsEntitiesList.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.hGameObjectsEntitiesList.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.hGameObjectsEntitiesList.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hGameObjectsEntitiesList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hGameObjectsEntitiesList.Caption = "Game object entities list";
            this.hGameObjectsEntitiesList.Dock = System.Windows.Forms.DockStyle.Top;
            this.hGameObjectsEntitiesList.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hGameObjectsEntitiesList.ForeColor = System.Drawing.Color.Black;
            this.hGameObjectsEntitiesList.Location = new System.Drawing.Point(0, 0);
            this.hGameObjectsEntitiesList.Name = "hGameObjectsEntitiesList";
            this.hGameObjectsEntitiesList.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.hGameObjectsEntitiesList.Size = new System.Drawing.Size(248, 22);
            this.hGameObjectsEntitiesList.TabIndex = 3;
            this.hGameObjectsEntitiesList.TabStop = false;
            this.hGameObjectsEntitiesList.TopGradientColor = System.Drawing.Color.LightGoldenrodYellow;
            // 
            // tvGameObjectsEntities
            // 
            this.tvGameObjectsEntities.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvGameObjectsEntities.BackColor = System.Drawing.Color.White;
            this.tvGameObjectsEntities.EditMode = false;
            this.tvGameObjectsEntities.Location = new System.Drawing.Point(0, 22);
            this.tvGameObjectsEntities.Name = "tvGameObjectsEntities";
            this.tvGameObjectsEntities.SelectedObject = null;
            this.tvGameObjectsEntities.Size = new System.Drawing.Size(248, 213);
            this.tvGameObjectsEntities.TabIndex = 1;
            // 
            // graphicEditor
            // 
            this.graphicEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.graphicEditor.BackColor = System.Drawing.Color.DimGray;
            this.graphicEditor.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.graphicEditor.GameObject = null;
            this.graphicEditor.Location = new System.Drawing.Point(0, 22);
            this.graphicEditor.Name = "graphicEditor";
            this.graphicEditor.ReadOnly = true;
            this.graphicEditor.Size = new System.Drawing.Size(274, 428);
            this.graphicEditor.TabIndex = 1;
            this.graphicEditor.TabStop = false;
            this.graphicEditor.DoubleClick += new System.EventHandler(this.GraphicEditorDoubleClick);
            this.graphicEditor.ItemSelected += new GMechanics.FlowchartControl.Flowchart.OnItemSelected(this.GraphicEditorItemSelected);
            // 
            // hGraphicEditor
            // 
            this.hGraphicEditor.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.hGraphicEditor.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.hGraphicEditor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hGraphicEditor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hGraphicEditor.Caption = "Graphic editor";
            this.hGraphicEditor.Dock = System.Windows.Forms.DockStyle.Top;
            this.hGraphicEditor.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hGraphicEditor.ForeColor = System.Drawing.Color.Black;
            this.hGraphicEditor.Location = new System.Drawing.Point(0, 0);
            this.hGraphicEditor.Name = "hGraphicEditor";
            this.hGraphicEditor.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.hGraphicEditor.Size = new System.Drawing.Size(274, 22);
            this.hGraphicEditor.TabIndex = 4;
            this.hGraphicEditor.TabStop = false;
            this.hGraphicEditor.TopGradientColor = System.Drawing.Color.LightGoldenrodYellow;
            // 
            // pgeGameObject
            // 
            this.pgeGameObject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.pgeGameObject.DocCommentDescription.AutoEllipsis = true;
            this.pgeGameObject.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this.pgeGameObject.DocCommentDescription.Location = new System.Drawing.Point(3, 19);
            this.pgeGameObject.DocCommentDescription.Name = "";
            this.pgeGameObject.DocCommentDescription.Size = new System.Drawing.Size(0, 52);
            this.pgeGameObject.DocCommentDescription.TabIndex = 1;
            this.pgeGameObject.DocCommentImage = null;
            // 
            // 
            // 
            this.pgeGameObject.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.pgeGameObject.DocCommentTitle.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.pgeGameObject.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.pgeGameObject.DocCommentTitle.Name = "";
            this.pgeGameObject.DocCommentTitle.Size = new System.Drawing.Size(0, 0);
            this.pgeGameObject.DocCommentTitle.TabIndex = 0;
            this.pgeGameObject.DocCommentTitle.UseMnemonic = false;
            this.pgeGameObject.EditMode = false;
            this.pgeGameObject.HelpVisible = false;
            this.pgeGameObject.Location = new System.Drawing.Point(-1, 21);
            this.pgeGameObject.Name = "pgeGameObject";
            this.pgeGameObject.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgeGameObject.Size = new System.Drawing.Size(214, 430);
            this.pgeGameObject.TabIndex = 4;
            this.pgeGameObject.ToolbarVisible = false;
            // 
            // 
            // 
            this.pgeGameObject.ToolStrip.AccessibleName = "ToolBar";
            this.pgeGameObject.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this.pgeGameObject.ToolStrip.AllowMerge = false;
            this.pgeGameObject.ToolStrip.AutoSize = false;
            this.pgeGameObject.ToolStrip.CanOverflow = false;
            this.pgeGameObject.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.pgeGameObject.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.pgeGameObject.ToolStrip.Location = new System.Drawing.Point(0, 1);
            this.pgeGameObject.ToolStrip.Name = "";
            this.pgeGameObject.ToolStrip.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.pgeGameObject.ToolStrip.Size = new System.Drawing.Size(0, 25);
            this.pgeGameObject.ToolStrip.TabIndex = 1;
            this.pgeGameObject.ToolStrip.TabStop = true;
            this.pgeGameObject.ToolStrip.Text = "PropertyGridToolBar";
            this.pgeGameObject.ToolStrip.Visible = false;
            this.pgeGameObject.ViewBackColor = System.Drawing.Color.White;
            // 
            // hGameObjectProperties
            // 
            this.hGameObjectProperties.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.hGameObjectProperties.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.hGameObjectProperties.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hGameObjectProperties.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hGameObjectProperties.Caption = "Properties";
            this.hGameObjectProperties.Dock = System.Windows.Forms.DockStyle.Top;
            this.hGameObjectProperties.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hGameObjectProperties.ForeColor = System.Drawing.Color.Black;
            this.hGameObjectProperties.Location = new System.Drawing.Point(0, 0);
            this.hGameObjectProperties.Name = "hGameObjectProperties";
            this.hGameObjectProperties.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.hGameObjectProperties.Size = new System.Drawing.Size(212, 22);
            this.hGameObjectProperties.TabIndex = 4;
            this.hGameObjectProperties.TabStop = false;
            this.hGameObjectProperties.TopGradientColor = System.Drawing.Color.LightGoldenrodYellow;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(796, 597);
            this.Controls.Add(this.tsMain);
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.menuMain);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuMain;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GMechanics Editor Beta";
            this.tcMain.ResumeLayout(false);
            this.tpAttributes.ResumeLayout(false);
            this.scAttributes.Panel1.ResumeLayout(false);
            this.scAttributes.Panel2.ResumeLayout(false);
            this.scAttributes.ResumeLayout(false);
            this.tpProperties.ResumeLayout(false);
            this.scProperties.Panel1.ResumeLayout(false);
            this.scProperties.Panel2.ResumeLayout(false);
            this.scProperties.ResumeLayout(false);
            this.tpFeatures.ResumeLayout(false);
            this.scFeatures.Panel1.ResumeLayout(false);
            this.scFeatures.Panel2.ResumeLayout(false);
            this.scFeatures.ResumeLayout(false);
            this.tpGameObjects.ResumeLayout(false);
            this.scGameObjects.Panel1.ResumeLayout(false);
            this.scGameObjects.Panel2.ResumeLayout(false);
            this.scGameObjects.ResumeLayout(false);
            this.scGameObjectsAndEntities.Panel1.ResumeLayout(false);
            this.scGameObjectsAndEntities.Panel2.ResumeLayout(false);
            this.scGameObjectsAndEntities.ResumeLayout(false);
            this.scGameObjectsEditor.Panel1.ResumeLayout(false);
            this.scGameObjectsEditor.Panel2.ResumeLayout(false);
            this.scGameObjectsEditor.ResumeLayout(false);
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.tsMain.ResumeLayout(false);
            this.tsMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tpProperties;
        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private TreeViewPathEdit tvpeProperties;
        private System.Windows.Forms.SplitContainer scProperties;
        private System.Windows.Forms.ToolStrip tsMain;
        private System.Windows.Forms.TabPage tpFeatures;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnReload;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private PropertiesTreeView tvProperties;
        private EntityPropertyGridEditor pgeProperties;
        private TreeViewPathEdit tvpeFeatures;
        private System.Windows.Forms.SplitContainer scFeatures;
        private FeaturesTreeView tvFeatures;
        private EntityPropertyGridEditor pgeFeatures;
        private System.Windows.Forms.TabPage tpAttributes;
        private TreeViewPathEdit tvpeAttributes;
        private System.Windows.Forms.SplitContainer scAttributes;
        private AttributesTreeView tvAttributes;
        private EntityPropertyGridEditor pgeAttributes;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.TabPage tpGameObjects;
        private System.Windows.Forms.SplitContainer scGameObjects;
        private System.Windows.Forms.ToolStripButton btnBackup;
        private System.Windows.Forms.ToolStripButton btnRestore;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restoreToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private TreeViewPathEdit tvpeGameObjects;
        private System.Windows.Forms.SplitContainer scGameObjectsEditor;
        private System.Windows.Forms.SplitContainer scGameObjectsAndEntities;
        private GameObjectsEntitiesTreeView tvGameObjectsEntities;
        private Header hGameObjectsEntitiesList;
        private Header hGraphicEditor;
        private Header hGameObjectsList;
        private Header hAttributesList;
        private Header hAttributeSettings;
        private Header hPropertiesList;
        private Header hPropertySettings;
        private Header hFeaturesList;
        private Header hFeatureSettings;
        private GraphicEditor graphicEditor;
        private GameObjectPropertyGridEditor pgeGameObject;
        private Header hGameObjectProperties;
        private GameObjectsTreeView tvGameObjects;
    }
}

