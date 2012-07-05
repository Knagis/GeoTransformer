/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;

namespace GeoTransformer.Transformers
{
    /// <summary>
    /// The interface that contains additional context information provided for the transformer execution.
    /// </summary>
    public interface IExecutionContext
    {
        /// <summary>
        /// Reports the current status of the execution to the user.
        /// </summary>
        /// <param name="severity">The severity of the message.</param>
        /// <param name="message">The message text.</param>
        /// <param name="args">The arguments to pass to <see cref="String.Format(string, object[])"/> function; if specified, the <paramref name="message"/> is considered
        /// to contain the formatting template.</param>
        void ReportStatus(StatusSeverity severity, string message, params object[] args);

        /// <summary>
        /// Reports the progress of the transformer execution to the user.
        /// </summary>
        /// <param name="initial">The initial value.</param>
        /// <param name="current">The current value. Automatically forced to be between <paramref name="initial"/> and <paramref name="maximum"/>.</param>
        /// <param name="maximum">The maximum value. Must be greater than or equal to <paramref name="initial"/>.</param>
        /// <param name="skipSupported">Specifies if the transformer supports skipping the long running operation. 
        /// To support the skipping the code must handle <see cref="TransformerCancelledException"/> exception thrown
        /// by <see cref="ThrowIfCancellationPending"/> method.</param>
        /// <exception cref="ArgumentException">when <paramref name="maximum"/> is smaller than <paramref name="initial"/>.</exception>
        void ReportProgress(decimal initial, decimal current, decimal maximum, bool skipSupported = false);

        /// <summary>
        /// Reports that the long running operation has finished and the progress bar can be hidden.
        /// </summary>
        void ReportProgressFinished();

        /// <summary>
        /// Throws a <see cref="TransformerCancelledException"/> exception if the engine has requested the transformer to cancel execution.
        /// </summary>
        void ThrowIfCancellationPending();

        /// <summary>
        /// Gets the options that instruct how the transformer should proceed.
        /// </summary>
        TransformerOptions Options { get; }

        /// <summary>
        /// Sets the parent window that should be used if the transformer has to show a dialog user interface.
        /// </summary>
        System.Windows.Forms.Form ParentWindow { get; }
    }
}
