/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Extensions
{
    /// <summary>
    /// The interface that provides the support for loading and processing GPX data.
    /// </summary>
    public interface ITransformer : IExtension
    {
        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        Transformers.ExecutionOrder ExecutionOrder { get; }

        /// <summary>
        /// Processes the specified GPX documents.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="context">The context for the transformer execution.</param>
        void Process(IList<Gpx.GpxDocument> documents, Transformers.IExecutionContext context);
    }
}
