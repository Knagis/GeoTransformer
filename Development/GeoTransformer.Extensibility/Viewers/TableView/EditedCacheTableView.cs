/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Viewers.TableView
{
    /// <summary>
    /// Table view that displays only those geocaches that have been modified by the user.
    /// </summary>
    public class EditedCacheTableView : TableView
    {
        /// <summary>
        /// Gets the icon to be displayed on the button.
        /// </summary>
        public override System.Drawing.Image ButtonImage
        {
            get { return Resources.Edited; }
        }

        /// <summary>
        /// Gets the text to be displayed on the button.
        /// </summary>
        public override string ButtonText
        {
            get { return "Edited caches"; }
        }

        /// <summary>
        /// Prepares the list view for display - sets common settings and adds any columns to <see cref="BrightIdeasSoftware.ObjectListView.AllColumns"/>
        /// collection.
        /// </summary>
        /// <param name="olv">The control that needs to be initialized.</param>
        protected override void PrepareListView(BrightIdeasSoftware.ObjectListView olv)
        {
            Columns.InitializeListView(olv);
            Columns.CreateColumns(olv, true);
        }

        /// <summary>
        /// Filters the data and returns only the waypoints that has to be displayed on the list view.
        /// </summary>
        /// <param name="data">A list of GPX documents containing the cache information.</param>
        /// <returns>
        /// List of waypoints that will be displayed.
        /// </returns>
        protected override IEnumerable<Gpx.GpxWaypoint> FilterData(IEnumerable<Gpx.GpxDocument> data)
        {
            return data.SelectMany(o => o.Waypoints)
                .Where(o => o.Geocache.IsDefined()
                && (string.Equals(bool.TrueString, o.FindExtensionAttributeValue("EditorOnly"), StringComparison.OrdinalIgnoreCase)
                   || o.ExtensionElements.Any(e => e.Name.Namespace == XmlExtensions.GeoTransformerSchema 
                                                    && e.ContainsSignificantInformation())));
        }
    }
}
