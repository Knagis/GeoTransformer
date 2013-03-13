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
    /// A transformer that allows the user to put additional hints in the GPX file (either as geocache hints or if the waypoint does not
    /// have geocache extensions, the comment of the waypoint.
    /// </summary>
    public class AdditionalHints : TransformerBase, Extensions.IEditor
    {
        /// <summary>
        /// Initializes static members of the <see cref="AdditionalHints"/> class.
        /// </summary>
        static AdditionalHints()
        {
            EditorOnlineBackup.EditorOnlineBackup.RegisterForBackup(wpt =>
                {
                    var configElement = wpt.FindExtensionElement(typeof(AdditionalHints));
                    if (configElement == null)
                        return null;

                    if (string.IsNullOrWhiteSpace(configElement.Value))
                        return null;

                    return configElement.Value;
                });
        }
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
        /// Processes the specified GPX waypoint.
        /// </summary>
        /// <param name="waypoint">The waypoint that has to be processed.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        protected override void Process(Gpx.GpxWaypoint waypoint, TransformerOptions options)
        {
            var ext = waypoint.FindExtensionElement(typeof(AdditionalHints));
            if (ext == null)
                return;

            if (string.IsNullOrWhiteSpace(ext.Value))
                return;

            if (waypoint.Geocache.IsDefined())
            {
                if (string.IsNullOrWhiteSpace(waypoint.Geocache.Hints))
                    waypoint.Geocache.Hints = ext.Value;
                else
                    waypoint.Geocache.Hints = string.Concat(ext.Value, Environment.NewLine, "-----", Environment.NewLine, waypoint.Geocache.Hints);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(waypoint.Comment))
                    waypoint.Comment = ext.Value;
                else
                    waypoint.Comment = string.Concat(ext.Value, Environment.NewLine, "-----", Environment.NewLine, waypoint.Comment);
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
        /// Binds the control to the given GPX <paramref name="waypoint"/> object. For consequent calls the method removes previous bindings and sets up new ones.
        /// </summary>
        /// <param name="waypoint">The GPX waypoint object that will be edited by the control.</param>
        public void BindControl(Gpx.GpxWaypoint waypoint)
        {
            this._editorControl.BoundElement = waypoint == null ? null : waypoint.FindExtensionElement(typeof(AdditionalHints), true);
        }
    }
}
