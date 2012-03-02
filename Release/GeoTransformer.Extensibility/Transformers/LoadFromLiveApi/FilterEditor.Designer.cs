namespace GeoTransformer.Transformers.LoadFromLiveApi
{
    partial class FilterEditor
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
            System.Windows.Forms.Label Label1;
            System.Windows.Forms.Label Label3;
            System.Windows.Forms.Label Label2;
            System.Windows.Forms.Label Label4;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.Label label9;
            System.Windows.Forms.Label label10;
            System.Windows.Forms.Label label11;
            System.Windows.Forms.Label label12;
            System.Windows.Forms.Label label13;
            System.Windows.Forms.Label label14;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterEditor));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.txtQueryTitle = new System.Windows.Forms.TextBox();
            this.txtCenterName = new System.Windows.Forms.TextBox();
            this.txtCenterCoords = new GeoTransformer.UI.CoordinateEditor();
            this.txtRadius = new System.Windows.Forms.NumericUpDown();
            this.chkIncludeDisabled = new System.Windows.Forms.CheckBox();
            this.txtMaxCaches = new System.Windows.Forms.NumericUpDown();
            this.drpMinDiff = new System.Windows.Forms.ComboBox();
            this.drpMaxDiff = new System.Windows.Forms.ComboBox();
            this.drpMinTerrain = new System.Windows.Forms.ComboBox();
            this.drpMaxTerrain = new System.Windows.Forms.ComboBox();
            this.txtMinFavPoints = new System.Windows.Forms.NumericUpDown();
            this.drpFoundByMe = new System.Windows.Forms.ComboBox();
            this.drpHiddenByMe = new System.Windows.Forms.ComboBox();
            this.labelBasicMembers = new System.Windows.Forms.LinkLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonCancel = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolTipForOptions = new System.Windows.Forms.ToolTip(this.components);
            Label1 = new System.Windows.Forms.Label();
            Label3 = new System.Windows.Forms.Label();
            Label2 = new System.Windows.Forms.Label();
            Label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            label12 = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            label14 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxCaches)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMinFavPoints)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Label1
            // 
            Label1.Location = new System.Drawing.Point(43, 206);
            Label1.Margin = new System.Windows.Forms.Padding(43, 4, 4, 4);
            Label1.MinimumSize = new System.Drawing.Size(0, 26);
            Label1.Name = "Label1";
            Label1.Size = new System.Drawing.Size(163, 26);
            Label1.TabIndex = 12;
            Label1.Text = "Difficulty:";
            Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label3
            // 
            Label3.AutoSize = true;
            Label3.Location = new System.Drawing.Point(286, 206);
            Label3.Margin = new System.Windows.Forms.Padding(4);
            Label3.MinimumSize = new System.Drawing.Size(0, 26);
            Label3.Name = "Label3";
            Label3.Size = new System.Drawing.Size(20, 26);
            Label3.TabIndex = 14;
            Label3.Text = "to";
            Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label2
            // 
            Label2.Location = new System.Drawing.Point(43, 240);
            Label2.Margin = new System.Windows.Forms.Padding(43, 4, 4, 4);
            Label2.MinimumSize = new System.Drawing.Size(0, 26);
            Label2.Name = "Label2";
            Label2.Size = new System.Drawing.Size(163, 26);
            Label2.TabIndex = 16;
            Label2.Text = "Terrain:";
            Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label4
            // 
            Label4.AutoSize = true;
            Label4.Location = new System.Drawing.Point(286, 240);
            Label4.Margin = new System.Windows.Forms.Padding(4);
            Label4.MinimumSize = new System.Drawing.Size(0, 26);
            Label4.Name = "Label4";
            Label4.Size = new System.Drawing.Size(20, 26);
            Label4.TabIndex = 18;
            Label4.Text = "to";
            Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            label5.Location = new System.Drawing.Point(43, 274);
            label5.Margin = new System.Windows.Forms.Padding(43, 4, 4, 4);
            label5.MinimumSize = new System.Drawing.Size(0, 26);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(163, 26);
            label5.TabIndex = 20;
            label5.Text = "Favorite points:";
            label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTipForOptions.SetToolTip(label5, "Enter the minimum number of favorite points the cache must\r\nhave to be included i" +
        "n the results.");
            // 
            // label6
            // 
            label6.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(label6, true);
            label6.Location = new System.Drawing.Point(285, 274);
            label6.Margin = new System.Windows.Forms.Padding(4);
            label6.MinimumSize = new System.Drawing.Size(0, 26);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(57, 26);
            label6.TabIndex = 22;
            label6.Text = "or more";
            label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            label7.Location = new System.Drawing.Point(43, 308);
            label7.Margin = new System.Windows.Forms.Padding(43, 4, 4, 4);
            label7.MinimumSize = new System.Drawing.Size(0, 26);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(163, 26);
            label7.TabIndex = 23;
            label7.Text = "Found by me:";
            label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTipForOptions.SetToolTip(label7, "Specify if you want to include caches you have already\r\nlogged as found to be inc" +
        "luded in the results.");
            // 
            // label8
            // 
            label8.Location = new System.Drawing.Point(43, 342);
            label8.Margin = new System.Windows.Forms.Padding(43, 4, 4, 4);
            label8.MinimumSize = new System.Drawing.Size(0, 26);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(163, 26);
            label8.TabIndex = 25;
            label8.Text = "Hidden by me:";
            label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTipForOptions.SetToolTip(label8, "Specify if you want caches that you own to be included\r\nin the results. You can a" +
        "lso choose the option \"only mine\"\r\nthat will make the search to ignore caches hi" +
        "dden by\r\nothers.");
            // 
            // label9
            // 
            label9.Location = new System.Drawing.Point(43, 78);
            label9.Margin = new System.Windows.Forms.Padding(43, 4, 4, 4);
            label9.MinimumSize = new System.Drawing.Size(0, 26);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(163, 26);
            label9.TabIndex = 4;
            label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTipForOptions.SetToolTip(label9, "Enter the starting coordinates of the search. If specified,\r\nthe search will retu" +
        "rn caches that are closest to this point,\r\nlimited either by the radius or numbe" +
        "r of caches.");
            // 
            // label10
            // 
            label10.Location = new System.Drawing.Point(43, 113);
            label10.Margin = new System.Windows.Forms.Padding(43, 4, 4, 4);
            label10.MinimumSize = new System.Drawing.Size(0, 26);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(163, 26);
            label10.TabIndex = 6;
            label10.Text = "Maximum radius:";
            label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTipForOptions.SetToolTip(label10, "Specify the maximum radius in kilometers for the search.\r\nOnly caches that are cl" +
        "oser than this to the starting coordinates\r\nwill be returned.");
            // 
            // label11
            // 
            label11.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(label11, true);
            label11.Location = new System.Drawing.Point(285, 113);
            label11.Margin = new System.Windows.Forms.Padding(4);
            label11.MinimumSize = new System.Drawing.Size(0, 26);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(72, 26);
            label11.TabIndex = 8;
            label11.Text = "kilometers";
            label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label12
            // 
            label12.Location = new System.Drawing.Point(43, 10);
            label12.Margin = new System.Windows.Forms.Padding(43, 4, 4, 4);
            label12.MinimumSize = new System.Drawing.Size(0, 26);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(163, 26);
            label12.TabIndex = 0;
            label12.Text = "Title of this query:";
            label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTipForOptions.SetToolTip(label12, "Enter the title of the query so that you can later\r\nidentify it more easily.");
            // 
            // label13
            // 
            label13.Location = new System.Drawing.Point(43, 172);
            label13.Margin = new System.Windows.Forms.Padding(43, 4, 4, 4);
            label13.MinimumSize = new System.Drawing.Size(0, 26);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(163, 26);
            label13.TabIndex = 10;
            label13.Text = "Maximum caches:";
            label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTipForOptions.SetToolTip(label13, "Enter the maximum number of caches you want this\r\nsearch to return. If more cache" +
        "s meet the search \r\ncriteria, only the closest are returned.");
            // 
            // label14
            // 
            label14.Location = new System.Drawing.Point(43, 44);
            label14.Margin = new System.Windows.Forms.Padding(43, 4, 4, 4);
            label14.MinimumSize = new System.Drawing.Size(0, 26);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(163, 26);
            label14.TabIndex = 2;
            label14.Text = "Starting coordinates:";
            label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTipForOptions.SetToolTip(label14, resources.GetString("label14.ToolTip"));
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(label12);
            this.flowLayoutPanel1.Controls.Add(this.txtQueryTitle);
            this.flowLayoutPanel1.Controls.Add(label14);
            this.flowLayoutPanel1.Controls.Add(this.txtCenterName);
            this.flowLayoutPanel1.Controls.Add(label9);
            this.flowLayoutPanel1.Controls.Add(this.txtCenterCoords);
            this.flowLayoutPanel1.Controls.Add(label10);
            this.flowLayoutPanel1.Controls.Add(this.txtRadius);
            this.flowLayoutPanel1.Controls.Add(label11);
            this.flowLayoutPanel1.Controls.Add(this.chkIncludeDisabled);
            this.flowLayoutPanel1.Controls.Add(label13);
            this.flowLayoutPanel1.Controls.Add(this.txtMaxCaches);
            this.flowLayoutPanel1.Controls.Add(Label1);
            this.flowLayoutPanel1.Controls.Add(this.drpMinDiff);
            this.flowLayoutPanel1.Controls.Add(Label3);
            this.flowLayoutPanel1.Controls.Add(this.drpMaxDiff);
            this.flowLayoutPanel1.Controls.Add(Label2);
            this.flowLayoutPanel1.Controls.Add(this.drpMinTerrain);
            this.flowLayoutPanel1.Controls.Add(Label4);
            this.flowLayoutPanel1.Controls.Add(this.drpMaxTerrain);
            this.flowLayoutPanel1.Controls.Add(label5);
            this.flowLayoutPanel1.Controls.Add(this.txtMinFavPoints);
            this.flowLayoutPanel1.Controls.Add(label6);
            this.flowLayoutPanel1.Controls.Add(label7);
            this.flowLayoutPanel1.Controls.Add(this.drpFoundByMe);
            this.flowLayoutPanel1.Controls.Add(label8);
            this.flowLayoutPanel1.Controls.Add(this.drpHiddenByMe);
            this.flowLayoutPanel1.Controls.Add(this.labelBasicMembers);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(523, 567);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // txtQueryTitle
            // 
            this.flowLayoutPanel1.SetFlowBreak(this.txtQueryTitle, true);
            this.txtQueryTitle.Location = new System.Drawing.Point(213, 8);
            this.txtQueryTitle.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtQueryTitle.MaxLength = 50;
            this.txtQueryTitle.Name = "txtQueryTitle";
            this.txtQueryTitle.Size = new System.Drawing.Size(215, 22);
            this.txtQueryTitle.TabIndex = 1;
            // 
            // txtCenterName
            // 
            this.flowLayoutPanel1.SetFlowBreak(this.txtCenterName, true);
            this.txtCenterName.Location = new System.Drawing.Point(213, 42);
            this.txtCenterName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtCenterName.MaxLength = 50;
            this.txtCenterName.Name = "txtCenterName";
            this.txtCenterName.Size = new System.Drawing.Size(215, 22);
            this.txtCenterName.TabIndex = 3;
            this.txtCenterName.Leave += new System.EventHandler(this.txtCenterName_Leave);
            // 
            // txtCenterCoords
            // 
            this.txtCenterCoords.BackColor = System.Drawing.Color.Transparent;
            this.txtCenterCoords.BoundElement = null;
            this.txtCenterCoords.Coordinates = null;
            this.flowLayoutPanel1.SetFlowBreak(this.txtCenterCoords, true);
            this.txtCenterCoords.Location = new System.Drawing.Point(215, 79);
            this.txtCenterCoords.Margin = new System.Windows.Forms.Padding(5);
            this.txtCenterCoords.Name = "txtCenterCoords";
            this.txtCenterCoords.ReadOnly = false;
            this.txtCenterCoords.Size = new System.Drawing.Size(215, 25);
            this.txtCenterCoords.TabIndex = 5;
            this.txtCenterCoords.WatermarkText = "or enter coordinates";
            this.txtCenterCoords.Leave += new System.EventHandler(this.txtCenterCoords_Leave);
            // 
            // txtRadius
            // 
            this.txtRadius.Location = new System.Drawing.Point(213, 111);
            this.txtRadius.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtRadius.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.txtRadius.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtRadius.Name = "txtRadius";
            this.txtRadius.Size = new System.Drawing.Size(65, 22);
            this.txtRadius.TabIndex = 7;
            this.txtRadius.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // chkIncludeDisabled
            // 
            this.chkIncludeDisabled.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.chkIncludeDisabled, true);
            this.chkIncludeDisabled.Location = new System.Drawing.Point(213, 145);
            this.chkIncludeDisabled.Margin = new System.Windows.Forms.Padding(213, 2, 3, 2);
            this.chkIncludeDisabled.Name = "chkIncludeDisabled";
            this.chkIncludeDisabled.Size = new System.Drawing.Size(181, 21);
            this.chkIncludeDisabled.TabIndex = 9;
            this.chkIncludeDisabled.Text = "Include disabled caches";
            this.toolTipForOptions.SetToolTip(this.chkIncludeDisabled, "Check if you want disabled caches to be included in the\r\nresults. By default only" +
        " active caches are downloaded.");
            this.chkIncludeDisabled.UseVisualStyleBackColor = true;
            // 
            // txtMaxCaches
            // 
            this.flowLayoutPanel1.SetFlowBreak(this.txtMaxCaches, true);
            this.txtMaxCaches.Location = new System.Drawing.Point(213, 170);
            this.txtMaxCaches.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMaxCaches.Maximum = new decimal(new int[] {
            6000,
            0,
            0,
            0});
            this.txtMaxCaches.Name = "txtMaxCaches";
            this.txtMaxCaches.Size = new System.Drawing.Size(65, 22);
            this.txtMaxCaches.TabIndex = 11;
            this.txtMaxCaches.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // drpMinDiff
            // 
            this.drpMinDiff.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpMinDiff.FormattingEnabled = true;
            this.drpMinDiff.Items.AddRange(new object[] {
            "1",
            "1.5",
            "2",
            "2.5",
            "3",
            "3.5",
            "4",
            "4.5",
            "5"});
            this.drpMinDiff.Location = new System.Drawing.Point(214, 206);
            this.drpMinDiff.Margin = new System.Windows.Forms.Padding(4);
            this.drpMinDiff.Name = "drpMinDiff";
            this.drpMinDiff.Size = new System.Drawing.Size(64, 24);
            this.drpMinDiff.TabIndex = 13;
            // 
            // drpMaxDiff
            // 
            this.drpMaxDiff.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpMaxDiff.FormattingEnabled = true;
            this.drpMaxDiff.Items.AddRange(new object[] {
            "1",
            "1.5",
            "2",
            "2.5",
            "3",
            "3.5",
            "4",
            "4.5",
            "5"});
            this.drpMaxDiff.Location = new System.Drawing.Point(314, 206);
            this.drpMaxDiff.Margin = new System.Windows.Forms.Padding(4);
            this.drpMaxDiff.Name = "drpMaxDiff";
            this.drpMaxDiff.Size = new System.Drawing.Size(64, 24);
            this.drpMaxDiff.TabIndex = 15;
            // 
            // drpMinTerrain
            // 
            this.drpMinTerrain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpMinTerrain.FormattingEnabled = true;
            this.drpMinTerrain.Items.AddRange(new object[] {
            "1",
            "1.5",
            "2",
            "2.5",
            "3",
            "3.5",
            "4",
            "4.5",
            "5"});
            this.drpMinTerrain.Location = new System.Drawing.Point(214, 240);
            this.drpMinTerrain.Margin = new System.Windows.Forms.Padding(4);
            this.drpMinTerrain.Name = "drpMinTerrain";
            this.drpMinTerrain.Size = new System.Drawing.Size(64, 24);
            this.drpMinTerrain.TabIndex = 17;
            // 
            // drpMaxTerrain
            // 
            this.drpMaxTerrain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpMaxTerrain.FormattingEnabled = true;
            this.drpMaxTerrain.Items.AddRange(new object[] {
            "1",
            "1.5",
            "2",
            "2.5",
            "3",
            "3.5",
            "4",
            "4.5",
            "5"});
            this.drpMaxTerrain.Location = new System.Drawing.Point(314, 240);
            this.drpMaxTerrain.Margin = new System.Windows.Forms.Padding(4);
            this.drpMaxTerrain.Name = "drpMaxTerrain";
            this.drpMaxTerrain.Size = new System.Drawing.Size(64, 24);
            this.drpMaxTerrain.TabIndex = 19;
            // 
            // txtMinFavPoints
            // 
            this.txtMinFavPoints.Location = new System.Drawing.Point(213, 272);
            this.txtMinFavPoints.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMinFavPoints.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtMinFavPoints.Name = "txtMinFavPoints";
            this.txtMinFavPoints.Size = new System.Drawing.Size(65, 22);
            this.txtMinFavPoints.TabIndex = 21;
            // 
            // drpFoundByMe
            // 
            this.drpFoundByMe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.flowLayoutPanel1.SetFlowBreak(this.drpFoundByMe, true);
            this.drpFoundByMe.FormattingEnabled = true;
            this.drpFoundByMe.Items.AddRange(new object[] {
            "skip",
            "include"});
            this.drpFoundByMe.Location = new System.Drawing.Point(214, 308);
            this.drpFoundByMe.Margin = new System.Windows.Forms.Padding(4);
            this.drpFoundByMe.Name = "drpFoundByMe";
            this.drpFoundByMe.Size = new System.Drawing.Size(213, 24);
            this.drpFoundByMe.TabIndex = 24;
            // 
            // drpHiddenByMe
            // 
            this.drpHiddenByMe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.flowLayoutPanel1.SetFlowBreak(this.drpHiddenByMe, true);
            this.drpHiddenByMe.FormattingEnabled = true;
            this.drpHiddenByMe.Items.AddRange(new object[] {
            "skip",
            "include",
            "only mine"});
            this.drpHiddenByMe.Location = new System.Drawing.Point(214, 342);
            this.drpHiddenByMe.Margin = new System.Windows.Forms.Padding(4);
            this.drpHiddenByMe.Name = "drpHiddenByMe";
            this.drpHiddenByMe.Size = new System.Drawing.Size(213, 24);
            this.drpHiddenByMe.TabIndex = 26;
            // 
            // labelBasicMembers
            // 
            this.labelBasicMembers.AutoSize = true;
            this.labelBasicMembers.BackColor = System.Drawing.SystemColors.Info;
            this.labelBasicMembers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelBasicMembers.ForeColor = System.Drawing.SystemColors.InfoText;
            this.labelBasicMembers.LinkArea = new System.Windows.Forms.LinkArea(396, 18);
            this.labelBasicMembers.Location = new System.Drawing.Point(13, 384);
            this.labelBasicMembers.Margin = new System.Windows.Forms.Padding(13, 12, 13, 12);
            this.labelBasicMembers.Name = "labelBasicMembers";
            this.labelBasicMembers.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.labelBasicMembers.Size = new System.Drawing.Size(488, 167);
            this.labelBasicMembers.TabIndex = 27;
            this.labelBasicMembers.TabStop = true;
            this.labelBasicMembers.Text = resources.GetString("labelBasicMembers.Text");
            this.labelBasicMembers.UseCompatibleTextRendering = true;
            this.labelBasicMembers.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelBasicMembers_LinkClicked);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonCancel,
            this.toolStripButtonSave});
            this.toolStrip1.Location = new System.Drawing.Point(0, 567);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(523, 31);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonCancel
            // 
            this.toolStripButtonCancel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonCancel.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCancel.Image")));
            this.toolStripButtonCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCancel.Name = "toolStripButtonCancel";
            this.toolStripButtonCancel.Size = new System.Drawing.Size(81, 28);
            this.toolStripButtonCancel.Text = "Cancel";
            this.toolStripButtonCancel.Click += new System.EventHandler(this.toolStripButtonCancel_Click);
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonSave.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSave.Image")));
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(68, 28);
            this.toolStripButtonSave.Text = "Save";
            this.toolStripButtonSave.Click += new System.EventHandler(this.toolStripButtonSave_Click);
            // 
            // toolTipForOptions
            // 
            this.toolTipForOptions.AutoPopDelay = 32767;
            this.toolTipForOptions.InitialDelay = 100;
            this.toolTipForOptions.IsBalloon = true;
            this.toolTipForOptions.ReshowDelay = 100;
            this.toolTipForOptions.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipForOptions.ToolTipTitle = "Description";
            // 
            // FilterEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 598);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FilterEditor";
            this.Text = "Query editor";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FilterEditor_KeyPress);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxCaches)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMinFavPoints)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
        private System.Windows.Forms.ToolStripButton toolStripButtonCancel;
        private System.Windows.Forms.ComboBox drpMinDiff;
        private System.Windows.Forms.ComboBox drpMaxDiff;
        private System.Windows.Forms.ComboBox drpMinTerrain;
        private System.Windows.Forms.ComboBox drpMaxTerrain;
        private System.Windows.Forms.NumericUpDown txtMinFavPoints;
        private System.Windows.Forms.ComboBox drpFoundByMe;
        private System.Windows.Forms.ComboBox drpHiddenByMe;
        private System.Windows.Forms.ToolTip toolTipForOptions;
        private System.Windows.Forms.TextBox txtQueryTitle;
        private UI.CoordinateEditor txtCenterCoords;
        private System.Windows.Forms.NumericUpDown txtRadius;
        private System.Windows.Forms.NumericUpDown txtMaxCaches;
        private System.Windows.Forms.CheckBox chkIncludeDisabled;
        private System.Windows.Forms.TextBox txtCenterName;
        private System.Windows.Forms.LinkLabel labelBasicMembers;
    }
}