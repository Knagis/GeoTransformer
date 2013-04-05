/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoTransformer.Transformers.UpdateCoordinates
{
    /// <summary>
    /// Edit control for <see cref="UpdateCoordinates"/> extension.
    /// </summary>
    public partial class EditorControl : UI.UserControlBase
    {
        private Gpx.GpxWaypoint _waypoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorControl"/> class.
        /// </summary>
        public EditorControl()
        {
            InitializeComponent();

            this.coordinateEditor1.CoordinatesChanged += this.CoordinatesChanged;
        }

        /// <summary>
        /// Handles the CoordinatesChanged event of the coordinate editor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CoordinatesChanged(object sender, EventArgs e)
        {
            var wpt = this.BoundElement;
            if (wpt == null)
                return;

            if (!this.coordinateEditor1.Coordinates.HasValue)
            {
                wpt.Coordinates = wpt.OriginalValues.Coordinates;
            }
            else
            {
                wpt.Coordinates = this.coordinateEditor1.Coordinates.Value;
            }
        }

        /// <summary>
        /// Gets or sets the element that is modified by this editor control.
        /// </summary>
        public Gpx.GpxWaypoint BoundElement 
        {
            get
            {
                return this._waypoint;
            }

            set
            {
                this._waypoint = value;
                this.coordinateEditor1.BoundElement = this._waypoint == null ? null : this._waypoint.FindExtensionElement(typeof(UpdateCoordinates), true);
            }
        }
    }
}
