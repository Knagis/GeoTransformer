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
    public class TableView : ICacheListViewer
    {
        public System.Drawing.Image ButtonImage
        {
            get { return Resources.Table; }
        }

        public string ButtonText
        {
            get { return "Show in table"; }
        }

        private BrightIdeasSoftware.ObjectListView _control;
        public System.Windows.Forms.Control Initialize()
        {
            this._control = new BrightIdeasSoftware.ObjectListView();
            ParsedCacheData.InitializeListView(this._control);
            this._control.SetObjects(new ParsedCacheData[0]);

            this._control.SelectionChanged += ListView_SelectedIndexChanged;

            return this._control;
        }

        void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = this._control.SelectedObject as ParsedCacheData;
            if (this.SelectedCacheChanged != null)
                this.SelectedCacheChanged(this, new SelectedCacheChangedEventArgs(selected == null ? null : selected.SourceElement));
        }

        public void DisplayCaches(IList<System.Xml.Linq.XDocument> data, string cacheCode)
        {
            var list = data.SelectMany(o => o.Root.Elements(XmlExtensions.GpxSchema_1_1 + "wpt"))
                .Select(o => new ParsedCacheData(o))
                .Where(o => o.IsGeocache && !o.IsEditorOnly)
                .ToList();

            this._control.Invoke(o => o.SetObjects(list));

            var i = list.FindIndex(o => string.Equals(o.CacheCode, cacheCode, StringComparison.OrdinalIgnoreCase));
            this._control.Invoke(o => o.SelectedIndex = i);
            if (i > -1)
                this._control.Invoke(o => o.SelectedItem.EnsureVisible());
        }

        public event EventHandler<SelectedCacheChangedEventArgs> SelectedCacheChanged;
    }
}
