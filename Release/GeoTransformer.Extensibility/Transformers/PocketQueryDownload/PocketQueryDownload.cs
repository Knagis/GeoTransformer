/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoTransformer.Extensions;

namespace GeoTransformer.Transformers.PocketQueryDownload
{
    public class PocketQueryDownload : TransformerBase, ILocalStorage, IConfigurable
    {
        private PocketQueryDownloadOptions _options;

        /// <summary>
        /// Class that describes a pocket query that has to be downloaded.
        /// </summary>
        private class DownloadInfo
        {
            public DownloadInfo(string localStoragePath, bool usingFullData)
            {
                this._localStoragePath = localStoragePath;
                this._usingFullData = usingFullData;
            }
            private string _localStoragePath;
            private bool? _cacheUpToDate;
            private bool _usingFullData;

            public Guid Id;
            public string Title;
            public DateTime LastGenerated;
            public bool NotAvailable;

            public bool CacheUpToDate
            {
                get
                {
                    if (_cacheUpToDate.HasValue)
                        return _cacheUpToDate.Value;

                    if (!System.IO.File.Exists(this.CacheKeyFileName) || !System.IO.File.Exists(this.CacheFileName))
                    {
                        _cacheUpToDate = false;
                        return false;
                    }

                    DateTime lastGenerated;
                    if (!DateTime.TryParse(System.IO.File.ReadAllText(this.CacheKeyFileName), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out lastGenerated))
                    {
                        _cacheUpToDate = false;
                        return false;
                    }

                    // this.LastGenerated is not applied when downloading full data since the PQ might be week old - we still will get
                    // fresh data using the full download.
                    if (this._usingFullData)
                        _cacheUpToDate = DateTime.UtcNow.Subtract(lastGenerated).TotalHours < 6;
                    else
                        _cacheUpToDate = Math.Abs(this.LastGenerated.Subtract(lastGenerated).TotalMinutes) < 5;

                    return _cacheUpToDate.Value;
                }
            }

            public string TempFileName
            {
                get
                {
                    return System.IO.Path.Combine(this._localStoragePath, this.Id.ToString() + ".zip.temp");
                }
            }
            public string CacheFileName
            {
                get
                {
                    var fn = this.Title;
                    if (string.IsNullOrWhiteSpace(fn))
                        fn = this.Id.ToString();

                    foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                        fn = fn.Replace(c, '_');

                    return System.IO.Path.Combine(this._localStoragePath, fn + ".zip.cache");
                }
            }
            public string CacheKeyFileName
            {
                get
                {
                    return this.CacheFileName + "key";
                }
            }

            /// <summary>
            /// Update the cache key file signaling that the cached data is refreshed.
            /// </summary>
            public void WriteCacheKey()
            {
                if (this._usingFullData)
                    System.IO.File.WriteAllText(this.CacheKeyFileName, DateTime.UtcNow.ToString(System.Globalization.CultureInfo.InvariantCulture));
                else
                    System.IO.File.WriteAllText(this.CacheKeyFileName, this.LastGenerated.ToUniversalTime().ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Download pocket queries"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.LoadSources; }
        }

        /// <summary>
        /// Loads the pocket queries and adds them to the <paramref name="documents"/> collection.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
            if ((options & TransformerOptions.UseLocalStorage) == TransformerOptions.UseLocalStorage)
                this.LoadFilesFromCache(documents);
            else
                this.LoadFiles(documents, options);
        }

        private void LoadFilesFromCache(IList<Gpx.GpxDocument> documents)
        {
            if (this._options.CheckedQueries.Count == 0)
            {
                this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "There are no pocket queries selected for download.");
                return;
            }

            int wptCount = 0;
            int notInCache = 0;
            foreach (var g in this._options.CheckedQueries)
            {
                var x = new DownloadInfo(this.LocalStoragePath, this._options.DownloadFullData) { Id = g.Key, Title = g.Value };

                var file = x.CacheFileName;
                if (!System.IO.File.Exists(x.CacheFileName) || !System.IO.File.Exists(x.CacheKeyFileName))
                {
                    this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "Query '{0}' has not been downloaded yet.", x.Title);
                    notInCache++;
                    continue;
                }

                var queryDate = DateTime.Parse(System.IO.File.ReadAllText(x.CacheKeyFileName), System.Globalization.CultureInfo.InvariantCulture);
                var queryAge = (int)DateTime.Now.Subtract(queryDate).TotalDays;
                if (queryAge > 15)
                    this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "The local copy of '{0}' is {1} day{2} old.", x.Title, queryAge, (queryAge % 10 == 1 && queryAge % 100 != 11) ? string.Empty : "s");

                try
                {
                    foreach (var xml in Gpx.Loader.Zip(file))
                    {
                        documents.Add(xml);
                        wptCount += xml.Waypoints.Count(o => o.Geocache.IsDefined());
                    }
                }
                catch (Transformers.TransformerCancelledException)
                {
                    throw;
                }
                catch
                {
                    notInCache++;
                    System.IO.File.Delete(x.CacheFileName);
                    continue;
                }
            }

            if (notInCache > 0)
            {
                this.ExecutionContext.ReportStatus(StatusSeverity.Warning,
                    "Done. {2} from cache, {1} not in cache. {0} geocaches.",
                    wptCount,
                    notInCache,
                    this._options.CheckedQueries.Count - notInCache);
            }
            else
            {
                this.ExecutionContext.ReportStatus("Done. {0} files loaded, {1} caches.", this._options.CheckedQueries.Count, wptCount);
            }
        }

        private void LoadFiles(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
            if (this._options.CheckedQueries.Count == 0)
            {
                this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "There are no pocket queries selected for download.");
                return;
            }

            if (!GeocachingService.LiveClient.IsEnabled)
            {
                this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "Using local copies because Live API is disabled.");
                this.LoadFilesFromCache(documents);
                return;
            }

            var downloadList = new List<DownloadInfo>(this._options.CheckedQueries.Count);

            using (var service = GeocachingService.LiveClient.CreateClientProxy())
            {
                this.ExecutionContext.ReportStatus("Retrieving pocket query list.");
                GeocachingService.GetPocketQueryListResponse pqList;
                try
                {
                    pqList = service.GetPocketQueryList(service.AccessToken);
                }
                catch (Exception ex)
                {
                    pqList = new GeocachingService.GetPocketQueryListResponse()
                    {
                        Status = new GeocachingService.StatusResponse()
                        {
                            StatusCode = -1,
                            StatusMessage = ex is System.ServiceModel.EndpointNotFoundException
                                ? "Network connection or geocaching.com site is not available."
                                : ex.Message
                        }
                    };
                }

                if (pqList.Status.StatusCode != 0)
                {
                    this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "Using local copies because of unable to connect to Live API: " + pqList.Status.StatusMessage);
                    this.LoadFilesFromCache(documents);
                    return;
                }

                this._options.SetPocketQueryList(pqList.PocketQueryList, false);

                foreach (var g in this._options.CheckedQueries)
                {
                    this.ExecutionContext.ThrowIfCancellationPending();

                    var x = new DownloadInfo(this.LocalStoragePath, this._options.DownloadFullData) { Id = g.Key, Title = g.Value };
                    downloadList.Add(x);
                    var actualPq = pqList.PocketQueryList.FirstOrDefault(o => o.GUID == x.Id || o.Name == x.Title);
                    x.Title = actualPq.Name;
                    if (actualPq == null)
                    {
                        this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "Query '{0}' is not available for download.", x.Title);
                        x.NotAvailable = true;
                        continue;
                    }

                    x.LastGenerated = actualPq.DateLastGenerated;

                    var queryAge = (int)DateTime.Now.Subtract(actualPq.DateLastGenerated).TotalDays;
                    if (queryAge > 8)
                        this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "Query '{0}' is {1} day{2} old.", x.Title, queryAge, (queryAge % 10 == 1 && queryAge % 100 != 11) ? string.Empty : "s");

                    if (x.CacheUpToDate)
                        continue;

                    if (!actualPq.IsDownloadAvailable)
                    {
                        this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "Query '{0}' is not available for download.", x.Title);
                        x.NotAvailable = true;
                        continue;
                    }
                }

                foreach (var x in downloadList.Where(o => !o.NotAvailable && !o.CacheUpToDate))
                {
                    this.ExecutionContext.ReportStatus("Downloading: " + x.Title);
                    x.NotAvailable = true;
                    try
                    {
                        DownloadPocketQuery(service, x);
                    }
                    catch (TransformerCancelledException ex)
                    {
                        if (!ex.CanContinue)
                            throw;

                        this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "Error while downloading: " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "Error while downloading: " + ex.Message);
                    }
                }
            }

            this.ExecutionContext.ReportStatus("Download complete, loading data.");

            int wptCount = 0;
            int notInCache = 0;
            int fromCache = 0;
            foreach (var x in downloadList)
            {
                bool useCache = x.CacheUpToDate || x.NotAvailable;
                if (useCache)
                    fromCache++;
                string file = useCache ? x.CacheFileName : x.TempFileName;
                if (useCache && !System.IO.File.Exists(file))
                {
                    notInCache++;
                    continue;
                }

                try
                {
                    foreach (var xml in Gpx.Loader.Zip(file))
                    {
                        documents.Add(xml);
                        wptCount += xml.Waypoints.Count(o => o.Geocache.IsDefined());
                    }
                }
                catch
                {
                    System.IO.File.Delete(file);
                    this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "The {1} copy for '{0}' could not be loaded.", x.Title, useCache ? "cached" : "downloaded");
                }

                if (!useCache)
                {
                    System.IO.File.Delete(x.CacheKeyFileName);
                    System.IO.File.Delete(x.CacheFileName);
                    System.IO.File.Move(x.TempFileName, x.CacheFileName);
                    x.WriteCacheKey();
                }
            }

            // remove all obsolete (older than a month and not currently selected) files.
            var validFiles = downloadList.Select(o => o.CacheFileName).Union(downloadList.Select(o => o.CacheKeyFileName));
            foreach (var f in System.IO.Directory.GetFiles(this.LocalStoragePath).Except(validFiles, StringComparer.OrdinalIgnoreCase))
            {
                try
                {
                    if (DateTime.Now.Subtract(System.IO.File.GetLastWriteTime(f)).TotalDays > 30)
                        System.IO.File.Delete(f);
                }
                catch { }
            }

            this.ExecutionContext.ReportStatus(notInCache > 0 ? StatusSeverity.Warning : StatusSeverity.Information,
                "Done. {1} downloaded, {2} from cache, {3} not available. {0} geocaches.",
                wptCount,
                downloadList.Count - fromCache,
                fromCache - notInCache,
                notInCache);
        }

        private void DownloadPocketQuery(GeocachingService.LiveClient service, DownloadInfo x)
        {
            if (this._options.DownloadFullData)
            {
                try
                {
                    int pqCount = 1000;
                    var data = new List<GeocachingService.Geocache>();
                    while (data.Count < pqCount)
                    {
                        this.ExecutionContext.ReportProgress(data.Count, pqCount, true);
                        this.ExecutionContext.ThrowIfCancellationPending();

                        var response = service.GetFullPocketQueryData(service.AccessToken, x.Id, data.Count, 75);
                        if (response.Status.StatusCode == 140)
                        {
                            System.Threading.Thread.Sleep(10000);
                            continue;
                        }
                        if (response.Status.StatusCode != 0)
                        {
                            this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "Error while downloading: " + response.Status.StatusMessage);
                            return;
                        }

                        pqCount = (int)response.PQCount;
                        data.AddRange(response.Geocaches);
                    }

                    this.ExecutionContext.ReportProgressFinished();
                    Gpx.Loader.Serialize(x.TempFileName, true, data, new Gpx.GpxMetadata() { LastRefresh = DateTime.Now });
                    x.NotAvailable = false;

                    return;
                }
                catch (TransformerCancelledException ex)
                {
                    if (!ex.CanContinue)
                        throw;

                    // fall back to the ZIP download instead.
                }
            }

            var zresponse = service.GetPocketQueryZippedFile(service.AccessToken, x.Id);
            if (zresponse.Status.StatusCode != 0)
            {
                this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "Error while downloading: " + zresponse.Status.StatusMessage);
            }
            else
            {
                System.IO.File.WriteAllBytes(x.TempFileName, System.Convert.FromBase64String(zresponse.ZippedFile));
                x.NotAvailable = false;
            }

            return;
        }

        #region [ ILocalStorage ]

        /// <summary>
        /// Sets the local storage path where the extension can store its cache if needed. The value is set by the main engine once the extension instance is created.
        /// </summary>
        public string LocalStoragePath
        {
            get;
            set;
        }

        #endregion

        #region [ IConfigurable ]

        internal PocketQueryDownloadOptions ConfigurationControl { get { return this._options; } }

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
            this._options = new PocketQueryDownloadOptions();
            this._options.DeserializeSettings(currentConfiguration);

            return this._options;
        }

        /// <summary>
        /// Retrieves the configuration from the extension's configuration UI control.
        /// </summary>
        /// <returns>
        /// The serialized configuration data.
        /// </returns>
        public byte[] SerializeConfiguration()
        {
            return this._options.SerializeSettings();
        }

        /// <summary>
        /// Gets a value indicating whether the this extension should be executed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this extension is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get { return this._options.DownloadEnabled; }
        }

        Category IHasCategory.Category { get { return new Category(Category.GeocacheSources.ConfigurationOrder, "Geocache source - geocaching.com Pocket Queries"); } }

        #endregion
    }
}
