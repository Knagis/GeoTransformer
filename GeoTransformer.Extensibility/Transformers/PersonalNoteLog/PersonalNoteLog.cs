/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoTransformer.Extensions;

namespace GeoTransformer.Transformers.PersonalNoteLog
{
    /// <summary>
    /// A transformer that puts the personal note (from geocaching.com GPX version 1.0.2) as an attribute so that it can be viewed on the GPS device.
    /// </summary>
    public class PersonalNoteLog : TransformerBase, IConfigurable, IRelatedConditional
    {
        private SimpleConfigurationControl Configuration;

        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Create personal note log entry"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get
            {
                // we would like all log entry related transformers to be done as we add a new log entry that would otherwise confuse them.
                return Transformers.ExecutionOrder.BeforePublish - 100;
            }
        }

        /// <summary>
        /// Processes the specified GPX waypoint.
        /// </summary>
        /// <param name="waypoint">The waypoint that has to be processed.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        protected override void Process(Gpx.GpxWaypoint waypoint, TransformerOptions options)
        {
            var note = waypoint.Geocache.PersonalNote;
            if (string.IsNullOrWhiteSpace(note))
                return;

            var log = new Gpx.GeocacheLog();
            log.Date = DateTime.Now.AddYears(1);
            log.Finder.Name = "GeoTransformer";
            log.LogType.Name = "Write note";
            log.Text.Text = "Personal note: " + note;

            waypoint.Geocache.Logs.Insert(0, log);
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
@"Enabling this option will automatically create a new log entry
for each geocache where you have entered a personal note.

For this to function, you must enable the 'Add missing data from
Live API' option or load the pocket query using the full data
option.");

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
        /// The transformer list includes also the transformers that have <see cref="IConditional.IsEnabled"/> set to <c>false</c>.
        /// </summary>
        /// <param name="transformers">The transformers that are pending execution.</param>
        /// <returns>
        ///   <c>true</c> if the current transformer is enabled; otherwise, <c>false</c>.
        /// </returns>
        bool IRelatedConditional.IsEnabled(IEnumerable<ITransformer> transformers)
        {
            var tr = transformers.OfType<RefreshFromLiveApi.RefreshFromLiveApi>().FirstOrDefault();
            return tr != null && tr.IsEnabled;
        }
    }
}
