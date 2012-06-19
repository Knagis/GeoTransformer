/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace GeoTransformer.Viewers.BingMaps
{
    /// <summary>
    /// Displays geocaches on Bing Maps.
    /// </summary>
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class BingMapsList : Extensions.IWaypointListViewer, Extensions.ILocalStorage
    {
        /// <summary>
        /// Gets the icon to be displayed on the button.
        /// </summary>
        public System.Drawing.Image ButtonImage
        {
            get { return Resources.icon; }
        }

        /// <summary>
        /// Gets the text to be displayed on the button.
        /// </summary>
        public string ButtonText
        {
            get { return "Bing maps"; }
        }

        private System.Windows.Forms.WebBrowser _browser;
        private bool _firstRequest = true;
        private bool _iconsCreated;
        private bool _setBoundsCalled;
        private Dictionary<string, Gpx.GpxWaypoint> _pushpinCache = new Dictionary<string, Gpx.GpxWaypoint>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, int> _pushpinScriptHashes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Creates the control that will display the caches in the main form. The method is called only once and after that the control is reused.
        /// </summary>
        /// <returns>
        /// An initialized control that display the cache list.
        /// </returns>
        public System.Windows.Forms.Control Initialize()
        {
            this._browser = new System.Windows.Forms.WebBrowser();
            return this._browser;
        }

        /// <summary>
        /// Converts the given image to a grayscale copy.
        /// </summary>
        /// <param name="original">The image to convert.</param>
        private static System.Drawing.Image GrayscaleIcon(System.Drawing.Image original)
        {
            //create a blank bitmap the same size as original 
            var newBitmap = new System.Drawing.Bitmap(original.Width, original.Height);

            //get a graphics object from the new image 
            using (var g = System.Drawing.Graphics.FromImage(newBitmap))
            {

                //create the grayscale ColorMatrix 
                var colorMatrix = new System.Drawing.Imaging.ColorMatrix(new float[][]  
                      { 
                         new float[] {.3f, .3f, .3f, 0, 0}, 
                         new float[] {.59f, .59f, .59f, 0, 0}, 
                         new float[] {.11f, .11f, .11f, 0, 0}, 
                         new float[] {0, 0, 0, 1, 0}, 
                         new float[] {.25f, .25f, .25f, 0, 1} 
                      });

                //create some image attributes 
                var attributes = new System.Drawing.Imaging.ImageAttributes();

                //set the color matrix attribute 
                attributes.SetColorMatrix(colorMatrix);

                //draw the original image on the new image 
                //using the grayscale color matrix 
                g.DrawImage(original, new System.Drawing.Rectangle(0, 0, original.Width, original.Height),
                   0, 0, original.Width, original.Height, System.Drawing.GraphicsUnit.Pixel, attributes);
            }

            return newBitmap; 
        }

        /// <summary>
        /// Creates the relative URI that points to the icon for the given waypoint.
        /// If <paramref name="waypoint"/> is <c>null</c>, returns the base URI for the icons.
        /// </summary>
        public Uri CreateCacheIconUri(Gpx.GpxWaypoint waypoint)
        {
            var uriB = new UriBuilder();
            if (waypoint == null)
            {
                uriB.Scheme = Uri.UriSchemeFile;
                uriB.Path = System.IO.Path.Combine(this.LocalStoragePath);
                return uriB.Uri;
            }

            var cacheType = waypoint.Geocache.CacheType.Name;
            if (string.IsNullOrEmpty(cacheType))
                return new Uri("about:blank");

            if (string.Equals(waypoint.Symbol, "Geocache Found", StringComparison.OrdinalIgnoreCase))
                cacheType = "Found Cache";

            bool enabled = !waypoint.Geocache.Archived && waypoint.Geocache.Available;

            cacheType = cacheType.Replace(" ", "").Replace("-", "");

            if (!this._iconsCreated && !System.IO.File.Exists(System.IO.Path.Combine(this.LocalStoragePath, cacheType + ".d.png")))
            {
                foreach (var x in typeof(UI.CacheTypeIcons).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
                {
                    if (typeof(System.Drawing.Image).IsAssignableFrom(x.PropertyType))
                    {
                        var img = ((System.Drawing.Image)x.GetValue(null, null));
                        img.Save(System.IO.Path.Combine(this.LocalStoragePath, x.Name + ".png"), System.Drawing.Imaging.ImageFormat.Png);
                        using (var grayscale = GrayscaleIcon(img))
                            grayscale.Save(System.IO.Path.Combine(this.LocalStoragePath, x.Name + ".d.png"), System.Drawing.Imaging.ImageFormat.Png);
                    }
                }

                this._iconsCreated = true;
            }

            uriB.Scheme = Uri.UriSchemeFile;
            var ext = enabled ? ".png" : ".d.png";
            uriB.Path = System.IO.Path.Combine(this.LocalStoragePath, cacheType + ext);

            return uriB.Uri;
        }

        /// <summary>
        /// Called by the main form to display the caches in the viewer control returned by <see cref="Initialize"/>. The method can be called multiple
        /// times and each time the previous data is overwritten. This method is called from a background thread.
        /// </summary>
        /// <param name="data">A list of GPX documents containing the cache information. The viewer may modify the list.</param>
        /// <param name="selected">The waypoints that are currently selected. If the viewer does not support multiple selection the first waypoint should be used.</param>
        public void DisplayCaches(IList<Gpx.GpxDocument> data, System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint> selected)
        {
            this._pushpinCache.Clear();
            this._pushpinScriptHashes.Clear();

            if (this._firstRequest)
            {
                this._firstRequest = false;

                this._browser.DocumentText = this.CreateMapPage(data);
            }
            else
            {
                this.CreateMapUpdateScript(data);
            }

            this._browser.ObjectForScripting = this;
        }

        /// <summary>
        /// Method called via COM when the user clicks on a pushpin within the browser.
        /// </summary>
        /// <param name="cacheCode">The geocache code.</param>
        public void PushpinOnClick(string cacheCode)
        {
            if (!this._pushpinCache.ContainsKey(cacheCode))
                return;

            var x = this._pushpinCache[cacheCode];
            if (this.SelectedCacheChanged != null)
                this.SelectedCacheChanged(this, new SelectedWaypointsChangedEventArgs(x));
        }

        private string CreateMapPage(IEnumerable<Gpx.GpxDocument> data)
        {
            var sb = new StringBuilder();
            sb.Append(@"iconRoot = '");
            sb.Append(this.CreateCacheIconUri(null).ToString());
            sb.Append(@"/';");
            this.CreateMapScript(sb, data);

            var html = Resources.MapTemplate;
            html = html.Replace("/*SCRIPT_GOES_HERE*/", sb.ToString());
            return html;
        }

        /// <summary>
        /// Executes the given JavaScript code on the embedded browser. Performs invoke on the UI thread if needed.
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        private void ExecuteScript(string script)
        {
            if (string.IsNullOrWhiteSpace(script))
                return;

            if (this._browser.InvokeRequired)
            {
                this._browser.Invoke(() => this.ExecuteScript(script));
                return;
            }

            var element = this._browser.Document.CreateElement("script");
            element.SetAttribute("type", "text/javascript");
            element.SetAttribute("text", script);
            this._browser.Document.Body.InsertAdjacentElement(System.Windows.Forms.HtmlElementInsertionOrientation.BeforeEnd, element);
        }

        private void CreateMapUpdateScript(IEnumerable<Gpx.GpxDocument> data)
        {
            var sb = new StringBuilder();
            sb.AppendLine("map.entities.clear();");

            this.CreatePushpins(sb, data.SelectMany(o => o.Waypoints));

            this.ExecuteScript(sb.ToString());
        }

        private void CreateMapScript(StringBuilder sb, IEnumerable<Gpx.GpxDocument> data)
        {
            sb.AppendLine(@"document.body.style.height=""" + this._browser.ClientSize.Height + @"px"";");
            sb.AppendLine(@"map = new Microsoft.Maps.Map(document.body, {credentials:""" + BingApiKeys.MapsKey + @""", enableSearchLogo: false, enableClickableLogo: false});");

            this.CreatePushpins(sb, data.SelectMany(o => o.Waypoints));
        }

        private void CreatePushpins(StringBuilder sb, IEnumerable<Gpx.GpxWaypoint> waypoints)
        {
            decimal minLat = decimal.MaxValue;
            decimal minLon = decimal.MaxValue;
            decimal maxLat = decimal.MinValue;
            decimal maxLon = decimal.MinValue;

            foreach (var wpt in waypoints)
            {
                if (!wpt.Geocache.IsDefined())
                    continue;

                // monitor all waypoint even those that currently are not displayed
                wpt.PropertyChanged -= this.UpdatePushpin;
                wpt.PropertyChanged += this.UpdatePushpin;

                var editorOnly = string.Equals(wpt.FindExtensionAttributeValue("EditorOnly"), bool.TrueString, StringComparison.OrdinalIgnoreCase);
                Transformers.ManualPublish.ManualPublishMode publishMode;
                Enum.TryParse(wpt.FindExtensionElement(typeof(Transformers.ManualPublish.ManualPublish)).GetValue(), out publishMode);
                if (publishMode == Transformers.ManualPublish.ManualPublishMode.AlwaysSkip)
                    continue;

                if (editorOnly && publishMode != Transformers.ManualPublish.ManualPublishMode.AlwaysPublish)
                    continue;

                this.CreatePushpin(sb, wpt, ref minLat, ref minLon, ref maxLat, ref maxLon);
            }

            if (!this._setBoundsCalled && minLat != decimal.MaxValue)
            {
                this._setBoundsCalled = true;
                sb.AppendLine("map.setView({ bounds: Microsoft.Maps.LocationRect.fromLocations(new Microsoft.Maps.Location(" + minLat.ToString(CultureInfo.InvariantCulture) + ", " + minLon.ToString(CultureInfo.InvariantCulture) + "), new Microsoft.Maps.Location(" + maxLat.ToString(CultureInfo.InvariantCulture) + ", " + maxLon.ToString(CultureInfo.InvariantCulture) + "))});");
            }
        }

        private void CreatePushpin(StringBuilder sb, Gpx.GpxWaypoint waypoint, ref decimal minLat, ref decimal minLon, ref decimal maxLat, ref decimal maxLon, string methodName = "createPP", bool avoidDuplicates = false)
        {
            var lat = waypoint.Coordinates.Latitude;
            var lon = waypoint.Coordinates.Longitude;
            var name = waypoint.Name;

            if (string.IsNullOrWhiteSpace(name))
                return;

            if (lat < minLat) minLat = lat;
            if (lat > maxLat) maxLat = lat;
            if (lon < minLon) minLon = lon;
            if (lon > maxLon) maxLon = lon;

            this._pushpinCache[name] = waypoint;

            var icon = this.CreateCacheIconUri(waypoint);

            var desc = waypoint.Description;

            string args = "(" + lat.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", "
                                      + lon.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", '" 
                                      + name.Replace("'", @"\'") + "', '" 
                                      + icon.Segments[icon.Segments.Length - 1] + "', '" 
                                      + desc.Replace("'", @"\'") + "')";

            var hash = args.GetHashCode();

            // this avoids sending multiple updatePP() calls to the browser if nothing has changed.
            if (avoidDuplicates && _pushpinScriptHashes.ContainsKey(name) && _pushpinScriptHashes[name] == hash)
                return;

            _pushpinScriptHashes[name] = hash;

            sb.Append(methodName);
            sb.AppendLine(args);
        }

        private void UpdatePushpin(object sender, Gpx.ObservableElementChangedEventArgs e)
        {
            var sb = new StringBuilder();
            var waypoint = (Gpx.GpxWaypoint)sender;
            bool exists = this._pushpinCache.ContainsKey(waypoint.Name);

            // determine if the waypoint should be shown at all
            var editorOnly = string.Equals(waypoint.FindExtensionAttributeValue("EditorOnly"), bool.TrueString, StringComparison.OrdinalIgnoreCase);
            Transformers.ManualPublish.ManualPublishMode publishMode;
            Enum.TryParse(waypoint.FindExtensionElement(typeof(Transformers.ManualPublish.ManualPublish)).GetValue(), out publishMode);
            if (!waypoint.Geocache.IsDefined() 
                || publishMode == Transformers.ManualPublish.ManualPublishMode.AlwaysSkip
                || (editorOnly && publishMode != Transformers.ManualPublish.ManualPublishMode.AlwaysPublish))
            {
                if (exists)
                {
                    this._pushpinCache.Remove(waypoint.Name);
                    this._pushpinScriptHashes.Remove(waypoint.Name);
                    this.ExecuteScript("removePP('" + waypoint.Name.Replace("'", @"\'") + "')");
                }
                return;
            }

            decimal temp = 0;
            this.CreatePushpin(sb, waypoint, ref temp, ref temp, ref temp, ref temp, 
                exists ? "updatePP" : "createPP", true);

            this.ExecuteScript(sb.ToString());
        }

        /// <summary>
        /// Occurs when the currently selected waypoints have been changed from within the control.
        /// </summary>
        public event EventHandler<SelectedWaypointsChangedEventArgs> SelectedCacheChanged;

        /// <summary>
        /// Contains the local storage path where the extension can store its cache if needed. The value is set by the main engine once the extension instance is created.
        /// </summary>
        private string LocalStoragePath;

        /// <summary>
        /// Sets the local storage path where the extension can store its cache if needed. The value is set by the main engine once the extension instance is created.
        /// </summary>
        string Extensions.ILocalStorage.LocalStoragePath
        {
            set { this.LocalStoragePath = value; }
        }
    }
}
