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
using GeoTransformer;

namespace FieldNoteManager
{
    /// <summary>
    /// The user interface control for the extension.
    /// </summary>
    internal partial class Window : UserControl
    {
        public Window()
        {
            InitializeComponent();
            this.dataGrid.AutoGenerateColumns = false;
        }

        private void ImportFile(string fileName)
        {
            try
            {
                this.toolStripStatusLabel.Text = "Importing " + fileName;
                this.dataGrid.Invoke(a => { a.Enabled = false; this.colResult.Visible = false; });
                List<FieldNote> parsed = new List<FieldNote>();

                var notes = System.IO.File.ReadAllLines(fileName);

                // if the file is in Unicode, the standard read might not detect that and read it as ASCII with \0 chars in the middle
                if (notes.Length > 0 && notes[0].IndexOf('\0') > -1)
                    notes = System.IO.File.ReadAllLines(fileName, Encoding.Unicode);

                foreach (var n in notes)
                {
                    if (string.IsNullOrEmpty(n))
                        continue;

                    var parts = n.Split(new char[] { ',' }, 4);
                    if (parts.Length != 4)
                        throw new InvalidOperationException("The line '" + n + "' is not a valid Garmin geocache_visits.txt file entry.");


                    parsed.Add(new FieldNote()
                    {
                        CacheCode = parts[0],
                        LogTime = DateTime.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal).ToLocalTime(),
                        LogType = parts[2],
                        Text = parts[3].Trim('"')
                    });
                }

                HashSet<string> duplicateCache = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (var service = GeoTransformer.GeocachingService.LiveClient.CreateClientProxy())
                {
                    for (int i = 0; i < parsed.Count; i += 50)
                    {
                        this.toolStripStatusLabel.Text = "Loading geocache data: " + (i * 100 / parsed.Count) + "%";
                        var subset = parsed.Skip(i).Take(50).ToList();
                        var req = new GeoTransformer.GeocachingService.SearchForGeocachesRequest();
                        req.AccessToken = service.AccessToken;
                        req.CacheCode = new GeoTransformer.GeocachingService.CacheCodeFilter() { CacheCodes = subset.Select(o => o.CacheCode).ToArray() };
                        req.MaxPerPage = subset.Count;
                        req.IsLite = true;
                        var res = service.SearchForGeocaches(req);

                        foreach (var n in subset)
                        {
                            if (res.Status.StatusCode != 0)
                            {
                                n.ErrorMessage = "Unable to retrieve geocache: " + res.Status.StatusMessage;
                                continue;
                            }

                            var cache = res.Geocaches.FirstOrDefault(o => string.Equals(o.Code, n.CacheCode, StringComparison.OrdinalIgnoreCase));
                            if (cache == null)
                            {
                                n.ErrorMessage = "The geocache does not exist on geocaching.com";
                                continue;
                            }

                            n.CacheTitle = cache.Name;
                            if (cache.HasbeenFoundbyUser.GetValueOrDefault())
                            {
                                n.ErrorMessage = "You have already logged this cache";
                            }
                            else if (duplicateCache.Contains(n.CacheCode + ";" + n.LogType))
                            {
                                n.ErrorMessage = "This entry is duplicate";
                            }
                            else
                            {
                                duplicateCache.Add(n.CacheCode + ";" + n.LogType);
                            }
                        }
                    }
                }

                this.dataGrid.Invoke((a, b) => { a.DataSource = b; a.AutoResizeColumns(); }, parsed);

                foreach (DataGridViewRow row in this.dataGrid.Rows)
                {
                    var fn = (FieldNote)row.DataBoundItem;
                    if (fn.ErrorMessage != null)
                    {
                        row.ErrorText = fn.ErrorMessage;
                        row.ReadOnly = true;
                    }
                }

                this.toolStripStatusLabel.Text = "Imported " + fileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to parse the file." + Environment.NewLine + Environment.NewLine + ex.Message);
                this.toolStripStatusLabel.Text = "Unable to import " + fileName;
                return;
            }
            finally
            {
                this.dataGrid.Invoke(a => a.Enabled = true);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (!GeoTransformer.GeocachingService.LiveClient.IsEnabled)
            {
                MessageBox.Show("Live API must be enabled for this extension to work.");
                return;
            }

            var dlg = new OpenFileDialog();
            dlg.Filter = "geocache_visits.txt|geocache_visits.txt";
            dlg.Title = "Locate geocache_visits.txt file";
            var result = dlg.ShowDialog(this);
            if (result != DialogResult.OK)
                return;

            var thread = new System.Threading.Thread((object a) => ImportFile((string)a));
            thread.Name = "Field note importer";
            thread.Start(dlg.FileName);
        }

        private void PublishLogs(IEnumerable<FieldNote> notes)
        {
            this.dataGrid.Invoke(a => { this.colResult.Visible = true; });
            this.toolStripStatusLabel.Text = "Publishing logs...";

            try
            {
                using (var service = GeoTransformer.GeocachingService.LiveClient.CreateClientProxy())
                {
                    var logTypes = service.GetWptLogTypes(service.AccessToken);
                    var profile = service.GetYourUserProfileCached();

                    foreach (var n in notes)
                    {
                        try
                        {
                            var logType = logTypes.WptLogTypes.FirstOrDefault(o => string.Equals(n.LogType, o.WptLogTypeName, StringComparison.OrdinalIgnoreCase));
                            if (logType == null)
                            {
                                n.Result = "Error: cannot find the appropriate log type on the server.";
                                continue;
                            }

                            this.toolStripStatusLabel.Text = "Publishing log for " + n.CacheTitle;
                            var req = new GeoTransformer.GeocachingService.CreateFieldNoteAndPublishRequest();
                            req.AccessToken = service.AccessToken;
                            req.CacheCode = n.CacheCode;
                            req.EncryptLogText = false;
                            req.FavoriteThisCache = false;
                            req.Note = n.Text;
                            req.PromoteToLog = true;
                            var visit = n.LogTime.ToLocalTime().Date;
                            req.UTCDateLogged = new DateTime(visit.Year, visit.Month, visit.Day, 12, 0, 0, DateTimeKind.Utc);
                            req.WptLogTypeId = logType.WptLogTypeId;

                            var result = service.CreateFieldNoteAndPublish(req);
                            if (result.Status.StatusCode != 0)
                                n.Result = "Error: " + result.Status.StatusMessage;
                            else
                            {
                                n.Result = "Done. Created log " + result.Log.Code;
                                n.ShouldPublish = false;
                                
                            }
                        }
                        catch (Exception ex)
                        {
                            n.Result = "Error: " + ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to publish logs." + Environment.NewLine + Environment.NewLine + ex.Message);
            }

            this.toolStripStatusLabel.Text = "Publish completed. Load a new file to continue.";
            this.dataGrid.Invoke(a => {
                a.Rows.OfType<DataGridViewRow>()
                    .Where(o => (((FieldNote)o.DataBoundItem).Result ?? string.Empty).StartsWith("Done.", StringComparison.OrdinalIgnoreCase))
                    .ToList()
                    .ForEach(o => o.ReadOnly = true);
                a.AutoResizeColumns();
            });
        }

        private void btnPublish_Click(object sender, EventArgs e)
        {
            var notes = (IEnumerable<FieldNote>)this.dataGrid.DataSource;
            notes = notes.Where(o => o.ShouldPublish);
            var cnt = notes.Count();
            if (cnt == 0)
            {
                MessageBox.Show("No field notes selected for publishing.");
                return;
            }

            var res = MessageBox.Show("Do you want to publish " + cnt + " log(-s)?", "Field notes", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (res != DialogResult.OK)
                return;

            var thread = new System.Threading.Thread((object a) => this.PublishLogs((IEnumerable<FieldNote>)a));
            thread.Name = "Field note publish";
            thread.Start(notes);
        }

        private void dataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != this.colLogText.DisplayIndex || e.RowIndex == -1)
                return;

            this.dataGrid[this.colPublish.DisplayIndex, e.RowIndex].Value = true;
        }

        private void dataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != this.colCacheCode.DisplayIndex || e.RowIndex == -1)
                return;

            var code = ((FieldNote)this.dataGrid.Rows[e.RowIndex].DataBoundItem).CacheCode;
            System.Diagnostics.Process.Start("http://coord.info/" + code);
        }
    }
}
