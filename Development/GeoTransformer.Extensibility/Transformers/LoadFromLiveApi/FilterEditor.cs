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

namespace GeoTransformer.Transformers.LoadFromLiveApi
{
    public partial class FilterEditor : Form
    {
        private Query _query;
        private string _previousPlaceName;
        private Coordinates.Wgs84Point? _previousCoordinates;

        /// <summary>
        /// Gets the extension instance.
        /// </summary>
        private LoadFromLiveApi Extension
        {
            get
            {
                return Extensions.ExtensionLoader.RetrieveExtensions<LoadFromLiveApi>().First();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterEditor"/> class.
        /// </summary>
        /// <param name="value">The <see cref="Query"/> that will be edited in this form.</param>
        public FilterEditor(Query value = null)
        {
            InitializeComponent();

            this.txtCenterName.SetWatermark("Enter name of a place");

            if (value == null)
            {
                this._query = new Query();
                this.drpMinDiff.SelectedIndex = 0;
                this.drpMaxDiff.SelectedIndex = this.drpMaxDiff.Items.Count - 1;
                this.drpMinTerrain.SelectedIndex = 0;
                this.drpMaxTerrain.SelectedIndex = this.drpMaxTerrain.Items.Count - 1;
                this.drpFoundByMe.SelectedIndex = 0;
                this.drpHiddenByMe.SelectedIndex = 0;
            }
            else
            {
                this._query = value;
                this.txtCenterCoords.Coordinates = value.CenterCoordinates;
                this.drpMinDiff.SelectedItem = value.Difficulty.Item1.ToString(System.Globalization.CultureInfo.InvariantCulture);
                this.drpMaxDiff.SelectedItem = value.Difficulty.Item2.ToString(System.Globalization.CultureInfo.InvariantCulture);
                this.chkIncludeDisabled.Checked = value.DownloadDisabled;
                this.drpFoundByMe.SelectedIndex = (int)value.FoundByMe;
                this.drpHiddenByMe.SelectedIndex = (int)value.HiddenByMe;
                this.txtMaxCaches.Value = value.MaximumCaches;
                this.txtRadius.Value = value.MaximumRadius;
                this.txtMinFavPoints.Value = value.MinimumFavoritePoints;
                this.drpMinTerrain.SelectedItem = value.Terrain.Item1.ToString(System.Globalization.CultureInfo.InvariantCulture);
                this.drpMaxTerrain.SelectedItem = value.Terrain.Item2.ToString(System.Globalization.CultureInfo.InvariantCulture);
                this.txtQueryTitle.Text = value.Title;
                this.txtCenterName.Text = value.CenterName;
            }

            this._previousPlaceName = this.txtCenterName.Text;
            this._previousCoordinates = this.txtCenterCoords.Coordinates;

            using (var service = GeocachingService.LiveClient.CreateClientProxy())
            {
                if (service.IsBasicMember() ?? false)
                {
                    // these filters are only available for premium members
                    this.drpMinDiff.Enabled = false;
                    this.drpMaxDiff.Enabled = false;
                    this.drpMinDiff.SelectedIndex = 0;
                    this.drpMaxDiff.SelectedIndex = this.drpMaxDiff.Items.Count - 1;

                    this.drpMaxTerrain.Enabled = false;
                    this.drpMinTerrain.Enabled = false;
                    this.drpMinTerrain.SelectedIndex = 0;
                    this.drpMaxTerrain.SelectedIndex = this.drpMaxTerrain.Items.Count - 1;

                    this.txtMinFavPoints.Enabled = false;
                    this.txtMinFavPoints.Value = 0;
                }
                else
                {
                    this.Height -= this.labelBasicMembers.Height + this.labelBasicMembers.Margin.Top;
                    this.labelBasicMembers.Visible = false;
                }
            }
        }

        /// <summary>
        /// Gets the query that is edited by this form. The value is only updated when the form is closed.
        /// </summary>
        public Query Query
        {
            get
            {
                return this._query;
            }
        }

        private void toolStripButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            var originalQuery = this._query.Description;

            this._query.CenterCoordinates = this.txtCenterCoords.Coordinates;
            this._query.Difficulty = Tuple.Create(double.Parse((string)this.drpMinDiff.SelectedItem, System.Globalization.CultureInfo.InvariantCulture),
                                                  double.Parse((string)this.drpMaxDiff.SelectedItem, System.Globalization.CultureInfo.InvariantCulture));
            this._query.DownloadDisabled = this.chkIncludeDisabled.Checked;
            this._query.FoundByMe = (FoundByMeFilter)this.drpFoundByMe.SelectedIndex;
            this._query.HiddenByMe = (HiddenByMeFilter)this.drpHiddenByMe.SelectedIndex;
            this._query.MaximumCaches = (int)this.txtMaxCaches.Value;
            this._query.MaximumRadius = (int)this.txtRadius.Value;
            this._query.MinimumFavoritePoints = (int)this.txtMinFavPoints.Value;
            this._query.Terrain = Tuple.Create(double.Parse((string)this.drpMinTerrain.SelectedItem, System.Globalization.CultureInfo.InvariantCulture),
                                               double.Parse((string)this.drpMaxTerrain.SelectedItem, System.Globalization.CultureInfo.InvariantCulture));
            this._query.Title = this.txtQueryTitle.Text;
            this._query.CenterName = this.txtCenterName.Text;

            var modifiedQuery = this._query.Description;
            if (!string.Equals(originalQuery, modifiedQuery, StringComparison.OrdinalIgnoreCase))
            {
                var files = System.IO.Directory.EnumerateFiles(this.Extension.LocalStoragePath, this._query.Id.ToString() + "*.*", System.IO.SearchOption.TopDirectoryOnly);
                if (files.Any())
                {
                    var shouldDelete = MessageBox.Show("You have modified the query. Do you want to delete the cached copies of the results?", "Query modified", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    if (shouldDelete == System.Windows.Forms.DialogResult.Yes)
                    {
                        foreach (var f in files.ToList())
                        {
                            try
                            {
                                System.IO.File.Delete(f);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Unable to delete '" + f + "'." + Environment.NewLine + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void txtCenterCoords_Leave(object sender, EventArgs e)
        {
            if (Coordinates.Wgs84Point.ApproximateEquals(this._previousCoordinates, this.txtCenterCoords.Coordinates))
                return;

            this._previousCoordinates = this.txtCenterCoords.Coordinates;

            if (!this._previousCoordinates.HasValue)
            {
                this.txtCenterName.SetWatermark("Enter name of a place");
                this.txtCenterName.Text = null;
                return;
            }

            this.txtCenterName.Text = null;
            this.txtCenterName.SetWatermark("searching...");
            this.txtCenterName.ReadOnly = true;
            System.Threading.ThreadPool.QueueUserWorkItem(_ => 
            {
                this.txtCenterName.Invoke((t, p) =>
                {
                    t.Text = p ?? "Unknown location";
                    this._previousPlaceName = t.Text;
                    t.SetWatermark("Enter name of a place");
                    t.ReadOnly = false;
                }, Coordinates.GeoNames.FindNearbyPlaceName(this._previousCoordinates.Value));
            });
        }

        private void txtCenterName_Leave(object sender, EventArgs e)
        {
            if (string.Equals(this.txtCenterName.Text, this._previousPlaceName, StringComparison.Ordinal))
                return;

            this._previousPlaceName = this.txtCenterName.Text;

            if (string.IsNullOrWhiteSpace(this.txtCenterName.Text))
            {
                this.txtCenterCoords.WatermarkText = "or enter coordinates";
                this.txtCenterCoords.Coordinates = null;
                return;
            }

            this.txtCenterCoords.Coordinates = null;
            this.txtCenterCoords.WatermarkText = "searching...";
            this.txtCenterCoords.ReadOnly = true;
            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
            {
                string name;
                var c = Coordinates.GeoNames.Search(this.txtCenterName.Text, out name);

                this._previousCoordinates = c;

                this.txtCenterCoords.Invoke((t, coords) =>
                {
                    t.Coordinates = coords;
                    t.WatermarkText = "or enter coordinates";
                    t.ReadOnly = false;
                }, c);

                if (name != null)
                    this.txtCenterName.Invoke((t, p) =>
                    {
                        this._previousPlaceName = name;
                        t.Text = name;
                    }, name);
            });
        }

        private void FilterEditor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                this.toolStripButtonSave.PerformClick();
                e.Handled = true;
            }
            if (e.KeyChar == (char)27)
            {
                this.toolStripButtonCancel.PerformClick();
                e.Handled = true;
            }
        }

        private void labelBasicMembers_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                System.Diagnostics.Process.Start("http://www.geocaching.com/membership/comparison.aspx");
        }
    }
}
