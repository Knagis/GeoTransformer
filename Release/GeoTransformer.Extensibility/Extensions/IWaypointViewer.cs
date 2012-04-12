/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Extensions
{
    /// <summary>
    /// An interface for extensions that is displayed when the user has selected a one or more waypoints.
    /// </summary>
    public interface IWaypointViewer : IExtension
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
        /// Creates the control that will display the detailed waypoint information. The method is called only once and after that the control is reused.
        /// </summary>
        /// <returns>An initialized control that displays waypoint(-s).</returns>
        System.Windows.Forms.Control Initialize();

        /// <summary>
        /// Called by the main application to determine if the viewer is enabled for the selected waypoint(-s).
        /// </summary>
        /// <param name="waypoints">An array of GPX elements containing the waypoint information (can be empty array).</param>
        /// <returns>
        ///   <c>true</c> if the viewer is enabled for the specified waypoints; otherwise, <c>false</c>.
        /// </returns>
        bool IsEnabled(System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint> waypoints);

        /// <summary>
        /// Called to display the waypoint details in the UI control. The method is only called for waypoints after <see cref="IsEnabled"/> returns <c>true</c>.
        /// </summary>
        /// <param name="waypoints">An array of GPX elements containing the waypoint information (can be empty array).</param>
        void DisplayWaypoints(System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint> waypoints);
    }
}
