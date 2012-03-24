/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoTransformer.Transformers
{
    /// <summary>
    /// A base class for transformers providing some basic implementations for <see cref="Extensions.ITransformer"/> interface.
    /// </summary>
    public abstract class TransformerBase : Extensions.ITransformer
    {
        /// <summary>
        /// Stores a value indication whether <see cref="CancelExecution"/> method has been called.
        /// </summary>
        private bool _cancelExecutionCalled;

        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Occurs when the transformer has a new status message to report to the user.
        /// </summary>
        public event EventHandler<StatusMessageEventArgs> StatusUpdate;

        /// <summary>
        /// Occurs when the transformer progress has changed.
        /// </summary>
        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public abstract ExecutionOrder ExecutionOrder { get; }

        /// <summary>
        /// Reports the current status of the execution to the user.
        /// </summary>
        /// <param name="severity">The severity of the message.</param>
        /// <param name="message">The message text.</param>
        /// <param name="args">The arguments to pass to <see cref="String.Format"/> function; if specified, the <paramref name="message"/> is considered
        /// to contain the formatting template.</param>
        protected virtual void ReportStatus(StatusSeverity severity, string message, params object[] args)
        {
            var h = this.StatusUpdate;
            if (h != null)
                h(this, new StatusMessageEventArgs(severity, message, args));
        }

        /// <summary>
        /// Reports the current status of the execution to the user. The message severity is set to <see cref="StatusSeverity.Information"/>.
        /// </summary>
        /// <param name="message">The message text.</param>
        /// <param name="args">The arguments to pass to <see cref="String.Format"/> function; if specified, the <paramref name="message"/> is considered
        /// to contain the formatting template.</param>
        protected void ReportStatus(string message, params object[] args)
        {
            this.ReportStatus(StatusSeverity.Information, message, args);
        }

        /// <summary>
        /// Reports the progress of the transformer execution to the user.
        /// </summary>
        /// <param name="initial">The initial value.</param>
        /// <param name="current">The current value. Automatically forced to be between <paramref name="initial"/> and <paramref name="maximum"/>.</param>
        /// <param name="maximum">The maximum value. Must be greater than or equal to <paramref name="initial"/>.</param>
        /// <exception cref="ArgumentException">when <paramref name="maximum"/> is smaller than <paramref name="initial"/>.</exception>
        protected virtual void ReportProgress(decimal initial, decimal current, decimal maximum)
        {
            var h = this.ProgressUpdate;
            if (h != null)
                h(this, new ProgressUpdateEventArgs(initial, current, maximum));
        }

        /// <summary>
        /// Reports the progress of the transformer execution to the user.
        /// </summary>
        /// <param name="current">The current value. Automatically forced to be between <c>zero</c> and <paramref name="maximum"/>.</param>
        /// <param name="maximum">The maximum value. Must be greater than or equal to <c>zero</c>.</param>
        /// <exception cref="ArgumentException">when <paramref name="maximum"/> is smaller than <c>zero</c>.</exception>
        protected void ReportProgress(decimal current, decimal maximum)
        {
            this.ReportProgress(0m, current, maximum);
        }

        /// <summary>
        /// Holds a value how many GPX waypoints have already been processed in previous documents in regards to
        /// the current <see cref="Process(Gpx.GpxDocument)"/> call.
        /// </summary>
        private int _waypointsAlreadyProcessed;

        /// <summary>
        /// Holds a value how many GPX waypoints were provided in the parameter to <see cref="Process(IList`1, Transformers.TransformerOptions)"/>.
        /// </summary>
        private int _waypointsTotal;

        /// <summary>
        /// Holds a value if <see cref="Process(Gpx.GpxDocument, Transformers.TransformerOptions)"/> report progress update
        /// after processing certain number of waypoints. This is enabled only when the default implementation of 
        /// <paramref name="Process(IList`1, Transformers.TransformerOptions)"/> is used.
        /// </summary>
        private bool _reportWaypointProgress;

        /// <summary>
        /// Processes the specified GPX documents. If the method is not overriden in the derived class,
        /// calls <see cref="Process(Gpx.GpxDocument, Transformers.TransformerOptions)"/> for each document in the list.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public virtual void Process(IList<Gpx.GpxDocument> documents, Transformers.TransformerOptions options)
        {
            this._reportWaypointProgress = true;
            this._waypointsTotal = documents.Sum(o => o.Waypoints.Count);
            this._waypointsAlreadyProcessed = 0;
            foreach (var gpx in documents)
            {
                this.ReportStatus("Processing file '{0}'.", gpx.Metadata.OriginalFileName ?? gpx.Metadata.Name);
                this.Process(gpx, options);

                this._waypointsAlreadyProcessed += gpx.Waypoints.Count;
                this.ReportProgress(this._waypointsAlreadyProcessed, this._waypointsTotal);

                if (this._waypointsAlreadyProcessed != this._waypointsTotal) this.TerminateExecutionIfNeeded();
            }

            this.ReportStatus(string.Empty);
        }

        /// <summary>
        /// Processes the specified GPX document. If the method is not overriden in the derived class,
        /// calls <see cref="Process(Gpx.GpxWaypoint, Transformers.TransformerOptions)"/> for each waypoint in the document.
        /// </summary>
        /// <param name="document">The document that has to be processed.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        protected virtual void Process(Gpx.GpxDocument document, Transformers.TransformerOptions options)
        {
            var currentlyProcessed = 0;
            foreach (var wpt in document.Waypoints)
            {
                this.Process(wpt, options);
                currentlyProcessed++;

                // do not spend the extra time each iteration.
                if (currentlyProcessed % 10 != 0)
                    continue;

                if (this._reportWaypointProgress)
                {
                    this.ReportProgress(this._waypointsAlreadyProcessed + currentlyProcessed, this._waypointsTotal);
                    if (this._waypointsAlreadyProcessed + currentlyProcessed != this._waypointsTotal)
                        this.TerminateExecutionIfNeeded();
                }
                else if (currentlyProcessed != document.Waypoints.Count)
                {
                    this.TerminateExecutionIfNeeded();
                }
            }
        }

        /// <summary>
        /// Processes the specified GPX waypoint.
        /// </summary>
        /// <param name="waypoint">The waypoint that has to be processed.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        protected virtual void Process(Gpx.GpxWaypoint waypoint, Transformers.TransformerOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if <see cref="Extensions.ITransformer.CancelExecution"/> is called and if it is then stops the execution of the transformer.
        /// </summary>
        protected void TerminateExecutionIfNeeded()
        {
            if (this._cancelExecutionCalled)
                this.ReportStatus(StatusSeverity.FatalError, "User cancelled");
        }

        /// <summary>
        /// Notifies the transformer that the <see cref="Execute"/> method should be now terminated. This is called from a separate thread by the controlling form.
        /// Note that the implementing method has to support multiple calls to <see cref="Process"/> method on the same instance so the cancellation
        /// has to leave the instance in valid state.
        /// </summary>
        void Extensions.ITransformer.CancelExecution()
        {
            this._cancelExecutionCalled = true;
        }

        /// <summary>
        /// Processes the specified GPX documents.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        void Extensions.ITransformer.Process(IList<Gpx.GpxDocument> documents, Transformers.TransformerOptions options)
        {
            this._cancelExecutionCalled = false;

            this.Process(documents, options);
        }
    }
}
