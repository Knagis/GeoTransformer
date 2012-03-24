/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Viewers.CacheWebPage
{
    public class CacheWebPage : Extensions.IWaypointViewer
    {
        private System.Windows.Forms.WebBrowser _browser;

        public System.Drawing.Image ButtonImage
        {
            get { return Resources.Web; }
        }

        public string ButtonText
        {
            get { return "Online version"; }
        }

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
            foreach (var l in wpt.Links)
                if (l != null && l.Href != null)
                {
                    this._browser.Navigate(l.Href);
                    return;
                }

            var name = wpt.Name;
            if (name != null && name.StartsWith("GC", StringComparison.OrdinalIgnoreCase))
            {
                this._browser.Navigate("http://coord.info/" + wpt.Name);
                return;
            }

            this._browser.Navigate("about:blank");
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

            var wpt = waypoints[0];
            foreach (var l in wpt.Links)
                if (l != null && l.Href != null)
                    return true;

            var name = wpt.Name;
            if (name != null && name.StartsWith("GC", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}
