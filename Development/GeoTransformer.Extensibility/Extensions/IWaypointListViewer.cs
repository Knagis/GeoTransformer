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

namespace GeoTransformer.Extensions
{
    /// <summary>
    /// An interface for viewers that display the list of loaded caches.
    /// </summary>
    public interface IWaypointListViewer : IExtension
    {
        /// <summary>
        /// Gets the icon to be displayed on the button.
        /// </summary>
        /// <remarks>The image should be 24x24 pixels.</remarks>
        System.Drawing.Image ButtonImage { get; }

        /// <summary>
        /// Gets the text to be displayed on the button.
        /// </summary>
        string ButtonText { get; }

        /// <summary>
        /// Creates the control that will display the caches in the main form. The method is called only once and after that the control is reused.
        /// </summary>
        /// <returns>An initialized control that display the cache list.</returns>
        Control Initialize();

        /// <summary>
        /// Called by the main form to display the caches in the viewer control returned by <see cref="Initialize"/>. The method can be called multiple
        /// times and each time the previous data is overwritten. This method is called from a background thread.
        /// </summary>
        /// <param name="data">A list of GPX documents containing the cache information. The viewer may modify the list.</param>
        /// <param name="selected">The waypoints that are currently selected. If the viewer does not support multiple selection the first waypoint should be used.</param>
        void DisplayCaches(IList<Gpx.GpxDocument> data, System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint> selected);

        /// <summary>
        /// Occurs when the currently selected waypoints have been changed from within the control.
        /// </summary>
        event EventHandler<Viewers.SelectedWaypointsChangedEventArgs> SelectedCacheChanged;
    }
}
