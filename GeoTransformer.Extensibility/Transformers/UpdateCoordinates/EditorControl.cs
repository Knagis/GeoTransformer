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
