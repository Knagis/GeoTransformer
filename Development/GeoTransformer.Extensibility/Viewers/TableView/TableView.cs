/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using GeoTransformer.Extensions;

namespace GeoTransformer.Viewers.TableView
{
    /// <summary>
    /// Waypoint viewer that displays them in a table form.
    /// Can be exteneded by derived classes to customize what data and how is displayed.
    /// </summary>
    public class TableView : IWaypointListViewer, IConfigurable
    {
        private const int searchPanelSize = 30;
        private List<Gpx.GpxWaypoint> _originialItemsList;
        private Panel _searchPanel;
        private Panel _listViewPanel;
        private ToolStripTextBox _searchBox;

        /// <summary>
        /// Gets the icon to be displayed on the button.
        /// </summary>
        public virtual System.Drawing.Image ButtonImage
        {
            get { return Resources.Table; }
        }

        /// <summary>
        /// Gets the text to be displayed on the button.
        /// </summary>
        public virtual string ButtonText
        {
            get { return "Show in table"; }
        }

        /// <summary>
        /// Prepares the list view for display - sets common settings and adds any columns to <see cref="BrightIdeasSoftware.ObjectListView.AllColumns"/>
        /// collection.
        /// </summary>
        /// <param name="olv">The control that needs to be initialized.</param>
        protected virtual void PrepareListView(BrightIdeasSoftware.ObjectListView olv)
        {
            Columns.InitializeListView(olv);
            Columns.CreateColumns(olv, false);
        }

        /// <summary>
        /// Filters the data and returns only the waypoints that has to be displayed on the list view.
        /// </summary>
        /// <param name="data">A list of GPX documents containing the cache information.</param>
        /// <returns>List of waypoints that will be displayed.</returns>
        protected virtual IEnumerable<Gpx.GpxWaypoint> FilterData(IEnumerable<Gpx.GpxDocument> data)
        {
            return data.SelectMany(o => o.Waypoints)
                    .Where(o => 
                        {
                            if (!o.Geocache.IsDefined())
                                return false;

                            var editorOnly = string.Equals(o.FindExtensionAttributeValue("EditorOnly"), bool.TrueString, StringComparison.OrdinalIgnoreCase);
                            Transformers.ManualPublish.ManualPublishMode publishMode;
                            Enum.TryParse(o.FindExtensionElement(typeof(Transformers.ManualPublish.ManualPublish)).GetValue(), out publishMode);
                            if (publishMode == Transformers.ManualPublish.ManualPublishMode.AlwaysSkip)
                                return false;

                            if (editorOnly && publishMode != Transformers.ManualPublish.ManualPublishMode.AlwaysPublish)
                                return false;

                            return true;
                        });
        }

        private BrightIdeasSoftware.ObjectListView _control;
        private byte[] _controlState;

        /// <summary>
        /// Creates the control that will display the caches in the main form. The method is called only once and after that the control is reused.
        /// </summary>
        /// <returns>
        /// An initialized control that display the cache list.
        /// </returns>
        public virtual System.Windows.Forms.Control Initialize()
        {
            // Draw search panel
            _searchPanel = new Panel();
            _searchPanel.Height = 0;
            _searchPanel.Visible = false;
            _searchPanel.Dock = DockStyle.Top;

            ToolStrip searchStrip = new ToolStrip();
            this._searchBox = new ToolStripTextBox();
            _searchBox.KeyDown += SearchBox_PreviewKeyDown;

            ToolStripButton searchButton = new ToolStripButton("Search", Resources.Search);
            searchButton.Click += SearchButton_Click;

            ToolStripButton clearButton = new ToolStripButton("Clear", Resources.Clear);
            clearButton.Click += ClearButton_Click;
            searchStrip.Items.Add(this._searchBox);
            searchStrip.Items.Add(searchButton);
            searchStrip.Items.Add(new ToolStripSeparator());
            searchStrip.Items.Add(clearButton);
            searchStrip.Dock = DockStyle.Fill;
            _searchPanel.Controls.Add(searchStrip);

            // Draw cache list, restore control state
            this._control = new BrightIdeasSoftware.ObjectListView();
            this.PrepareListView(this._control);
            this._control.Columns.AddRange(this._control.AllColumns.ToArray());
            this._control.SetObjects(new Gpx.GpxWaypoint[0]);

            if (this._controlState != null)
            {
                this._control.RestoreState(this._controlState);
                this._controlState = null;
            }

            this._control.SelectionChanged += ListView_SelectedIndexChanged;
            this._control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            this._control.Dock = DockStyle.Fill;
            this._control.PreviewKeyDown += Panel_PreviewKeyDown;

            // Draw panel for list view
            _listViewPanel = new Panel();
            _listViewPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _listViewPanel.Controls.Add(this._control);

            // Draw table layout to display common form
            Panel panel = new Panel();
            panel.Controls.Add(_searchPanel);
            panel.Controls.Add(_listViewPanel);
            panel.PreviewKeyDown += Panel_PreviewKeyDown;
            return panel;
        }

        void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            var handler = this.SelectedCacheChanged;
            if (this.SelectedCacheChanged != null)
            {
                var selected = this._control.SelectedObject as Gpx.GpxWaypoint;
                this.SelectedCacheChanged(this, new SelectedWaypointsChangedEventArgs(selected));
            }
        }

        /// <summary>
        /// Called by the main form to display the caches in the viewer control returned by <see cref="Initialize"/>. The method can be called multiple
        /// times and each time the previous data is overwritten. This method is called from a background thread.
        /// </summary>
        /// <param name="data">A list of GPX documents containing the cache information. The viewer may modify the list.</param>
        /// <param name="selected">The waypoints that are currently selected. If the viewer does not support multiple selection the first waypoint should be used.</param>
        public virtual void DisplayCaches(IList<Gpx.GpxDocument> data, System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint> selected)
        {
            this._originialItemsList = this.FilterData(data).ToList();
            this._control.Invoke(o => o.SetObjects(this._originialItemsList));

            if (selected != null && selected.Count > 0)
            {
                var i = this._originialItemsList.IndexOf(selected[0]);
                this._control.Invoke(o => o.SelectedIndex = i);
                if (i > -1)
                    this._control.Invoke(o => o.SelectedItem.EnsureVisible());
            }
        }

        /// <summary>
        /// Occurs when the currently selected waypoints have been changed from within the control.
        /// </summary>
        public event EventHandler<SelectedWaypointsChangedEventArgs> SelectedCacheChanged;

        #region [ IConfigurable members ]

        /// <summary>
        /// Initializes the extension with the specified current configuration (can be <c>null</c> if the extension is initialized for the very first time) and
        /// returns the configuration UI control (can return <c>null</c> if the user interface is not needed).
        /// </summary>
        /// <param name="currentConfiguration">The current configuration.</param>
        /// <returns>
        /// The configuration UI control.
        /// </returns>
        Control IConfigurable.Initialize(byte[] currentConfiguration)
        {
            this._controlState = currentConfiguration;
            return null;
        }

        /// <summary>
        /// Retrieves the configuration from the extension's configuration UI control.
        /// </summary>
        /// <returns>
        /// The serialized configuration data.
        /// </returns>
        byte[] IConfigurable.SerializeConfiguration()
        {
            // if Initialize() has not yet been called...
            if (this._control == null)
                return this._controlState;

            return this._control.SaveState();
        }

        /// <summary>
        /// Gets a value indicating whether the this extension should be executed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this extension is enabled; otherwise, <c>false</c>.
        /// </value>
        bool IConditional.IsEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the category of the extension.
        /// </summary>
        Category IHasCategory.Category
        {
            get { return Category.Viewers; }
        }

        #endregion

        #region [ Search control event handlers ]

        void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.SearchButton_Click(sender, e);
        }

        void SearchButton_Click(object sender, EventArgs e)
        {
            var filteredItems = this._originialItemsList
                .Where(a => a.Name.ToUpper().Contains(this._searchBox.Text.ToUpper()) || a.Description.ToUpper().Contains(this._searchBox.Text.ToUpper()));
            this._control.Invoke(o => o.SetObjects(filteredItems));
        }

        void Panel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F && !_searchPanel.Visible)
            {
                _searchPanel.Visible = true;
                _searchPanel.Height = searchPanelSize;
                _listViewPanel.Top = searchPanelSize;
                _listViewPanel.Height -= searchPanelSize;
                _searchBox.Focus();
            }
        }

        void ClearButton_Click(object sender, EventArgs e)
        {
            this._control.Invoke(o => o.SetObjects(this._originialItemsList));
            this._searchBox.Text = string.Empty;
            _searchPanel.Height = 0;
            _searchPanel.Visible = false;
            _listViewPanel.Top = 0;
            _listViewPanel.Height += searchPanelSize;
            this._control.Focus();
        }

        #endregion
    }
}
