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
    /// The extension that persists changes made by different editors.
    /// </summary>
    public class SaveEditorExtensions : Extensions.ISaveData
    {
        /// <summary>
        /// Persists the editor extension data in the database.
        /// </summary>
        /// <param name="documents">The GPX documents that are currently loaded.</param>
        public void Save(IEnumerable<Gpx.GpxDocument> documents)
        {
            var table = Extensions.ExtensionLoader.RetrieveExtensions<Extensions.ExtensionEditorDataTable>().Single();
            using (var scope = table.Database().BeginTransaction())
            {
                table.Delete().Execute();

                var savedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var wpt in documents.SelectMany(o => o.Waypoints))
                    foreach (var ext in wpt.ExtensionElements)
                    {
                        if (ext.Name.Namespace != XmlExtensions.GeoTransformerSchema)
                            continue;

                        if (!ext.ContainsSignificantInformation())
                            continue;

                        var name = wpt.Name;
                        if (string.IsNullOrWhiteSpace(name))
                            continue;

                        var iq = table.Insert();
                        iq.Value(o => o.CacheCode, name);
                        iq.Value(o => o.Data, ext.ToString());
                        iq.Execute();

                        // for each cache that contains extensions, persist the cached copy
                        if (!savedCodes.Contains(name))
                        {
                            savedCodes.Add(name);

                            iq = table.Insert();
                            iq.Value(o => o.CacheCode, name);
                            var copy = new XElement(XmlExtensions.GeoTransformerSchema + "CachedCopy");
                            copy.Add(wpt.OriginalValues.Serialize(Gpx.GpxSerializationOptions.Roundtrip));
                            iq.Value(o => o.Data, copy.ToString());
                            iq.Execute();
                        }
                    }

                scope.Commit();
            }
        }
    }
}
