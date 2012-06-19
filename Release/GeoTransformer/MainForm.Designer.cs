namespace GeoTransformer
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label3;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.PictureBox pictureBoxLiveLogo;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            System.Windows.Forms.ToolStripMenuItem openPocketQueriesToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem openGeocachingcomToolStripMenuItem;
            this.tabPageCaches = new System.Windows.Forms.TabPage();
            this.cachePanel = new System.Windows.Forms.SplitContainer();
            this.listViewers = new GeoTransformer.Modules.ListViewers();
            this.cacheViewers = new GeoTransformer.Modules.CacheViewers();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageConfiguration = new System.Windows.Forms.TabPage();
            this.tabPageAbout = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.labelVersion2 = new System.Windows.Forms.Label();
            this.labelAbout = new System.Windows.Forms.Label();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTipForOptions = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBoxPayPal = new System.Windows.Forms.PictureBox();
            this.toolStripOpenDefaultWebPage = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripWebPageChangeDefault = new System.Windows.Forms.ToolStripMenuItem();
            this.openGeoTransformerHomePageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openGpxTautaiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripExport = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownHelp = new System.Windows.Forms.ToolStripDropDownButton();
            this.launchWizardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onlineDocumentationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            label1 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            pictureBoxLiveLogo = new System.Windows.Forms.PictureBox();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            openPocketQueriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openGeocachingcomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPageCaches.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cachePanel)).BeginInit();
            this.cachePanel.Panel1.SuspendLayout();
            this.cachePanel.Panel2.SuspendLayout();
            this.cachePanel.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageAbout.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(pictureBoxLiveLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPayPal)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(75, 16);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(150, 13);
            label1.TabIndex = 1;
            label1.Text = "Powered by Geocaching Live.";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(75, 44);
            label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(575, 13);
            label3.TabIndex = 3;
            label3.Text = "Groundspeak’s Geocaching.com Cache Type Icons © 2011 Groundspeak Inc. All rights " +
    "reserved. Used with Permission.";
            // 
            // tabPageCaches
            // 
            this.tabPageCaches.Controls.Add(this.cachePanel);
            this.tabPageCaches.ImageKey = "Treasure";
            this.tabPageCaches.Location = new System.Drawing.Point(4, 31);
            this.tabPageCaches.Name = "tabPageCaches";
            this.tabPageCaches.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCaches.Size = new System.Drawing.Size(837, 512);
            this.tabPageCaches.TabIndex = 0;
            this.tabPageCaches.Text = "Cache viewer and editor";
            this.tabPageCaches.UseVisualStyleBackColor = true;
            // 
            // cachePanel
            // 
            this.cachePanel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.cachePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cachePanel.Location = new System.Drawing.Point(3, 3);
            this.cachePanel.Margin = new System.Windows.Forms.Padding(0);
            this.cachePanel.Name = "cachePanel";
            this.cachePanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // cachePanel.Panel1
            // 
            this.cachePanel.Panel1.BackColor = System.Drawing.SystemColors.Window;
            this.cachePanel.Panel1.Controls.Add(this.listViewers);
            // 
            // cachePanel.Panel2
            // 
            this.cachePanel.Panel2.Controls.Add(this.cacheViewers);
            this.cachePanel.Size = new System.Drawing.Size(831, 506);
            this.cachePanel.SplitterDistance = 342;
            this.cachePanel.SplitterWidth = 2;
            this.cachePanel.TabIndex = 1;
            // 
            // listViewers
            // 
            this.listViewers.AutoSize = true;
            this.listViewers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewers.Location = new System.Drawing.Point(0, 0);
            this.listViewers.Margin = new System.Windows.Forms.Padding(0);
            this.listViewers.MinimumSize = new System.Drawing.Size(100, 50);
            this.listViewers.Name = "listViewers";
            this.listViewers.Size = new System.Drawing.Size(831, 342);
            this.listViewers.TabIndex = 1;
            // 
            // cacheViewers
            // 
            this.cacheViewers.AutoSize = true;
            this.cacheViewers.BackColor = System.Drawing.SystemColors.Window;
            this.cacheViewers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cacheViewers.Location = new System.Drawing.Point(0, 0);
            this.cacheViewers.Margin = new System.Windows.Forms.Padding(0);
            this.cacheViewers.MinimumSize = new System.Drawing.Size(100, 50);
            this.cacheViewers.Name = "cacheViewers";
            this.cacheViewers.Size = new System.Drawing.Size(831, 162);
            this.cacheViewers.TabIndex = 0;
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(24, 24);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageCaches);
            this.tabControl.Controls.Add(this.tabPageConfiguration);
            this.tabControl.Controls.Add(this.tabPageAbout);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.ImageList = this.imageList;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(845, 547);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageConfiguration
            // 
            this.tabPageConfiguration.AutoScroll = true;
            this.tabPageConfiguration.ImageKey = "Settings";
            this.tabPageConfiguration.Location = new System.Drawing.Point(4, 31);
            this.tabPageConfiguration.Name = "tabPageConfiguration";
            this.tabPageConfiguration.Size = new System.Drawing.Size(837, 512);
            this.tabPageConfiguration.TabIndex = 5;
            this.tabPageConfiguration.Text = "Configuration";
            this.tabPageConfiguration.UseVisualStyleBackColor = true;
            // 
            // tabPageAbout
            // 
            this.tabPageAbout.AutoScroll = true;
            this.tabPageAbout.Controls.Add(this.panel1);
            this.tabPageAbout.Controls.Add(this.labelVersion2);
            this.tabPageAbout.Controls.Add(this.labelAbout);
            this.tabPageAbout.Controls.Add(this.pictureBoxPayPal);
            this.tabPageAbout.ImageKey = "About";
            this.tabPageAbout.Location = new System.Drawing.Point(4, 31);
            this.tabPageAbout.Name = "tabPageAbout";
            this.tabPageAbout.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAbout.Size = new System.Drawing.Size(837, 512);
            this.tabPageAbout.TabIndex = 4;
            this.tabPageAbout.Text = "About";
            this.tabPageAbout.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(label1);
            this.panel1.Controls.Add(pictureBoxLiveLogo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 423);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(831, 73);
            this.panel1.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(75, 30);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(507, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "The Groundspeak Geocaching Logo is a registered trademark of Groundspeak, Inc. Us" +
    "ed with permission.";
            // 
            // labelVersion2
            // 
            this.labelVersion2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelVersion2.Location = new System.Drawing.Point(3, 496);
            this.labelVersion2.Name = "labelVersion2";
            this.labelVersion2.Size = new System.Drawing.Size(831, 13);
            this.labelVersion2.TabIndex = 2;
            this.labelVersion2.Text = "Application version: 2.0";
            this.labelVersion2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.toolTipForOptions.SetToolTip(this.labelVersion2, "Reference this number if you want to check for a new version\r\non the product\'s we" +
        "b page.");
            // 
            // labelAbout
            // 
            this.labelAbout.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelAbout.Location = new System.Drawing.Point(3, 69);
            this.labelAbout.Name = "labelAbout";
            this.labelAbout.Size = new System.Drawing.Size(831, 202);
            this.labelAbout.TabIndex = 0;
            this.labelAbout.Text = resources.GetString("labelAbout.Text");
            this.labelAbout.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripOpenDefaultWebPage,
            this.toolStripSave,
            this.toolStripExport,
            this.toolStripDropDownHelp});
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip.Location = new System.Drawing.Point(0, 547);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(845, 31);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.TabStop = true;
            this.toolStrip.Text = "toolStrip";
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Pick the folder where you want to publish the .gpx files to. Note that both cache" +
    "s and waypoints will be published there.";
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
            // pictureBoxLiveLogo
            // 
            pictureBoxLiveLogo.Cursor = System.Windows.Forms.Cursors.Hand;
            pictureBoxLiveLogo.Image = global::GeoTransformer.Properties.Resources.Geocaching_LIVE_poweredby_64;
            pictureBoxLiveLogo.Location = new System.Drawing.Point(-1, 0);
            pictureBoxLiveLogo.Margin = new System.Windows.Forms.Padding(2);
            pictureBoxLiveLogo.Name = "pictureBoxLiveLogo";
            pictureBoxLiveLogo.Size = new System.Drawing.Size(64, 73);
            pictureBoxLiveLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBoxLiveLogo.TabIndex = 0;
            pictureBoxLiveLogo.TabStop = false;
            pictureBoxLiveLogo.Click += new System.EventHandler(this.pictureBoxLiveLogo_Click);
            // 
            // pictureBoxPayPal
            // 
            this.pictureBoxPayPal.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxPayPal.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBoxPayPal.Image = global::GeoTransformer.Properties.Resources.Donate;
            this.pictureBoxPayPal.Location = new System.Drawing.Point(3, 3);
            this.pictureBoxPayPal.Name = "pictureBoxPayPal";
            this.pictureBoxPayPal.Size = new System.Drawing.Size(831, 66);
            this.pictureBoxPayPal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxPayPal.TabIndex = 1;
            this.pictureBoxPayPal.TabStop = false;
            this.pictureBoxPayPal.Click += new System.EventHandler(this.pictureBoxPayPal_Click);
            // 
            // toolStripOpenDefaultWebPage
            // 
            this.toolStripOpenDefaultWebPage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripWebPageChangeDefault,
            toolStripSeparator1,
            this.openGeoTransformerHomePageToolStripMenuItem,
            this.openGpxTautaiToolStripMenuItem,
            openPocketQueriesToolStripMenuItem,
            openGeocachingcomToolStripMenuItem});
            this.toolStripOpenDefaultWebPage.Image = global::GeoTransformer.Properties.Resources.Geocaching;
            this.toolStripOpenDefaultWebPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripOpenDefaultWebPage.Name = "toolStripOpenDefaultWebPage";
            this.toolStripOpenDefaultWebPage.Size = new System.Drawing.Size(168, 28);
            this.toolStripOpenDefaultWebPage.Tag = "http://www.geocaching.com/";
            this.toolStripOpenDefaultWebPage.Text = "Open geocaching.com";
            this.toolStripOpenDefaultWebPage.ButtonClick += new System.EventHandler(this.toolStripWebPage_Click);
            this.toolStripOpenDefaultWebPage.DropDownOpening += new System.EventHandler(this.toolStripOpenDefaultWebPage_DropDownOpening);
            // 
            // toolStripWebPageChangeDefault
            // 
            this.toolStripWebPageChangeDefault.Name = "toolStripWebPageChangeDefault";
            this.toolStripWebPageChangeDefault.Size = new System.Drawing.Size(263, 30);
            this.toolStripWebPageChangeDefault.Text = "Change the default";
            this.toolStripWebPageChangeDefault.Click += new System.EventHandler(this.toolStripWebPageChangeDefault_Click);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(260, 6);
            // 
            // openGeoTransformerHomePageToolStripMenuItem
            // 
            this.openGeoTransformerHomePageToolStripMenuItem.Image = global::GeoTransformer.Properties.Resources.GeoTransformer;
            this.openGeoTransformerHomePageToolStripMenuItem.Name = "openGeoTransformerHomePageToolStripMenuItem";
            this.openGeoTransformerHomePageToolStripMenuItem.Size = new System.Drawing.Size(263, 30);
            this.openGeoTransformerHomePageToolStripMenuItem.Tag = "http://knagis.miga.lv/blog/?tag=/GeoTransformer";
            this.openGeoTransformerHomePageToolStripMenuItem.Text = "Open GeoTransformer home page";
            this.openGeoTransformerHomePageToolStripMenuItem.ToolTipText = "http://knagis.miga.lv/blog/?tag=/GeoTransformer";
            this.openGeoTransformerHomePageToolStripMenuItem.Click += new System.EventHandler(this.toolStripWebPage_Click);
            // 
            // openGpxTautaiToolStripMenuItem
            // 
            this.openGpxTautaiToolStripMenuItem.Image = global::GeoTransformer.Properties.Resources.Latvia;
            this.openGpxTautaiToolStripMenuItem.Name = "openGpxTautaiToolStripMenuItem";
            this.openGpxTautaiToolStripMenuItem.Size = new System.Drawing.Size(263, 30);
            this.openGpxTautaiToolStripMenuItem.Tag = "http://www.geoforums.lv/";
            this.openGpxTautaiToolStripMenuItem.Text = "Open GPX Tautai forum page";
            this.openGpxTautaiToolStripMenuItem.ToolTipText = "http://www.geoforums.lv/";
            this.openGpxTautaiToolStripMenuItem.Click += new System.EventHandler(this.toolStripWebPage_Click);
            // 
            // openPocketQueriesToolStripMenuItem
            // 
            openPocketQueriesToolStripMenuItem.Image = global::GeoTransformer.Properties.Resources.PocketQuery;
            openPocketQueriesToolStripMenuItem.Name = "openPocketQueriesToolStripMenuItem";
            openPocketQueriesToolStripMenuItem.Size = new System.Drawing.Size(263, 30);
            openPocketQueriesToolStripMenuItem.Tag = "http://www.geocaching.com/pocket/";
            openPocketQueriesToolStripMenuItem.Text = "Open pocket query page";
            openPocketQueriesToolStripMenuItem.ToolTipText = "http://www.geocaching.com/pocket/";
            openPocketQueriesToolStripMenuItem.Click += new System.EventHandler(this.toolStripWebPage_Click);
            // 
            // openGeocachingcomToolStripMenuItem
            // 
            openGeocachingcomToolStripMenuItem.Image = global::GeoTransformer.Properties.Resources.Geocaching;
            openGeocachingcomToolStripMenuItem.Name = "openGeocachingcomToolStripMenuItem";
            openGeocachingcomToolStripMenuItem.Size = new System.Drawing.Size(263, 30);
            openGeocachingcomToolStripMenuItem.Tag = "http://www.geocaching.com/";
            openGeocachingcomToolStripMenuItem.Text = "Open geocaching.com";
            openGeocachingcomToolStripMenuItem.ToolTipText = "http://www.geocaching.com/";
            openGeocachingcomToolStripMenuItem.Click += new System.EventHandler(this.toolStripWebPage_Click);
            // 
            // toolStripSave
            // 
            this.toolStripSave.Image = global::GeoTransformer.Properties.Resources.Save;
            this.toolStripSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSave.Name = "toolStripSave";
            this.toolStripSave.Size = new System.Drawing.Size(59, 28);
            this.toolStripSave.Text = "Save";
            this.toolStripSave.Click += new System.EventHandler(this.toolStripSave_Click);
            // 
            // toolStripExport
            // 
            this.toolStripExport.Image = global::GeoTransformer.Properties.Resources.SaveAndPublish;
            this.toolStripExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripExport.Name = "toolStripExport";
            this.toolStripExport.Size = new System.Drawing.Size(83, 28);
            this.toolStripExport.Text = "Export / Publish";
            // 
            // toolStripDropDownHelp
            // 
            this.toolStripDropDownHelp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripDropDownHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.launchWizardToolStripMenuItem,
            this.onlineDocumentationToolStripMenuItem});
            this.toolStripDropDownHelp.Image = global::GeoTransformer.Properties.Resources.Help;
            this.toolStripDropDownHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownHelp.Name = "toolStripDropDownHelp";
            this.toolStripDropDownHelp.Size = new System.Drawing.Size(69, 28);
            this.toolStripDropDownHelp.Text = "Help";
            // 
            // launchWizardToolStripMenuItem
            // 
            this.launchWizardToolStripMenuItem.Name = "launchWizardToolStripMenuItem";
            this.launchWizardToolStripMenuItem.Size = new System.Drawing.Size(202, 30);
            this.launchWizardToolStripMenuItem.Text = "Launch wizard";
            this.launchWizardToolStripMenuItem.Click += new System.EventHandler(this.launchWizardToolStripMenuItem_Click);
            // 
            // onlineDocumentationToolStripMenuItem
            // 
            this.onlineDocumentationToolStripMenuItem.Image = global::GeoTransformer.Properties.Resources.FAQ;
            this.onlineDocumentationToolStripMenuItem.Name = "onlineDocumentationToolStripMenuItem";
            this.onlineDocumentationToolStripMenuItem.Size = new System.Drawing.Size(202, 30);
            this.onlineDocumentationToolStripMenuItem.Tag = "http://geotransformer.codeplex.com/documentation";
            this.onlineDocumentationToolStripMenuItem.Text = "Online documentation";
            this.onlineDocumentationToolStripMenuItem.Click += new System.EventHandler(this.toolStripWebPage_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(845, 578);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.toolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "GeoTransformer";
            this.tabPageCaches.ResumeLayout(false);
            this.cachePanel.Panel1.ResumeLayout(false);
            this.cachePanel.Panel1.PerformLayout();
            this.cachePanel.Panel2.ResumeLayout(false);
            this.cachePanel.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cachePanel)).EndInit();
            this.cachePanel.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPageAbout.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(pictureBoxLiveLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPayPal)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripSave;
        private System.Windows.Forms.ToolStripDropDownButton toolStripExport;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ToolStripMenuItem toolStripWebPageChangeDefault;
        private System.Windows.Forms.ToolStripSplitButton toolStripOpenDefaultWebPage;
        private System.Windows.Forms.ToolStripMenuItem openGeoTransformerHomePageToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTipForOptions;
        private System.Windows.Forms.TabPage tabPageAbout;
        private System.Windows.Forms.Label labelAbout;
        private System.Windows.Forms.PictureBox pictureBoxPayPal;
        private System.Windows.Forms.Label labelVersion2;
        private System.Windows.Forms.ToolStripMenuItem openGpxTautaiToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPageConfiguration;
        private System.Windows.Forms.SplitContainer cachePanel;
        internal Modules.ListViewers listViewers;
        internal Modules.CacheViewers cacheViewers;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownHelp;
        private System.Windows.Forms.ToolStripMenuItem launchWizardToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPageCaches;
        private System.Windows.Forms.ToolStripMenuItem onlineDocumentationToolStripMenuItem;
    }
}

