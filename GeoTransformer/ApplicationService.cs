﻿/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer
{
    /// <summary>
    /// Contains the implementation of the application service interface that provides ways for the extensions to interact with the main application.
    /// </summary>
    internal class ApplicationService : Extensions.IApplicationService
    {
        private MainForm _form;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationService"/> class.
        /// </summary>
        /// <param name="form">The application's main form.</param>
        internal ApplicationService(MainForm form)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            this._form = form;
        }

        /// <summary>
        /// Refreshes the whole displayed cache list. Should be called when the list contents are changed (a cache is removed or added to it).
        /// Note that this method does not refresh the cache list from the sources.
        /// </summary>
        public void RefreshCacheList()
        {
            new System.Threading.Thread((a) => this._form.listViewers.LoadListViewerData((bool)a)).Start(false);
        }

        /// <summary>
        /// The GPX document into which the new placeholder elements are injected.
        /// </summary>
        private Gpx.GpxDocument _PlaceholderCacheTarget;

        /// <summary>
        /// Instructs the cache list and individual view to select a cache with a particular code (the value of the GPX <c>name</c> element).
        /// If the code cannot be found, the selection will be cleared or a new blank placeholder will be created depending on the <paramref name="selectForEditing"/> parameter.
        /// </summary>
        /// <param name="name">The name of the waypoint that will be selected.</param>
        /// <param name="selectForEditing">If set to <c>true</c> then an empty placeholder cache will be created for the editor.</param>
        public void SelectWaypoint(string name, bool selectForEditing)
        {
            this._form.listViewers.ChangeSelectedWaypoint(name);

            if (string.IsNullOrWhiteSpace(name))
            {
                this._form.cacheViewers.DisplayWaypoints(new System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint>(new Gpx.GpxWaypoint[0]));
                return;
            }

            var data = this._form.listViewers.LoadedGpxFiles;
            if (data == null)
            {
                if (selectForEditing)
                    System.Windows.Forms.MessageBox.Show("The geocaches are still being loaded. Please wait until that finishes and then try again.");

                this._form.cacheViewers.DisplayWaypoints(null);
                return;
            }

            var waypoint = data.SelectMany(o => o.Waypoints)
                               .FirstOrDefault(o => string.Equals(o.Name, name, StringComparison.OrdinalIgnoreCase));

            if (waypoint == null && selectForEditing)
            {
                // TODO: this should be job for the extensions, not the main app
                if (GeocachingService.LiveClient.IsEnabled && name.StartsWith("GC", StringComparison.OrdinalIgnoreCase))
                {
                    using (var service = GeocachingService.LiveClient.CreateClientProxy())
                    {
                        var online = service.GetGeocacheByCode(name);
                        if (online != null)
                            waypoint = new Gpx.GpxWaypoint(online);
                        else
                            System.Windows.Forms.MessageBox.Show("geocaching.com Live API did not recognize the cache code " + name + ". An empty placeholder will be created but you should double check if the code is correct.", "Geocache retrieval", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    }
                }

                // if the API did not load the cache (or it was not from geocaching.com), fall back to an empty placeholder
                if (waypoint == null)
                    waypoint = new Gpx.GpxWaypoint();

                waypoint.ExtensionAttributes.Add(new System.Xml.Linq.XAttribute(XmlExtensions.GeoTransformerSchema + "EditorOnly", true));
                if (this._PlaceholderCacheTarget == null || !data.Contains(this._PlaceholderCacheTarget))
                    data.Add(this._PlaceholderCacheTarget = new Gpx.GpxDocument());

                this._PlaceholderCacheTarget.Waypoints.Add(waypoint);
            }

            if (waypoint == null)
                this._form.cacheViewers.DisplayWaypoints(new System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint>(new Gpx.GpxWaypoint[0]));
            else
                this._form.cacheViewers.DisplayWaypoints(new System.Collections.ObjectModel.ReadOnlyCollection<Gpx.GpxWaypoint>(new Gpx.GpxWaypoint[] { waypoint }));
        }

        /// <summary>
        /// Retrieves the caches that are currently being displayed in the main form.
        /// </summary>
        /// <returns>
        /// The GPX documents that are currently displayed or <c>null</c> if the loading of caches is not yet completed.
        /// </returns>
        public IEnumerable<Gpx.GpxDocument> RetrieveDisplayedCaches()
        {
            return this._form.listViewers.LoadedGpxFiles;
        }
    }
}
