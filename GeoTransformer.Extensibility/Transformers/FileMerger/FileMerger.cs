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

namespace GeoTransformer.Transformers.FileMerger
{
    public class FileMerger : TransformerBase, IConfigurable
    {
        private SimpleConfigurationControl Configuration;

        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Merge .gpx files"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.BeforePublish; }
        }

        public override void Process(IList<System.Xml.Linq.XDocument> xmlFiles, TransformerOptions options)
        {
            var xmlCaches = Gpx.Loader.CreateEmptyDocument("GeoTransformer.gpx");
            var xmlWaypoints = Gpx.Loader.CreateEmptyDocument("GeoTransformer-wpts.gpx");

            bool something = false;

            foreach (var wpt in xmlFiles.SelectMany(o => o.Root.WaypointElements("wpt")))
            {
                something = true;
                if (wpt.CacheElement("cache") != null)
                    xmlCaches.Root.Add(wpt);
                else
                    xmlWaypoints.Root.Add(wpt);
            }

            this.ReportStatus(xmlFiles.Count + " file" + (xmlFiles.Count % 10 == 1 && xmlFiles.Count != 11 ? "" : "s") + " merged.");
            xmlFiles.Clear();

            // just in case the loading of the files failed, let's not delete the old data still on the device.
            if (something)
            {
                xmlFiles.Add(xmlCaches);
                xmlFiles.Add(xmlWaypoints);
            }
        }

        public System.Windows.Forms.Control Initialize(byte[] currentConfiguration)
        {
            this.Configuration = new SimpleConfigurationControl(this.Title, 
@"This option takes all given GPX files and merges them in two:
GeoTransformer.gpx and GeoTransformer-wpts.gpx.

This is useful if you publish lot of files to the same location - this
way each publish will remove everything previously published,
even if you publish different files.

To ensure this behavior empty files will be created if there are no
geocaches or waypoints loaded.");

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
