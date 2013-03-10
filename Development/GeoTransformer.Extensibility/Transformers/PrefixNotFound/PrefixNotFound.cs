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

namespace GeoTransformer.Transformers.PrefixNotFound
{
    /// <summary>
    /// Transformer that adds a configurable prefix caches that have DNF and NM logs. It modifies <see cref="Gpx.Geocache.Name"/>
    /// property.
    /// </summary>
    public class PrefixDisabled : TransformerBase, IConfigurable
    {
        private int _count;

        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Prefix caches with DNFs"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.Process; }
        }

        /// <summary>
        /// Processes the specified GPX document. If the method is not overriden in the derived class,
        /// calls <see cref="Process(Gpx.GpxWaypoint, Transformers.TransformerOptions)"/> for each waypoint in the document.
        /// </summary>
        /// <param name="document">The document that has to be processed.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        protected override void Process(Gpx.GpxDocument document, TransformerOptions options)
        {
            this._count = 0;
            base.Process(document, options);
            this.ExecutionContext.ReportStatus(this._count + " cache" + ((this._count % 10 == 1 && this._count != 11) ? string.Empty : "s") + " prefixed.");
        }

        /// <summary>
        /// Processes the specified GPX waypoint.
        /// </summary>
        /// <param name="waypoint">The waypoint that has to be processed.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        protected override void Process(Gpx.GpxWaypoint waypoint, TransformerOptions options)
        {
            var gc = waypoint.Geocache;
            if (!gc.IsDefined())
                return;

            var goodLogs = new HashSet<string>(new[] { "Found it", "Enable Listing", "Webcam Photo Taken", "Owner Maintenance" }, StringComparer.OrdinalIgnoreCase);
            var badLogs = new HashSet<string>(new[] { "Didn't find it", "Needs Archived", "Needs Maintenance" }, StringComparer.OrdinalIgnoreCase);

            var badLogCount = gc.Logs.TakeWhile(o => !goodLogs.Contains(o.LogType.Name)).Count(o => badLogs.Contains(o.LogType.Name));

            if (badLogCount > 0)
            {
                this._count++;

                var txt = this.Configuration.textBoxPrefix.Text;
                txt = txt.Replace("?", badLogCount.ToString(System.Globalization.CultureInfo.InvariantCulture));
                if (string.IsNullOrWhiteSpace(gc.Name))
                    gc.Name = txt;
                else if (!gc.Name.Contains(txt))
                    gc.Name = txt + " " + gc.Name;
            }
        }

        #region [ IConfigurable ]

        private ConfigurationControl Configuration;

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
            this.Configuration = new ConfigurationControl();

            if (currentConfiguration != null && currentConfiguration.Length > 2)
            {
                var version = currentConfiguration[0];
                this.Configuration.checkBoxPrefix.Checked = currentConfiguration[0] == (byte)1;
                this.Configuration.textBoxPrefix.Text = System.Text.Encoding.UTF8.GetString(currentConfiguration, 2, currentConfiguration.Length - 2);
            }

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
            var text = this.Configuration.textBoxPrefix.Text;
            var b = new byte[2 + System.Text.Encoding.UTF8.GetByteCount(text)];

            // store the version of configuration
            b[0] = (byte)1;

            System.Text.Encoding.UTF8.GetBytes(text, 0, text.Length, b, 2);
            b[1] = this.Configuration.checkBoxPrefix.Checked ? (byte)1 : (byte)0;

            return b;
        }

        /// <summary>
        /// Gets a value indicating whether the this extension should be executed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this extension is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get { return this.Configuration.checkBoxPrefix.Checked; }
        }

        /// <summary>
        /// Gets the category of the extension.
        /// </summary>
        Category IHasCategory.Category { get { return Category.Transformers; } }

        #endregion
    }
}
