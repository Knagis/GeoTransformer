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
        /// Processes the specified XML files.
        /// </summary>
        /// <param name="xmlFiles">The xml files being processed. The transformer can add a new document for editor only caches.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<System.Xml.Linq.XDocument> xmlFiles, TransformerOptions options)
        {
            var table = Extensions.ExtensionLoader.RetrieveExtensions<Extensions.ExtensionEditorDataTable>().Single();
            var q = table.Select();
            q.SelectAll();
            var cachedResults = q.Execute().AsEnumerable()
                                    .ToLookup(r => r.Value(t => t.CacheCode), r => XElement.Parse(r.Value(t => t.Data)), StringComparer.OrdinalIgnoreCase);

            var existingCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var wpt in xmlFiles.SelectMany(o => o.Root.WaypointElements("wpt")))
            {
                var name = wpt.WaypointElement("name").GetValue();
                existingCodes.Add(name);

                foreach (var elem in cachedResults[name])
                {
                    if (string.Equals(elem.Name.LocalName, "CachedCopy", StringComparison.Ordinal))
                        continue;

                    wpt.Add(elem);
                }
            }

            if ((options & TransformerOptions.LoadingViewerCache) == TransformerOptions.LoadingViewerCache)
            {
                var newDoc = Gpx.Loader.CreateEmptyDocument("EditorCachedCopies.gpx");
                
                using (var service = GeocachingService.LiveClient.CreateClientProxy())
                foreach (var tempResult in cachedResults)
                {
                    if (existingCodes.Contains(tempResult.Key))
                        continue;

                    var wpt = tempResult.LastOrDefault(o => string.Equals(o.Name.LocalName, "CachedCopy", StringComparison.Ordinal));
                    if (wpt != null)
                    {
                        wpt = wpt.Elements().FirstOrDefault();

                        // ignore situations when the cached copy is empty copy from the previous versions.
                        if (wpt.WaypointElement("desc") == null)
                            wpt = null;
                    }

                    if (wpt == null && tempResult.Key.StartsWith("GC", StringComparison.OrdinalIgnoreCase)
                        && (options & TransformerOptions.UseLocalStorage) == 0)
                    {
                        this.TerminateExecutionIfNeeded();

                        this.ReportStatus("Loading " + tempResult.Key + " from Live API.");
                        var online = service.GetGeocacheByCode(tempResult.Key, true);
                        if (online != null)
                        {
                            wpt = Gpx.Loader.Convert(online);

                            // as the CachedCopy should have been in the database already and loading from the Live API
                            // is an exception, save it right now.
                            var iq = table.Insert();
                            iq.Value(o => o.CacheCode, tempResult.Key);
                            iq.Value(o => o.Data, wpt.GetCachedCopy().Parent.ToString()); // .Parent makes sure that the extension element is saved, not the copy.
                            iq.Execute();
                        }
                        this.ReportStatus(string.Empty);
                    }

                    if (wpt == null)
                        wpt = new XElement(XmlExtensions.GpxSchema_1_0 + "wpt",
                                                new XElement(XmlExtensions.GpxSchema_1_0 + "name", tempResult.Key),
                                                new XElement(XmlExtensions.GeocacheSchema101 + "cache"));

                    wpt.SetAttributeValue(XmlExtensions.GeoTransformerSchema + "EditorOnly", true);

                    foreach (var elem in tempResult)
                    {
                        if (string.Equals(elem.Name.LocalName, "CachedCopy", StringComparison.Ordinal))
                            continue;

                        wpt.Add(elem);
                    }

                    newDoc.Root.Add(wpt);
                }

                if (newDoc.Root.HasElements)
                    xmlFiles.Add(newDoc);
            }
        }
    }
}
