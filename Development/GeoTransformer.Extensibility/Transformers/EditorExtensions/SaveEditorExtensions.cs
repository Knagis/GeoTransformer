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
        /// <param name="xmlFiles">The XML files.</param>
        public void Save(IEnumerable<XDocument> xmlFiles)
        {
            var table = Extensions.ExtensionLoader.RetrieveExtensions<Extensions.ExtensionEditorDataTable>().Single();
            using (var scope = table.Database().BeginTransaction())
            {
                table.Delete().Execute();

                var extensionElements = xmlFiles.SelectMany(o => o.Root.Elements(XmlExtensions.GpxSchema_1_1 + "wpt"))
                                               .SelectMany(o => o.Elements())
                                               .Where(o => o.Name.Namespace == XmlExtensions.GeoTransformerSchema)
                                               .Where(o => !string.Equals(o.Name.LocalName, "CachedCopy")); // the copy exists for all elements but is saved only for those that contain other extensions

                var savedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var e in extensionElements)
                {
                    if (!e.ContainsSignificantInformation())
                        continue;

                    var name = e.Parent.Element(XmlExtensions.GpxSchema_1_1 + "name").GetValue();
                    if (string.IsNullOrWhiteSpace(name))
                        continue;

                    var iq = table.Insert();
                    iq.Value(o => o.CacheCode, name);
                    iq.Value(o => o.Data, e.ToString());
                    iq.Execute();

                    // save the copy of each edited cache only once
                    if (!savedCodes.Contains(name))
                    {
                        savedCodes.Add(name);

#warning Implement saving of cached copies again
                        //iq = table.Insert();
                        //iq.Value(o => o.CacheCode, name);
                        //iq.Value(o => o.Data, e.Parent.GetCachedCopy().Parent.ToString()); // .Parent makes sure that the extension element is saved, not the copy.
                        //iq.Execute();
                    }
                }

                scope.Commit();
            }
        }
    }
}
