/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Transformers.UpdateCoordinates
{
    public class UpdateCoordinates : TransformerBase, Extensions.IEditor
    {
        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Update coordinates"; }
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
            var configElement = waypoint.FindExtensionElement(typeof(UpdateCoordinates));
            if (configElement == null)
                return;

            // the control sets the values, let's rely on it to parse it as well
            var coords = UI.CoordinateEditor.ReadXmlConfiguration(configElement);
            if (!coords.HasValue)
                return;

            waypoint.Coordinates = coords.Value;
        }

        #region [ IEditor ]

        private EditorControl _editorControl;

        /// <summary>
        /// Creates the control that is used to edit the data for the waypoints. Note that the same control is reused for all caches.
        /// </summary>
        /// <returns>
        /// The user interface editor control.
        /// </returns>
        public System.Windows.Forms.Control CreateControl()
        {
            this._editorControl = new EditorControl();
            return this._editorControl;
        }

        /// <summary>
        /// Binds the control to the given GPX <paramref name="waypoint"/> object. For consequent calls the method removes previous bindings and sets up new ones.
        /// </summary>
        /// <param name="waypoint">The GPX waypoint object that will be edited by the control.</param>
        public void BindControl(Gpx.GpxWaypoint waypoint)
        {
            this._editorControl.BoundElement = waypoint;
        }

        #endregion

    }
}
