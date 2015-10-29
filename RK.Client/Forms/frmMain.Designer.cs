using RK.Client.Controls;
using RK.Client.UserActivityMonitor;

namespace RK.Client.Forms
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpMap = new System.Windows.Forms.TabPage();
            this.pMapCtrl = new System.Windows.Forms.Panel();
            this.pMiniMap = new System.Windows.Forms.Panel();
            this.miniMapCtrl = new MiniMapControl();
            this.mapCtrl = new MapControl();
            this.toolStrip4 = new System.Windows.Forms.ToolStrip();
            this.btnLoadLabyrinth = new System.Windows.Forms.ToolStripButton();
            this.lblLabyrinthTilePos = new System.Windows.Forms.ToolStripLabel();
            this.tpLabyrinthGenerator = new System.Windows.Forms.TabPage();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.chkLabyrinthAC = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel6 = new System.Windows.Forms.ToolStripLabel();
            this.tbLabyrinthACLow = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel13 = new System.Windows.Forms.ToolStripLabel();
            this.tbLabyrinthACHigh = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel14 = new System.Windows.Forms.ToolStripLabel();
            this.cbLabyrinthACCombine = new System.Windows.Forms.ToolStripComboBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.chkLabyrinthSel = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel7 = new System.Windows.Forms.ToolStripLabel();
            this.tbLabyrinthSelLow = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel8 = new System.Windows.Forms.ToolStripLabel();
            this.tbLabyrinthSelHigh = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel9 = new System.Windows.Forms.ToolStripLabel();
            this.tbLabyrinthSelThreshold = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel10 = new System.Windows.Forms.ToolStripLabel();
            this.tbLabyrinthSelFalloff = new System.Windows.Forms.ToolStripTextBox();
            this.pLabyrinth = new System.Windows.Forms.Panel();
            this.cbLabyrinthRnd = new System.Windows.Forms.CheckBox();
            this.pbLabyrinth = new System.Windows.Forms.PictureBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnGenerateLabyrinth = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.cbLabyrinthType = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.cbLabyrinthBasis = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.cbLabyrinthInterp = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tbLabyrinthOctaves = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.tbLabyrinthFrequency = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel11 = new System.Windows.Forms.ToolStripLabel();
            this.tbLabyrinthAngle = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel12 = new System.Windows.Forms.ToolStripLabel();
            this.tbLabyrinthLacunarity = new System.Windows.Forms.ToolStripTextBox();
            this.eventsProvider = new GlobalEventProvider();
            this.tcMain.SuspendLayout();
            this.tpMap.SuspendLayout();
            this.pMapCtrl.SuspendLayout();
            this.pMiniMap.SuspendLayout();
            this.toolStrip4.SuspendLayout();
            this.tpLabyrinthGenerator.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.pLabyrinth.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLabyrinth)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tpMap);
            this.tcMain.Controls.Add(this.tpLabyrinthGenerator);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(0, 0);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(735, 625);
            this.tcMain.TabIndex = 0;
            // 
            // tpMap
            // 
            this.tpMap.Controls.Add(this.pMapCtrl);
            this.tpMap.Controls.Add(this.toolStrip4);
            this.tpMap.Location = new System.Drawing.Point(4, 22);
            this.tpMap.Name = "tpMap";
            this.tpMap.Padding = new System.Windows.Forms.Padding(3);
            this.tpMap.Size = new System.Drawing.Size(727, 599);
            this.tpMap.TabIndex = 0;
            this.tpMap.Text = "Whole map";
            this.tpMap.UseVisualStyleBackColor = true;
            // 
            // pMapCtrl
            // 
            this.pMapCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pMapCtrl.Controls.Add(this.pMiniMap);
            this.pMapCtrl.Controls.Add(this.mapCtrl);
            this.pMapCtrl.Location = new System.Drawing.Point(3, 31);
            this.pMapCtrl.Name = "pMapCtrl";
            this.pMapCtrl.Size = new System.Drawing.Size(721, 565);
            this.pMapCtrl.TabIndex = 3;
            // 
            // pMiniMap
            // 
            this.pMiniMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pMiniMap.BackColor = System.Drawing.Color.DimGray;
            this.pMiniMap.Controls.Add(this.miniMapCtrl);
            this.pMiniMap.Location = new System.Drawing.Point(519, 363);
            this.pMiniMap.Name = "pMiniMap";
            this.pMiniMap.Padding = new System.Windows.Forms.Padding(1);
            this.pMiniMap.Size = new System.Drawing.Size(202, 202);
            this.pMiniMap.TabIndex = 5;
            // 
            // miniMapCtrl
            // 
            this.miniMapCtrl.BackColor = System.Drawing.Color.Black;
            this.miniMapCtrl.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.miniMapCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.miniMapCtrl.Location = new System.Drawing.Point(1, 1);
            this.miniMapCtrl.MapControl = this.mapCtrl;
            this.miniMapCtrl.Name = "miniMapCtrl";
            this.miniMapCtrl.Size = new System.Drawing.Size(200, 200);
            this.miniMapCtrl.TabIndex = 0;
            this.miniMapCtrl.TabStop = false;
            // 
            // mapCtrl
            // 
            this.mapCtrl.BackColor = System.Drawing.Color.Black;
            this.mapCtrl.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mapCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapCtrl.Font = new System.Drawing.Font("Verdana", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.mapCtrl.ForeColor = System.Drawing.Color.DarkGray;
            this.mapCtrl.Location = new System.Drawing.Point(0, 0);
            this.mapCtrl.Name = "mapCtrl";
            this.mapCtrl.PosX = 0;
            this.mapCtrl.PosY = 0;
            this.mapCtrl.ShowTileNumber = false;
            this.mapCtrl.Size = new System.Drawing.Size(721, 565);
            this.mapCtrl.TabIndex = 1;
            this.mapCtrl.TabStop = false;
            this.mapCtrl.Text = "map1";
            this.mapCtrl.MouseWheel += new MouseWheel(this.MapMouseWheel);
            this.mapCtrl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MapMouseDown);
            this.mapCtrl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MapMouseMove);
            this.mapCtrl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MapMouseUp);
            // 
            // toolStrip4
            // 
            this.toolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLoadLabyrinth,
            this.lblLabyrinthTilePos});
            this.toolStrip4.Location = new System.Drawing.Point(3, 3);
            this.toolStrip4.Name = "toolStrip4";
            this.toolStrip4.Size = new System.Drawing.Size(721, 25);
            this.toolStrip4.TabIndex = 2;
            this.toolStrip4.Text = "toolStrip4";
            // 
            // btnLoadLabyrinth
            // 
            this.btnLoadLabyrinth.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLoadLabyrinth.Image = ((System.Drawing.Image)(resources.GetObject("btnLoadLabyrinth.Image")));
            this.btnLoadLabyrinth.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoadLabyrinth.Name = "btnLoadLabyrinth";
            this.btnLoadLabyrinth.Size = new System.Drawing.Size(23, 22);
            this.btnLoadLabyrinth.Text = "Load labyrinth";
            this.btnLoadLabyrinth.Click += new System.EventHandler(this.BtnLoadLabyrinthClick);
            // 
            // lblLabyrinthTilePos
            // 
            this.lblLabyrinthTilePos.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblLabyrinthTilePos.Name = "lblLabyrinthTilePos";
            this.lblLabyrinthTilePos.Size = new System.Drawing.Size(112, 22);
            this.lblLabyrinthTilePos.Text = "No tile under cursor";
            // 
            // tpLabyrinthGenerator
            // 
            this.tpLabyrinthGenerator.Controls.Add(this.toolStrip3);
            this.tpLabyrinthGenerator.Controls.Add(this.toolStrip2);
            this.tpLabyrinthGenerator.Controls.Add(this.pLabyrinth);
            this.tpLabyrinthGenerator.Controls.Add(this.toolStrip1);
            this.tpLabyrinthGenerator.Location = new System.Drawing.Point(4, 22);
            this.tpLabyrinthGenerator.Name = "tpLabyrinthGenerator";
            this.tpLabyrinthGenerator.Padding = new System.Windows.Forms.Padding(3);
            this.tpLabyrinthGenerator.Size = new System.Drawing.Size(727, 599);
            this.tpLabyrinthGenerator.TabIndex = 1;
            this.tpLabyrinthGenerator.Text = "Labyrinth";
            this.tpLabyrinthGenerator.UseVisualStyleBackColor = true;
            // 
            // toolStrip3
            // 
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chkLabyrinthAC,
            this.toolStripSeparator3,
            this.toolStripLabel6,
            this.tbLabyrinthACLow,
            this.toolStripLabel13,
            this.tbLabyrinthACHigh,
            this.toolStripLabel14,
            this.cbLabyrinthACCombine});
            this.toolStrip3.Location = new System.Drawing.Point(3, 53);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(721, 25);
            this.toolStrip3.TabIndex = 5;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // chkLabyrinthAC
            // 
            this.chkLabyrinthAC.Checked = true;
            this.chkLabyrinthAC.CheckOnClick = true;
            this.chkLabyrinthAC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLabyrinthAC.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.chkLabyrinthAC.Image = ((System.Drawing.Image)(resources.GetObject("chkLabyrinthAC.Image")));
            this.chkLabyrinthAC.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.chkLabyrinthAC.Name = "chkLabyrinthAC";
            this.chkLabyrinthAC.Size = new System.Drawing.Size(87, 22);
            this.chkLabyrinthAC.Text = "Autocorrect →";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel6
            // 
            this.toolStripLabel6.Name = "toolStripLabel6";
            this.toolStripLabel6.Size = new System.Drawing.Size(29, 22);
            this.toolStripLabel6.Text = "Low";
            // 
            // tbLabyrinthACLow
            // 
            this.tbLabyrinthACLow.Name = "tbLabyrinthACLow";
            this.tbLabyrinthACLow.Size = new System.Drawing.Size(38, 25);
            this.tbLabyrinthACLow.Text = "0";
            // 
            // toolStripLabel13
            // 
            this.toolStripLabel13.Name = "toolStripLabel13";
            this.toolStripLabel13.Size = new System.Drawing.Size(33, 22);
            this.toolStripLabel13.Text = "High";
            // 
            // tbLabyrinthACHigh
            // 
            this.tbLabyrinthACHigh.Name = "tbLabyrinthACHigh";
            this.tbLabyrinthACHigh.Size = new System.Drawing.Size(38, 25);
            this.tbLabyrinthACHigh.Text = "0.5";
            // 
            // toolStripLabel14
            // 
            this.toolStripLabel14.Name = "toolStripLabel14";
            this.toolStripLabel14.Size = new System.Drawing.Size(56, 22);
            this.toolStripLabel14.Text = "Combine";
            // 
            // cbLabyrinthACCombine
            // 
            this.cbLabyrinthACCombine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLabyrinthACCombine.Name = "cbLabyrinthACCombine";
            this.cbLabyrinthACCombine.Size = new System.Drawing.Size(75, 25);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chkLabyrinthSel,
            this.toolStripSeparator2,
            this.toolStripLabel7,
            this.tbLabyrinthSelLow,
            this.toolStripLabel8,
            this.tbLabyrinthSelHigh,
            this.toolStripLabel9,
            this.tbLabyrinthSelThreshold,
            this.toolStripLabel10,
            this.tbLabyrinthSelFalloff});
            this.toolStrip2.Location = new System.Drawing.Point(3, 28);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(721, 25);
            this.toolStrip2.TabIndex = 4;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // chkLabyrinthSel
            // 
            this.chkLabyrinthSel.Checked = true;
            this.chkLabyrinthSel.CheckOnClick = true;
            this.chkLabyrinthSel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLabyrinthSel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.chkLabyrinthSel.Image = ((System.Drawing.Image)(resources.GetObject("chkLabyrinthSel.Image")));
            this.chkLabyrinthSel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.chkLabyrinthSel.Name = "chkLabyrinthSel";
            this.chkLabyrinthSel.Size = new System.Drawing.Size(55, 22);
            this.chkLabyrinthSel.Text = "Select →";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel7
            // 
            this.toolStripLabel7.Name = "toolStripLabel7";
            this.toolStripLabel7.Size = new System.Drawing.Size(29, 22);
            this.toolStripLabel7.Text = "Low";
            // 
            // tbLabyrinthSelLow
            // 
            this.tbLabyrinthSelLow.Name = "tbLabyrinthSelLow";
            this.tbLabyrinthSelLow.Size = new System.Drawing.Size(38, 25);
            this.tbLabyrinthSelLow.Text = "0";
            // 
            // toolStripLabel8
            // 
            this.toolStripLabel8.Name = "toolStripLabel8";
            this.toolStripLabel8.Size = new System.Drawing.Size(33, 22);
            this.toolStripLabel8.Text = "High";
            // 
            // tbLabyrinthSelHigh
            // 
            this.tbLabyrinthSelHigh.Name = "tbLabyrinthSelHigh";
            this.tbLabyrinthSelHigh.Size = new System.Drawing.Size(38, 25);
            this.tbLabyrinthSelHigh.Text = "1";
            // 
            // toolStripLabel9
            // 
            this.toolStripLabel9.Name = "toolStripLabel9";
            this.toolStripLabel9.Size = new System.Drawing.Size(60, 22);
            this.toolStripLabel9.Text = "Threshold";
            // 
            // tbLabyrinthSelThreshold
            // 
            this.tbLabyrinthSelThreshold.Name = "tbLabyrinthSelThreshold";
            this.tbLabyrinthSelThreshold.Size = new System.Drawing.Size(38, 25);
            this.tbLabyrinthSelThreshold.Text = "0.01";
            // 
            // toolStripLabel10
            // 
            this.toolStripLabel10.Name = "toolStripLabel10";
            this.toolStripLabel10.Size = new System.Drawing.Size(40, 22);
            this.toolStripLabel10.Text = "Falloff";
            // 
            // tbLabyrinthSelFalloff
            // 
            this.tbLabyrinthSelFalloff.Name = "tbLabyrinthSelFalloff";
            this.tbLabyrinthSelFalloff.Size = new System.Drawing.Size(38, 25);
            this.tbLabyrinthSelFalloff.Text = "0";
            // 
            // pLabyrinth
            // 
            this.pLabyrinth.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pLabyrinth.BackColor = System.Drawing.Color.Black;
            this.pLabyrinth.Controls.Add(this.cbLabyrinthRnd);
            this.pLabyrinth.Controls.Add(this.pbLabyrinth);
            this.pLabyrinth.Location = new System.Drawing.Point(3, 81);
            this.pLabyrinth.Name = "pLabyrinth";
            this.pLabyrinth.Size = new System.Drawing.Size(721, 515);
            this.pLabyrinth.TabIndex = 2;
            // 
            // cbLabyrinthRnd
            // 
            this.cbLabyrinthRnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbLabyrinthRnd.AutoSize = true;
            this.cbLabyrinthRnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbLabyrinthRnd.ForeColor = System.Drawing.Color.White;
            this.cbLabyrinthRnd.Location = new System.Drawing.Point(672, 3);
            this.cbLabyrinthRnd.Name = "cbLabyrinthRnd";
            this.cbLabyrinthRnd.Size = new System.Drawing.Size(49, 17);
            this.cbLabyrinthRnd.TabIndex = 5;
            this.cbLabyrinthRnd.Text = "Rnd";
            this.cbLabyrinthRnd.UseVisualStyleBackColor = true;
            // 
            // pbLabyrinth
            // 
            this.pbLabyrinth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbLabyrinth.Location = new System.Drawing.Point(0, 0);
            this.pbLabyrinth.Name = "pbLabyrinth";
            this.pbLabyrinth.Size = new System.Drawing.Size(721, 515);
            this.pbLabyrinth.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbLabyrinth.TabIndex = 1;
            this.pbLabyrinth.TabStop = false;
            this.pbLabyrinth.Click += new System.EventHandler(this.PbLabyrinthClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnGenerateLabyrinth,
            this.toolStripSeparator1,
            this.toolStripLabel3,
            this.cbLabyrinthType,
            this.toolStripLabel4,
            this.cbLabyrinthBasis,
            this.toolStripLabel5,
            this.cbLabyrinthInterp,
            this.toolStripLabel1,
            this.tbLabyrinthOctaves,
            this.toolStripLabel2,
            this.tbLabyrinthFrequency,
            this.toolStripLabel11,
            this.tbLabyrinthAngle,
            this.toolStripLabel12,
            this.tbLabyrinthLacunarity});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(721, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnGenerateLabyrinth
            // 
            this.btnGenerateLabyrinth.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnGenerateLabyrinth.Image = ((System.Drawing.Image)(resources.GetObject("btnGenerateLabyrinth.Image")));
            this.btnGenerateLabyrinth.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGenerateLabyrinth.Name = "btnGenerateLabyrinth";
            this.btnGenerateLabyrinth.Size = new System.Drawing.Size(23, 22);
            this.btnGenerateLabyrinth.Text = "Generate labyrinth";
            this.btnGenerateLabyrinth.Click += new System.EventHandler(this.BtnGenerateLabyrinthClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(33, 22);
            this.toolStripLabel3.Text = "Type";
            // 
            // cbLabyrinthType
            // 
            this.cbLabyrinthType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLabyrinthType.Name = "cbLabyrinthType";
            this.cbLabyrinthType.Size = new System.Drawing.Size(100, 25);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(33, 22);
            this.toolStripLabel4.Text = "Basis";
            // 
            // cbLabyrinthBasis
            // 
            this.cbLabyrinthBasis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLabyrinthBasis.Name = "cbLabyrinthBasis";
            this.cbLabyrinthBasis.Size = new System.Drawing.Size(80, 25);
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(38, 22);
            this.toolStripLabel5.Text = "Interp";
            // 
            // cbLabyrinthInterp
            // 
            this.cbLabyrinthInterp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLabyrinthInterp.Name = "cbLabyrinthInterp";
            this.cbLabyrinthInterp.Size = new System.Drawing.Size(75, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(49, 22);
            this.toolStripLabel1.Text = "Octaves";
            // 
            // tbLabyrinthOctaves
            // 
            this.tbLabyrinthOctaves.Name = "tbLabyrinthOctaves";
            this.tbLabyrinthOctaves.Size = new System.Drawing.Size(38, 25);
            this.tbLabyrinthOctaves.Text = "1";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(30, 22);
            this.toolStripLabel2.Text = "Freq";
            // 
            // tbLabyrinthFrequency
            // 
            this.tbLabyrinthFrequency.Name = "tbLabyrinthFrequency";
            this.tbLabyrinthFrequency.Size = new System.Drawing.Size(38, 25);
            this.tbLabyrinthFrequency.Text = "60";
            // 
            // toolStripLabel11
            // 
            this.toolStripLabel11.Name = "toolStripLabel11";
            this.toolStripLabel11.Size = new System.Drawing.Size(38, 22);
            this.toolStripLabel11.Text = "Angle";
            // 
            // tbLabyrinthAngle
            // 
            this.tbLabyrinthAngle.Name = "tbLabyrinthAngle";
            this.tbLabyrinthAngle.Size = new System.Drawing.Size(38, 25);
            this.tbLabyrinthAngle.Text = "0";
            // 
            // toolStripLabel12
            // 
            this.toolStripLabel12.Name = "toolStripLabel12";
            this.toolStripLabel12.Size = new System.Drawing.Size(39, 22);
            this.toolStripLabel12.Text = "Lacun";
            // 
            // tbLabyrinthLacunarity
            // 
            this.tbLabyrinthLacunarity.Name = "tbLabyrinthLacunarity";
            this.tbLabyrinthLacunarity.Size = new System.Drawing.Size(38, 25);
            this.tbLabyrinthLacunarity.Text = "2";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 625);
            this.Controls.Add(this.tcMain);
            this.KeyPreview = true;
            this.Name = "frmMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RK.Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMainFormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EventsProviderKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.EventsProviderKeyUp);
            this.tcMain.ResumeLayout(false);
            this.tpMap.ResumeLayout(false);
            this.tpMap.PerformLayout();
            this.pMapCtrl.ResumeLayout(false);
            this.pMiniMap.ResumeLayout(false);
            this.toolStrip4.ResumeLayout(false);
            this.toolStrip4.PerformLayout();
            this.tpLabyrinthGenerator.ResumeLayout(false);
            this.tpLabyrinthGenerator.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.pLabyrinth.ResumeLayout(false);
            this.pLabyrinth.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLabyrinth)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tpMap;
        private System.Windows.Forms.TabPage tpLabyrinthGenerator;
        private System.Windows.Forms.Panel pLabyrinth;
        private System.Windows.Forms.PictureBox pbLabyrinth;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnGenerateLabyrinth;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripTextBox tbLabyrinthOctaves;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox tbLabyrinthFrequency;
        private System.Windows.Forms.ToolStripComboBox cbLabyrinthType;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripComboBox cbLabyrinthBasis;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripComboBox cbLabyrinthInterp;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel7;
        private System.Windows.Forms.ToolStripTextBox tbLabyrinthSelLow;
        private System.Windows.Forms.ToolStripLabel toolStripLabel8;
        private System.Windows.Forms.ToolStripTextBox tbLabyrinthSelHigh;
        private System.Windows.Forms.ToolStripLabel toolStripLabel9;
        private System.Windows.Forms.ToolStripTextBox tbLabyrinthSelThreshold;
        private System.Windows.Forms.ToolStripTextBox tbLabyrinthSelFalloff;
        private System.Windows.Forms.ToolStripLabel toolStripLabel10;
        private System.Windows.Forms.ToolStripLabel toolStripLabel11;
        private System.Windows.Forms.ToolStripTextBox tbLabyrinthAngle;
        private System.Windows.Forms.ToolStripLabel toolStripLabel12;
        private System.Windows.Forms.ToolStripTextBox tbLabyrinthLacunarity;
        private System.Windows.Forms.ToolStripButton chkLabyrinthSel;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripButton chkLabyrinthAC;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel6;
        private System.Windows.Forms.ToolStripTextBox tbLabyrinthACLow;
        private System.Windows.Forms.ToolStripLabel toolStripLabel13;
        private System.Windows.Forms.ToolStripTextBox tbLabyrinthACHigh;
        private System.Windows.Forms.ToolStripLabel toolStripLabel14;
        private System.Windows.Forms.ToolStripComboBox cbLabyrinthACCombine;
        private System.Windows.Forms.CheckBox cbLabyrinthRnd;
        private System.Windows.Forms.ToolStrip toolStrip4;
        private System.Windows.Forms.ToolStripButton btnLoadLabyrinth;
        private System.Windows.Forms.Panel pMapCtrl;
        private Controls.MapControl mapCtrl;
        private System.Windows.Forms.ToolStripLabel lblLabyrinthTilePos;
        private System.Windows.Forms.Panel pMiniMap;
        private Controls.MiniMapControl miniMapCtrl;
        private UserActivityMonitor.GlobalEventProvider eventsProvider;

    }
}

