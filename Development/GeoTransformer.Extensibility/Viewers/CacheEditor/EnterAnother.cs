/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoTransformer.Viewers.CacheEditor
{
    public partial class EnterAnother : Form
    {
        private Dictionary<string, string> _cacheNameCache;

        public EnterAnother()
        {
            InitializeComponent();

            this.textCacheCode.SelectAll();
            this.textCacheCode.Focus();

            this._cacheNameCache = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
            var caches = Extensions.ExtensionLoader.ApplicationService.RetrieveDisplayedCaches();
            if (caches != null)
            {
                foreach (var c in caches.SelectMany(o => o.Root.Elements(XmlExtensions.GpxSchema_1_1 + "wpt"))
                                        .Select(o => new { Code = o.Element(XmlExtensions.GpxSchema_1_1 + "name").GetValue(), Name = o.Element(XmlExtensions.GpxSchema_1_1 + "desc").GetValue() }))
                {
                    if (!this._cacheNameCache.ContainsKey(c.Code))
                        this._cacheNameCache.Add(c.Code, c.Name);
                }
            }                                
        }

        public string EnteredCacheCode
        {
            get
            {
                return this.textCacheCode.Text.Trim();
            }
        }

        private void textCacheCode_Enter(object sender, EventArgs e)
        {
            this.textCacheCode.SelectAll();
        }

        private void textCacheCode_TextChanged(object sender, EventArgs e)
        {
            string t;
            if (this._cacheNameCache.TryGetValue(this.EnteredCacheCode, out t))
                this.lblCacheTitle.Text = t;
            else
            {
                this.lblCacheTitle.Text = "The entered geocache code does not correspond to any currently loaded cache.";
                if (this.EnteredCacheCode.StartsWith("GC", StringComparison.OrdinalIgnoreCase))
                {
                    if (GeocachingService.LiveClient.IsEnabled)
                        this.lblCacheTitle.Text += " The cache data will be retrieved using Live API.";
                    else
                        this.lblCacheTitle.Text += " Live API is disabled - an empty placeholder will be created.";
                }
                else
                {
                    this.lblCacheTitle.Text += " An empty placeholder will be created.";
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.EnteredCacheCode))
            {
                MessageBox.Show("You need to enter the GC code of the cache that you want to modify.", "Missing data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (this.EnteredCacheCode.Length < 5 || this.EnteredCacheCode.Length > 7)
            {
                var res = MessageBox.Show("The entered code is not between 5 and 7 symbols long. Is it really correct?", "Geocache code", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res != System.Windows.Forms.DialogResult.Yes) 
                    return;
            }

            if (!this.EnteredCacheCode.StartsWith("GC", StringComparison.OrdinalIgnoreCase))
            {
                var res = MessageBox.Show("The entered code does not start with 'GC'. Is it really correct?", "Geocache code", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res != System.Windows.Forms.DialogResult.Yes)
                    return;
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
