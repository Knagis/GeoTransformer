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
        /// Occurs when the transformer has a new status message to report to the user.
        /// </summary>
        event EventHandler<Transformers.StatusMessageEventArgs> StatusUpdate;

        /// <summary>
        /// Occurs when the transformer progress has changed.
        /// </summary>
        event EventHandler<Transformers.ProgressUpdateEventArgs> ProgressUpdate;

        /// <summary>
        /// Notifies the transformer that the <see cref="Execute"/> method should be now terminated. This is called from a separate thread by the controlling form.
        /// Note that the implementing method has to support multiple calls to <see cref="Process"/> method on the same instance so the cancellation
        /// has to leave the instance in valid state.
        /// </summary>
        void CancelExecution();

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        Transformers.ExecutionOrder ExecutionOrder { get; }

        /// <summary>
        /// Processes the specified GPX documents.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        void Process(IList<Gpx.GpxDocument> documents, Transformers.TransformerOptions options);
    }
}
