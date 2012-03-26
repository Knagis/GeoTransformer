/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;

namespace GeoTransformer.Extensions
{
    /// <summary>
    /// An interface that enables the extension to provide controls for the waypoint editor.
    /// </summary>
    public interface IEditor
    {
        /// <summary>
        /// Creates the control that is used to edit the data for the waypoints. Note that the same control is reused for all caches.
        /// </summary>
        /// <returns>The user interface editor control.</returns>
        System.Windows.Forms.Control CreateControl();

        /// <summary>
        /// Binds the control to the given GPX <paramref name="waypoint"/> object. For consequent calls the method removes previous bindings and sets up new ones.
        /// </summary>
        /// <param name="waypoint">The GPX waypoint object that will be edited by the control.</param>
        void BindControl(Gpx.GpxWaypoint waypoint);
    }
}
