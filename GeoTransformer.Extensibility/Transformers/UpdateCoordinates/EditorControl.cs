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
    public partial class EditorControl : UI.UserControlBase
    {
        private Gpx.GpxWaypoint _waypoint;

        public EditorControl()
        {
            InitializeComponent();
        }

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
