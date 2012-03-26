/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using GeoTransformer.Extensions;

namespace GeoTransformer.Transformers.LoadLocalFiles
{
    class LoadFiles : TransformerBase, Extensions.IConfigurable
    {
        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Load local GPX files"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override Transformers.ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.LoadSources; }
        }

        /// <summary>
        /// Loads GPX data ands adds it to <paramref name="documents"/> collection.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
            int files = 0;
            int wptCount = 0;

            foreach (var gpx in this._configurationControl.Process())
                if (gpx != null)
                {
                    documents.Add(gpx);
                    wptCount += gpx.Waypoints.Count(o => o.Geocache.IsDefined());
                    files++;
                }

            this.ReportStatus(files + " files loaded, " + wptCount + " caches loaded.");
        }

        private Options _configurationControl;

        public Control Initialize(byte[] currentConfiguration)
        {
            return this._configurationControl = new Options(currentConfiguration);
        }

        public byte[] SerializeConfiguration()
        {
            return this._configurationControl.SerializeConfiguration();
        }

        public bool IsEnabled
        {
            get { return true; }
        }

        Category IHasCategory.Category { get { return new Category(Category.GeocacheSources.ConfigurationOrder, "Geocache source - local folder"); } }
    }
}
