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

namespace GeoTransformer.Transformers.HtmlEntityDecoder
{
    /// <summary>
    /// Transformer for decoding HTML entities (characters) - such as <c>&#039;</c> that is decoded to <c>'</c>.
    /// This is useful because some devices do not recognize HTML entities but can handle Unicode characters that these entities represent.
    /// Characters that can not be handled by the output XML document will be automatically encoded during serialization.
    /// </summary>
    public class HtmlEntityDecoder : TransformerBase, IConfigurable
    {
        private SimpleConfigurationControl Configuration;

        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Decode HTML entities"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get 
            { 
                // -100 will enable other transformers (such as automatic translation) to benefit from the clean Unicode text.
                return Transformers.ExecutionOrder.Process - 100; 
            }
        }

        /// <summary>
        /// Processes the specified GPX waypoint.
        /// </summary>
        /// <param name="waypoint">The waypoint that has to be processed.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        protected override void Process(Gpx.GpxWaypoint waypoint, TransformerOptions options)
        {
            if (!string.IsNullOrWhiteSpace(waypoint.Comment))
                waypoint.Comment = System.Net.WebUtility.HtmlDecode(waypoint.Comment);

            if (!string.IsNullOrWhiteSpace(waypoint.Description))
                waypoint.Description = System.Net.WebUtility.HtmlDecode(waypoint.Description);

            var gc = waypoint.Geocache;

            if (!string.IsNullOrWhiteSpace(gc.Hints))
                gc.Hints = System.Net.WebUtility.HtmlDecode(gc.Hints);

            if (!string.IsNullOrWhiteSpace(gc.ShortDescription.Text))
                gc.ShortDescription.Text = System.Net.WebUtility.HtmlDecode(gc.ShortDescription.Text);

            if (!string.IsNullOrWhiteSpace(gc.LongDescription.Text))
                gc.LongDescription.Text = System.Net.WebUtility.HtmlDecode(gc.LongDescription.Text);

            foreach (var log in gc.Logs)
                if (!string.IsNullOrWhiteSpace(log.Text.Text))
                    log.Text.Text = System.Net.WebUtility.HtmlDecode(log.Text.Text);
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
@"Garmin GPS devices cannot display HTML entities (such as &#039;) 
but these entities were used by lots of people to write the cache 
descriptions and logs in their own language (such as Russian, 
Chinese etc.) before geocaching.com supported UTF-8. 

This option enables decoding these entities so that the symbols 
are not missing on any device. 

Has no known side effects.");

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
        ///   <c>true</c> if this extension is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get { return this.Configuration.checkBoxEnabled.Checked; }
        }

        /// <summary>
        /// Gets the category of the extension.
        /// </summary>
        Category IHasCategory.Category { get { return Category.Transformers; } }
    }
}
