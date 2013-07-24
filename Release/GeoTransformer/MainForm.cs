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

                if (this.cachePanel.Height > 0)
                    query.Value(o => o.ListViewerHeight, this.cachePanel.SplitterDistance * 1000 / this.cachePanel.Height);

                query.Value(o => o.RecentPublishers, this.RecentPublishers.Take(10).Select(o => o.ToString()).Aggregate("", (a,b) => a + ',' + b));
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
                if (res.Value(o => o.ListViewerHeight) != 0) this.cachePanel.SplitterDistance = res.Value(o => o.ListViewerHeight) * this.cachePanel.Height / 1000;
                this.RecentPublishers = new List<Guid>((res.Value(o => o.RecentPublishers) ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(o => Guid.Parse(o)));
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

            var xmlFiles = this.listViewers.LoadedGpxFiles;
            if (xmlFiles != null)
            {
                foreach (var ext in ExtensionLoader.RetrieveExtensions<Extensions.ISaveData>())
                    ext.Save(xmlFiles);
            }

            this.UnsavedData = false;
        }

        #endregion

        #region [ Transforming and publishing ]

        /// <summary>
        /// Holds the most recent publisher target IDs.
        /// </summary>
        private List<Guid> RecentPublishers = new List<Guid>();

        private IList<IPublisher> _publisherExtensions = new List<IPublisher>();

        /// <summary>
        /// Synchronization for the <see cref="NotifyPublishers"/> method. Used so that when many device related messages
        /// arrive, the notifications are not raised simultaneously.
        /// </summary>
        private System.Threading.AutoResetEvent _publisherNotificationRequired = new System.Threading.AutoResetEvent(false);

        /// <summary>
        /// Notifies the publishers that devices have been changed.
        /// </summary>
        private void NotifyPublishers()
        {
            while (_publisherNotificationRequired.WaitOne())
            {
                // sleep for 150ms and then reset the synchronization event so that all device events that happened within
                // these 150ms are handled by a single iteration to publishers.
                System.Threading.Thread.Sleep(150);
                this._publisherNotificationRequired.Reset();

                foreach (var p in this._publisherExtensions)
                    p.NotifyDevicesChanged();
            }
        }

        private void PublisherClick(object sender, EventArgs args)
        {
            var item = sender as ToolStripItem;
            if (item == null)
                return;

            var mitem = sender as ToolStripMenuItem;
            if (mitem != null && mitem.DropDownItems.Count > 0)
                return;

            var target = item.Tag as Publishers.PublisherTarget;
            if (target == null)
                return;

            this.RecentPublishers.Remove(target.Key);
            this.RecentPublishers.Insert(0, target.Key);
            this.UpdateMostRecentPublisher();

            bool cancel;
            IEnumerable<Extensions.ITransformer> additionalTransformers = null;
            try
            {
                additionalTransformers = target.Publisher.GetSpecialTransformers(target, out cancel);
                if (cancel)
                    return;

                var transformers = ExtensionLoader.RetrieveExtensions<Extensions.ITransformer>().ToList();
                if (additionalTransformers != null)
                    transformers.AddRange(additionalTransformers);

                using (var frm = new TransformProgress(transformers, Transformers.TransformerOptions.None))
                    frm.ShowDialog(this);
            }
            finally
            {
                if (additionalTransformers != null)
                    foreach (var at in additionalTransformers)
                    {
                        var disp = at as IDisposable;
                        if (disp != null)
                            disp.Dispose();
                    }
            }
        }

        private void UpdateMostRecentPublisher()
        {
            List<ToolStripMenuItem> all = new List<ToolStripMenuItem>();
            all.AddRange(this.toolStripExport.DropDownItems.OfType<ToolStripMenuItem>());
            for (int i = 0; i < all.Count; i++)
                all.AddRange(all[i].DropDownItems.OfType<ToolStripMenuItem>());

            int bestPosition = int.MaxValue;
            Publishers.PublisherTarget bestTarget = null;

            for (int i = 0; i < all.Count; i++)
            {
                var target = all[i].Tag as Publishers.PublisherTarget;
                var pos = this.RecentPublishers.IndexOf(target.Key);
                if (pos != -1 && pos < bestPosition)
                {
                    bestTarget = target;
                    if (pos == 0)
                        break;
                }
            }

            this.Invoke(() =>
                {
                    if (bestTarget != null)
                    {
                        this.toolStripRecentPublisher.Visible = true;
                        this.toolStripRecentPublisher.Text = bestTarget.Text;
                        this.toolStripRecentPublisher.Tag = bestTarget;
                        this.toolStripRecentPublisher.Image = bestTarget.Icon;
                    }
                    else
                    {
                        this.toolStripRecentPublisher.Visible = false;
                    }
                });
        }

        private void UpdatePublisherTargetsOuter(Extensions.IPublisher publisher, IEnumerable<Publishers.PublisherTarget> targets)
        {
            this.UpdatePublisherTargets(publisher, targets);
            this.UpdateMostRecentPublisher();
        }

        private void UpdatePublisherTargets(Extensions.IPublisher publisher, IEnumerable<Publishers.PublisherTarget> targets)
        {
            for (int i = this.toolStripExport.DropDownItems.Count - 1; i >= 0; i--)
            {
                var btn = this.toolStripExport.DropDownItems[i];
                var target = btn.Tag as Publishers.PublisherTarget;
                if (target != null && target.Publisher == publisher)
                    this.Invoke(() => this.toolStripExport.DropDownItems.RemoveAt(i));
            }

            var existing = this.toolStripExport.DropDownItems.Cast<ToolStripItem>().Select(o => o.Text).ToList();
            int added = 0;
            int index = 0;
            foreach (var t in targets.OrderBy(o => o.Text))
            {
                var btn = new ToolStripMenuItem(t.Text);
                btn.Tag = t;
                btn.Image = t.Icon;
                btn.Click += this.PublisherClick;

                if (t.Children.Count > 0)
                    this.InitializePublisherChildItems(btn, t);

                index = existing.FindIndex(index, o => string.CompareOrdinal(o, t.Text) > 0);
                if (index == -1) index = existing.Count;
                this.Invoke(() => this.toolStripExport.DropDownItems.Insert(index + added, btn));
                added++;
            }
        }

        private void InitializePublisherChildItems(ToolStripMenuItem parent, Publishers.PublisherTarget target)
        {
            foreach (var c in target.Children)
            {
                var cbtn = new ToolStripMenuItem(c.Text);
                cbtn.Tag = c;
                cbtn.Image = c.Icon;
                cbtn.Click += this.PublisherClick;
                parent.DropDownItems.Add(cbtn);

                if (target.Children.Count > 0)
                    this.InitializePublisherChildItems(cbtn, c);
            }
        }

        private void InitializePublishers()
        {
            this._publisherNotificationRequired.Reset();

            foreach (var publisher in Extensions.ExtensionLoader.RetrieveExtensions<Extensions.IPublisher>())
            {
                publisher.TargetsChanged += (a, b) => this.UpdatePublisherTargetsOuter((IPublisher)a, b.Targets);
                this.UpdatePublisherTargets(publisher, publisher.Initialize());
                this._publisherExtensions.Add(publisher);
            }

            this.UpdateMostRecentPublisher();

            var thread = new System.Threading.Thread(this.NotifyPublishers);
            thread.Name = "MainForm.NotifyPublishers";
            thread.Start();
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

            // the icon is rather large so we reuse the resource from welcome screen form
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WelcomeScreen.WelcomeScreen));
            using (var icon = new Icon((System.Drawing.Icon)resources.GetObject("$this.Icon"), new Size(24, 24)))
                this.launchWizardToolStripMenuItem.Image = icon.ToBitmap();

            UI.SingletonTooltip.SetRealToolTip(this.toolTipForOptions);

            this.imageList.Images.Add("Treasure", Properties.Resources.Treasure);
            this.imageList.Images.Add("Settings", Properties.Resources.Settings);
            this.imageList.Images.Add("About", Properties.Resources.About);

            this.LoadSettings();
            this.InitializeExtensionConfiguration();
            new System.Threading.Thread(this.InitializePublishers).Start();

            this.labelVersion2.Text = "Application version: " + Application.ProductVersion;

            this.listViewers.SelectedWaypointsChanged += (sender, args) => { this.cacheViewers.DisplayWaypoints(args.Selection); };

            foreach (var tabPage in ExtensionLoader.RetrieveExtensions<Extensions.ITopLevelTabPage>())
            {
                var tp = new TabPage(tabPage.TabPageTitle);
                if (tabPage.TabPageImage != null)
                {
                    this.imageList.Images.Add(tabPage.TabPageImage);
                    tp.ImageIndex = this.imageList.Images.Count - 1;
                }

                tp.Tag = tabPage;
                tp.Enter += this.CustomTabPageEntered;

                this.tabControl.Controls.Add(tp);
            }
        }

        /// <summary>
        /// Processes the Windows message.
        /// </summary>
        /// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message"/> to process.</param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0219 /*WM_DEVICECHANGE*/)
            {
                var wp = m.WParam.ToInt32();
                if (wp == 0x0007 /*DBT_DEVNODES_CHANGED - raised by "Portable devices" in My Computer*/
                    || wp == 0x8000 /*DBT_DEVICEARRIVAL*/
                    || wp == 0x8004 /*DBT_DEVICEREMOVECOMPLETE*/)
                {
                    this._publisherNotificationRequired.Set();
                }
            }

            base.WndProc(ref m);
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

                tp.Enter -= this.CustomTabPageEntered;
            }
            catch (Exception ex)
            {
                tp.Controls.Add(new Label() { Text = "Unable to initialize this extension:" + Environment.NewLine + Environment.NewLine + ex.ToString(), AutoSize = true });
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
