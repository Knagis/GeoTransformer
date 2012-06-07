/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoTransformer.Transformers.RefreshFromLiveApi
{
    public class RefreshImagesFromLiveApi : TransformerBase, Extensions.IConfigurable, Extensions.ILocalStorage
    {
        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Add missing images from Live API"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        /// <remarks>Configured to execute right after <see cref="RefreshFromLiveApi"/>.</remarks>
        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.PreProcess - 49; }
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
automatically download information about images that
are attached to the geocaches.

The data loaded from Live API is cached for 2 weeks.

This function is also available for Basic Members.");

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
        /// Updates the waypoint with array of <see cref="GeocachingService.ImageData"/> serialized in <paramref name="imageData"/>.
        /// </summary>
        /// <param name="waypoint">The waypoint to update.</param>
        /// <param name="imageData">The image data in binary serialized format.</param>
        /// <returns><c>True</c> if the update suceeded, <c>false</c> otherwise.</returns>
        private static bool UpdateWaypoint(Gpx.GpxWaypoint waypoint, byte[] imageData)
        {
            if (imageData == null)
                return false;

            // empty array is serialized as zero-length byte array to conserve space.
            if (imageData.Length == 0)
                return true;

            var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            try
            {
                using (var ms = new System.IO.MemoryStream(imageData))
                    UpdateWaypoint(waypoint, (GeocachingService.ImageData[])bf.Deserialize(ms));

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Updates the waypoint with <paramref name="imageData"/>.
        /// </summary>
        /// <param name="waypoint">The waypoint to update.</param>
        /// <param name="imageData">The image data.</param>
        private static void UpdateWaypoint(Gpx.GpxWaypoint waypoint, IEnumerable<GeocachingService.ImageData> imageData)
        {
            var collection = waypoint.Geocache.Images;
            foreach (var img in imageData)
            {
                var uri = new Uri(img.Url);
                if (!collection.Any(o => o.Address == uri))
                    collection.Add(new Gpx.GeocacheImage() { Title = img.Name, Address = new Uri(img.Url) });
            }
        }

        /// <summary>
        /// Serializes the specified image data into byte array.
        /// </summary>
        /// <param name="imageData">The image data.</param>
        /// <returns>The images in binary serialized format.</returns>
        private static byte[] Serialize(GeocachingService.ImageData[] imageData)
        {
            // empty array is serialized as zero-length byte array to conserve space.
            if (imageData.Length == 0)
                return new byte[0];

            var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (var ms = new System.IO.MemoryStream())
            {
                bf.Serialize(ms, imageData);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Processes the specified GPX documents. If the method is not overriden in the derived class,
        /// calls <see cref="Process(Gpx.GpxDocument, Transformers.TransformerOptions)"/> for each document in the list.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
            using (var db = new CachedDataSchema(System.IO.Path.Combine(this.LocalStoragePath, "images.db")))
            {
                bool useLocalStorage = (options & TransformerOptions.UseLocalStorage) == TransformerOptions.UseLocalStorage;
                bool downloadFailed = false;

                var cachedDataQ = db.Geocaches.Select();
                cachedDataQ.SelectAll();
                var cachedData = cachedDataQ.Execute().AsEnumerable().Select(o => new
                {
                    Code = o.Value(t => t.Code),
                    RetrievedOn = o.Value(t => t.RetrievedOn),
                    Data = o.Value(t => t.Data)
                }).ToLookup(o => o.Code, StringComparer.OrdinalIgnoreCase);

                var waypointsToDownload = new Queue<Gpx.GpxWaypoint>();
                var waypointsToUpdateIfDownloadFails = new List<Gpx.GpxWaypoint>();

                // STEP 1: create list of all waypoints that need update. If cached copy is up-to-date, then apply it
                foreach (var doc in documents)
                    foreach (var wpt in doc.Waypoints)
                    {
                        // if the waypoint already contains images, skip it
                        if (wpt.Geocache.Images.Count > 0)
                            continue;

                        // ignore additional waypoints and caches that are not from geocaching.com
                        if (wpt.Name == null || !wpt.Name.StartsWith("GC", StringComparison.OrdinalIgnoreCase))
                            continue;

                        var fromCache = cachedData[wpt.Name].FirstOrDefault();
                        if (fromCache == null)
                            waypointsToDownload.Enqueue(wpt);
                        else if (fromCache.RetrievedOn < DateTime.Now.AddDays(-14))
                            waypointsToUpdateIfDownloadFails.Add(wpt);
                        else
                            UpdateWaypoint(wpt, fromCache.Data);

                    }

                // STEP 2: download missing data
                if (!useLocalStorage && GeocachingService.LiveClient.IsEnabled && waypointsToDownload.Count > 0)
                {
                    try
                    {
                        using (var service = GeocachingService.LiveClient.CreateClientProxy())
                        {
                            int i = 0;
                            int errors = 0;
                            this.ReportStatus("Downloading geocache information using Live API.");
                            while (waypointsToDownload.Count > 0)
                            {
                                var wpt = waypointsToDownload.Dequeue();
                                i++;

                                var downloaded = service.GetImagesForGeocache(service.AccessToken, wpt.Name);
                                if (downloaded.Status.StatusCode != 0)
                                {
                                    // 140 - too many calls per minute (usual limit is 30 calls per minute).
                                    if (downloaded.Status.StatusCode == 140)
                                    {
                                        waypointsToDownload.Enqueue(wpt);

                                        // sleep for 10 seconds
                                        System.Threading.Thread.Sleep(10000);
                                        continue;
                                    }

                                    this.ReportStatus(StatusSeverity.Warning, "Unable to download image data: " + downloaded.Status.StatusMessage);
                                    errors++;
                                    if (errors > 20)
                                    {
                                        downloadFailed = true;
                                        break;
                                    }

                                    continue;
                                }

                                UpdateWaypoint(wpt, downloaded.Images);
                                waypointsToUpdateIfDownloadFails.Remove(wpt);

                                var insQ = db.Geocaches.Replace();
                                insQ.Value(o => o.Code, wpt.Name);
                                insQ.Value(o => o.Data, Serialize(downloaded.Images));
                                insQ.Value(o => o.RetrievedOn, DateTime.Now);
                                insQ.Execute();

                                this.ReportProgress(i, i + waypointsToDownload.Count);

                                //TODO: remove once the transformer progress shows progress indicator
                                this.ReportStatus("Progress: {0}% (press cancel to skip download)", i * 100 / (i + waypointsToDownload.Count));

                                this.TerminateExecutionIfNeeded();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        downloadFailed = true;
                        this.ReportStatus(StatusSeverity.Warning, "Unable to download image data: " + ex.Message);
                    }
                }

                // STEP 3: apply cached data where download did not succeed
                foreach (var wpt in waypointsToUpdateIfDownloadFails)
                {
                    var fromCache = cachedData[wpt.Name].FirstOrDefault();
                    if (fromCache == null)
                        continue;

                    UpdateWaypoint(wpt, fromCache.Data);
                }

                // STEP 4: cleanup old cached data
                if (!useLocalStorage && !downloadFailed)
                {
                    // remove all cached data that is older than 1 month
                    // 1 month is used so that the older copies can still be used when needed (e.g., download fails
                    // or network is not available).
                    var delQ = db.Geocaches.Delete();
                    delQ.Where(o => o.RetrievedOn, Data.WhereOperator.Smaller, DateTime.Now.AddMonths(-1));
                    delQ.Execute();
                }
            }
        }

        /// <summary>
        /// Sets the local storage path where the extension can store its cache if needed. The value is set by the main engine once the extension instance is created.
        /// </summary>
        public string LocalStoragePath
        {
            get;
            set;
        }
    }
}
