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
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public abstract ExecutionOrder ExecutionOrder { get; }

        /// <summary>
        /// Gets the execution context. <c>Null</c> if the transformer is not currently being executed by the engine.
        /// </summary>
        protected IExecutionContext ExecutionContext { get; private set; }

        /// <summary>
        /// Holds a value how many GPX waypoints have already been processed in previous documents in regards to
        /// the current <see cref="Process(Gpx.GpxDocument, Transformers.TransformerOptions)"/> call.
        /// </summary>
        private int _waypointsAlreadyProcessed;

        /// <summary>
        /// Holds a value how many GPX waypoints were provided in the parameter to <see cref="Process(IList{Gpx.GpxDocument}, Transformers.TransformerOptions)"/>.
        /// </summary>
        private int _waypointsTotal;

        /// <summary>
        /// Holds the timer that is set if <see cref="Process(Gpx.GpxDocument, Transformers.TransformerOptions)"/> 
        /// reports progress update after processing certain number of waypoints. This is enabled only when the 
        /// default implementation of <see name="Process(IList{Gpx.GpxDocument}, Transformers.TransformerOptions)"/> 
        /// is used. The progress bar is only shown if the process runs for more than 1 second.
        /// </summary>
        private System.Diagnostics.Stopwatch _reportWaypointProgress;

        /// <summary>
        /// Processes the specified GPX documents. If the method is not overriden in the derived class,
        /// calls <see cref="Process(Gpx.GpxDocument, Transformers.TransformerOptions)"/> for each document in the list.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public virtual void Process(IList<Gpx.GpxDocument> documents, Transformers.TransformerOptions options)
        {
            try
            {
                this._reportWaypointProgress = new System.Diagnostics.Stopwatch();
                this._reportWaypointProgress.Start();
                this._waypointsTotal = documents.Sum(o => o.Waypoints.Count);
                this._waypointsAlreadyProcessed = 0;
                foreach (var gpx in documents)
                {
                    this.ExecutionContext.ReportStatus("Processing file '{0}'.", gpx.Metadata.OriginalFileName ?? gpx.Metadata.Name);
                    this.Process(gpx, options);

                    this._waypointsAlreadyProcessed += gpx.Waypoints.Count;
                    if (this._reportWaypointProgress.ElapsedMilliseconds > 1000)
                        this.ExecutionContext.ReportProgress(this._waypointsAlreadyProcessed, this._waypointsTotal);

                    if (this._waypointsAlreadyProcessed != this._waypointsTotal)
                        this.ExecutionContext.ThrowIfCancellationPending();
                }

                this.ExecutionContext.ReportStatus(string.Empty);
            }
            finally
            {
                if (this._reportWaypointProgress != null)
                {
                    this._reportWaypointProgress.Stop();
                    this._reportWaypointProgress = null;
                }
            }
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

                if (this._reportWaypointProgress != null && this._reportWaypointProgress.ElapsedMilliseconds > 1000)
                {
                    this.ExecutionContext.ReportProgress(this._waypointsAlreadyProcessed + currentlyProcessed, this._waypointsTotal);
                    if (this._waypointsAlreadyProcessed + currentlyProcessed != this._waypointsTotal)
                        this.ExecutionContext.ThrowIfCancellationPending();
                }
                else if (currentlyProcessed != document.Waypoints.Count)
                {
                    this.ExecutionContext.ThrowIfCancellationPending();
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
        /// Processes the specified GPX documents.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="context">The context for the transformer execution.</param>
        void Extensions.ITransformer.Process(IList<Gpx.GpxDocument> documents, Transformers.IExecutionContext context)
        {
            try
            {
                this.ExecutionContext = context;
                this.Process(documents, context.Options);
            }
            finally
            {
                this.ExecutionContext = null;
            }
        }
    }
}
