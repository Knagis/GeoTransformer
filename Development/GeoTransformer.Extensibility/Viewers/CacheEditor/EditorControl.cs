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
        private System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint> _elements;
        private CacheEditor _editor;

        public EditorControl(CacheEditor editor)
        {
            this._editor = editor;

            InitializeComponent();

            this.toolStripButtonEditAnother.Image = global::GeoTransformer.Viewers.CacheEditor.Resources.Add;
            this.toolStripButtonRemove.Image = global::GeoTransformer.Viewers.CacheEditor.Resources.Remove;
        }

        public void BindElements(System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint> waypoints)
        {
            var single = (waypoints == null || waypoints.Count != 1) ? null : waypoints[0];

            this._elements = waypoints;

            if (waypoints == null || waypoints.Count == 0)
            {
                this.toolStripButtonRemove.Enabled = false;
                this.SetTitle(null);
            }
            else if (single == null)
            {
                this.toolStripButtonRemove.Enabled = true;
                this.SetTitle(waypoints.Count + " geocache" + ((waypoints.Count % 10 == 1 && waypoints.Count % 100 != 11) ? string.Empty : "s") + " selected");
            }
            else
            {
                this.toolStripButtonRemove.Enabled = true;
                this.SetTitle(single.Description ?? single.Name);
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
            string msg;

            if (this._elements.Count == 1)
                msg = "Do you really want to remove all of the custom data for this cache? This cannot be undone!";
            else
                msg = "Do you really want to remove all of the custom data for " + this._elements.Count + " geocache" + ((this._elements.Count % 10 == 1 && this._elements.Count % 100 != 11) ? string.Empty : "s") + "? This cannot be undone!";
            
            var res = MessageBox.Show(msg, "Remove customizations", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (res != DialogResult.Yes)
                return;

            var remaining = new List<Gpx.GpxWaypoint>();

            foreach (var el in this._elements)
            {
                var extelems = el.ExtensionElements;
                for (int i = extelems.Count - 1; i >= 0; i--)
                    if (extelems[i].Name.Namespace == XmlExtensions.GeoTransformerSchema)
                        extelems.RemoveAt(i);

                if (string.Equals(el.FindExtensionAttributeValue("EditorOnly"), bool.TrueString, StringComparison.OrdinalIgnoreCase))
                {
                    // find the document that contains the waypoint and remove the waypoint from it
                    foreach (var doc in Extensions.ExtensionLoader.ApplicationService.RetrieveDisplayedCaches())
                        if (doc.Waypoints.Remove(el))
                            break;
                }
                else
                {
                    remaining.Add(el);
                }
            }

            // reset the selection as some waypoints may no longer exist
            this.BindElements(remaining.AsReadOnly());

            Extensions.ExtensionLoader.ApplicationService.SelectWaypoint(remaining.Count == 1 ? remaining[0].Name : null);
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
