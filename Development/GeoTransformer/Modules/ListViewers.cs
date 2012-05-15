/*
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

        private List<Tuple<IWaypointListViewer, Control>> _listViewers = new List<Tuple<IWaypointListViewer, Control>>();
        private Gpx.ObservableCollection<Gpx.GpxDocument> _viewerCache;

        /// <summary>
        /// Gets the list of loaded GPX files.
        /// </summary>
        internal IList<Gpx.GpxDocument> LoadedGpxFiles
        {
            get { return this._viewerCache; }
        }

        private void InitializeViewers()
        {
            foreach (var instance in Extensions.ExtensionLoader.RetrieveExtensions<IWaypointListViewer>()
                                            .OrderBy(o => o is Viewers.TableView.EditedCacheTableView ? 0 : 1)
                                            .ThenBy(o => o.ButtonText))
            {
                instance.SelectedCacheChanged += ViewerSelectedCacheChanged;

                var btn = new ToolStripButton(instance.ButtonText, instance.ButtonImage);
                btn.TextImageRelation = TextImageRelation.ImageAboveText;
                btn.Tag = this._listViewers.Count;

                var cond = instance as Extensions.IConditional;
                if (cond != null && !cond.IsEnabled)
                    btn.Enabled = false;
                
                this._listViewers.Add(Tuple.Create<IWaypointListViewer, Control>(instance, null));
                this.toolStripViewers.Items.Add(btn);
            }
        }

        void ViewerSelectedCacheChanged(object sender, Viewers.SelectedWaypointsChangedEventArgs e)
        {
            var viewer = this._listViewers.FirstOrDefault(o => o.Item1 == sender as IWaypointListViewer);
            if (viewer == null || !viewer.Item2.Visible)
                return;

            if (this.SelectedWaypoints == null && e.Selection == null)
                return;

            if (this.SelectedWaypoints != null && this.SelectedWaypoints.SequenceEqual(e.Selection))
                return;

            this.SelectedWaypoints = e.Selection;

            var handler = this.Events[SelectedWaypointsChangedEvent] as EventHandler<Viewers.SelectedWaypointsChangedEventArgs>;
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

        internal void ChangeSelectedWaypoint(string name)
        {
            // TODO: could save the name in a temp variable and select it once the load completes
            if (this._viewerCache == null)
                return;

            this.SelectedWaypoints = new System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint>(
                    this._viewerCache.SelectMany(o => o.Waypoints)
                                     .Where(o => string.Equals(o.Name, name, StringComparison.OrdinalIgnoreCase))
                                     .ToList()
                );

            new System.Threading.Thread(() => { this.LoadListViewerData(false); }).Start();
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
                            doc.PropertyChanged -= _viewerCache_GpxDocumentChanged;
                        this._viewerCache.CollectionChanged -= this._viewerCache_CollectionChanged;
                    }

                    this._viewerCache = new Gpx.ObservableCollection<Gpx.GpxDocument>(specialTransformer.Data);
                    foreach (var doc in this._viewerCache)
                        doc.PropertyChanged += _viewerCache_GpxDocumentChanged;

                    this._viewerCache.CollectionChanged += this._viewerCache_CollectionChanged;
                }
            }

            if (this._viewerCache != null)
            {
                var currentViewerBtn = this.toolStripViewers.Items.OfType<ToolStripButton>().First(o => o.Checked);
                var tag = (int)currentViewerBtn.Tag;
                var viewer = this._listViewers[tag];
                viewer.Item1.DisplayCaches(this._viewerCache, this.SelectedWaypoints);
            }

            this.Invoke(() => { this.toolStripViewersRefresh.Enabled = true; this.toolStripViewersRefresh.Text = "Refresh"; });
        }

        void _viewerCache_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (Gpx.GpxDocument doc in e.OldItems)
                    doc.PropertyChanged -= this._viewerCache_GpxDocumentChanged;

            if (e.NewItems != null)
                foreach (Gpx.GpxDocument doc in e.NewItems)
                    doc.PropertyChanged += this._viewerCache_GpxDocumentChanged;
        }

        void _viewerCache_GpxDocumentChanged(object sender, Gpx.ObservableElementChangedEventArgs e)
        {
            // if the marker is already set, no need to spend any more time on this
            if (this.ParentForm.UnsavedData)
                return;

            // find the inner most change that triggered the event.
            var innerMost = e.FindFirstChange();

            // for now the change detection is limited to extension elements as those are the only data that is saved
            // once "Save" is clicked (assuming no ISaveData external extensions are added).
            // TODO: this should be forwarded to the extensions doing the actual saving
            if (innerMost.XObjectChange.HasValue && (innerMost.XObjectChange == System.Xml.Linq.XObjectChange.Value || innerMost.Target is System.Xml.Linq.XText))
                this.ParentForm.UnsavedData = true;

            // handle the case when a editor extensions are removed from an object
            if (string.Equals(innerMost.PropertyName, "ExtensionElements", StringComparison.Ordinal) 
                    && innerMost.CollectionChange != null 
                    && innerMost.CollectionChange.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                this.ParentForm.UnsavedData = true;
        }

        /// <summary>
        /// Gets the the GC code of the currently selected cache.
        /// </summary>
        public System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint> SelectedWaypoints
        {
            get;
            private set;
        }

        private static object SelectedWaypointsChangedEvent = new object();
        /// <summary>
        /// Occurs when <see cref="SelectedCacheCode"/> has changed.
        /// </summary>
        public event EventHandler<Viewers.SelectedWaypointsChangedEventArgs> SelectedWaypointsChanged
        {
            add { this.Events.AddHandler(SelectedWaypointsChangedEvent, value); }
            remove { this.Events.AddHandler(SelectedWaypointsChangedEvent, value); }
        }
    }
}
