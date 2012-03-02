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
        /// Processes the specified XML files. The default implementation calls <see cref="Process(XDocument)"/> for each file.
        /// </summary>
        /// <param name="xmlFiles">The XML documents currently in the process. The list can be changed if needed</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<System.Xml.Linq.XDocument> xmlFiles, TransformerOptions options)
        {
            var queries = this._configurationControl.Queries.ToList();

            if ((options & TransformerOptions.UseLocalStorage) == TransformerOptions.UseLocalStorage)
                this.LoadFromCache(xmlFiles, queries);
            else
                this.LoadRegular(xmlFiles, queries);
        }

        private void LoadFromCache(IList<System.Xml.Linq.XDocument> xmlFiles, IList<Query> queries)
        {
            this.ReportStatus("{0} queries pending", queries.Count);
            int cached = 0;
            int notAvailable = 0;
            int wptCount = 0;

            foreach (var q in queries)
            {
                var cacheFile = System.IO.Path.Combine(this.LocalStoragePath, q.Id.ToString() + ".gpx");
                if (!System.IO.File.Exists(cacheFile))
                {
                    notAvailable++;
                }
                else
                {
                    var doc = Gpx.Loader.Gpx(cacheFile);
                    wptCount += doc.CacheDescendants("cache").Count() / 2; // divide because of the CachedCopy elements.
                    xmlFiles.Add(doc);
                    cached++;
                }

                this.ReportStatus("{0} queries pending, {1} cached, {2} not available", queries.Count - cached - notAvailable, cached, notAvailable);
            }

            this.ReportStatus("{0}/{1} queries loaded, {2} caches", cached, queries.Count, wptCount);
        }

        private void LoadRegular(IList<System.Xml.Linq.XDocument> xmlFiles, IList<Query> queries)
        {
            if (!GeocachingService.LiveClient.IsEnabled)
                this.ReportStatus(true, "Live API is not enabled");

            this.ReportStatus("{0} queries pending", queries.Count);
            int cached = 0;
            int downloaded = 0;
            int wptCount = 0;

            foreach (var q in queries)
            {
                var cacheFile = System.IO.Path.Combine(this.LocalStoragePath, q.Id.ToString() + ".gpx");
                var age = DateTime.Now.Subtract(System.IO.File.GetLastWriteTime(cacheFile)).TotalHours;
                if (age < 18)
                {
                    var doc = Gpx.Loader.Gpx(cacheFile);
                    wptCount += doc.CacheDescendants("cache").Count() / 2; // divide because of the CachedCopy elements.
                    xmlFiles.Add(doc);
                    cached++;
                }
                else
                {
                    var doc = DownloadQuery(q);
                    wptCount += doc.CacheDescendants("cache").Count() / 2; // divide because of the CachedCopy elements.
                    xmlFiles.Add(doc);
                    downloaded++;
                    doc.Save(cacheFile);
                }

                this.ReportStatus("{0} queries pending, {1} cached, {2} downloaded", queries.Count - cached - downloaded, cached, downloaded);
            }

            this.ReportStatus("{0} cached, {1} downloaded, {2} caches", cached, downloaded, wptCount);
        }

        private System.Xml.Linq.XDocument DownloadQuery(Query q)
        {
            var doc = Gpx.Loader.CreateEmptyDocument(q.Id.ToString() + ".gpx");
            int perPage = 15;

            using (var service = GeocachingService.LiveClient.CreateClientProxy())
            {
                var basic = service.IsBasicMember();
                if (!basic.HasValue)
                    this.ReportStatus(true, "Live API is not available.");

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
                        doc.Root.Add(Gpx.Loader.Convert(gc));

                    loaded += result.Geocaches.Length;
                    this.ReportStatus("Downloading: {0}% of {1}", loaded * 100 / q.MaximumCaches, q.Title);

                    // if the maximum has been loaded or there are no more caches to be loaded, break.
                    if (result.Geocaches.Length == 0 || loaded >= q.MaximumCaches)
                        break;

                    mreq.MaxPerPage = Math.Min(perPage, q.MaximumCaches - loaded);
                    mreq.StartIndex = loaded;
                    result = service.GetMoreGeocaches(mreq);
                }

                if (result.Status.StatusCode != 0)
                    this.ReportStatus(true, result.Status.StatusMessage);

                q.LastDownloadCacheCount = loaded;
            }

            return doc;
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
