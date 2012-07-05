/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoTransformer.Extensions;

namespace GeoTransformer.Transformers.LoadFromLiveApi
{
    public class LoadFromLiveApi : TransformerBase, IConfigurable, ILocalStorage
    {
        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Load from geocaching.com Live API"; }
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
        /// Loads queries and adds them to the <paramref name="documents"/> list.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
            var queries = this._configurationControl.Queries.ToList();

            IEnumerable<Gpx.GpxDocument> data;
            if ((options & TransformerOptions.UseLocalStorage) == TransformerOptions.UseLocalStorage)
                data = this.LoadFromCache(queries);
            else
                data = this.LoadRegular(queries);

            foreach (var gpx in data)
                documents.Add(gpx);
        }

        /// <summary>
        /// Loads all queries from cache ignoring those that cannot be found.
        /// </summary>
        /// <param name="queries">The queries that should be processed. All given queries will be processed.</param>
        /// <returns>A list of GPX documents.</returns>
        private IEnumerable<Gpx.GpxDocument> LoadFromCache(IList<Query> queries)
        {
            int cached = 0;
            int notAvailable = 0;
            int wptCount = 0;

            foreach (var q in queries)
            {
                this.ExecutionContext.ReportStatus("Loading '{0}'.", q.Title);
                var cacheFile = System.IO.Path.Combine(this.LocalStoragePath, q.Id.ToString() + ".gpx");
                if (!System.IO.File.Exists(cacheFile))
                {
                    notAvailable++;
                }
                else
                {
                    var gpx = Gpx.Loader.Gpx(cacheFile);
                    gpx.Metadata.OriginalFileName = q.Title + ".gpx";
                    wptCount += gpx.Waypoints.Count(o => o.Geocache.IsDefined());
                    cached++;
                    yield return gpx;
                }

                this.ExecutionContext.ReportProgress(cached + notAvailable, queries.Count);
            }

            if (notAvailable > 0)
                this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "{0}/{1} queries loaded, {2} caches", cached, queries.Count, wptCount);
            else
                this.ExecutionContext.ReportStatus(StatusSeverity.Information, "{0} queries loaded, {1} caches", queries.Count, wptCount);
        }

        /// <summary>
        /// Loads all queries from cache. Queries that cannot be found (or cannot be downloaded) are loaded from cache or ignored.
        /// </summary>
        /// <param name="queries">The queries that should be processed. All given queries will be processed.</param>
        /// <returns>A list of GPX documents.</returns>
        private IEnumerable<Gpx.GpxDocument> LoadRegular(IList<Query> queries)
        {
            if (!GeocachingService.LiveClient.IsEnabled)
                this.ExecutionContext.ReportStatus(StatusSeverity.Error, "Live API is not enabled.");

            int cached = 0;
            int downloaded = 0;
            int wptCount = 0;
            int notAvailable = 0;

            foreach (var q in queries)
            {
                this.ExecutionContext.ReportStatus("Loading {0}/{1}: '{2}'.", queries.Count - notAvailable - cached - downloaded, queries.Count, q.Title);

                var cacheFile = System.IO.Path.Combine(this.LocalStoragePath, q.Id.ToString() + ".gpx");
                var age = DateTime.Now.Subtract(System.IO.File.GetLastWriteTime(cacheFile)).TotalHours;
                
                Gpx.GpxDocument gpx = null;
                if (age > 18)
                {
                    gpx = DownloadQuery(q);
                    if (gpx != null)
                    {
                        gpx.Metadata.OriginalFileName = q.Title + ".gpx";
                        wptCount += gpx.Waypoints.Count(o => o.Geocache.IsDefined());
                        downloaded++;
                        gpx.Serialize(Gpx.GpxSerializationOptions.Roundtrip).Save(cacheFile);
                        yield return gpx;
                    }
                }

                // if there was an error while downloading the query, it should also be loaded from cache.
                if (gpx == null && System.IO.File.Exists(cacheFile))
                {
                    gpx = Gpx.Loader.Gpx(cacheFile);
                    gpx.Metadata.OriginalFileName = q.Title + ".gpx";
                    wptCount += gpx.Waypoints.Count(o => o.Geocache.IsDefined());
                    cached++;
                    yield return gpx;
                }

                if (gpx == null)
                    notAvailable++;
            }

            if (notAvailable > 0)
                this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "{0} cached, {1} downloaded, {2} not available, {3} caches", cached, downloaded, notAvailable, wptCount);
            else
                this.ExecutionContext.ReportStatus(StatusSeverity.Information, "{0} cached, {1} downloaded, {2} caches", cached, downloaded, wptCount);
        }

        private Gpx.GpxDocument DownloadQuery(Query q)
        {
            var gpx = new Gpx.GpxDocument();
            int perPage = 15;

            using (var service = GeocachingService.LiveClient.CreateClientProxy())
            {
                var basic = service.IsBasicMember();
                if (!basic.HasValue)
                {
                    this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "Cannot download '{0}' because Live API is not available.", q.Title);
                    return null;
                }

                int loaded = 0;

                var req = q.CreateRequest();
                
                req.AccessToken = service.AccessToken;
                req.MaxPerPage = Math.Min(perPage, q.MaximumCaches - loaded);
                req.GeocacheLogCount = 5;
                req.TrackableLogCount = 5;
                req.IsLite = basic.Value;

                var mreq = new GeocachingService.GetMoreGeocachesRequest();
                mreq.AccessToken = service.AccessToken;
                mreq.IsLite = req.IsLite;
                mreq.GeocacheLogCount = 5;
                mreq.TrackableLogCount = 5;
                
                var result = service.SearchForGeocaches(req);
                while (result.Status.StatusCode == 0)
                {
                    foreach (var gc in result.Geocaches)
                        gpx.Waypoints.Add(new Gpx.GpxWaypoint(gc) { LastRefresh = DateTime.Now });

                    loaded += result.Geocaches.Length;
                    this.ExecutionContext.ReportProgress(loaded, q.MaximumCaches);

                    // if the maximum has been loaded or there are no more caches to be loaded, break.
                    if (result.Geocaches.Length == 0 || loaded >= q.MaximumCaches)
                        break;

                    mreq.MaxPerPage = Math.Min(perPage, q.MaximumCaches - loaded);
                    mreq.StartIndex = loaded;
                    result = service.GetMoreGeocaches(mreq);
                }

                if (result.Status.StatusCode != 0)
                {
                    this.ExecutionContext.ReportStatus(StatusSeverity.Warning, "Unable to download '{0}': " + result.Status.StatusMessage);
                    return null;
                }

                q.LastDownloadCacheCount = loaded;
            }

            return gpx;
        }

        private ConfigurationControl _configurationControl;

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
            return this._configurationControl = new ConfigurationControl(currentConfiguration);
        }

        /// <summary>
        /// Retrieves the configuration from the extension's configuration UI control.
        /// </summary>
        /// <returns>
        /// The serialized configuration data.
        /// </returns>
        public byte[] SerializeConfiguration()
        {
            return this._configurationControl.SerializeConfiguration();
        }

        /// <summary>
        /// Gets a value indicating whether the this extension should be executed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this extension is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get { return this._configurationControl.Queries.Any(); }
        }

        /// <summary>
        /// Gets the category of the extension.
        /// </summary>
        Category IHasCategory.Category { get { return new Category(Category.GeocacheSources.ConfigurationOrder, "Geocache source - geocaching.com Live API"); } }

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
