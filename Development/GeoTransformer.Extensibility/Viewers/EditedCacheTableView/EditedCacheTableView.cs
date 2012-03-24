/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Viewers.EditedCacheTableView
{
    public class EditedCacheTableView : Extensions.ICacheListViewer
    {
        public System.Drawing.Image ButtonImage
        {
            get { return Resources.icon; }
        }

        public string ButtonText
        {
            get { return "Edited caches"; }
        }

        private BrightIdeasSoftware.ObjectListView _control;
        public System.Windows.Forms.Control Initialize()
        {
            this._control = new BrightIdeasSoftware.ObjectListView();
            ParsedEditedCacheData.InitializeListView(this._control);
            this._control.SetObjects(new ParsedEditedCacheData[0]);

            this._control.SelectionChanged += ListView_SelectedIndexChanged;

            return this._control;
        }

        void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = this._control.SelectedObject as ParsedEditedCacheData;
            if (this.SelectedCacheChanged != null)
                this.SelectedCacheChanged(this, new SelectedCacheChangedEventArgs(selected == null ? null : selected.SourceElement));
        }

        public void DisplayCaches(IList<System.Xml.Linq.XDocument> data, string cacheCode)
        {
            var editedCaches = data.SelectMany(o => o.Root.Elements(XmlExtensions.GpxSchema_1_1 + "wpt"))
                .Select(o => new ParsedEditedCacheData(o))
                .Where(o => o.IsGeocache && (o.ContainsExtensionElements || o.IsEditorOnly))
                .ToList();

            this._control.Invoke((control, ec) =>
                {
                    control.SetObjects(ec);

                    var i = ec.FindIndex(o => string.Equals(o.CacheCode, cacheCode, StringComparison.OrdinalIgnoreCase));
                    control.Invoke(o => o.SelectedIndex = i);
                    if (i > -1)
                        this._control.Invoke(o => o.SelectedItem.EnsureVisible());

                }, editedCaches);
        }

        public event EventHandler<SelectedCacheChangedEventArgs> SelectedCacheChanged;
    }
}
