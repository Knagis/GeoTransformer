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
            public DownloadInfo(string localStoragePath)
            {
                this._localStoragePath = localStoragePath;
            }
            private string _localStoragePath;
            private bool? _cacheUpToDate;

            public Guid Id;
            public string Title;
            public string LastGenerated;
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

                    _cacheUpToDate = string.Equals(this.LastGenerated, System.IO.File.ReadAllText(this.CacheKeyFileName), StringComparison.Ordinal);

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
            int wptCount = 0;
            int notInCache = 0;
            foreach (var g in this._options.CheckedQueries)
            {
                var x = new DownloadInfo(this.LocalStoragePath) { Id = g.Key, Title = g.Value };

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
                    "Done. {2} from cache, {1} not in cache. {0} caches.",
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
            if (!GeocachingService.LiveClient.IsEnabled)
                this.ExecutionContext.ReportStatus(StatusSeverity.Error, "Live API must be enabled to download PQs.");

            var downloadList = new List<DownloadInfo>(this._options.CheckedQueries.Count);
            using (var service = GeocachingService.LiveClient.CreateClientProxy())
            {
                this.ExecutionContext.ReportStatus("Retrieving pocket query list.");
                var pqList = service.GetPocketQueryList(service.AccessToken);
                if (pqList.Status.StatusCode != 0)
                {
                    this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "Using local copies because of unable to connect to Live API: " + pqList.Status.StatusMessage);

                    // fall back to the cached copies.
                    this.LoadFilesFromCache(documents);
                    return;
                }

                this._options.SetPocketQueryList(pqList.PocketQueryList, false);
                
                foreach (var g in this._options.CheckedQueries)
                {
                    var x = new DownloadInfo(this.LocalStoragePath) { Id = g.Key, Title = g.Value };
                    downloadList.Add(x);
                    var actualPq = pqList.PocketQueryList.FirstOrDefault(o => o.GUID == x.Id || o.Name == x.Title);
                    x.Title = actualPq.Name;
                    if (actualPq == null)
                    {
                        this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "Query '{0}' is not available for download.", x.Title);
                        x.NotAvailable = true;
                        continue;
                    }

                    x.LastGenerated = actualPq.DateLastGenerated.ToString(System.Globalization.CultureInfo.InvariantCulture);

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
                    var response = service.GetPocketQueryZippedFile(service.AccessToken, x.Id);
                    if (response.Status.StatusCode != 0)
                    {
                        x.NotAvailable = true;
                        this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "Error while downloading: " + response.Status.StatusMessage);
                    }
                    else
                    {
                        System.IO.File.WriteAllBytes(x.TempFileName, System.Convert.FromBase64String(response.ZippedFile));
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
                    System.IO.File.WriteAllText(x.CacheKeyFileName, x.LastGenerated);
                }
            }

            // remove all obsolete files
            var validFiles = downloadList.Select(o => o.CacheFileName).Union(downloadList.Select(o => o.CacheKeyFileName));
            foreach (var f in System.IO.Directory.GetFiles(this.LocalStoragePath).Except(validFiles, StringComparer.OrdinalIgnoreCase))
            {
                try
                {
                    System.IO.File.Delete(f);
                }
                catch { }
            }

            this.ExecutionContext.ReportStatus(notInCache > 0 ? StatusSeverity.Warning : StatusSeverity.Information,
                "Done. {1} downloaded, {2} from cache, {3} not available. {0} caches.",
                wptCount,
                downloadList.Count - fromCache - notInCache,
                fromCache,
                notInCache);
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
