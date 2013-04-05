/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Viewers.GeoHack
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
            get { return "GeoHack maps"; }
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

            var sb = new StringBuilder();
            sb.Append("http://toolserver.org/~geohack/geohack.php?pagename=");
            sb.Append(Uri.EscapeDataString(wpt.Description));
            sb.Append("&params=");

            var n = Math.Abs(coords.Latitude);
            var n1 = Math.Truncate(n);
            var n2 = Math.Truncate((n - n1) * 60);
            var n3 = Math.Round((n - n1 - n2 / 60) * 3600, 4);
            var n4 = coords.Latitude < 0 ? "S" : "N";

            var e = Math.Abs(coords.Longitude);
            var e1 = Math.Truncate(e);
            var e2 = Math.Truncate((e - e1) * 60);
            var e3 = Math.Round((e - e1 - e2 / 60) * 3600, 4);
            var e4 = coords.Longitude < 0 ? "W" : "E";

            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}_type:landmark", n1, n2, n3, n4, e1, e2, e3, e4);

            this._browser.Navigate(sb.ToString());
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
