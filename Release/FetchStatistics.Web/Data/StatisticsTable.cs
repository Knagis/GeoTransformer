/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FetchStatistics.Web.Data
{
    public class StatisticsTable : GeoTransformer.Data.DatabaseTable
    {
        public StatisticsTable()
            : base("Geocachers", 1)
        {
            this.AddColumn<string>("ID", 1);
            this.AddColumn<DateTime?>("LastUpdated", 1);
            this.AddColumn<string>("StatisticsXml", 1);

            this.AddIndex(new GeoTransformer.Data.DatabaseIndex(this, "UniqueID", true, 1, this.Id));
        }

        public GeoTransformer.Data.DatabaseColumn<string> Id { get { return this.GetColumn<string>("ID"); } }
        public GeoTransformer.Data.DatabaseColumn<DateTime?> LastUpdated { get { return this.GetColumn<DateTime?>("LastUpdated"); } }
        public GeoTransformer.Data.DatabaseColumn<string> StatisticsXml { get { return this.GetColumn<string>("StatisticsXml"); } }
    }
}