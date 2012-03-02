/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Data.TransformerSchema
{
    internal class MovedCacheTable : DatabaseTable
    {
        public MovedCacheTable()
            : base("MovedCache", 1)
        {
            this.AddColumn<string>("GeocacheCode", 1);
            this.AddColumn<DateTime?>("LastPublished", 1);
            this.AddColumn<double?>("CoordinateX", 1);
            this.AddColumn<double?>("CoordinateY", 1);
            this.AddColumn<bool>("Solved", 1);
            this.AddColumn<string>("Description", 2);

            this.AddIndex(new DatabaseIndex(this, "GeocacheCodeUnique", true, 1, this.GetColumn("GeocacheCode")));
        }

        public DatabaseColumn<string> GeocacheCode { get { return this.GetColumn<string>("GeocacheCode"); } }
        public DatabaseColumn<DateTime?> LastPublished { get { return this.GetColumn<DateTime?>("LastPublished"); } }
        public DatabaseColumn<double?> CoordinateX { get { return this.GetColumn<double?>("CoordinateX"); } }
        public DatabaseColumn<double?> CoordinateY { get { return this.GetColumn<double?>("CoordinateY"); } }
        public DatabaseColumn<bool> Solved { get { return this.GetColumn<bool>("Solved"); } }
        public DatabaseColumn<string> Description { get { return this.GetColumn<string>("Description"); } }
    }
}
