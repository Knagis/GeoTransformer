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
    class PocketQueryDownload : TransformerBase, ILocalStorage, IConfigurable
    {
        private PocketQueryDownloadOptions _options;

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

        public override string Title
        {
            get { return "Download pocket queries"; }
        }

        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.LoadSources; }
        }

        public override void Process(IList<System.Xml.Linq.XDocument> xmlFiles, TransformerOptions options)
        {
            if ((options & TransformerOptions.UseLocalStorage) == TransformerOptions.UseLocalStorage)
                this.LoadFilesFromCache(xmlFiles);
            else
                this.LoadFiles(xmlFiles, options);
        }

        private void LoadFilesFromCache(IList<System.Xml.Linq.XDocument> xmlFiles)
        {
            int wptCount = 0;
            int notInCache = 0;
            foreach (var g in this._options.CheckedQueries)
            {
                var x = new DownloadInfo(this.LocalStoragePath) { Id = g.Key, Title = g.Value };

                if (!System.IO.File.Exists(x.CacheFileName))
                {
                    notInCache++;
                    continue;
                }

                var file = x.CacheFileName;
                try
                {
                    foreach (var xml in Gpx.Loader.Zip(file))
                    {
                        xmlFiles.Add(xml);
                        wptCount += xml.CacheDescendants("cache").Count() / 2; // divide because of the CachedCopy elements.
                    }
                }
                catch
                {
                    notInCache++;
                    System.IO.File.Delete(x.CacheFileName);
                    continue;
                }
            }

            this.ReportStatus(string.Format("Done. {2} from cache, {1} not in cache. {0} caches.",
                wptCount,
                notInCache,
                this._options.CheckedQueries.Count - notInCache));
        }

        private void LoadFiles(IList<System.Xml.Linq.XDocument> xmlFiles, TransformerOptions options)
        {
            if (!GeocachingService.LiveClient.IsEnabled)
                this.ReportStatus(true, "Live API must be enabled to download PQs.");

            var downloadList = new List<DownloadInfo>(this._options.CheckedQueries.Count);
            using (var service = GeocachingService.LiveClient.CreateClientProxy())
            {
                this.ReportStatus("Retrieving pocket query list.");
                var pqList = service.GetPocketQueryList(service.AccessToken);
                if (pqList.Status.StatusCode != 0)
                    this.ReportStatus(true, pqList.Status.StatusMessage);

                this._options.SetPocketQueryList(pqList.PocketQueryList, false);
                
                foreach (var g in this._options.CheckedQueries)
                {
                    var x = new DownloadInfo(this.LocalStoragePath) { Id = g.Key, Title = g.Value };
                    downloadList.Add(x);
                    var actualPq = pqList.PocketQueryList.FirstOrDefault(o => o.GUID == x.Id || o.Name == x.Title);
                    if (actualPq == null)
                    {
                        x.NotAvailable = true;
                        continue;
                    }

                    x.LastGenerated = actualPq.DateLastGenerated.ToString(System.Globalization.CultureInfo.InvariantCulture);

                    if (x.CacheUpToDate)
                        continue;

                    if (!actualPq.IsDownloadAvailable)
                    {
                        x.NotAvailable = true;
                        continue;
                    }
                }

                foreach (var x in downloadList.Where(o => !o.NotAvailable && !o.CacheUpToDate))
                {
                    this.ReportStatus("Downloading: " + x.Title);
                    var response = service.GetPocketQueryZippedFile(service.AccessToken, x.Id);
                    if (response.Status.StatusCode != 0)
                        this.ReportStatus(true, response.Status.StatusMessage);

                    System.IO.File.WriteAllBytes(x.TempFileName, System.Convert.FromBase64String(response.ZippedFile));
                }
            }

            this.ReportStatus("Loading queries.");

            int wptCount = 0;
            int notInCache = 0;
            foreach (var x in downloadList)
            {
                if (x.NotAvailable)
                    continue;

                bool useCache = x.CacheUpToDate;
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
                        xmlFiles.Add(xml);
                        wptCount += xml.CacheDescendants("cache").Count() / 2; // divide because of the CachedCopy elements.
                    }
                }
                catch
                {
                    if (useCache)
                    {
                        System.IO.File.Delete(x.CacheFileName);
                        this.Process(xmlFiles, options);
                        return;
                    }
                    this.ReportStatus(true, "Could not download valid GPX file. Please try again.");
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

            this.ReportStatus(string.Format("Done. {1} downloaded, {2} from cache, {3} not available. {0} caches.",
                wptCount,
                downloadList.Count(o => !o.CacheUpToDate),
                downloadList.Count(o => o.CacheUpToDate),
                downloadList.Count(o => o.NotAvailable)));
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
