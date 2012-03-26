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
    public class TableView : IWaypointListViewer
    {
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
                    .Where(o => o.Geocache.IsDefined() 
                    && !string.Equals(bool.TrueString, o.FindExtensionAttributeValue("EditorOnly"), StringComparison.OrdinalIgnoreCase));
        }

        private BrightIdeasSoftware.ObjectListView _control;
        
        /// <summary>
        /// Creates the control that will display the caches in the main form. The method is called only once and after that the control is reused.
        /// </summary>
        /// <returns>
        /// An initialized control that display the cache list.
        /// </returns>
        public System.Windows.Forms.Control Initialize()
        {
            this._control = new BrightIdeasSoftware.ObjectListView();
            this.PrepareListView(this._control);
            this._control.Columns.AddRange(this._control.AllColumns.ToArray());
            this._control.SetObjects(new Gpx.GpxWaypoint[0]);

            this._control.SelectionChanged += ListView_SelectedIndexChanged;

            return this._control;
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
        public void DisplayCaches(IList<Gpx.GpxDocument> data, System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint> selected)
        {
            var wpts = this.FilterData(data).ToList();
            this._control.Invoke(o => o.SetObjects(wpts));

            if (selected != null && selected.Count > 0)
            {
                var i = wpts.IndexOf(selected[0]);
                this._control.Invoke(o => o.SelectedIndex = i);
                if (i > -1)
                    this._control.Invoke(o => o.SelectedItem.EnsureVisible());
            }
        }

        /// <summary>
        /// Occurs when the currently selected waypoints have been changed from within the control.
        /// </summary>
        public event EventHandler<SelectedWaypointsChangedEventArgs> SelectedCacheChanged;
    }
}
