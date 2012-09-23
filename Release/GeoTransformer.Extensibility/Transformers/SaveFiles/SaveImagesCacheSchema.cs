/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoTransformer.Data;

namespace GeoTransformer.Transformers.SaveFiles
{
    /// <summary>
    /// The SQLite database schema for data cached by <see cref="SaveImages"/> transformer.
    /// </summary>
    /// <remarks>
    /// Schema history:
    ///     1 - base table structure
    /// </remarks>
    internal class SaveImagesCacheSchema : DatabaseSchema
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CachedDataSchema"/> class.
        /// </summary>
        /// <param name="databaseFileName">The path to the database file.</param>
        public SaveImagesCacheSchema(string databaseFileName)
            : base(databaseFileName)
        {
            this.AddTable(new ImageLastUsedTable());
        }

        /// <summary>
        /// Gets the table containing cached geocache information.
        /// </summary>
        public ImageLastUsedTable LastUsed { get { return this.GetTable<ImageLastUsedTable>(); } }
    }

    /// <summary>
    /// Table that stores when each image was last used.
    /// </summary>
    /// <seealso cref="SaveImagesCacheSchema"/>
    internal class ImageLastUsedTable : DatabaseTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheTable"/> class.
        /// </summary>
        public ImageLastUsedTable()
            : base("ImageLastUsed", 1)
        {
            this.AddColumn<string>("Path", 1);
            this.AddColumn<DateTime>("LastUsed", 1);

            this.AddIndex(new DatabaseIndex(this, "Path", true, 1, this.Path));
        }

        /// <summary>
        /// Gets the column that stores the relative path of the image file.
        /// </summary>
        public DatabaseColumn<string> Path { get { return this.GetColumn<string>("Path"); } }

        /// <summary>
        /// Gets the column that stores when the image was last published to the GPS.
        /// </summary>
        public DatabaseColumn<DateTime> LastUsed { get { return this.GetColumn<DateTime>("LastUsed"); } }
    }
}
