/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoTransformer.Extensions;

namespace GeoTransformer.Transformers.CreateViewerCache
{
    /// <summary>
    /// A special, internal transformer that enables the main form to retrieve the GPX data from the transformer queue.
    /// </summary>
    internal class CreateViewerCache : TransformerBase, ISpecial
    {
        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Set in-memory copy for viewers"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.CreateViewerCache; }
        }

        /// <summary>
        /// Processes the specified GPX documents. If the method is not overriden in the derived class,
        /// calls <see cref="Process(Gpx.GpxDocument, Transformers.TransformerOptions)"/> for each document in the list.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
            var list = new List<System.Xml.Linq.XDocument>();

            foreach (var gpx in documents)
                list.Add(gpx.Serialize(Gpx.GpxSerializationOptions.Default));

            this.Data = list;
        }

        /// <summary>
        /// Gets the list of GPX documents that were captured by this transformer.
        /// </summary>
        public IList<System.Xml.Linq.XDocument> Data { get; private set; }
    }
}
