/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoTransformer.Transformers.EditorExtensions
{
    /// <summary>
    /// The transformer that loads the data saved by editors and puts the data in the XML files.
    /// </summary>
    public class LoadEditorExtensions : TransformerBase
    {
        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Load edited values"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.PreProcess - 100; }
        }

        /// <summary>
        /// Processes the specified GPX documents.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
            var table = Extensions.ExtensionLoader.RetrieveExtensions<Extensions.ExtensionEditorDataTable>().Single();
            var q = table.Select();
            q.SelectAll();
            var cachedResults = q.Execute().AsEnumerable()
                                    .ToLookup(r => r.Value(t => t.CacheCode),
                                              r => new
                                                  {
                                                      RowId = r.Value(t => t.RowId),
                                                      Xml = XElement.Parse(r.Value(t => t.Data))
                                                  },
                                              StringComparer.OrdinalIgnoreCase);

            var existingCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var wpt in documents.SelectMany(o => o.Waypoints))
            {
                var name = wpt.Name;
                existingCodes.Add(name);

                foreach (var elem in cachedResults[name])
                {
                    if (elem.Xml.Name == XmlExtensions.GeoTransformerSchema + "CachedCopy")
                        continue;

                    wpt.ExtensionElements.Add(elem.Xml);
                }
            }

            // create placeholder waypoints that have been edited but are not loaded from the persistent sources
            // by default ManualPublish extension will clean these out before publishing
            var newDoc = new Gpx.GpxDocument();
            newDoc.Metadata.OriginalFileName = "EditorCachedCopies.gpx";

            var pendingDownload = new Dictionary<string, Tuple<long, List<XElement>>>(StringComparer.OrdinalIgnoreCase);

            foreach (var tempResult in cachedResults)
            {
                if (existingCodes.Contains(tempResult.Key))
                    continue;

                Gpx.GpxWaypoint gpx = null;
                var wpt = tempResult.LastOrDefault(o => o.Xml.Name == XmlExtensions.GeoTransformerSchema + "CachedCopy");
                if (wpt != null)
                {
                    var xml = wpt.Xml.Elements().FirstOrDefault();
                    gpx = new Gpx.GpxWaypoint(xml);

                    // ignore situations when the cached copy is empty copy from the previous versions.
                    if (gpx.Description == null)
                        gpx = null;
                }

                // refresh the cached copy if it is older than 7 days
                if ((gpx == null || gpx.LastRefresh.GetValueOrDefault() < DateTime.Now.AddDays(-7) || wpt.Xml.GetAttributeValue("cacheVersion") == null)
                    && tempResult.Key.StartsWith("GC", StringComparison.OrdinalIgnoreCase)
                    && (options & TransformerOptions.UseLocalStorage) == 0
                    && GeocachingService.LiveClient.IsEnabled)
                {
                    pendingDownload.Add(tempResult.Key, Tuple.Create(wpt == null ? 0L : wpt.RowId, tempResult.Select(o => o.Xml).ToList()));
                    continue;
                }

                if (gpx == null)
                {
                    gpx = new Gpx.GpxWaypoint();
                    gpx.Name = tempResult.Key;
                }

                gpx.ExtensionAttributes.Add(new XAttribute(XmlExtensions.GeoTransformerSchema + "EditorOnly", true));

                foreach (var elem in tempResult)
                {
                    if (elem.Xml.Name == XmlExtensions.GeoTransformerSchema + "CachedCopy")
                        continue;

                    gpx.ExtensionElements.Add(elem.Xml);
                }

                newDoc.Waypoints.Add(gpx);
            }

            if (pendingDownload.Count > 0)
            {
                int i = 0;
                using (var service = GeocachingService.LiveClient.CreateClientProxy())
                {
                    this.ExecutionContext.ReportStatus("Downloading " + pendingDownload.Count + " edited caches from Live API.");

                    foreach (var online in service.GetGeocachesByCode(pendingDownload.Keys))
                    {
                        var pending = pendingDownload[online.Code];
                        i++;
                        this.ExecutionContext.ThrowIfCancellationPending();
                        this.ExecutionContext.ReportProgress(i, pendingDownload.Count);

                        var gpx = new Gpx.GpxWaypoint(online);
                        gpx.LastRefresh = DateTime.Now;

                        if (pending.Item1 != 0)
                        {
                            // if the cached copy was expired, delete it
                            var dq = table.Delete();
                            dq.Where(t => t.RowId, pending.Item1);
                            dq.Execute();
                        }

                        // as the CachedCopy should have been in the database already and loading from the Live API
                        // is an exception, save it right now.
                        var iq = table.Insert();
                        iq.Value(o => o.CacheCode, online.Code);
                        var copy = new XElement(XmlExtensions.GeoTransformerSchema + "CachedCopy");
                        copy.SetAttributeValue("cacheVersion", "1");
                        copy.Add(gpx.Serialize(Gpx.GpxSerializationOptions.Roundtrip));
                        iq.Value(o => o.Data, copy.ToString());
                        iq.Execute();

                        gpx.ExtensionAttributes.Add(new XAttribute(XmlExtensions.GeoTransformerSchema + "EditorOnly", true));

                        foreach (var elem in pending.Item2)
                        {
                            if (elem.Name == XmlExtensions.GeoTransformerSchema + "CachedCopy")
                                continue;

                            gpx.ExtensionElements.Add(elem);
                        }

                        newDoc.Waypoints.Add(gpx);
                    }

                    this.ExecutionContext.ReportStatus(string.Empty);
                    this.ExecutionContext.ReportProgress(pendingDownload.Count, pendingDownload.Count);
                }
            }

            if (newDoc.Waypoints.Count > 0)
                documents.Add(newDoc);
        }
    }
}
