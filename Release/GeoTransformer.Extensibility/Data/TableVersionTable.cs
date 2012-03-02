/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Data
{
    /// <summary>
    /// The system table for storing table versions that makes the incremental schema updates possible.
    /// </summary>
    internal class TableVersionTable : DatabaseTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableVersionTable"/> class.
        /// </summary>
        /// <param name="schema">The database schema this table is created for.</param>
        public TableVersionTable()
            : base("TableVersionTable", 10000)
        {
            this.AddColumn<string>("TableName", 10000);
            this.AddColumn<long>("Version", 10000);

            this.AddIndex(new DatabaseIndex(this, "TableNameUnique", true, 10000, this.TableName));
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        public DatabaseColumn<string> TableName { get { return this.GetColumn<string>("TableName"); } }

        /// <summary>
        /// Gets the current version of the table in the database.
        /// </summary>
        public DatabaseColumn<long> Version { get { return this.GetColumn<long>("Version"); } }
    }
}
