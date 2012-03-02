/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace FetchStatistics
{
    [DataContract]
    public sealed class StatisticsData
    {
        public static StatisticsData Deserialize(string xml)
        {
            var ser = new DataContractSerializer(typeof(StatisticsData));
            using (var reader = new System.IO.StringReader(xml))
            using (var xmlReader = System.Xml.XmlReader.Create(reader))
                return (StatisticsData)ser.ReadObject(xmlReader);
        }

        public string Serialize()
        {
            var ser = new DataContractSerializer(typeof(StatisticsData));
            using (var writer = new System.IO.StringWriter())
            using (var xmlWriter = System.Xml.XmlWriter.Create(writer))
            {
                ser.WriteObject(xmlWriter, this);
                xmlWriter.Flush();
                return writer.ToString();
            }
        }

        [DataMember] 
        public Guid UserId { get; set; }

        [DataMember]
        [UISettings("Slēpņotājs")]
        public string UserName { get; set; }

        [DataMember]
        [UISettings("Atrasti slēpņi", OrderByDescendingFirst = true)]
        public int CacheFinds { get; set; }

        [DataMember]
        [UISettings("Reģistrēti ceļotāji", OrderByDescendingFirst = true)]
        public int TrackablesLogged { get; set; }

        [DataMember]
        [UISettings("Izpildīti izaicinājumi", OrderByDescendingFirst = true)]
        public int ChallengesCompleted { get; set; }

        [DataMember]
        [UISettings("Reģistrējies")]
        public DateTime MemberSince { get; set; }

        [DataMember]
        [UISettings("Pēdējais apmeklējums", OrderByDescendingFirst = true)]
        public DateTime LastVisit { get; set; }

        [DataMember]
        [UISettings("Statistika atjaunota")]
        public DateTime Updated { get; set; }

        [DataMember]
        [UISettings("Noslēpti slēpņi", OrderByDescendingFirst = true)]
        public int CachesHidden { get; set; }

        [DataMember]
        [UISettings("Detalizēta statistika")]
        public bool DetailedStatisticsAvailable { get; set; }

        [DataMember]
        [UISettings("Atrasti unikāli slēpņi", OrderByDescendingFirst = true)]
        public int? DistinctCacheFinds { get; set; }

        [DataMember]
        [UISettings("Pirmais atradums")]
        public DateTime? FirstCacheDate { get; set; }

        [DataMember]
        [UISettings("Vidēji slēpņi dienā", OrderByDescendingFirst = true)]
        public decimal? FindRate { get; set; }

        [DataMember]
        [UISettings("Visvairāk secīgu dienu ar atradumiem", OrderByDescendingFirst = true)]
        public int? LongestStreak { get; set; }

        [DataMember]
        [UISettings("Visvairāk secīgu dienu bez atradumiem")]
        public int? LongestSlump { get; set; }

        [DataMember]
        [UISettings("Veiksmīgākā diena", OrderByDescendingFirst = true)]
        public int? BestDay { get; set; }

        [DataMember]
        [UISettings("Veiksmīgākais mēnesis", OrderByDescendingFirst = true)]
        public int? BestMonth { get; set; }

        [DataMember]
        [UISettings("Veiksmīgākais gads", OrderByDescendingFirst = true)]
        public int? BestYear { get; set; }

        [DataMember]
        [UISettings("Vidēji slēpņi dienā šogad", OrderByDescendingFirst = true)]
        public decimal? CurrentYearFindRate { get; set; }

        [DataMember]
        [UISettings("Kalendāra aizpildījums", OrderByDescendingFirst = true)]
        public int? DaysInYearCached { get; set; }

        [DataMember]
        [UISettings("Vidējā grūtība", OrderByDescendingFirst = true)]
        public decimal? AverageDifficulty { get; set; }

        [DataMember]
        [UISettings("Vidējā piekļuves grūtība", OrderByDescendingFirst = true)]
        public decimal? AverageTerrain { get; set; }

        [DataMember]
        [UISettings("D/T kombinācijas", OrderByDescendingFirst = true)]
        public int? DifficultyTerrainCombinations { get; set; }

        [DataMember]
        [UISettings("Atrasti T5 slēpņi", OrderByDescendingFirst = true)]
        public int? Terrain5Caches { get; set; }

        [DataMember]
        [UISettings("Atrasti D5 slēpņi", OrderByDescendingFirst = true)]
        public int? Difficulty5Caches { get; set; }

        [DataMember] public Dictionary<string, int> CacheFindsByType { get; set; }
        [DataMember] public Dictionary<string, int> CacheFindsBySize { get; set; }
        [DataMember] public Dictionary<string, int> CacheFindsByCountry { get; set; }

        [UISettings("Atrasti slēpņi", OrderByDescendingFirst = true)]
        public int? CacheFindsInCountry 
        { 
            get 
            {
                var country = "Latvia";
                if (CurrentCountryFunction != null)
                    country = CurrentCountryFunction() ?? country;
                
                if (this.CacheFindsByCountry == null) 
                    return null; 
                 
                if (!this.CacheFindsByCountry.ContainsKey(country))
                    return 0;

                return this.CacheFindsByCountry[country]; 
            }
        }

        [UISettings("Valstu skaits", OrderByDescendingFirst = true)]
        public int? CountryCount
        {
            get
            {
                if (this.CacheFindsByCountry == null)
                    return null;

                return this.CacheFindsByCountry.Count;
            }
        }

        public static Func<string> CurrentCountryFunction { get; set; }
    }
}
