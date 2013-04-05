/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoTransformer.Data;

namespace GeoTransformer.Transformers.EditorOnlineBackup
{
    /// <summary>
    /// The table that contains information on what edited data has already been backed up.
    /// </summary>
    internal class CacheTable : DatabaseTable, IExtensionTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheTable"/> class.
        /// </summary>
        public CacheTable()
            : base("CacheTable", 1)
        {
            this.AddColumn<string>("CacheCode", 1);
            this.AddColumn<string>("Data", 1);

            this.AddIndex(new DatabaseIndex(this, "CacheCode", true, 1, this.GetColumn("CacheCode")));
        }

        /// <summary>
        /// Gets the GC code for the cache for which the data is for.
        /// </summary>
        public DatabaseColumn<string> CacheCode { get { return this.GetColumn<string>("CacheCode"); } }

        /// <summary>
        /// Gets the hash of the data that was backed up.
        /// </summary>
        public DatabaseColumn<string> Data { get { return this.GetColumn<string>("Data"); } }
    }
}
