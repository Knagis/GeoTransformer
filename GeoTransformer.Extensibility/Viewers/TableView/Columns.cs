/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GeoTransformer.Gpx;

namespace GeoTransformer.Viewers.TableView
{
    /// <summary>
    /// Contains code for creating and manipulating columns in the <see cref="TableView"/>.
    /// </summary>
    internal static class Columns
    {
        public static void InitializeListView(BrightIdeasSoftware.ObjectListView olv)
        {
            var imgList = new System.Windows.Forms.ImageList();
            imgList.ImageSize = new System.Drawing.Size(16, 16);
            foreach (var x in typeof(UI.CacheTypeIcons).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
            {
                if (typeof(System.Drawing.Image).IsAssignableFrom(x.PropertyType))
                    imgList.Images.Add(x.Name, (System.Drawing.Image)x.GetValue(null, null));
            }
            olv.SmallImageList = imgList;

            olv.FullRowSelect = true;
            olv.UseFiltering = true;
            olv.UseFilterIndicator = true;
            olv.ShowGroups = true;
            olv.ShowImagesOnSubItems = true;
            olv.ShowCommandMenuOnRightClick = true;
            olv.SortGroupItemsByPrimaryColumn = false;
            olv.MultiSelect = false;
            olv.AllowColumnReorder = true;
            olv.HeaderUsesThemes = false;
            olv.HideSelection = false;
            olv.BorderStyle = System.Windows.Forms.BorderStyle.None;
        }

        #region [ Wrapper methods to display custom data in the view ]

        private static string CacheTypeImage(object obj)
        {
            var wpt = (GpxWaypoint)obj;
            if (wpt.Geocache.CacheType.Name == null)
                return null;

            return wpt.Geocache.CacheType.Name.Replace(" ", "").Replace("-","");
        }

        private static string StatusAspect(object obj)
        {
            var wpt = (GpxWaypoint)obj;
            if (wpt.Geocache.Archived)
                return "Archived";
            if (!wpt.Geocache.Available)
                return "Disabled";

            foreach (var attr in wpt.Geocache.Attributes)
                if (attr.IsInclusive && attr.Id == 42)
                    return "Needs maintenance";

            return "Active";
        }

        /// <summary>
        /// Creates the sortable value for the "Currently loaded" column.
        /// It is required so that the values are sorted in a specific order within one group.
        /// The sort order is important because otherwise the list gets re-ordered every time
        /// the user clicks "Remove customizations".
        /// </summary>
        private static string CurrentlyLoadedAspect(object obj)
        {
            var wpt = (GpxWaypoint)obj;

            return (wpt.FindExtensionAttributeValue("EditorOnly") ?? bool.FalseString).ToUpperInvariant() + wpt.Name;
        }

        /// <summary>
        /// Gets the value by which to group the rows when sorting by Currently Loaded column.
        /// </summary>
        private static object CurrentlyLoadedGroupKey(object obj)
        {
            var wpt = (GpxWaypoint)obj;
            return string.Equals(bool.TrueString, wpt.FindExtensionAttributeValue("EditorOnly"), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the display value for the "Currently loaded" column.
        /// Handles the value produced by <see cref="CreateSortableKey"/> method.
        /// </summary>
        private static string CurrentlyLoadedDisplayValue(object value)
        {
            // boolean values will be when the group title has to be determined.
            if (value is bool)
                return ((bool)value) ? "Not loaded" : "Currently loaded";

            var v = (string)value;
            if (v[0] == 'F')
                return "Currently loaded";
            else
                return "Not loaded";
        }

        #endregion

        /// <summary>
        /// Creates the columns for the given list view.
        /// </summary>
        /// <param name="olv">The <see cref="BrightIdeasSoftware.ObjectListView"/> in which to create the columns.</param>
        /// <param name="forEditor"><c>True</c> when the columns will be shown in the editor view, <c>false</c> for the standard view.</param>
        /// <remarks>The columns are only added in <see cref="BrightIdeasSoftware.ObjectListView.AllColumns"/> collection. 
        /// The caller must add the required subset to <see cref="BrightIdeasSoftware.ObjectListView.Columns"/> collection. 
        /// This enables the caller to customize which columns are being displayed and in which order.</remarks>
        public static void CreateColumns(BrightIdeasSoftware.ObjectListView olv, bool forEditor)
        {
            if (olv == null)
                throw new ArgumentNullException("olv");

            if (forEditor)
            {
                olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn()
                {
                    Text = "Currently loaded",
                    AspectGetter = CurrentlyLoadedAspect,
                    AspectToStringConverter = CurrentlyLoadedDisplayValue,
                    GroupKeyGetter = CurrentlyLoadedGroupKey
                });
            }
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn() 
            { 
                Text = "Cache type",
                AspectGetter = w => ((GpxWaypoint)w).Geocache.CacheType.Name,
                ImageGetter = CacheTypeImage, 
                MinimumWidth = 22, 
                MaximumWidth = 22 
            });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn()
            { 
                Text = "Cache code",
                AspectGetter = w => ((GpxWaypoint)w).Name,
                UseFiltering = false, 
                UseInitialLetterForGroup = true 
            });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn()
            {
                Text = "Title",
                AspectGetter = w => ((GpxWaypoint)w).Geocache.Name,
                UseInitialLetterForGroup = true 
            });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn() 
            {
                Text = "Status",
                AspectGetter = StatusAspect
            });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn() 
            {
                Text = "Container size",
                AspectGetter = w => ((GpxWaypoint)w).Geocache.Container.Name
            });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn() 
            {
                Text = "Country",
                AspectGetter = w => ((GpxWaypoint)w).Geocache.Country
            });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn() 
            {
                Text = "State",
                AspectGetter = w => ((GpxWaypoint)w).Geocache.CountryState
            });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn() 
            {
                Text = "Placed by",
                AspectGetter = w => ((GpxWaypoint)w).Geocache.PlacedBy
            });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn() 
            {
                Text = "Owner",
                AspectGetter = w => ((GpxWaypoint)w).Geocache.Owner.Name
            });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn() 
            {
                Text = "Difficulty",
                AspectGetter = w => ((GpxWaypoint)w).Geocache.Difficulty
            });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn() 
            {
                Text = "Terrain",
                AspectGetter = w => ((GpxWaypoint)w).Geocache.Terrain
            });
        }
    }
}
