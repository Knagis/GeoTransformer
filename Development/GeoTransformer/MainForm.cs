/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoTransformer.Extensions;

namespace GeoTransformer
{
    public partial class MainForm : Form
    {
        #region [ Data saving and loading ]

        private void SaveSettings()
        {
            using (var scope = Program.Database.Database().BeginTransaction())
            {
                var table = Program.Database.Settings;
                var rowId = table.Select().ExecuteScalar(o => o.RowId);
                var cleanup = table.Delete();
                cleanup.Where(o => o.RowId, Data.WhereOperator.NotEqual, rowId);
                cleanup.Execute();

                var query = rowId == 0 ? table.Insert() : table.Update();
                query.Value(o => o.MainFormWindowState, (int)this.WindowState);
                query.Value(o => o.MainFormWindowLeft, this.RestoreBounds.Left);
                query.Value(o => o.MainFormWindowTop, this.RestoreBounds.Top);
                query.Value(o => o.MainFormWindowWidth, this.RestoreBounds.Width);
                query.Value(o => o.MainFormWindowHeight, this.RestoreBounds.Height);
                query.Value(o => o.MainFormDefaultUrl, this.toolStripOpenDefaultWebPage.Tag as string);
                query.Value(o => o.DoNotShowWelcomeScreen, WelcomeScreen.WelcomeScreen.DoNotShowWelcomeScreen);
                query.Execute();

                Extensions.ExtensionLoader.PersistExtensionConfiguration();

                scope.Commit();
            }
        }

        private void LoadSettings()
        {
            var query = Program.Database.Settings.Select();
            query.SelectAll();
            foreach (var res in query.Execute())
            {
                WelcomeScreen.WelcomeScreen.DoNotShowWelcomeScreen = res.Value(o => o.DoNotShowWelcomeScreen);
                this.WindowState = (FormWindowState)res.Value(o => o.MainFormWindowState);
                if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;
                if (res.Value(o => o.MainFormWindowLeft) != 0) this.Left = res.Value(o => o.MainFormWindowLeft);
                if (res.Value(o => o.MainFormWindowTop) != 0) this.Top = res.Value(o => o.MainFormWindowTop);
                if (res.Value(o => o.MainFormWindowWidth) != 0) this.Width = res.Value(o => o.MainFormWindowWidth);
                if (res.Value(o => o.MainFormWindowHeight) != 0) this.Height = res.Value(o => o.MainFormWindowHeight);
                var durl = res.Value(o => o.MainFormDefaultUrl);
                if (!string.IsNullOrWhiteSpace(durl))
                {
                    var x = this.toolStripOpenDefaultWebPage.DropDownItems.Cast<ToolStripItem>().FirstOrDefault(o => o.Tag as string == durl);
                    if (x != null)
                    {
                        this.toolStripOpenDefaultWebPage.Tag = durl;
                        this.toolStripOpenDefaultWebPage.Image = x.Image;
                        this.toolStripOpenDefaultWebPage.Text = x.Text;
                    }
                }
            }
        }

        internal void SaveData()
        {
            this.SaveSettings();

            var xmlFiles = this.listViewers.LoadedXmlFiles;
            if (xmlFiles != null)
            {
                foreach (var ext in ExtensionLoader.RetrieveExtensions<Extensions.ISaveData>())
                    ext.Save(xmlFiles);
            }

            this.UnsavedData = false;
        }

        #endregion

        #region [ Transforming and publishing ]

        private void TransformToFolder(string targetCacheDirectory, string targetWaypointDirectory, bool isUsbDevice)
        {
            this.SaveData();

            if (targetCacheDirectory != null && !System.IO.Directory.Exists(targetCacheDirectory)) System.IO.Directory.CreateDirectory(targetCacheDirectory);
            if (targetWaypointDirectory != null && !System.IO.Directory.Exists(targetWaypointDirectory)) System.IO.Directory.CreateDirectory(targetWaypointDirectory);

            var transformers = ExtensionLoader.RetrieveExtensions<Extensions.ITransformer>().ToList();
            transformers.Add(new Transformers.SaveFiles(targetCacheDirectory, targetWaypointDirectory));

            if (isUsbDevice)
                transformers.Add(new Transformers.SafelyRemoveGps(new System.IO.FileInfo(targetCacheDirectory).Directory.Root.FullName));

            using (var frm = new TransformProgress(transformers, Transformers.TransformerOptions.None))
                frm.ShowDialog(this);
        }

        #endregion

        private void InitializeExtensionConfiguration()
        {
            Data.LegacyDataImport.ConvertExtensionConfiguration();

            var configArray = Extensions.ExtensionLoader.Extensions.OfType<Extensions.ExtensionConfigurationTable>().Single().RetrieveAll();

            this.tabPageConfiguration.SuspendLayout();

            foreach (var extensionCategory in Extensions.ExtensionLoader.RetrieveExtensions<Extensions.IConfigurable>()
                .OrderBy(o => o.Category.ConfigurationOrder)
                .ThenBy(o => o.Category.Title)
                .ThenBy(o => (o is Extensions.ITransformer) ? (int)((Extensions.ITransformer)o).ExecutionOrder : int.MaxValue)
                .ThenBy(o => o.GetType().Name)
                .Reverse()
                .GroupBy(o => o.Category))
            {
                var header = new Label();
                header.BackColor = this.BackColor;
                header.ForeColor = this.ForeColor;
                header.Font = new System.Drawing.Font(header.Font.FontFamily, 12, FontStyle.Bold);
                header.Text = extensionCategory.Key.Title;
                header.Dock = DockStyle.Top;
                header.TextAlign = ContentAlignment.BottomLeft;
                header.Height = 32;

                bool headerNeeded = false;

                foreach (var instance in extensionCategory)
                {
                    var config = configArray.ContainsKey(instance.GetType().FullName) ? configArray[instance.GetType().FullName] : new Extensions.ExtensionConfigurationTable.DataClass();
                    var control = instance.Initialize(config.Configuration);

                    // there are configurable extensions that do not have this UI.
                    if (control == null)
                        continue;

                    headerNeeded = true;

                    control.Dock = DockStyle.Top;
                    control.Margin = new Padding(4);
                    this.tabPageConfiguration.Controls.Add(control);
                }

                if (headerNeeded)
                    this.tabPageConfiguration.Controls.Add(header);
            }

            this.tabPageConfiguration.ResumeLayout();
        }

        #region [ Form level events, constructor and properties ]

        public MainForm()
        {
            InitializeComponent();
            this.cachePanel.SplitterWidth = 2; // need to be set here because of bug in the framework

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WelcomeScreen.WelcomeScreen));
            using (var icon = new Icon((System.Drawing.Icon)resources.GetObject("$this.Icon"), new Size(24, 24)))
                this.launchWizardToolStripMenuItem.Image = icon.ToBitmap();

            UI.SingletonTooltip.SetRealToolTip(this.toolTipForOptions);

            this.imageList.Images.Add("Treasure", Properties.Resources.Treasure);
            this.imageList.Images.Add("TreeView", Properties.Resources.TreeView);
            this.imageList.Images.Add("Settings", Properties.Resources.Settings);
            this.imageList.Images.Add("About", Properties.Resources.About);

            this.LoadSettings();
            this.InitializeExtensionConfiguration();

            this.labelVersion2.Text = "Application version: " + Application.ProductVersion;

            this.listViewers.SelectedCacheChanged += (sender, args) => { this.cacheViewers.DisplayCache(args.CacheXmlData); };

            foreach (var tabPage in ExtensionLoader.RetrieveExtensions<Extensions.ITopLevelTabPage>())
            {
                var tp = new TabPage(tabPage.TabPageTitle);
                if (tabPage.TabPageImage != null)
                    this.imageList.Images.Add(tp.ImageKey = tabPage.GetType().AssemblyQualifiedName, tabPage.TabPageImage);

                tp.Tag = tabPage;
                tp.Enter += this.CustomTabPageEntered;

                this.tabControl.Controls.Add(tp);
            }
        }

        private void CustomTabPageEntered(object sender, EventArgs e)
        {
            var tp = (TabPage)sender;
            var tabPage = (Extensions.ITopLevelTabPage)tp.Tag;

            try
            {
                var c = tabPage.Initialize();
                c.Dock = DockStyle.Fill;
                tp.Controls.Add(c);
            }
            catch (Exception ex)
            {
                tp.Controls.Add(new Label() { Text = "Unable to initialize this extension:" + Environment.NewLine + Environment.NewLine + ex.ToString() });
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            this.SaveSettings();

            if (!this.UnsavedData)
                return;

            var res = MessageBox.Show("The data on the grid is not saved. Do you want to save it before exiting?", "GeoTransformer", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (res == System.Windows.Forms.DialogResult.Yes)
            {
                this.SaveData();
                return;
            }
            else if (res == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            e.Cancel = true;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            ReleaseNotes.ShowReleaseNotes();

            if (!WelcomeScreen.WelcomeScreen.DoNotShowWelcomeScreen)
                new WelcomeScreen.WelcomeScreen(this).Show(this);
        }

        internal void OpenCacheTab()
        {
            this.tabControl.SelectTab(this.tabPageCaches);
        }

        private bool _unsavedData;

        /// <summary>
        /// Gets or sets a value indicating whether there are unsaved data currently on the form.
        /// </summary>
        /// <value>
        ///   <c>true</c> if there are unsaved data; otherwise, <c>false</c>.
        /// </value>
        internal bool UnsavedData
        {
            get
            {
                return this._unsavedData;
            }
            set
            {
                if (this._unsavedData != value)
                    this.Text = value ? "GeoTransformer*" : "GeoTransformer";
                this._unsavedData = value;
            }
        }
        #endregion

        #region [ Event handlers for buttons ]

        private void toolStripExport_DropDownOpening(object sender, EventArgs e)
        {
            this.toolStripExport.DropDownItems.Clear();

            this.toolStripExport.DropDownItems.Add("Browse...", Properties.Resources.Folder, (s, args) =>
            {
                this.folderBrowserDialog.SelectedPath = Program.Database.RecentPublishFolders.ReadPaths().FirstOrDefault() ?? Application.StartupPath;
                var res = this.folderBrowserDialog.ShowDialog();
                if (res != System.Windows.Forms.DialogResult.OK)
                    return;

                Program.Database.RecentPublishFolders.SaveRecentFolder(this.folderBrowserDialog.SelectedPath);
                this.TransformToFolder(this.folderBrowserDialog.SelectedPath, null, false);
            });

            var paths = Program.Database.RecentPublishFolders.ReadPaths().ToList();
            if (paths.Count > 0)
            {
                var menu = new ToolStripMenuItem("Recent folders");
                foreach (var p in paths)
                {
                    var localP = p;
                    menu.DropDownItems.Add(p, Properties.Resources.Folder, (s, args) => { Program.Database.RecentPublishFolders.SaveRecentFolder(localP); this.TransformToFolder(localP, null, false); });
                }
                this.toolStripExport.DropDownItems.Add(menu);
            }

            foreach (var drive in System.IO.DriveInfo.GetDrives())
            {
                if (drive.DriveType != System.IO.DriveType.Removable || !drive.IsReady)
                    continue;

                var img = Properties.Resources.Removable;
                var label = drive.VolumeLabel;
                if (System.IO.File.Exists(System.IO.Path.Combine(drive.RootDirectory.FullName, "autorun.inf")))
                {
                    var lines = System.IO.File.ReadAllLines(System.IO.Path.Combine(drive.RootDirectory.FullName, "autorun.inf"))
                        .SkipWhile(o => !string.Equals(o.Trim(), "[autorun]", StringComparison.OrdinalIgnoreCase))
                        .Skip(1)
                        .TakeWhile(o => !o.Trim().StartsWith("[", StringComparison.Ordinal));

                    var imgFile = lines.Where(o => o.StartsWith("icon", StringComparison.OrdinalIgnoreCase))
                                       .Select(o => System.IO.Path.Combine(drive.RootDirectory.FullName, o.Substring(o.IndexOf("=") + 1)))
                                       .FirstOrDefault();
                    if (imgFile != null && imgFile.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))
                    {
                        using (var icon = new Icon(imgFile, 24, 24))
                            img = icon.ToBitmap();
                    }
                    label = lines.Where(o => o.StartsWith("label", StringComparison.OrdinalIgnoreCase))
                                  .Select(o => o.Substring(o.IndexOf("=") + 1))
                                  .FirstOrDefault() ?? label;
                }

                if (drive.VolumeLabel.StartsWith("Garmin", StringComparison.OrdinalIgnoreCase) || label.StartsWith("Garmin", StringComparison.OrdinalIgnoreCase))
                {
                    var dir = System.IO.Path.Combine(drive.RootDirectory.FullName, "Garmin", "GPX");
                    this.toolStripExport.DropDownItems.Add(label + " (" + drive.RootDirectory.FullName + ")", img, (s, args) => { this.TransformToFolder(dir, null, true); });
                    continue;
                }
                if (drive.VolumeLabel.StartsWith("MAGELLAN", StringComparison.OrdinalIgnoreCase) || label.StartsWith("MAGELLAN", StringComparison.OrdinalIgnoreCase))
                {
                    var dir = System.IO.Path.Combine(drive.RootDirectory.FullName, "Geocaches");
                    var wptdir = System.IO.Path.Combine(drive.RootDirectory.FullName, "Waypoints");
                    this.toolStripExport.DropDownItems.Add(label + " (" + drive.RootDirectory.FullName + ")", img, (s, args) => { this.TransformToFolder(dir, wptdir, true); });
                    continue;
                }
            }

        }

        private void toolStripSave_Click(object sender, EventArgs e)
        {
            this.SaveData();
        }

        private void toolStripWebPage_Click(object sender, EventArgs e)
        {
            var ts = (ToolStripItem)sender;
            var url = (string)ts.Tag;

            if (!this.toolStripWebPageChangeDefault.Enabled)
            {
                this.toolStripOpenDefaultWebPage.Tag = url;
                this.toolStripOpenDefaultWebPage.Image = ts.Image;
                this.toolStripOpenDefaultWebPage.Text = ts.Text;
                return;
            }

            System.Diagnostics.Process.Start(url);
        }

        private void toolStripWebPageChangeDefault_Click(object sender, EventArgs e)
        {
            this.toolStripOpenDefaultWebPage.ShowDropDown();
            this.toolStripWebPageChangeDefault.Text = "Choose new default";
            this.toolStripWebPageChangeDefault.Enabled = false;
        }

        private void toolStripOpenDefaultWebPage_DropDownOpening(object sender, EventArgs e)
        {
            this.toolStripWebPageChangeDefault.Text = "Change the default";
            this.toolStripWebPageChangeDefault.Enabled = true;
        }

        private void pictureBoxPayPal_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=TPC5CT7JLNSTG&lc=LV&item_name=GeoTransformer&currency_code=EUR&bn=PP-DonationsBF:btn_donateCC_LG.gif:NonHosted");
        }

        #endregion

        private void pictureBoxLiveLogo_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.geocaching.com/live/");
        }

        private void launchWizardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var currentForm = Application.OpenForms.OfType<WelcomeScreen.WelcomeScreen>().FirstOrDefault();
            if (currentForm != null)
                currentForm.Activate();
            else
                new WelcomeScreen.WelcomeScreen(this).Show();
        }
    }
}
