/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Viewers.GoogleMaps
{
    /// <summary>
    /// Waypoint viewer that displays Google Maps web site pointing to the waypoint coordinates.
    /// </summary>
    public class GoogleMaps : Extensions.IWaypointViewer
    {
        private System.Windows.Forms.WebBrowser _browser;

        /// <summary>
        /// Gets the icon to be displayed on the button.
        /// </summary>
        public System.Drawing.Image ButtonImage
        {
            get { return Resources.Web; }
        }

        /// <summary>
        /// Gets the text to be displayed on the button.
        /// </summary>
        public string ButtonText
        {
            get { return "Google maps"; }
        }

        /// <summary>
        /// Creates the control that will display the detailed waypoint information. The method is called only once and after that the control is reused.
        /// </summary>
        /// <returns>
        /// An initialized control that displays waypoint(-s).
        /// </returns>
        public System.Windows.Forms.Control Initialize()
        {
            this._browser = new System.Windows.Forms.WebBrowser();

            return this._browser;
        }

        /// <summary>
        /// Called to display the cache details in the UI control. The method is only called for waypoints after <see cref="IsEnabled"/> returns <c>true</c>.
        /// </summary>
        /// <param name="waypoints">An array of GPX elements containing the waypoint information (can be empty array).</param>
        public void DisplayWaypoints(System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint> waypoints)
        {
            if (waypoints.Count == 0)
            {
                this._browser.Navigate("about:blank");
                return;
            }

            var wpt = waypoints[0];
            var coords = wpt.Coordinates;

            this._browser.Navigate(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        "https://maps.google.com/maps?q={3} ({2})&ll={0:0.00000},{1:0.00000}&z=16&mrt=loc", 
                        coords.Latitude, 
                        coords.Longitude,
                        Uri.EscapeDataString(wpt.Name),
                        coords.ToString()));
        }

        /// <summary>
        /// Called by the main application to determine if the viewer is enabled for the selected waypoint(-s).
        /// </summary>
        /// <param name="waypoints">An array of GPX elements containing the waypoint information (can be empty array).</param>
        /// <returns>
        ///   <c>true</c> if the viewer is enabled for the specified waypoints; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEnabled(System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint> waypoints)
        {
            if (waypoints == null || waypoints.Count != 1)
                return false;

            return true;
        }
    }
}
