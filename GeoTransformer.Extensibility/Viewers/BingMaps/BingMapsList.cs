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
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class BingMapsList : Extensions.ICacheListViewer, Extensions.ILocalStorage
    {
        public System.Drawing.Image ButtonImage
        {
            get { return Resources.icon; }
        }

        public string ButtonText
        {
            get { return "Bing maps"; }
        }

        private System.Windows.Forms.WebBrowser _browser;
        private bool _firstRequest = true;
        private bool _iconsCreated;
        private bool _setBoundsCalled;
        private Dictionary<string, System.Xml.Linq.XElement> _pushpinCache = new Dictionary<string, System.Xml.Linq.XElement>();
        public System.Windows.Forms.Control Initialize()
        {
            this._browser = new System.Windows.Forms.WebBrowser();
            return this._browser;
        }

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
        public Uri CreateCacheIconUri(System.Xml.Linq.XElement waypoint)
        {
            var uriB = new UriBuilder();
            if (waypoint == null)
            {
                uriB.Scheme = Uri.UriSchemeFile;
                uriB.Path = System.IO.Path.Combine(this.LocalStoragePath);
                return uriB.Uri;
            }

            var cache = waypoint.CacheElement("cache");
            if (cache == null)
                return new Uri("about:blank");

            var cacheType = cache.CacheElement("type").GetValue();
            if (string.IsNullOrEmpty(cacheType))
                return new Uri("about:blank");

            if (string.Equals(waypoint.WaypointElement("sym").GetValue(), "Geocache Found", StringComparison.OrdinalIgnoreCase))
                cacheType = "Found Cache";

            bool enabled = !string.Equals(cache.GetAttributeValue("available"), "false", StringComparison.OrdinalIgnoreCase)
                            && string.Equals(cache.GetAttributeValue("archived"), "false", StringComparison.OrdinalIgnoreCase);

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

        public void DisplayCaches(IList<System.Xml.Linq.XDocument> data, string cacheCode)
        {
            this._pushpinCache.Clear();

            if (this._firstRequest)
            {
                this._firstRequest = false;

                this._browser.DocumentText = this.CreateMapPage(data);
            }
            else
            {
                this._browser.Invoke(() => this.CreateMapUpdateScript(data));
            }

            this._browser.ObjectForScripting = this;
        }

        public void PushpinOnClick(string cacheCode)
        {
            if (!this._pushpinCache.ContainsKey(cacheCode))
                return;

            var x = this._pushpinCache[cacheCode];
            if (this.SelectedCacheChanged != null)
                this.SelectedCacheChanged(this, new SelectedCacheChangedEventArgs(x));
        }

        private string CreateMapPage(IEnumerable<System.Xml.Linq.XDocument> data)
        {
            var iconRoot = this.CreateCacheIconUri(null);

            var sb = new StringBuilder();
            sb.Append(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head >
    <script src=""http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=7.0"" type=""text/javascript""></script>
    <meta http-equiv=""Content-type"" content=""text/html;charset=UTF-8"" /> 
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" /> 
    <script type=""text/javascript"">
        var map;
        var pushpin;
        var iconRoot = '");
            sb.Append(iconRoot.ToString());
            sb.Append(@"/';
        function createPP(lat, lon, code, icon, desc) {
            pushpin = new Microsoft.Maps.Pushpin(new Microsoft.Maps.Location(lat, lon), { icon: iconRoot + icon, width: 16, height: 16, typeName: 'pin', id: code });
            Microsoft.Maps.Events.addHandler(pushpin, 'click', function() { window.external.PushpinOnClick(code) });
            map.entities.push(pushpin);
            var ppel = document.getElementById(code);
            if (ppel != null) ppel.title = desc;
        }
        function createMap() {");
            this.CreateMapScript(sb, data);
            sb.AppendLine(@"
            //var tileSource = new Microsoft.Maps.TileSource({ 
            //    uriConstructor: function(tile) { return 'http://tile.openstreetmap.org/' + tile.levelOfDetail + '/' + tile.x + '/' + tile.y + '.png'; }
            //});
            //var tileLayer = new Microsoft.Maps.TileLayer({ mercator: tileSource, opacity: 1 });
            //map.entities.push(tileLayer);
        }
    </script>
    <style type=""text/css"">
        body, html { overflow: hidden }
        .pin { cursor: pointer; }
    </style>
</head>
<body onload=""createMap();""></body>
</html>");
            return sb.ToString();
        }

        private void CreateMapUpdateScript(IEnumerable<System.Xml.Linq.XDocument> data)
        {
            var sb = new StringBuilder();
            sb.AppendLine("map.entities.clear();");

            decimal minLat = decimal.MaxValue;
            decimal minLon = decimal.MaxValue;
            decimal maxLat = decimal.MinValue;
            decimal maxLon = decimal.MinValue;

            foreach (var wpt in data.SelectMany(o => o.Root.WaypointElements("wpt")))
            {
                if (wpt.CacheElement("cache") != null)
                {
                    if (string.Equals(wpt.GetAttributeValue(XmlExtensions.GeoTransformerSchema + "EditorOnly"), bool.TrueString, StringComparison.OrdinalIgnoreCase))
                        continue;

                    this.CreatePushpin(sb, wpt, ref minLat, ref minLon, ref maxLat, ref maxLon);
                }
            }

            if (!this._setBoundsCalled && minLat != decimal.MaxValue)
            {
                this._setBoundsCalled = true;
                sb.AppendLine("map.setView({ bounds: Microsoft.Maps.LocationRect.fromLocations(new Microsoft.Maps.Location(" + minLat.ToString(CultureInfo.InvariantCulture) + ", " + minLon.ToString(CultureInfo.InvariantCulture) + "), new Microsoft.Maps.Location(" + maxLat.ToString(CultureInfo.InvariantCulture) + ", " + maxLon.ToString(CultureInfo.InvariantCulture) + "))});");
            }

            var script = this._browser.Document.CreateElement("script");
            script.SetAttribute("type", "text/javascript");
            script.SetAttribute("text", sb.ToString());
            this._browser.Document.Body.InsertAdjacentElement(System.Windows.Forms.HtmlElementInsertionOrientation.BeforeEnd, script);
        }

        private void CreateMapScript(StringBuilder sb, IEnumerable<System.Xml.Linq.XDocument> data)
        {
            sb.AppendLine(@"document.body.style.height=""" + this._browser.ClientSize.Height + @"px"";");
            sb.AppendLine(@"map = new Microsoft.Maps.Map(document.body, {credentials:""" + BingApiKeys.MapsKey + @""", enableSearchLogo: false, enableClickableLogo: false});");

            decimal minLat = decimal.MaxValue;
            decimal minLon = decimal.MaxValue;
            decimal maxLat = decimal.MinValue;
            decimal maxLon = decimal.MinValue;

            foreach (var wpt in data.SelectMany(o => o.Root.WaypointElements("wpt")))
            {
                if (wpt.CacheElement("cache") != null)
                {
                    if (string.Equals(wpt.GetAttributeValue(XmlExtensions.GeoTransformerSchema + "EditorOnly"), bool.TrueString, StringComparison.OrdinalIgnoreCase))
                        continue;

                    this.CreatePushpin(sb, wpt, ref minLat, ref minLon, ref maxLat, ref maxLon);
                }
            }

            if (minLat != decimal.MaxValue)
            {
                this._setBoundsCalled = true;
                sb.AppendLine("map.setView({ bounds: Microsoft.Maps.LocationRect.fromLocations(new Microsoft.Maps.Location(" + minLat.ToString(CultureInfo.InvariantCulture) + ", " + minLon.ToString(CultureInfo.InvariantCulture) + "), new Microsoft.Maps.Location(" + maxLat.ToString(CultureInfo.InvariantCulture) + ", " + maxLon.ToString(CultureInfo.InvariantCulture) + "))});");
            }
        }

        private void CreatePushpin(StringBuilder sb, System.Xml.Linq.XElement waypoint, ref decimal minLat, ref decimal minLon, ref decimal maxLat, ref decimal maxLon)
        {
            var lat = waypoint.Attribute("lat").GetValue();
            var lon = waypoint.Attribute("lon").GetValue();
            var name = waypoint.WaypointElement("name").GetValue();

            if (string.IsNullOrWhiteSpace(lat) || string.IsNullOrWhiteSpace(lon) || string.IsNullOrWhiteSpace(name))
                return;

            decimal d;
            if (decimal.TryParse(lat, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d))
            {
                if (d < minLat) minLat = d;
                if (d > maxLat) maxLat = d;
            }
            if (decimal.TryParse(lon, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d))
            {
                if (d < minLon) minLon = d;
                if (d > maxLon) maxLon = d;
            }

            this._pushpinCache[name] = waypoint;

            var icon = this.CreateCacheIconUri(waypoint);

            var desc = waypoint.WaypointElement("desc").GetValue();

            sb.AppendLine("createPP("+lat+", "+lon+", '"+ name.Replace("'", @"\'") +"', '" + icon.Segments[icon.Segments.Length-1] +"', '" + desc.Replace("'", @"\'") + "')");
        }

        public event EventHandler<SelectedCacheChangedEventArgs> SelectedCacheChanged;

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
