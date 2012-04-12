/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Viewers
{
    /// <summary>
    /// The arguments for the event when the currently selected waypoint is changed in the <see cref="Extensions.IWaypointListViewer"/>.
    /// </summary>
    public class SelectedWaypointsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedWaypointsChangedEventArgs"/> class.
        /// </summary>
        /// <param name="waypoint">The first of selected GPX waypoints. Use <c>null</c> if the selection was cleared.</param>
        public SelectedWaypointsChangedEventArgs(Gpx.GpxWaypoint waypoint)
        {
            if (waypoint == null)
                this.Selection = new System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint>(new Gpx.GpxWaypoint[0]);
            else
                this.Selection = new System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint>(new Gpx.GpxWaypoint[] { waypoint });
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedWaypointsChangedEventArgs"/> class.
        /// </summary>
        /// <param name="waypoints">The GPX waypoints that are now selected. This data will be passed to the individual cache viewers. An empty array if the selection was cleared.</param>
        public SelectedWaypointsChangedEventArgs(params Gpx.GpxWaypoint[] waypoints)
        {
            if (waypoints == null)
                this.Selection = new System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint>(new Gpx.GpxWaypoint[0]);
            else
            {
                foreach (var x in waypoints)
                    if (x == null)
                        throw new ArgumentNullException("waypoints");

                this.Selection = new System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint>(waypoints);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedWaypointsChangedEventArgs"/> class.
        /// </summary>
        /// <param name="waypoints">The GPX waypoints that are now selected. This data will be passed to the individual cache viewers. Use <c>null</c> or an empty array if the selection was cleared.</param>
        public SelectedWaypointsChangedEventArgs(IEnumerable<Gpx.GpxWaypoint> waypoints)
        {
            if (waypoints == null)
                this.Selection = new System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint>(new Gpx.GpxWaypoint[0]);
            else
                this.Selection = new System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint>(waypoints.ToList());
        }

        /// <summary>
        /// Gets the GPX waypoints that are currently selected.
        /// </summary>
        public System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint> Selection { get; private set; }
    }
}
