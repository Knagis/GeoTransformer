/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Transformers.MarkSolved
{
    /// <summary>
    /// Transformer that can change the icon of the cache to mark it as solved on the GPS
    /// </summary>
    public class MarkSolved : TransformerBase, Extensions.IEditor
    {
        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Mark as solved"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.Process; }
        }

        /// <summary>
        /// Processes the specified input document.
        /// </summary>
        public override void Process(System.Xml.Linq.XDocument xml)
        {
            foreach (var wpt in xml.Root.WaypointElements("wpt"))
            {
                var configElement = wpt.ExtensionElement(typeof(MarkSolved));
                if (configElement == null)
                    continue;

                bool val;
                if (!bool.TryParse(configElement.Value, out val) || !val)
                    continue;

                wpt.SetElementValue(wpt.Name.Namespace + "type", "Geocache|Solved Cache");
                var gc = wpt.CacheElement("cache");
                if (gc != null)
                {
                    gc = gc.CacheElement("type");
                    if (gc != null)
                        gc.Value = "Solved Cache";
                }
            }
        }

        private EditorControl _editorControl;
        /// <summary>
        /// Creates the control that is used to edit the data for the caches. Note that the same control is reused for all caches.
        /// </summary>
        /// <returns>
        /// The user interface editor control.
        /// </returns>
        public System.Windows.Forms.Control CreateControl()
        {
            return this._editorControl = new EditorControl();
        }

        /// <summary>
        /// Binds the control to the given XML <paramref name="data"/> object. For consequent calls the method removes previous bindings and sets up new ones.
        /// </summary>
        /// <param name="data">The data object.</param>
        public void BindControl(System.Xml.Linq.XElement data)
        {
            this._editorControl.BoundElement = data;
        }
    }
}
