/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoTransformer;

namespace PersonalNoteManager
{
    /// <summary>
    /// The user interface control for the extension.
    /// </summary>
    internal partial class Window : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        public Window()
        {
            InitializeComponent();
            this.dataGrid.AutoGenerateColumns = false;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (!GeoTransformer.GeocachingService.LiveClient.IsEnabled)
            {
                MessageBox.Show("Live API must be enabled for this extension to work.");
                return;
            }

            new System.Threading.Thread(this.LoadPersonalNotes).Start();
        }

        private void LoadPersonalNotes()
        {
            this.btnLoad.Enabled = false;
            this.toolStripStatusLabel.Text = "Started loading personal notes...";

            using (var service = GeoTransformer.GeocachingService.LiveClient.CreateClientProxy())
            {
                var col = new List<PersonalNote>();
                while (true)
                {
                    var result = service.GetUsersCacheNotes(service.AccessToken, col.Count, 100);
                    if (result.Status.StatusCode == 140)
                    {
                        this.toolStripStatusLabel.Text = "Too many requests per second. Waiting 5 seconds...";
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
                        continue;
                    }
                    else if (result.Status.StatusCode != 0)
                    {
                        System.Windows.Forms.MessageBox.Show("Error while downloading data using Live API: " + Environment.NewLine + result.Status.StatusMessage, "Personal notes", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                        return;
                    }

                    foreach (var n in result.CacheNotes)
                        col.Add(new PersonalNote() { CacheCode = n.CacheCode, NoteText = n.Note });

                    this.toolStripStatusLabel.Text =  col.Count.ToString(System.Globalization.CultureInfo.InvariantCulture) + " notes already loaded.";

                    if (result.CacheNotes.Length < 100)
                        break;
                }

                int i = 0;
                int step = 10;
                while (i < col.Count)
                {
                    this.toolStripStatusLabel.Text = string.Format("Loading cache titles ({0:P0})...", (decimal)i / col.Count);
                    var subset = col.Skip(i).Take(step).ToDictionary(o => o.CacheCode, StringComparer.OrdinalIgnoreCase);
                    var req = new GeoTransformer.GeocachingService.SearchForGeocachesRequest();
                    req.AccessToken = service.AccessToken;
                    req.IsLite = true;
                    req.MaxPerPage = step;
                    req.CacheCode = new GeoTransformer.GeocachingService.CacheCodeFilter() { CacheCodes = subset.Keys.ToArray() };
                    var result = service.SearchForGeocaches(req);
                    if (result.Status.StatusCode == 140)
                    {
                        this.toolStripStatusLabel.Text = "Too many requests per second. Waiting 5 seconds...";
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
                        continue;
                    }
                    else if (result.Status.StatusCode != 0)
                    {
                        System.Windows.Forms.MessageBox.Show("Error while downloading data using Live API: " + Environment.NewLine + result.Status.StatusMessage, "Personal notes", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                        return;
                    }

                    foreach (var r in result.Geocaches)
                    {
                        var n = subset[r.Code];
                        n.IsArchived = r.Archived.GetValueOrDefault();
                        n.IsFound = r.HasbeenFoundbyUser.GetValueOrDefault();
                        n.CacheTitle = r.Name;
                    }

                    i += step;
                }

                this.toolStripStatusLabel.Text = "Data loaded.";
                this.Invoke(o => o.dataGrid.DataSource = col);
                this.btnLoad.Enabled = true;
            }
        }

        private void dataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != this.colCacheCode.DisplayIndex || e.RowIndex == -1)
                return;

            var code = ((PersonalNote)this.dataGrid.Rows[e.RowIndex].DataBoundItem).CacheCode;
            System.Diagnostics.Process.Start("http://coord.info/" + code);
        }
    }
}
