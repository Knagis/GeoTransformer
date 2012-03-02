/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Viewers.EditedCacheTableView
{
    internal class ParsedEditedCacheData : TableView.ParsedCacheData
    {
        public ParsedEditedCacheData(System.Xml.Linq.XElement sourceElement)
            : base(sourceElement)
        {
        }

        /// <summary>
        /// Creates the sortable value for the "Currently loaded" column.
        /// It is required so that the values are sorted in a specific order within one group.
        /// The sort order is important because otherwise the list gets re-ordered every time
        /// the user clicks "Remove customizations".
        /// </summary>
        private static string CreateSortableKey(object obj)
        {
            var p = (ParsedEditedCacheData)obj;

            return (p.IsEditorOnly ? "B" : "A") + p.CacheCode;
        }

        /// <summary>
        /// Gets the display value for the "Currently loaded" column.
        /// Handles the value produced by <see cref="CreateSortableKey"/> method.
        /// </summary>
        private static string GetDisplayValue(object value)
        {
            // boolean values will be when the group title has to be determined.
            if (value is bool)
                return ((bool)value) ? "Not loaded" : "Currently loaded";

            var v = (string)value;
            if (v[0] == 'A')
                return "Currently loaded";
            else
                return "Not loaded";
        }

        public new static void InitializeListView(BrightIdeasSoftware.ObjectListView olv)
        {
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn("Currently loaded", "IsEditorOnly")
                {
                    AspectGetter = CreateSortableKey,
                    AspectToStringConverter = GetDisplayValue,
                    GroupKeyGetter = o => ((ParsedEditedCacheData)o).IsEditorOnly
                });

            TableView.ParsedCacheData.InitializeListView(olv);
        }
    }
}
