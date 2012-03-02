/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GeoTransformer.Extensions;

namespace GeoTransformer.Transformers.RemoveDuplicates
{
    public class RemoveDuplicates : TransformerBase, IConfigurable
    {
        private SimpleConfigurationControl Configuration;

        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Remove duplicates"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.PreProcess; }
        }

        public override void Process(IList<System.Xml.Linq.XDocument> xmlFiles, TransformerOptions options)
        {
            var allElements = xmlFiles.SelectMany(o => o.Root.WaypointElements("wpt"));
            var initialCount = allElements.Count(o => o.CacheDescendants("cache").Any());
            var duplicates = allElements.GroupBy(o => o.WaypointElement("name").GetValue())
                                        .Where(o => o.Count() > 1)
                                        .Where(o => !string.IsNullOrEmpty(o.Key))
                                        .ToList();

            foreach (var group in duplicates)
            {
                group.OrderByDescending(o => ReadGpxTime(o))
                     .Skip(1)
                     .Remove();
            }

            var currentCount = allElements.Count(o => o.CacheDescendants("cache").Any());
            var removedCount = initialCount - currentCount;
            this.ReportStatus("Removed " + removedCount + " duplicate cache" + (removedCount % 10 == 1 && removedCount != 11 ? string.Empty : "s"));
        }

        private static DateTime ReadGpxTime(System.Xml.Linq.XElement wpt)
        {
            var val = wpt.Document.Root.WaypointElement("time").GetValue();
            if (string.IsNullOrEmpty(val))
                return DateTime.UtcNow;
            try
            {
                return System.Xml.XmlConvert.ToDateTime(val, System.Xml.XmlDateTimeSerializationMode.Utc);
            }
            catch (FormatException)
            {
                return DateTime.UtcNow;
            }
        }

        public System.Windows.Forms.Control Initialize(byte[] currentConfiguration)
        {
            this.Configuration = new SimpleConfigurationControl(this.Title,
@"Enables automatic deletion of duplicate waypoints and 
geocaches. This is useful if you have more than one source
of GPX files such as overlapping pocket queries, download
single cache separately to get the latest information etc.

This option will check the generation time of each file and
in cases of duplicates (the duplicates are checked by cache
codes) leaves only the newest copy.");

            this.Configuration.checkBoxEnabled.Checked = currentConfiguration == null || (currentConfiguration.Length > 0 && currentConfiguration[0] == 1);

            return this.Configuration;
        }

        public byte[] SerializeConfiguration()
        {
            return new byte[] { this.IsEnabled ? (byte)1 : (byte)0 };
        }

        public bool IsEnabled
        {
            get { return this.Configuration.checkBoxEnabled.Checked; }
        }

        Category IHasCategory.Category { get { return Category.Transformers; } }

    }
}
