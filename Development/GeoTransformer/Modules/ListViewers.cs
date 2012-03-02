﻿/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoTransformer.Extensions;

namespace GeoTransformer.Modules
{
    public partial class ListViewers : UserControl
    {
        /// <summary>
        /// Gets the form that the container control is assigned to.
        /// </summary>
        /// <returns>The <see cref="T:System.Windows.Forms.Form"/> that the container control is assigned to. This property will return null if the control is hosted inside of Internet Explorer or in another hosting context where there is no parent form. </returns>
        private new MainForm ParentForm
        {
            get { return (MainForm)base.ParentForm; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewers"/> class.
        /// </summary>
        public ListViewers()
        {
            InitializeComponent();

            this.InitializeViewers();
        }

        private List<Tuple<ICacheListViewer, Control>> _listViewers = new List<Tuple<ICacheListViewer, Control>>();
        private System.Collections.ObjectModel.ObservableCollection<System.Xml.Linq.XDocument> _viewerCache;

        /// <summary>
        /// Gets the list of loaded XML files.
        /// </summary>
        internal IList<System.Xml.Linq.XDocument> LoadedXmlFiles
        {
            get { return this._viewerCache; }
        }

        private void InitializeViewers()
        {
            foreach (var instance in Extensions.ExtensionLoader.RetrieveExtensions<ICacheListViewer>()
                                            .OrderBy(o => o is Viewers.EditedCacheTableView.EditedCacheTableView ? 0 : 1)
                                            .ThenBy(o => o.ButtonText))
            {
                instance.SelectedCacheChanged += ViewerSelectedCacheChanged;

                var btn = new ToolStripButton(instance.ButtonText, instance.ButtonImage);
                btn.TextImageRelation = TextImageRelation.ImageAboveText;
                btn.Tag = this._listViewers.Count;

                var cond = instance as Extensions.IConditional;
                if (cond != null && !cond.IsEnabled)
                    btn.Enabled = false;
                
                this._listViewers.Add(Tuple.Create<ICacheListViewer, Control>(instance, null));
                this.toolStripViewers.Items.Add(btn);
            }
        }

        void ViewerSelectedCacheChanged(object sender, Viewers.SelectedCacheChangedEventArgs e)
        {
            var viewer = this._listViewers.FirstOrDefault(o => o.Item1 == sender as ICacheListViewer);
            if (viewer == null || !viewer.Item2.Visible)
                return;

            var newCode = e.CacheCode;
            if (string.Equals(this.SelectedCacheCode, newCode, StringComparison.OrdinalIgnoreCase))
                return;

            this.SelectedCacheCode = newCode;

            var handler = this.Events[SelectedCacheChangedEvent] as EventHandler<Viewers.SelectedCacheChangedEventArgs>;
            if (handler != null)
                handler(this, e);
        }

        internal void OpenSpecificViewer(Type viewerType)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((o, vt) => o.OpenSpecificViewer(vt), viewerType);
                return;
            }

            var i = this._listViewers.FindIndex(a => viewerType.IsAssignableFrom(a.Item1.GetType()));
            if (i == -1)
                return;

            var btn = this.toolStripViewers.Items.OfType<ToolStripButton>().FirstOrDefault(o => o.Tag is int && (int)o.Tag == i);
            if (btn != null)
                btn.PerformClick();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.UserControl.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var firstButton = this.toolStripViewers.Items.OfType<ToolStripButton>().FirstOrDefault(o => o.Tag != null);

            if (!this.DesignMode)
                firstButton.PerformClick();
        }

        private void toolStripViewers_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var btn = e.ClickedItem as ToolStripButton;
            if (btn == null || btn.Checked || btn == this.toolStripViewersRefresh)
                return;

            var current = this.toolStripViewers.Items.OfType<ToolStripButton>().FirstOrDefault(o => o.Checked);
            if (current != null)
            {
                this._listViewers[(int)current.Tag].Item2.Visible = false;
                current.Checked = false;
            }

            btn.Checked = true;

            var tag = (int)btn.Tag;

            var viewer = this._listViewers[tag];

            if (viewer.Item2 == null)
            {
                this._listViewers[tag] = viewer = Tuple.Create(viewer.Item1, viewer.Item1.Initialize());
                viewer.Item2.Dock = DockStyle.Fill;
                viewer.Item2.Visible = false;
                this.Controls.Add(viewer.Item2);
                viewer.Item2.BringToFront(); // so that the Dock works properly.
            }

            viewer.Item2.Visible = true;
            viewer.Item2.Focus();
            new System.Threading.Thread((a) => LoadListViewerData((bool)a)).Start(false);
        }

        private void ListViewerRefresh_Click(object sender, EventArgs e)
        {
            if (this.ParentForm.UnsavedData)
            {
                var res = MessageBox.Show("There are unsaved data. Continuing with the refresh will remove it." + Environment.NewLine + Environment.NewLine + "Do you want to save the data before refresh?", "Unsaved data", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (res == DialogResult.Cancel)
                    return;
                if (res == DialogResult.Yes)
                    this.ParentForm.SaveData();

                this.ParentForm.UnsavedData = false;
            }

            var thread = new System.Threading.Thread((a) => LoadListViewerData((bool)a));
            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start(true);
        }

        internal void ChangeSelectedCache(string code)
        {
            new System.Threading.Thread((a) => { this.SelectedCacheCode = (string)a; this.LoadListViewerData(false); }).Start(code);
        }

        /// <summary>
        /// Loads the geocache data into the currently selected viewer. If needed, executes the transformers.
        /// </summary>
        /// <param name="forceRefresh">if set to <c>true</c>, forces refresh from external sources.</param>
        internal void LoadListViewerData(bool forceRefresh)
        {
            this.Invoke(() => { this.toolStripViewersRefresh.Enabled = false; this.toolStripViewersRefresh.Text = "Loading"; });

            if (this._viewerCache == null || forceRefresh)
            {
                var specialTransformer = new Transformers.CreateViewerCache.CreateViewerCache();
                var transformers = ExtensionLoader.RetrieveExtensions<ITransformer>().ToList();
                transformers.Add(specialTransformer);
                transformers.RemoveAll(o => o.ExecutionOrder > Transformers.ExecutionOrder.CreateViewerCache);

                TransformProgress form = null;
                try
                {
                    var wakeUp = new System.Threading.ManualResetEventSlim(false);
                    var options = forceRefresh ? Transformers.TransformerOptions.LoadingViewerCache : Transformers.TransformerOptions.LoadingViewerCache | Transformers.TransformerOptions.UseLocalStorage;
                    this.Invoke(() => 
                    { 
                        form = new TransformProgress(transformers, options); 
                        form.AllowAutomaticClose = !forceRefresh; 
                        form.FormClosed += (a, b) => wakeUp.Set(); 
                        if (!forceRefresh) 
                            form.WindowState = FormWindowState.Minimized; 
                        form.Show(this.ParentForm); 
                    });

                    wakeUp.Wait();
                }
                finally
                {
                    if (form != null)
                        this.Invoke(() => form.Dispose());
                }

                // handle some sort of exception
                if (specialTransformer.Data != null)
                {
                    if (this._viewerCache != null)
                    {
                        foreach (var doc in this._viewerCache)
                            doc.Changed -= _viewerCache_XmlDocumentChanged;
                        this._viewerCache.CollectionChanged -= this._viewerCache_CollectionChanged;
                    }

                    this._viewerCache = new System.Collections.ObjectModel.ObservableCollection<System.Xml.Linq.XDocument>(specialTransformer.Data);
                    foreach (var doc in this._viewerCache)
                        doc.Changed += _viewerCache_XmlDocumentChanged;

                    this._viewerCache.CollectionChanged += this._viewerCache_CollectionChanged;
                }
            }

            if (this._viewerCache != null)
            {
                var currentViewerBtn = this.toolStripViewers.Items.OfType<ToolStripButton>().First(o => o.Checked);
                var tag = (int)currentViewerBtn.Tag;
                var viewer = this._listViewers[tag];
                viewer.Item1.DisplayCaches(this._viewerCache, this.SelectedCacheCode);
            }

            this.Invoke(() => { this.toolStripViewersRefresh.Enabled = true; this.toolStripViewersRefresh.Text = "Refresh"; });
        }

        void _viewerCache_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (System.Xml.Linq.XDocument doc in e.OldItems)
                    doc.Changed -= this._viewerCache_XmlDocumentChanged;

            if (e.NewItems != null)
                foreach (System.Xml.Linq.XDocument doc in e.NewItems)
                    doc.Changed += this._viewerCache_XmlDocumentChanged;
        }

        void _viewerCache_XmlDocumentChanged(object sender, System.Xml.Linq.XObjectChangeEventArgs e)
        {
            if (e.ObjectChange == System.Xml.Linq.XObjectChange.Value || sender is System.Xml.Linq.XText)
                this.ParentForm.UnsavedData = true;

            if (e.ObjectChange == System.Xml.Linq.XObjectChange.Remove)
                this.ParentForm.UnsavedData = true;
        }

        /// <summary>
        /// Gets the the GC code of the currently selected cache.
        /// </summary>
        public string SelectedCacheCode
        {
            get;
            private set;
        }

        private static object SelectedCacheChangedEvent = new object();
        /// <summary>
        /// Occurs when <see cref="SelectedCacheCode"/> has changed.
        /// </summary>
        public event EventHandler<Viewers.SelectedCacheChangedEventArgs> SelectedCacheChanged
        {
            add { this.Events.AddHandler(SelectedCacheChangedEvent, value); }
            remove { this.Events.AddHandler(SelectedCacheChangedEvent, value); }
        }
    }
}
