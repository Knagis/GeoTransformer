/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoTransformer.Extensions;

namespace GeoTransformer.Transformers.ManualPublish
{
    /// <summary>
    /// An editor for geocaches that enable the user to control whether the cache is published to GPS - user can force a cache not to be included
    /// even if it is loaded or force a cached copy to be published even if it is not loaded from sources at the time.
    /// </summary>
    public class ManualPublish : TransformerBase, IEditor
    {
        private EditorControl _editorControl;

        /// <summary>
        /// Creates the control that is used to edit the data for the waypoints. Note that the same control is reused for all caches.
        /// </summary>
        /// <returns>
        /// The user interface editor control.
        /// </returns>
        public System.Windows.Forms.Control CreateControl()
        {
            return this._editorControl = new EditorControl();
        }

        /// <summary>
        /// Binds the control to the given GPX <paramref name="waypoint"/> object. For consequent calls the method removes previous bindings and sets up new ones.
        /// </summary>
        /// <param name="waypoint">The GPX waypoint object that will be edited by the control.</param>
        public void BindControl(Gpx.GpxWaypoint waypoint)
        {
            this._editorControl.BoundElement = waypoint == null ? null : waypoint.FindExtensionElement(typeof(ManualPublish), true);
        }

        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Apply manual publish settings"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.BeforePublish; }
        }

        /// <summary>
        /// Counter for how many waypoints have been removed by the transformer.
        /// </summary>
        private int _removed;

        /// <summary>
        /// Counter for how many additional waypoints have been removed.
        /// </summary>
        private int _removedAdditionalWaypoints;

        /// <summary>
        /// Counter for how many waypoints have been published from cached copies.
        /// </summary>
        private int _published;

        /// <summary>
        /// The transformer will populate this with the cache IDs that were removed. It will then perform a second pass and
        /// remove any additional waypoints with matching names.
        /// </summary>
        private HashSet<string> _pendingWaypointRemove = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Processes the specified GPX documents. If the method is not overriden in the derived class,
        /// calls <see cref="TransformerBase.Process(Gpx.GpxDocument, Transformers.TransformerOptions)"/> for each document in the list.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
            // do not perform any clean-up for the viewer cache
            if ((options & TransformerOptions.LoadingViewerCache) == TransformerOptions.LoadingViewerCache)
                return;

            // reset counters
            this._removed = 0;
            this._published = 0;
            this._removedAdditionalWaypoints = 0;
            
            // just in case the extension failed last time and did not clear the collection
            this._pendingWaypointRemove.Clear(); 

            base.Process(documents, options);

            if (this._removed > 0)
            {
                this.ReportStatus("Removing additional waypoints");
                this.RemoveAdditionalWaypoints(documents);
            }

            this.ReportStatus("Forced publish: {0}. Forced skip: {1}.", this._published, this._removed);

            this._pendingWaypointRemove.Clear();
        }

        /// <summary>
        /// Performs another pass through the documents and removes any waypoints that are associated with removed caches.
        /// </summary>
        private void RemoveAdditionalWaypoints(IList<Gpx.GpxDocument> documents)
        {
            foreach (var doc in documents)
            {
                for (int i = doc.Waypoints.Count - 1; i >= 0; i--)
                {
                    var name = doc.Waypoints[i].Name;
                    if (name != null && name.Length > 2 && this._pendingWaypointRemove.Contains(name.Substring(2)))
                    {
                        this._removedAdditionalWaypoints++;
                        doc.Waypoints.RemoveAt(i);
                    }
                }                
            }
        }

        /// <summary>
        /// Processes the specified GPX document. If the method is not overriden in the derived class,
        /// calls <see cref="TransformerBase.Process(Gpx.GpxWaypoint, Transformers.TransformerOptions)"/> for each waypoint in the document.
        /// </summary>
        /// <param name="document">The document that has to be processed.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        protected override void Process(Gpx.GpxDocument document, TransformerOptions options)
        {
            for (int i = document.Waypoints.Count - 1; i >= 0; i--)
            {
                var wpt = document.Waypoints[i];
                ManualPublishMode manualMode;
                Enum.TryParse(wpt.FindExtensionElement(typeof(ManualPublish)).GetValue(), out manualMode);
                var editorOnly = string.Equals(bool.TrueString, wpt.FindExtensionAttributeValue("EditorOnly"), StringComparison.OrdinalIgnoreCase);

                if (manualMode == ManualPublishMode.AlwaysPublish && editorOnly)
                {
                    // reset the flag so the waypoint does not get removed
                    // no need to actually remove the attribute from the waypoint as it will not be published using default settings
                    editorOnly = false;
                    
                    _published++;
                }

                if (manualMode == ManualPublishMode.AlwaysSkip || editorOnly)
                {
                    // count only those that would otherwise be published
                    if (!editorOnly)
                        this._removed++;

                    // a geocache may have additional waypoints that are identified by comparing the names (except for first 2 chars)
                    if (wpt.Name != null && wpt.Name.Length > 2 && wpt.Geocache.IsDefined())
                        this._pendingWaypointRemove.Add(wpt.Name.Substring(2));

                    document.Waypoints.RemoveAt(i);
                }
            }
        }
    }
}
