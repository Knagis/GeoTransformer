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
    public class CacheWebPage : Extensions.ICacheViewer
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

        public void DisplayCache(System.Xml.Linq.XElement data)
        {
            var url = data.WaypointElement("url").GetValue();
            if (string.IsNullOrWhiteSpace(url))
            {
                var name = data.WaypointElement("name").GetValue();
                if (name != null && name.StartsWith("GC", StringComparison.OrdinalIgnoreCase))
                    url = "http://coord.info/" + data.WaypointElement("name").GetValue();
            }

            this._browser.Navigate(url);
        }

        public bool IsEnabled(System.Xml.Linq.XElement data)
        {
            if (data == null)
                return false;

            var url = data.WaypointElement("url").GetValue();
            if (!string.IsNullOrWhiteSpace(url))
                return true;

            var name = data.WaypointElement("name").GetValue();
            if (name != null && name.StartsWith("GC", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}
