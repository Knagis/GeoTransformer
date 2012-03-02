/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoTransformer.Viewers.CacheEditor
{
    internal partial class EditorControl : UserControl
    {
        private System.Xml.Linq.XElement _element;
        private CacheEditor _editor;

        public EditorControl(CacheEditor editor)
        {
            this._editor = editor;

            InitializeComponent();

            this.toolStripButtonEditAnother.Image = global::GeoTransformer.Viewers.CacheEditor.Resources.Add;
            this.toolStripButtonRemove.Image = global::GeoTransformer.Viewers.CacheEditor.Resources.Remove;
        }

        public void BindElement(System.Xml.Linq.XElement element)
        {
            this._element = element;

            if (element == null)
            {
                this.toolStripButtonRemove.Enabled = false;
                this.SetTitle(null);
            }
            else
            {
                this.toolStripButtonRemove.Enabled = true;
                this.SetTitle(element.WaypointElement("desc").GetValue() ?? element.WaypointElement("name").GetValue());
            }
        }

        public void SetTitle(string title)
        {
            if (title == null)
                title = "Select a cache";

            this.toolStripLabel.Text = title.Replace("&", "&&");
        }

        private void toolStripButtonRemove_Click(object sender, EventArgs e)
        {
            var res = MessageBox.Show("Do you really want to remove all of the custom data for this cache? This cannot be undone!", "Remove customizations", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (res != DialogResult.Yes)
                return;

            foreach (var elem in this._element.Elements().ToList())
                if (string.Equals(elem.Name.NamespaceName, XmlExtensions.GeoTransformerSchemaClean, StringComparison.Ordinal))
                    elem.Remove();

            if (!this._element.ContainsSignificantInformation() 
                || string.Equals(this._element.GetAttributeValue(XmlExtensions.GeoTransformerSchema + "EditorOnly"), bool.TrueString, StringComparison.OrdinalIgnoreCase))
                this._element.Remove();

            Extensions.ExtensionLoader.ApplicationService.RefreshCacheList();
            Extensions.ExtensionLoader.ApplicationService.SelectCache(this._element.WaypointElement("name").GetValue());
        }

        private void toolStripButtonEditAnother_Click(object sender, EventArgs e)
        {
            using (var frm = new EnterAnother())
            {
                frm.ShowDialog(this);
                if (frm.DialogResult != DialogResult.OK)
                    return;

                Extensions.ExtensionLoader.ApplicationService.SelectCache(frm.EnteredCacheCode, true);
            }

        }
    }
}
