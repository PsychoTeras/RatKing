using GMechanics.Editor.UIControls.Common;
using GMechanics.Editor.UIControls.TreeViewPathEdit;
using GMechanics.Editor.UIControls.TreeViews;
using GMechanics.ScriptTextBox;

namespace GMechanics.Editor.Forms.ScriptEditor
{
    partial class ScriptEditorForm
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
            this.pTreeViewPathEdit = new System.Windows.Forms.Panel();
            this.tvpeObjects = new GMechanics.Editor.UIControls.TreeViewPathEdit.TreeViewPathEdit();
            this.tvObjects = new GMechanics.Editor.UIControls.TreeViews.GameObjectsEntitiesTreeView();
            this.pClient = new System.Windows.Forms.Panel();
            this.pDivider1 = new System.Windows.Forms.Panel();
            this.scMain = new System.Windows.Forms.SplitContainer();
            this.pTreeViewObjects = new System.Windows.Forms.Panel();
            this.pScriptEditorClient = new System.Windows.Forms.Panel();
            this.tcScriptEditor = new System.Windows.Forms.TabControl();
            this.tbScript = new System.Windows.Forms.TabPage();
            this.scScriptEditor = new System.Windows.Forms.SplitContainer();
            this.cbEventType = new GMechanics.Editor.UIControls.Common.ComboBoxEx();
            this.pScript = new System.Windows.Forms.Panel();
            this.teScript = new GMechanics.ScriptTextBox.ScriptTextBoxControl();
            this.tsScriptEditor = new System.Windows.Forms.ToolStrip();
            this.cbScriptActive = new System.Windows.Forms.ToolStripButton();
            this.btnCompileScript = new System.Windows.Forms.ToolStripButton();
            this.pScriptOut = new System.Windows.Forms.Panel();
            this.tbScriptOut = new System.Windows.Forms.TextBox();
            this.ttMain = new System.Windows.Forms.ToolTip(this.components);
            this.pTreeViewPathEdit.SuspendLayout();
            this.pClient.SuspendLayout();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            this.pTreeViewObjects.SuspendLayout();
            this.pScriptEditorClient.SuspendLayout();
            this.tcScriptEditor.SuspendLayout();
            this.tbScript.SuspendLayout();
            this.scScriptEditor.Panel1.SuspendLayout();
            this.scScriptEditor.Panel2.SuspendLayout();
            this.scScriptEditor.SuspendLayout();
            this.pScript.SuspendLayout();
            this.tsScriptEditor.SuspendLayout();
            this.pScriptOut.SuspendLayout();
            this.SuspendLayout();
            // 
            // pTreeViewPathEdit
            // 
            this.pTreeViewPathEdit.AutoSize = true;
            this.pTreeViewPathEdit.BackColor = System.Drawing.Color.White;
            this.pTreeViewPathEdit.Controls.Add(this.tvpeObjects);
            this.pTreeViewPathEdit.Dock = System.Windows.Forms.DockStyle.Top;
            this.pTreeViewPathEdit.Location = new System.Drawing.Point(12, 5);
            this.pTreeViewPathEdit.Name = "pTreeViewPathEdit";
            this.pTreeViewPathEdit.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.pTreeViewPathEdit.Size = new System.Drawing.Size(668, 24);
            this.pTreeViewPathEdit.TabIndex = 3;
            this.pTreeViewPathEdit.Visible = false;
            // 
            // tvpeObjects
            // 
            this.tvpeObjects.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvpeObjects.Dock = System.Windows.Forms.DockStyle.Top;
            this.tvpeObjects.Location = new System.Drawing.Point(0, 0);
            this.tvpeObjects.Name = "tvpeObjects";
            this.tvpeObjects.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.tvpeObjects.ShowNodesIcons = false;
            this.tvpeObjects.Size = new System.Drawing.Size(667, 24);
            this.tvpeObjects.TabIndex = 0;
            this.tvpeObjects.TreeView = this.tvObjects;
            this.tvpeObjects.ButtonClick += new GMechanics.Editor.UIControls.TreeViewPathEdit.TreeViewPathEdit.ButtonClickHandler(this.TvpeMainButtonClick);
            // 
            // tvObjects
            // 
            this.tvObjects.BackColor = System.Drawing.Color.White;
            this.tvObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvObjects.EditMode = false;
            this.tvObjects.Location = new System.Drawing.Point(0, 0);
            this.tvObjects.Name = "tvObjects";
            this.tvObjects.SelectedObject = null;
            this.tvObjects.Size = new System.Drawing.Size(254, 95);
            this.tvObjects.TabIndex = 1;
            this.tvObjects.NodeSelect += new GMechanics.Editor.UIControls.TreeViews.BaseTreeView.NodeSelectHandler(this.TvObjectsNodeSelect);
            // 
            // pClient
            // 
            this.pClient.BackColor = System.Drawing.Color.White;
            this.pClient.Controls.Add(this.pDivider1);
            this.pClient.Controls.Add(this.scMain);
            this.pClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pClient.Location = new System.Drawing.Point(12, 29);
            this.pClient.Name = "pClient";
            this.pClient.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.pClient.Size = new System.Drawing.Size(668, 496);
            this.pClient.TabIndex = 4;
            // 
            // pDivider1
            // 
            this.pDivider1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pDivider1.Location = new System.Drawing.Point(306, 4);
            this.pDivider1.Name = "pDivider1";
            this.pDivider1.Size = new System.Drawing.Size(359, 2);
            this.pDivider1.TabIndex = 3;
            // 
            // scMain
            // 
            this.scMain.BackColor = System.Drawing.Color.White;
            this.scMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scMain.Location = new System.Drawing.Point(0, 4);
            this.scMain.Name = "scMain";
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.BackColor = System.Drawing.Color.White;
            this.scMain.Panel1.Controls.Add(this.pTreeViewObjects);
            this.scMain.Panel1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 1);
            this.scMain.Panel1Collapsed = true;
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.pScriptEditorClient);
            this.scMain.Panel2.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.scMain.Size = new System.Drawing.Size(668, 492);
            this.scMain.SplitterDistance = 256;
            this.scMain.SplitterWidth = 7;
            this.scMain.TabIndex = 3;
            this.scMain.TabStop = false;
            // 
            // pTreeViewObjects
            // 
            this.pTreeViewObjects.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pTreeViewObjects.Controls.Add(this.tvObjects);
            this.pTreeViewObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pTreeViewObjects.Location = new System.Drawing.Point(0, 2);
            this.pTreeViewObjects.Name = "pTreeViewObjects";
            this.pTreeViewObjects.Size = new System.Drawing.Size(256, 97);
            this.pTreeViewObjects.TabIndex = 0;
            // 
            // pScriptEditorClient
            // 
            this.pScriptEditorClient.Controls.Add(this.tcScriptEditor);
            this.pScriptEditorClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pScriptEditorClient.Location = new System.Drawing.Point(0, 1);
            this.pScriptEditorClient.Name = "pScriptEditorClient";
            this.pScriptEditorClient.Size = new System.Drawing.Size(668, 491);
            this.pScriptEditorClient.TabIndex = 0;
            // 
            // tcScriptEditor
            // 
            this.tcScriptEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcScriptEditor.Controls.Add(this.tbScript);
            this.tcScriptEditor.Location = new System.Drawing.Point(0, 1);
            this.tcScriptEditor.Name = "tcScriptEditor";
            this.tcScriptEditor.SelectedIndex = 0;
            this.tcScriptEditor.Size = new System.Drawing.Size(670, 491);
            this.tcScriptEditor.TabIndex = 1;
            this.tcScriptEditor.TabStop = false;
            // 
            // tbScript
            // 
            this.tbScript.Controls.Add(this.scScriptEditor);
            this.tbScript.Location = new System.Drawing.Point(4, 22);
            this.tbScript.Name = "tbScript";
            this.tbScript.Padding = new System.Windows.Forms.Padding(3);
            this.tbScript.Size = new System.Drawing.Size(662, 465);
            this.tbScript.TabIndex = 0;
            this.tbScript.Text = "Script editor";
            this.tbScript.UseVisualStyleBackColor = true;
            // 
            // scScriptEditor
            // 
            this.scScriptEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scScriptEditor.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.scScriptEditor.Location = new System.Drawing.Point(3, 3);
            this.scScriptEditor.Name = "scScriptEditor";
            this.scScriptEditor.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scScriptEditor.Panel1
            // 
            this.scScriptEditor.Panel1.Controls.Add(this.cbEventType);
            this.scScriptEditor.Panel1.Controls.Add(this.pScript);
            this.scScriptEditor.Panel1.Controls.Add(this.tsScriptEditor);
            this.scScriptEditor.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            // 
            // scScriptEditor.Panel2
            // 
            this.scScriptEditor.Panel2.Controls.Add(this.pScriptOut);
            this.scScriptEditor.Panel2.Padding = new System.Windows.Forms.Padding(0, 0, 2, 1);
            this.scScriptEditor.Size = new System.Drawing.Size(656, 459);
            this.scScriptEditor.SplitterDistance = 337;
            this.scScriptEditor.SplitterWidth = 6;
            this.scScriptEditor.TabIndex = 1;
            this.scScriptEditor.TabStop = false;
            // 
            // cbEventType
            // 
            this.cbEventType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbEventType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEventType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cbEventType.FormattingEnabled = true;
            this.cbEventType.Location = new System.Drawing.Point(506, 0);
            this.cbEventType.Name = "cbEventType";
            this.cbEventType.Size = new System.Drawing.Size(145, 23);
            this.cbEventType.TabIndex = 28;
            this.cbEventType.TabStop = false;
            this.cbEventType.DroppingDown += new GMechanics.Editor.UIControls.Common.ComboBoxEx.DroppingDownHandler(this.cbEventType_DroppingDown);
            this.cbEventType.SelectedIndexChanged += new System.EventHandler(this.CbEventTypeSelectedIndexChanged);
            // 
            // pScript
            // 
            this.pScript.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.pScript.Controls.Add(this.teScript);
            this.pScript.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pScript.Location = new System.Drawing.Point(0, 25);
            this.pScript.Name = "pScript";
            this.pScript.Padding = new System.Windows.Forms.Padding(1);
            this.pScript.Size = new System.Drawing.Size(654, 312);
            this.pScript.TabIndex = 26;
            // 
            // teScript
            // 
            this.teScript.AllowDrop = true;
            this.teScript.AutoScrollMinSize = new System.Drawing.Size(27, 14);
            this.teScript.BackBrush = null;
            this.teScript.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.teScript.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.teScript.Dock = System.Windows.Forms.DockStyle.Fill;
            this.teScript.Font = new System.Drawing.Font("Courier New", 9.75F);
            this.teScript.Language = GMechanics.ScriptTextBox.Language.GameScript;
            this.teScript.LeftBracket = '(';
            this.teScript.LeftBracket2 = '{';
            this.teScript.Location = new System.Drawing.Point(1, 1);
            this.teScript.Name = "teScript";
            this.teScript.Paddings = new System.Windows.Forms.Padding(0);
            this.teScript.RightBracket = ')';
            this.teScript.RightBracket2 = '}';
            this.teScript.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.teScript.Size = new System.Drawing.Size(652, 310);
            this.teScript.TabIndex = 25;
            this.teScript.TextChanged += new System.EventHandler<GMechanics.ScriptTextBox.TextChangedEventArgs>(this.TeScriptTextChanged);
            this.teScript.KeyPressing += new System.Windows.Forms.KeyPressEventHandler(this.TeScriptKeyPressing);
            // 
            // tsScriptEditor
            // 
            this.tsScriptEditor.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsScriptEditor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbScriptActive,
            this.btnCompileScript});
            this.tsScriptEditor.Location = new System.Drawing.Point(0, 0);
            this.tsScriptEditor.Name = "tsScriptEditor";
            this.tsScriptEditor.Size = new System.Drawing.Size(654, 25);
            this.tsScriptEditor.TabIndex = 25;
            // 
            // cbScriptActive
            // 
            this.cbScriptActive.CheckOnClick = true;
            this.cbScriptActive.Image = global::GMechanics.Editor.Properties.Resources.bullet_ball_glass_red;
            this.cbScriptActive.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cbScriptActive.Name = "cbScriptActive";
            this.cbScriptActive.Size = new System.Drawing.Size(112, 22);
            this.cbScriptActive.Text = "Script is inactive";
            this.cbScriptActive.Click += new System.EventHandler(this.CbScriptActiveClick);
            // 
            // btnCompileScript
            // 
            this.btnCompileScript.Image = global::GMechanics.Editor.Properties.Resources.scroll_run;
            this.btnCompileScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCompileScript.Name = "btnCompileScript";
            this.btnCompileScript.Size = new System.Drawing.Size(104, 22);
            this.btnCompileScript.Text = "Compile script";
            this.btnCompileScript.ToolTipText = "Compile script (F5)";
            this.btnCompileScript.Click += new System.EventHandler(this.BtnCompileScriptClick);
            // 
            // pScriptOut
            // 
            this.pScriptOut.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.pScriptOut.Controls.Add(this.tbScriptOut);
            this.pScriptOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pScriptOut.Location = new System.Drawing.Point(0, 0);
            this.pScriptOut.Name = "pScriptOut";
            this.pScriptOut.Padding = new System.Windows.Forms.Padding(1);
            this.pScriptOut.Size = new System.Drawing.Size(654, 115);
            this.pScriptOut.TabIndex = 0;
            // 
            // tbScriptOut
            // 
            this.tbScriptOut.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbScriptOut.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbScriptOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbScriptOut.Location = new System.Drawing.Point(1, 1);
            this.tbScriptOut.Multiline = true;
            this.tbScriptOut.Name = "tbScriptOut";
            this.tbScriptOut.ReadOnly = true;
            this.tbScriptOut.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbScriptOut.Size = new System.Drawing.Size(652, 113);
            this.tbScriptOut.TabIndex = 1;
            // 
            // ScriptEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(691, 536);
            this.Controls.Add(this.pClient);
            this.Controls.Add(this.pTreeViewPathEdit);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.KeyPreview = true;
            this.Name = "ScriptEditorForm";
            this.Padding = new System.Windows.Forms.Padding(12, 5, 11, 11);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Script editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScriptEditorFormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ScriptEditorFormKeyDown);
            this.pTreeViewPathEdit.ResumeLayout(false);
            this.pClient.ResumeLayout(false);
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel2.ResumeLayout(false);
            this.scMain.ResumeLayout(false);
            this.pTreeViewObjects.ResumeLayout(false);
            this.pScriptEditorClient.ResumeLayout(false);
            this.tcScriptEditor.ResumeLayout(false);
            this.tbScript.ResumeLayout(false);
            this.scScriptEditor.Panel1.ResumeLayout(false);
            this.scScriptEditor.Panel1.PerformLayout();
            this.scScriptEditor.Panel2.ResumeLayout(false);
            this.scScriptEditor.ResumeLayout(false);
            this.pScript.ResumeLayout(false);
            this.tsScriptEditor.ResumeLayout(false);
            this.tsScriptEditor.PerformLayout();
            this.pScriptOut.ResumeLayout(false);
            this.pScriptOut.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pTreeViewPathEdit;
        private TreeViewPathEdit tvpeObjects;
        private System.Windows.Forms.Panel pClient;
        private System.Windows.Forms.SplitContainer scMain;
        private System.Windows.Forms.Panel pTreeViewObjects;
        private System.Windows.Forms.Panel pScriptEditorClient;
        private System.Windows.Forms.TabControl tcScriptEditor;
        private System.Windows.Forms.TabPage tbScript;
        private System.Windows.Forms.SplitContainer scScriptEditor;
        private System.Windows.Forms.Panel pScript;
        private ScriptTextBoxControl teScript;
        private System.Windows.Forms.ToolStrip tsScriptEditor;
        private System.Windows.Forms.Panel pDivider1;
        private System.Windows.Forms.Panel pScriptOut;
        private System.Windows.Forms.TextBox tbScriptOut;
        private System.Windows.Forms.ToolStripButton btnCompileScript;
        private GameObjectsEntitiesTreeView tvObjects;
        private ComboBoxEx cbEventType;
        private System.Windows.Forms.ToolTip ttMain;
        private System.Windows.Forms.ToolStripButton cbScriptActive;
    }
}