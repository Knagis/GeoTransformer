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

        public override void Process(IList<XDocument> xmlFiles, TransformerOptions options)
        {
            int files = 0;
            int wptCount = 0;

            foreach (var xml in this._configurationControl.Process())
                if (xml != null)
                {
                    xmlFiles.Add(xml);
                    wptCount += xml.Root.WaypointElements("wpt").Where(o => o.CacheElement("cache") != null).Count();
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
