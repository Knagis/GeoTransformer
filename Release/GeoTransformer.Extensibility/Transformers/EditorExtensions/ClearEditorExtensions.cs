/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Transformers.EditorExtensions
{
    /// <summary>
    /// Transformer that removes the temporary editor values from the XML document.
    /// </summary>
    public class ClearEditorExtensions : TransformerBase
    {
        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Remove temporary editor values"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.Publish - 500; }
        }

        /// <summary>
        /// Processes the specified input document.
        /// </summary>
        public override void Process(System.Xml.Linq.XDocument xml)
        {
            foreach (var elem in xml.Root.WaypointElements("wpt").SelectMany(o => o.Elements()).ToList())
            {
                if (string.Equals(elem.Name.NamespaceName, XmlExtensions.GeoTransformerSchemaClean, StringComparison.Ordinal))
                    elem.Remove();
            }
            foreach (var attr in xml.Root.WaypointElements("wpt").SelectMany(o => o.Attributes()).ToList())
            {
                if (string.Equals(attr.Name.NamespaceName, XmlExtensions.GeoTransformerSchemaClean, StringComparison.Ordinal))
                    attr.Remove();
            }
        }
    }
}
