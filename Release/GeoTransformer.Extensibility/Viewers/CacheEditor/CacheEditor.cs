/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Viewers.CacheEditor
{
    /// <summary>
    /// Displays waypoint editor controls (extensions derived from <see cref="Extensions.IEditor"/>).
    /// </summary>
    public class CacheEditor : Extensions.IWaypointViewer
    {
        /// <summary>
        /// Gets the icon to be displayed on the button.
        /// </summary>
        public System.Drawing.Image ButtonImage
        {
            get { return Resources.Edit; }
        }

        /// <summary>
        /// Gets the text to be displayed on the button.
        /// </summary>
        public string ButtonText
        {
            get { return "Editor"; }
        }

        private EditorControl _container;
        private List<Tuple<Extensions.IEditor, System.Windows.Forms.Control>> _editors = new List<Tuple<Extensions.IEditor,System.Windows.Forms.Control>>();

        /// <summary>
        /// Creates the control that will display the detailed waypoint information. The method is called only once and after that the control is reused.
        /// </summary>
        /// <returns>
        /// An initialized control that displays waypoint(-s).
        /// </returns>
        public System.Windows.Forms.Control Initialize()
        {
            this._container = new EditorControl(this);

            foreach (var ext in Extensions.ExtensionLoader.RetrieveExtensions<Extensions.IEditor>())
            {
                var c = ext.CreateControl();
                this._container.flowLayoutPanel.Controls.Add(c);

                // workaround for a bug in flow layout panel
                this._container.flowLayoutPanel.Controls.Add(new System.Windows.Forms.Control() { Width = 0, Height = 0 });

                this._editors.Add(Tuple.Create(ext, c));
            }

            return this._container;
        }

        /// <summary>
        /// Called to display the cache details in the UI control. The method is only called for waypoints after <see cref="IsEnabled"/> returns <c>true</c>.
        /// </summary>
        /// <param name="waypoints">An array of GPX elements containing the waypoint information (can be empty array).</param>
        public void DisplayWaypoints(System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint> waypoints)
        {
            var wpt = waypoints.FirstOrDefault();

            this._container.BindElement(wpt);

            this._container.flowLayoutPanel.Enabled = wpt != null;

            foreach (var editor in this._editors)
            {
                editor.Item1.BindControl(wpt);
            }
        }

        /// <summary>
        /// Called by the main application to determine if the viewer is enabled for the particular waypoint.
        /// </summary>
        /// <param name="waypoints">An array of GPX elements containing the waypoint information (can be empty array).</param>
        /// <returns>
        ///   <c>true</c> if the viewer is enabled for the specified waypoint; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEnabled(System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint> waypoints)
        {
            if (waypoints == null)
                return true;

            return waypoints.Count <= 1;
        }
    }
}
