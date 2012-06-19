/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using GeoTransformer;

namespace FetchStatistics.Web
{
    [ServiceContract]
    public class Service
    {
        private static Data.DatabaseSchema InitializeConnection()
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Database.sdf");
            return new Data.DatabaseSchema(path);
        }

        public StatisticsData[] RetrieveData()
        {
            using (var schema = InitializeConnection())
            {
                var q = schema.Statistics.Select();
                q.Select(o => o.StatisticsXml);
                q.Where(o => o.StatisticsXml, GeoTransformer.Data.WhereOperator.NotIsNull, true);

                return q.Execute().AsEnumerable().Select(o => StatisticsData.Deserialize(o.Value(t => t.StatisticsXml))).ToArray();
            }
        }

        [OperationContract]
        public Guid[] RetrieveWork()
        {
            using (var schema = InitializeConnection())
            {
                var q = schema.Statistics.Select();
                q.Limit(30);
                q.Where(o => o.LastUpdated, GeoTransformer.Data.WhereOperator.IsNull, true, "lastUpdated");
                q.Where(o => o.LastUpdated, GeoTransformer.Data.WhereOperator.Smaller, DateTime.Now.AddDays(-1), "lastUpdated");
                q.OrderBy(o => o.LastUpdated);
                q.Select(o => o.Id);

                var randomizer = new Random();

                // the code tries to divide the work even if multiple users request the work at the same time
                return q.Execute()
                            .AsEnumerable()
                            .Select(o => Guid.Parse(o.Value(t => t.Id)))
                            .OrderBy(o => randomizer.Next()) // randomize the 30 oldest entries
                            .Take(10) // return 10 of them
                            .ToArray();
            }
        }

        [OperationContract]
        public void SubmitWork(StatisticsData data)
        {
            if (data == null)
                return;

            if (data.CacheFinds < 400)
            {
                using (var schema = InitializeConnection())
                {
                    var q = schema.Statistics.Select();
                    q.Select(o => o.StatisticsXml);
                    q.Select(o => o.LastUpdated);
                    q.Where(o => o.Id, data.UserId.ToString());
                    var res = q.Execute();

                    // if the user is not present in the database already, do not add him
                    if (!res.Read())
                        return;

                    // do not remove the data from database if it was OK during the last month
                    // this will prevent a bad user from easily deleting a user.
                    var upd = res.Value(o => o.LastUpdated);
                    if (upd == null || upd.Value > DateTime.Now.AddMonths(-1))
                        return;

                    var delq = schema.Statistics.Delete();
                    delq.Where(o => o.Id, data.UserId.ToString());
                    delq.Execute();
                }

                return;
            }

            if (string.IsNullOrWhiteSpace(data.UserName))
                return;

            if (data.DetailedStatisticsAvailable && data.CacheFindsByCountry == null)
                throw new ArgumentException("You are using an old version of FetchStatistics that do not parse the finds per country information.", "data");

            data.Updated = DateTime.Now;

            using (var schema = InitializeConnection())
            {
                var q = schema.Statistics.Replace();
                q.Value(o => o.Id, data.UserId.ToString());
                q.Value(o => o.StatisticsXml, data.Serialize());
                q.Value(o => o.LastUpdated, DateTime.Now);
                q.Execute();
            }
        }
    }
}