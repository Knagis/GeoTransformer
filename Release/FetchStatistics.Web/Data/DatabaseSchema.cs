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
    public class DatabaseSchema : GeoTransformer.Data.DatabaseSchema
    {
        public DatabaseSchema(string fileName)
            : base(fileName)
        {
            this.AddTable(new StatisticsTable());
        }

        public StatisticsTable Statistics { get { return this.GetTable<StatisticsTable>(); } }
    }
}