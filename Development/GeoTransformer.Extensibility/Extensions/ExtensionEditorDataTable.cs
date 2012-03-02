/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoTransformer.Data;

namespace GeoTransformer.Extensions
{
    /// <summary>
    /// The table that contains the data that editors create for individual caches.
    /// </summary>
    internal class ExtensionEditorDataTable : DatabaseTable, IExtensionTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionEditorDataTable"/> class.
        /// </summary>
        public ExtensionEditorDataTable()
            : base("ExtensionEditorData", 1)
        {
            this.AddColumn<string>("CacheCode", 1);
            this.AddColumn<string>("Data", 1);

            this.AddIndex(new DatabaseIndex(this, "CacheCode", false, 1, this.GetColumn("CacheCode")));
        }

        /// <summary>
        /// Gets the GC code for the cache for which the data is for.
        /// </summary>
        public DatabaseColumn<string> CacheCode { get { return this.GetColumn<string>("CacheCode"); } }

        /// <summary>
        /// Gets the XML data element that contains the configuration.
        /// </summary>
        public DatabaseColumn<string> Data { get { return this.GetColumn<string>("Data"); } }
    }
}
