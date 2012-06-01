/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Transformers.RefreshFromLiveApi
{
    /// <summary>
    /// Transformer that will automatically refresh geocache information from Live API or 
    /// merge information missing in GPX if the GPX file is newer.
    /// </summary>
    internal class RefreshFromLiveApi : TransformerBase, Extensions.IConfigurable, Extensions.ILocalStorage
    {
        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Add missing data from Live API"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.PreProcess - 50; }
        }

        /// <summary>
        /// The configuration control created by <see cref="Initialize"/> method.
        /// </summary>
        private SimpleConfigurationControl _configurationControl;

        /// <summary>
        /// Initializes the extension with the specified current configuration (can be <c>null</c> if the extension is initialized for the very first time) and
        /// returns the configuration UI control (can return <c>null</c> if the user interface is not needed).
        /// </summary>
        /// <param name="currentConfiguration">The current configuration.</param>
        /// <returns>
        /// The configuration UI control.
        /// </returns>
        public System.Windows.Forms.Control Initialize(byte[] currentConfiguration)
        {
            this._configurationControl = new SimpleConfigurationControl(this.Title,
@"Enabling this option will enable GeoTransformer to
automatically download geocache information using
Live API (if it is enabled) and extend data in the GPX 
files with additional information such as favorite 
points and images.

The data loaded from Live API is cached for 1 week 
since the most important data will be updated from 
the data source (such as GPX file or pocket query)
directly.");

            this._configurationControl.checkBoxEnabled.Checked = currentConfiguration == null || (currentConfiguration.Length > 0 && currentConfiguration[0] == 1);
            return this._configurationControl;
        }

        /// <summary>
        /// Retrieves the configuration from the extension's configuration UI control.
        /// </summary>
        /// <returns>
        /// The serialized configuration data.
        /// </returns>
        public byte[] SerializeConfiguration()
        {
            return new byte[] { this.IsEnabled ? (byte)1 : (byte)0 };
        }

        /// <summary>
        /// Gets a value indicating whether the this extension should be executed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this extension is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get { return this._configurationControl.checkBoxEnabled.Checked; }
        }

        /// <summary>
        /// Gets the category of the extension.
        /// </summary>
        public Extensions.Category Category
        {
            get { return Extensions.Category.GeocacheSources; }
        }

        /// <summary>
        /// Determines whether the waypoint is missing advanced data (that is available only in 1.0.2 schema and
        /// Live API). If the waypoint does not have geocache extensions, returns <c>false</c>.
        /// </summary>
        /// <param name="waypoint">The waypoint to verify.</param>
        /// <returns>
        ///   <c>True</c> if the waypoint is missing advanced data; otherwise, <c>false</c>.
        /// </returns>
        private static bool ShouldRefresh(Gpx.GpxWaypoint waypoint)
        {
            if (!waypoint.Name.StartsWith("GC", StringComparison.OrdinalIgnoreCase))
                return false;

            var gc = waypoint.Geocache;
            if (!gc.IsDefined())
                return false;
            if (gc.FavoritePoints == null)
                return true;
            if (gc.MemberOnly == null)
                return true;

            return false;
        }

        /// <summary>
        /// Retrieves the last update time of the given <paramref name="waypoint"/> assuming that it is located in the <paramref name="document"/>.
        /// </summary>
        /// <param name="document">The GPX document that contains the waypoint.</param>
        /// <param name="waypoint">The GPX waypoint.</param>
        /// <returns>Date and time when the waypoint was updated (usually when the GPX file was initially created). Returns <see cref="DateTime.MinValue"/> if the document does not have a creation time.</returns>
        private static DateTime ReadGpxTime(Gpx.GpxDocument document, Gpx.GpxWaypoint waypoint)
        {
            return waypoint.LastRefresh ?? document.Metadata.LastRefresh ?? DateTime.MinValue;
        }

        /// <summary>
        /// Merges the <paramref name="source"/> waypoint into <paramref name="target"/> waypoint.
        /// </summary>
        /// <param name="document">The document that contains <paramref name="target"/>.</param>
        /// <param name="target">The existing waypoint that will be updated.</param>
        /// <param name="source">The waypoint that contains missing or newer data.</param>
        private static void Merge(Gpx.GpxDocument document, Gpx.GpxWaypoint target, Gpx.GpxWaypoint source)
        {
            // source is always loaded from Live API so it will always have LastRefresh set.
            if (ReadGpxTime(document, target) < source.LastRefresh)
            {
                // the target is older version that source so it will be completely replaced
                var index = document.Waypoints.IndexOf(target);
                if (index == -1)
                    throw new InvalidOperationException("The waypoint is not present in the document.");

                document.Waypoints.RemoveAt(index);
                document.Waypoints.Insert(index, source);
            }
            else
            {
                Gpx.Loader.Merge(target, source);
            }
        }

        /// <summary>
        /// Serializes the <paramref name="cache"/> to a byte array.
        /// </summary>
        private static byte[] Serialize(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter serializer, GeocachingService.Geocache cache)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                serializer.Serialize(ms, cache);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Deserializes the specified cache from a byte array.
        /// </summary>
        private static GeocachingService.Geocache Deserialize(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter serializer, byte[] data)
        {
            using (var ms = new System.IO.MemoryStream(data))
                return (GeocachingService.Geocache)serializer.Deserialize(ms);
        }

        /// <summary>
        /// Sets the local storage path where the extension can store its cache if needed. The value is set by the main engine once the extension instance is created.
        /// </summary>
        public string LocalStoragePath { get; set; }

        /// <summary>
        /// Processes the specified GPX documents.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
            using (var db = new CachedDataSchema(System.IO.Path.Combine(this.LocalStoragePath, "cache.db")))
            {
                bool useLocalStorage = (options & TransformerOptions.UseLocalStorage) == TransformerOptions.UseLocalStorage;

                var waypointsToDownload = new Dictionary<string, Tuple<Gpx.GpxDocument, Gpx.GpxWaypoint>>(StringComparer.OrdinalIgnoreCase);
                var waypointsToUpdateIfDownloadFails = new Dictionary<string, Tuple<Gpx.GpxDocument, Gpx.GpxWaypoint, DateTime, byte[]>>(StringComparer.OrdinalIgnoreCase);

                var serializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                var cachedDataQ = db.Geocaches.Select();
                cachedDataQ.SelectAll();
                var cachedData = cachedDataQ.Execute().AsEnumerable().Select(o => new
                {
                    Code = o.Value(t => t.Code),
                    RetrievedOn = o.Value(t => t.RetrievedOn),
                    Data = o.Value(t => t.Data)
                }).ToLookup(o => o.Code, StringComparer.OrdinalIgnoreCase);

                // .ToArray() is used because Merge can modify the waypoint list.
                foreach (var doc in documents)
                    foreach (var wpt in doc.Waypoints.ToArray())
                    {
                        var cachedWpt = cachedData[wpt.Name].FirstOrDefault();

                        // if there is nothing cached, download the new version only if required
                        if (cachedWpt == null)
                        {
                            if (ShouldRefresh(wpt))
                                waypointsToDownload[wpt.Name] = Tuple.Create(doc, wpt);

                            continue;
                        }

                        // if the cached version is too old, do nothing if the waypoint does not need to be refreshed
                        if (ReadGpxTime(doc, wpt) > cachedWpt.RetrievedOn && !ShouldRefresh(wpt))
                            continue;

                        // if the cached copy is too old, update it only when needed (when the download of newer version fails
                        if (cachedWpt.RetrievedOn < DateTime.Now.AddDays(-7))
                        {
                            waypointsToDownload[wpt.Name] = Tuple.Create(doc, wpt);
                            waypointsToUpdateIfDownloadFails[wpt.Name] = Tuple.Create(doc, wpt, cachedWpt.RetrievedOn, cachedWpt.Data);
                            continue;
                        }

                        // at this point the cached copy is up-to-date and is required
                        try
                        {
                            var cache = Deserialize(serializer, cachedWpt.Data);
                            Merge(doc, wpt, new Gpx.GpxWaypoint(cache) { LastRefresh = cachedWpt.RetrievedOn });
                        }
                        catch (Exception ex)
                        {
                            this.ReportStatus(StatusSeverity.Warning, ex.Message);
                            waypointsToDownload[wpt.Name] = Tuple.Create(doc, wpt);
                        }
                    }

                // download geocaches that are not in cache or cached version has expired.
                if (!useLocalStorage && GeocachingService.LiveClient.IsEnabled && waypointsToDownload.Count > 0)
                {
                    try
                    {
                        using (var service = GeocachingService.LiveClient.CreateClientProxy())
                        {
                            if (service.IsBasicMember().GetValueOrDefault(true))
                            {
                                this.ReportStatus(StatusSeverity.Warning, "You are not premium member on geocaching.com and cannot download full data using the API.");
                            }
                            else
                            {
                                int i = 0;
                                this.ReportStatus("Downloading geocache information using Live API.");
                                foreach (var result in service.GetGeocachesByCode(
                                            waypointsToDownload.Values.Select(o => o.Item2.Name),
                                            false,
                                            err => this.ReportStatus(StatusSeverity.Error, err)))
                                {
                                    i++;
                                    var wpt = waypointsToDownload[result.Code];
                                    Merge(wpt.Item1, wpt.Item2, new Gpx.GpxWaypoint(result) { LastRefresh = DateTime.Now });
                                    waypointsToUpdateIfDownloadFails.Remove(result.Code);

                                    var insQ = db.Geocaches.Replace();
                                    insQ.Value(o => o.Code, result.Code);
                                    insQ.Value(o => o.Data, Serialize(serializer, result));
                                    insQ.Value(o => o.RetrievedOn, DateTime.Now);
                                    insQ.Execute();

                                    this.ReportProgress(i, waypointsToDownload.Count);

                                    //TODO: remove once the transformer progress shows progress indicator
                                    this.ReportStatus("Progress: {0}%", i * 100 / waypointsToDownload.Count);

                                    this.TerminateExecutionIfNeeded();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.ReportStatus(StatusSeverity.Warning, "Unable to download geocaches: " + ex.Message);
                    }
                }

                // update any waypoints where we have an expired cached version and a newer one was not downloaded
                foreach (var wpt in waypointsToUpdateIfDownloadFails.Values)
                {
                    try
                    {
                        var cache = Deserialize(serializer, wpt.Item4);
                        Merge(wpt.Item1, wpt.Item2, new Gpx.GpxWaypoint(cache) { LastRefresh = wpt.Item3 });
                    }
                    catch (Exception ex)
                    {
                        this.ReportStatus(StatusSeverity.Warning, ex.Message);
                    }
                }

                if (!useLocalStorage)
                {
                    // remove all cached data that is older than 1 month
                    // 1 month is used so that the older copies can still be used when needed
                    var delQ = db.Geocaches.Delete();
                    delQ.Where(o => o.RetrievedOn, Data.WhereOperator.Smaller, DateTime.Now.AddMonths(1));
                    delQ.Execute();
                }
            }
        }
    }
}
