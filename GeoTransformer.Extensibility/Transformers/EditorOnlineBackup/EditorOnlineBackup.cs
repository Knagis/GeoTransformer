/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoTransformer.Extensions;

namespace GeoTransformer.Transformers.EditorOnlineBackup
{
    /// <summary>
    /// Extension that saves the edited values online to <c>geocaching.com</c> as personal notes.
    /// </summary>
    public class EditorOnlineBackup : ISaveData, IConfigurable
    {
        /// <summary>
        /// A collection of delegates that retrieve the values to be backed up from various extensions.
        /// </summary>
        private static IList<Func<Gpx.GpxWaypoint, string>> _backupFuncs = new List<Func<Gpx.GpxWaypoint, string>>();

        /// <summary>
        /// Registers a delegate that retrieves a value from the waypoint to be backed up.
        /// </summary>
        /// <param name="func">The delegate that returns the value. Has to return <c>null</c> if there is nothing to be backed up.</param>
        public static void RegisterForBackup(Func<Gpx.GpxWaypoint, string> func)
        {
            if (func == null)
                throw new ArgumentNullException("func");

            _backupFuncs.Add(func);
        }

        /// <summary>
        /// The configuration control for this extension.
        /// </summary>
        private SimpleConfigurationControl _Configuration;

        /// <summary>
        /// Initializes the extension with the specified current configuration (can be <c>null</c> if the extension is initialized for the very first time) and
        /// returns the configuration UI control (can return <c>null</c> if the user interface is not needed).
        /// </summary>
        /// <param name="currentConfiguration">The current configuration.</param>
        /// <returns>The configuration UI control.</returns>
        public System.Windows.Forms.Control Initialize(byte[] currentConfiguration)
        {
            this._Configuration = new SimpleConfigurationControl("Backup data on geocaching.com", 
@"Enable GeoTransformer to backup up updated coordinates and
other information in the geocache personal note on geocaching.com.

No other user will be able to see it but you will have access
to the information even without using GeoTransformer.

If you have already entered personal note on the web site it
will not be overwritten, the information will be appended
after it.

This extension requires Live API to be enabled.");

            if (currentConfiguration != null && currentConfiguration.Length > 0)
                this._Configuration.IsChecked = currentConfiguration[0] == (byte)1;

            return this._Configuration;
        }

        /// <summary>
        /// Retrieves the configuration from the extension's configuration UI control.
        /// </summary>
        /// <returns>The serialized configuration data.</returns>
        public byte[] SerializeConfiguration()
        {
            return new [] { this._Configuration.IsChecked ? (byte)1 : (byte)0};
        }

        /// <summary>
        /// Gets a value indicating whether the this extension should be executed.
        /// </summary>
        /// <value><c>true</c> if this extension is enabled; otherwise, <c>false</c>.</value>
        public bool IsEnabled
        {
            get { return this._Configuration.IsChecked; }
        }

        /// <summary>
        /// Gets the category of the extension.
        /// </summary>
        /// <value>The category.</value>
        /// <exception cref="System.NotImplementedException"></exception>
        public Category Category
        {
            get { return Category.Miscellaneous; }
        }

        /// <summary>
        /// Saves the data that the extension needs in the specified GPX documents.
        /// </summary>
        /// <param name="documents">The GPX documents that are currently loaded.</param>
        public void Save(IEnumerable<Gpx.GpxDocument> documents)
        {
            if (!GeocachingService.LiveClient.IsEnabled)
                return;

#if DEBUG
            if (System.Windows.Forms.MessageBox.Show("Do you want to backup edited data online?", "GeoTransformer running in DEBUG", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question, System.Windows.Forms.MessageBoxDefaultButton.Button2) != System.Windows.Forms.DialogResult.Yes)
                return;
#endif

            var table = Extensions.ExtensionLoader.RetrieveExtensions<CacheTable>().Single();
            var q = table.Select();
            q.SelectAll();
            var cache = q.Execute().AsEnumerable().ToDictionary(o => o.Value(t => t.CacheCode), o => o.Value(t => t.Data), StringComparer.OrdinalIgnoreCase);
            

            var sb = new StringBuilder();
            using (var service = GeocachingService.LiveClient.CreateClientProxy())
            using (var scope = table.Database().BeginTransaction())
            {
                // STEP 1 - retrieve all notes that should be saved online (filters out all notes that are already stored).
                var newNotes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var wpt in documents.SelectMany(o => o.Waypoints))
                {
                    var code = wpt.Name.ToUpperInvariant();
                    if (!code.StartsWith("GC", StringComparison.OrdinalIgnoreCase) || !wpt.Geocache.IsDefined())
                        continue;

                    sb.Clear();
                    foreach (var f in _backupFuncs)
                    {
                        var t = f(wpt);
                        if (string.IsNullOrWhiteSpace(t))
                            continue;

                        if (sb.Length != 0)
                            sb.Append("; ");

                        sb.Append(t);
                    }

                    if (sb.Length == 0)
                        continue;

                    var note = "[GT]" + sb.ToString() + "[/GT]";
                    string curr;
                    if (cache.TryGetValue(code, out curr) && string.Equals(curr, note, StringComparison.Ordinal))
                        continue;

                    var qr = table.Replace();
                    qr.Value(o => o.CacheCode, code);
                    qr.Value(o => o.Data, note);
                    qr.Execute();

                    newNotes.Add(code, note);
                }

                // if there are no new notes, no need to start calling the service
                if (newNotes.Count == 0)
                    return;

                // STEP 2 - retrieve all existing notes from geocaching.com. This is needed so that we can append the note
                // instead of overwriting.
                var existingNotes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                while (true)
                {
                    var result = service.GetUsersCacheNotes(service.AccessToken, existingNotes.Count, 100);
                    if (result.Status.StatusCode == 140)
                    {
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
                        continue;
                    }
                    else if (result.Status.StatusCode != 0)
                    {
                        System.Windows.Forms.MessageBox.Show("Error while backing up the data using Live API: " + Environment.NewLine + result.Status.StatusMessage, "Online backup", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                        return;
                    }

                    foreach (var n in result.CacheNotes)
                        existingNotes.Add(n.CacheCode, n.Note);

                    if (result.CacheNotes.Length < 100)
                        break;
                }

                // STEP 3 - update the notes online.
                var removeOld = new System.Text.RegularExpressions.Regex(@"\s*\[GT\].+\[\/GT\]", System.Text.RegularExpressions.RegexOptions.Singleline);
                foreach (var note in newNotes)
                {
                    string text;
                    if (!existingNotes.TryGetValue(note.Key, out text))
                    {
                        text = note.Value;
                    }
                    else
                    {
                        text = removeOld.Replace(text, string.Empty);
                        text = text + " " + note.Value;
                    }

                    var req = new GeocachingService.UpdateCacheNoteRequest();
                    req.AccessToken = service.AccessToken;
                    req.CacheCode = note.Key;
                    req.Note = text;

                    while (true)
                    {
                        var result = service.UpdateCacheNote(req);

                        if (result.StatusCode == 140)
                        {
                            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
                            continue;
                        }
                        else if (result.StatusCode != 0)
                        {
                            System.Windows.Forms.MessageBox.Show("Error while backing up the data using Live API: " + Environment.NewLine + result.StatusMessage, "Online backup", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                            return;
                        }

                        break;
                    }
                }

                scope.Commit();
            }
        }
    }
}
