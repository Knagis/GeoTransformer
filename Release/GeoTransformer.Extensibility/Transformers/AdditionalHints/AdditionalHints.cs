/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Transformers.AdditionalHints
{
    /// <summary>
    /// A transformer that allows the user to put additional hints in the GPX file.
    /// </summary>
    public class AdditionalHints : TransformerBase, Extensions.IEditor
    {
        private EditorControl _editorControl;

        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Add additional hints"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get { return ExecutionOrder.Process; }
        }

        /// <summary>
        /// Processes the specified input document.
        /// </summary>
        /// <param name="xml"></param>
        public override void Process(System.Xml.Linq.XDocument xml)
        {
            foreach (var element in xml.ExtensionElements(typeof(AdditionalHints)))
            {
                if (string.IsNullOrWhiteSpace(element.Value))
                    continue;

                var cache = element.Parent.CacheElement("cache");
                if (cache == null)
                    continue;

                var hints = cache.CacheElement("encoded_hints");
                if (hints == null)
                    cache.Add(hints = new System.Xml.Linq.XElement(System.Xml.Linq.XName.Get("encoded_hints", cache.Name.NamespaceName)));

                hints.Value = string.Concat(element.Value, Environment.NewLine, "-----", Environment.NewLine, hints.Value);
            }
        }

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
