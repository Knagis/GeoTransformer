/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoTransformer.Transformers.WaypointEnhancer
{
    /// <summary>
    /// Transformer that converts waypoint data to be better suited for display on GPS.
    /// </summary>
    internal class WaypointEnhancer : TransformerBase, Extensions.IConfigurable
    {
        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Enhance waypoints"; }
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
        /// Processes the specified GPX documents. If the method is not overriden in the derived class,
        /// calls <see cref="TransformerBase.Process(Gpx.GpxDocument, Transformers.TransformerOptions)"/> for each document in the list.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
            // the mapping of cache code -> cache title
            var caches = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var waypoints = new List<Gpx.GpxWaypoint>();

            foreach (var wpt in documents.SelectMany(o => o.Waypoints))
            {
                var c = wpt.Geocache;
                if (!c.IsDefined())
                {
                    waypoints.Add(wpt);
                    continue;
                }

                var code = wpt.Name;
                if (string.IsNullOrWhiteSpace(code) || !code.StartsWith("GC", StringComparison.OrdinalIgnoreCase))
                    continue;

                caches[code.Substring(2)] = c.Name;
            }

            // these are the navaid colors as supported by Garmin GPS units
            var colors = new List<string> { "Blue", "Green", "Red", "White", "Amber", "Black", "Orange", "Violet" };
            foreach (var wpt in waypoints)
            {
                // copy the name of the waypoint in the description
                if (!string.IsNullOrWhiteSpace(wpt.Description))
                    wpt.Comment = wpt.Description + Environment.NewLine + Environment.NewLine + wpt.Comment; 

                string code = wpt.Name;
                if (code == null || code.Length < 2)
                    continue;

                // strip the first two characters as the remainder of the code is identical for all waypoints of a single cache.
                code = code.Substring(2);

                // copy the parent geocache reference in the description
                if (caches.ContainsKey(code))
                    wpt.Comment = "Geocache: " + caches[code] + Environment.NewLine + Environment.NewLine + wpt.Comment;

                // change the symbol for trail head
                var curSym = wpt.Symbol;
                if (string.Equals(curSym, "Trailhead", StringComparison.OrdinalIgnoreCase))
                    wpt.Symbol = "Trail Head";
                else if (string.Equals(curSym, "Final Location", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(curSym, "Stages of a Multicache", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(curSym, "Question to Answer", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(curSym, "Reference Point", StringComparison.OrdinalIgnoreCase))
                {
                    // randomize the color - use the GetHashCode so that the color persists during multiple
                    // updates.
                    wpt.Symbol = "Navaid, " + colors[Math.Abs(code.GetHashCode()) % colors.Count];
                }
            }
        }

        /// <summary>
        /// Gets or sets the configuration control.
        /// </summary>
        private SimpleConfigurationControl Configuration { get; set; }

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
@"Enhances how the additional waypoints are displayed
on Garmin GPS unit. It puts the title of the waypoint
in the description together with the name of the cache
that the waypoint references.

Waypoint symbols will also be changed - the correct 
trail head symbol will be used and for other waypoints
a circle symbol will be used instead of the blue flag.
The color of the circle will be random for each cache
to visually group waypoints for a single geocache.");

            this.Configuration.checkBoxEnabled.Checked = currentConfiguration == null || (currentConfiguration.Length > 0 && currentConfiguration[0] == 1);

            return this.Configuration;
        }

        /// <summary>
        /// Serializes the configuration.
        /// </summary>
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
        Extensions.Category Extensions.IHasCategory.Category { get { return Extensions.Category.Transformers; } }
    }
}
