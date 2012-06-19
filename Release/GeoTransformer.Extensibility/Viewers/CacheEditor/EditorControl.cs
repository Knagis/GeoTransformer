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
        private Gpx.GpxWaypoint _element;
        private CacheEditor _editor;

        public EditorControl(CacheEditor editor)
        {
            this._editor = editor;

            InitializeComponent();

            this.toolStripButtonEditAnother.Image = global::GeoTransformer.Viewers.CacheEditor.Resources.Add;
            this.toolStripButtonRemove.Image = global::GeoTransformer.Viewers.CacheEditor.Resources.Remove;
        }

        public void BindElement(Gpx.GpxWaypoint waypoint)
        {
            this._element = waypoint;

            if (waypoint == null)
            {
                this.toolStripButtonRemove.Enabled = false;
                this.SetTitle(null);
            }
            else
            {
                this.toolStripButtonRemove.Enabled = true;
                this.SetTitle(waypoint.Description ?? waypoint.Name);
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

            var extelems = this._element.ExtensionElements;
            for (int i = extelems.Count - 1; i >= 0; i--)
                if (extelems[i].Name.Namespace == XmlExtensions.GeoTransformerSchema)
                    extelems.RemoveAt(i);

            if (string.Equals(this._element.FindExtensionAttributeValue("EditorOnly"), bool.TrueString, StringComparison.OrdinalIgnoreCase))
            {
                // find the document that contains the waypoint and remove the waypoint from it
                foreach (var doc in Extensions.ExtensionLoader.ApplicationService.RetrieveDisplayedCaches())
                    if (doc.Waypoints.Remove(this._element))
                        break;

                // clear the selection as the waypoint no longer exists
                this.BindElement(null);
            }

            Extensions.ExtensionLoader.ApplicationService.SelectWaypoint(this._element == null ? null : this._element.Name);
        }

        private void toolStripButtonEditAnother_Click(object sender, EventArgs e)
        {
            using (var frm = new EnterAnother())
            {
                frm.ShowDialog(this);
                if (frm.DialogResult != DialogResult.OK)
                    return;

                Extensions.ExtensionLoader.ApplicationService.SelectWaypoint(frm.EnteredCacheCode, true);
            }

        }
    }
}
