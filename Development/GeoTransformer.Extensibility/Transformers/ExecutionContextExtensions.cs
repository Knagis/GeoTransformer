/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;

namespace GeoTransformer.Transformers
{
    /// <summary>
    /// Extension methods for the <see cref="IExecutionContext"/> interface.
    /// </summary>
    public static class ExecutionContextExtensions
    {
        /// <summary>
        /// Reports the current status of the execution to the user.
        /// </summary>
        /// <param name="context">The execution context instance.</param>
        /// <param name="message">The message text.</param>
        /// <param name="args">The arguments to pass to <see cref="String.Format(string, object[])"/> function; if specified, the <paramref name="message"/> is considered
        /// to contain the formatting template.</param>
        public static void ReportStatus(this IExecutionContext context, string message, params object[] args)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.ReportStatus(StatusSeverity.Information, message, args);
        }

        /// <summary>
        /// Reports the progress of the transformer execution to the user.
        /// </summary>
        /// <param name="context">The execution context instance.</param>
        /// <param name="current">The current value. Automatically forced to be between <c>0</c> and <paramref name="maximum"/>.</param>
        /// <param name="maximum">The maximum value. Must be greater than or equal to <c>0</c>.</param>
        /// <param name="skipSupported">Specifies if the transformer supports skipping the long running operation. 
        /// To support the skipping the code must handle <see cref="TransformerCancelledException"/> exception thrown
        /// by <see cref="ThrowIfCancellationPending"/> method.</param>
        /// <exception cref="ArgumentException">when <paramref name="maximum"/> is smaller than <c>0</c>.</exception>
        public static void ReportProgress(this IExecutionContext context, decimal current, decimal maximum, bool skipSupported = false)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.ReportProgress(0, current, maximum, skipSupported);
        }
    }
}
