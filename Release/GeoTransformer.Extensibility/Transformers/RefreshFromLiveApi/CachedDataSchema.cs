/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoTransformer.Data;

namespace GeoTransformer.Transformers.RefreshFromLiveApi
{
    /// <summary>
    /// The SQLite database schema for data cached by <see cref="RefreshFromLiveApi"/> transformer.
    /// </summary>
    /// <remarks>
    /// Schema history:
    ///     1 - base table structure
    /// </remarks>
    public class CachedDataSchema : DatabaseSchema
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CachedDataSchema"/> class.
        /// </summary>
        /// <param name="databaseFileName">The path to the database file.</param>
        public CachedDataSchema(string databaseFileName)
            : base(databaseFileName)
        {
            this.AddTable(new GeocacheTable());
        }

        /// <summary>
        /// Gets the value indicating if string columns will be created as "collate nocase".
        /// </summary>
        protected override bool CreateStringColumnsCaseInsensitive
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the table containing cached geocache information.
        /// </summary>
        public GeocacheTable Geocaches { get { return this.GetTable<GeocacheTable>(); } }
    }

    /// <summary>
    /// Table that stores geocache data loaded from Live API.
    /// </summary>
    /// <seealso cref="CachedDataSchema"/>
    public class GeocacheTable : DatabaseTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheTable"/> class.
        /// </summary>
        public GeocacheTable()
            : base("Geocaches", 1)
        {
            this.AddColumn<string>("Code", 1);
            this.AddColumn<DateTime>("RetrievedOn", 1);
            this.AddColumn<byte[]>("Data", 1);

            this.AddIndex(new DatabaseIndex(this, "Code", true, 1, this.Code));
        }

        /// <summary>
        /// Gets the column that stores the GC code of the geocache.
        /// </summary>
        public DatabaseColumn<string> Code { get { return this.GetColumn<string>("Code"); } }

        /// <summary>
        /// Gets the column that stores when the data was retrieved from Live API.
        /// </summary>
        public DatabaseColumn<DateTime> RetrievedOn { get { return this.GetColumn<DateTime>("RetrievedOn"); } }

        /// <summary>
        /// Gets the column that stores the serialized representation of <see cref="GeocachingService.Geocache"/>
        /// instance.
        /// </summary>
        public DatabaseColumn<byte[]> Data { get { return this.GetColumn<byte[]>("Data"); } }
    }
}
