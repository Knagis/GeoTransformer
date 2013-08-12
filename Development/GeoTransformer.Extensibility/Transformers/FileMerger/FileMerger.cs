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
    /// <summary>
    /// Transformer that merges all loaded <see cref="Gpx.GpxDocument"/> in two files - one for geocaches and one for additional waypoints.
    /// </summary>
    public class FileMerger : TransformerBase, IConfigurable, IRelatedConditional
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

        /// <summary>
        /// Processes the specified GPX documents.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
            var xmlCaches = new Gpx.GpxDocument();
            xmlCaches.Metadata.OriginalFileName = "GeoTransformer.gpx";
            var xmlWaypoints = new Gpx.GpxDocument();
            xmlWaypoints.Metadata.OriginalFileName = "GeoTransformer-wpts.gpx";

            bool something = false;

            foreach (var wpt in documents.SelectMany(o => o.Waypoints))
            {
                something = true;
                if (wpt.Geocache.IsDefined())
                    xmlCaches.Waypoints.Add(wpt);
                else
                    xmlWaypoints.Waypoints.Add(wpt);
            }

            this.ExecutionContext.ReportStatus(documents.Count + " file" + (documents.Count % 10 == 1 && documents.Count != 11 ? "" : "s") + " merged.");
            documents.Clear();

            // just in case the loading of the files failed, let's not delete the old data still on the device.
            if (something)
            {
                documents.Add(xmlCaches);
                documents.Add(xmlWaypoints);
            }
        }

        /// <summary>
        /// Initializes the extension with the specified current configuration (can be <c>null</c> if the extension is initialized for the very first time) and
        /// returns the configuration UI control (can return <c>null</c> if the user interface is not needed).
        /// </summary>
        /// <param name="currentConfiguration">The current configuration.</param>
        /// <returns>
        /// The configuration UI control.
        /// </returns>
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

        /// <summary>
        /// Retrieves the configuration from the extension's configuration UI control.
        /// </summary>
        /// <returns>
        /// The serialized configuration data.
        /// </returns>
        public byte[] SerializeConfiguration()
        {
            return new byte[] { this.IsEnabled ? (byte)1 : (byte)0 };
        }

        /// <summary>
        /// Gets a value indicating whether the this extension should be executed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this extension is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get { return this.Configuration.checkBoxEnabled.Checked; }
        }

        /// <summary>
        /// Gets the category of the extension.
        /// </summary>
        Category IHasCategory.Category { get { return Category.Transformers; } }

        /// <summary>
        /// Determines whether the transformer is enabled by reviewing the current list of transformers that will be executed for any relations.
        /// The transformer list includes also the transformers that have <see cref="IConditional.IsEnabled" /> set to <c>false</c>.
        /// </summary>
        /// <param name="transformers">The transformers that are pending execution.</param>
        /// <returns><c>true</c> if the current transformer is enabled; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        bool IRelatedConditional.IsEnabled(IEnumerable<ITransformer> transformers)
        {
            if (transformers.Any(o => o is Transformers.SaveFiles.SaveGarminZip))
                return false;

            return true;
        }
    }
}
