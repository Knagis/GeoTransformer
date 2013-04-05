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
    /// Transformer that can change the icon of the cache to mark it as solved on the GPS. Current implementation sets a custom symbol that
    /// will get rendered using the generic geocache icon on the GPS.
    /// </summary>
    public class MarkSolved : TransformerBase, Extensions.IEditor
    {
        /// <summary>
        /// Initializes static members of the <see cref="MarkSolved"/> class.
        /// </summary>
        static MarkSolved()
        {
            EditorOnlineBackup.EditorOnlineBackup.RegisterForBackup(wpt =>
                {
                    var configElement = wpt.FindExtensionElement(typeof(MarkSolved));
                    if (configElement == null)
                        return null;

                    bool val;
                    if (!bool.TryParse(configElement.Value, out val) || !val)
                        return null;

                    return "Solved";
                });
        }

        private EditorControl _editorControl;

        private Gpx.GpxWaypoint _boundWaypoint;
        
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
        /// Processes the specified GPX waypoint.
        /// </summary>
        /// <param name="waypoint">The waypoint that has to be processed.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        protected override void Process(Gpx.GpxWaypoint waypoint, TransformerOptions options)
        {
            var configElement = waypoint.FindExtensionElement(typeof(MarkSolved));
            if (configElement == null)
                return;

            bool val;
            if (!bool.TryParse(configElement.Value, out val) || !val)
                return;

            UpdateWaypoint(waypoint, true);
        }

        /// <summary>
        /// Updates the waypoint by either setting the custom waypoint type or resetting it to the original value.
        /// </summary>
        /// <param name="waypoint">The waypoint that will be updated.</param>
        /// <param name="markAsSolved">if set to <c>true</c> marks the waypoint as solved, otherwise resets the original value.</param>
        private static void UpdateWaypoint(Gpx.GpxWaypoint waypoint, bool markAsSolved)
        {
            if (markAsSolved)
            {
                waypoint.WaypointType = "Geocache|Solved Cache";
                if (waypoint.Geocache.IsDefined())
                {
                    waypoint.Geocache.CacheType.Id = null;
                    waypoint.Geocache.CacheType.Name = "Solved Cache";
                }
            }
            else
            {
                waypoint.WaypointType = waypoint.OriginalValues.WaypointType;
                if (waypoint.Geocache.IsDefined())
                {
                    waypoint.Geocache.CacheType.Id = waypoint.OriginalValues.Geocache.CacheType.Id;
                    waypoint.Geocache.CacheType.Name = waypoint.OriginalValues.Geocache.CacheType.Name;
                }
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
            this._editorControl = new EditorControl();
            this._editorControl.ValueChanged += EditorControlValueChanged;
            return this._editorControl;
        }

        private void EditorControlValueChanged(object sender, EventArgs e)
        {
            if (this._boundWaypoint == null)
                return;

            var configElement = this._boundWaypoint.FindExtensionElement(typeof(MarkSolved), true);

            bool v = !string.IsNullOrEmpty(configElement.Value) && bool.Parse(configElement.Value);
            if (v == this._editorControl.Value)
                return;
                
            configElement.Value = this._editorControl.Value ? Boolean.TrueString : string.Empty;
            UpdateWaypoint(this._boundWaypoint, this._editorControl.Value);
        }

        /// <summary>
        /// Binds the control to the given GPX <paramref name="waypoint"/> object. For consequent calls the method removes previous bindings and sets up new ones.
        /// </summary>
        /// <param name="waypoint">The GPX waypoint object that will be edited by the control.</param>
        public void BindControl(Gpx.GpxWaypoint waypoint)
        {
            this._boundWaypoint = waypoint;

            if (waypoint == null)
                this._editorControl.Value = false;
            else
            {
                var configElement = waypoint.FindExtensionElement(typeof(MarkSolved));
                bool val;
                this._editorControl.Value = configElement != null && bool.TryParse(configElement.Value, out val) & val;
            }
        }
    }
}
