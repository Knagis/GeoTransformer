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
        /// Processes the specified XML files. The default implementation calls <see cref="Process(XDocument)"/> for each file.
        /// </summary>
        /// <param name="xmlFiles">The XML documents currently in the process. The list can be changed if needed</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<XDocument> xmlFiles, TransformerOptions options)
        {
            var _caches = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var _waypoints = new List<XElement>();

            foreach (var wpt in xmlFiles.SelectMany(o => o.Root.WaypointElements("wpt")))
            {
                var c = wpt.CacheElement("cache");
                if (c != null)
                {
                    var code = wpt.WaypointElement("name").GetValue();
                    if (string.IsNullOrWhiteSpace(code) || !code.StartsWith("GC", StringComparison.OrdinalIgnoreCase))
                        continue;

                    _caches[code.Substring(2)] = c.CacheElement("name").GetValue();

                    continue;
                }

                _waypoints.Add(wpt);
            }

            var colors = new List<string> { "Blue", "Green", "Red", "White", "Amber", "Black", "Orange", "Violet" };
            foreach (var wpt in _waypoints)
            {
                XName nname = wpt.Name.Namespace + "name";
                XName ncmt = wpt.Name.Namespace + "cmt";
                XName ndesc = wpt.Name.Namespace + "desc";
                XName nsym = wpt.Name.Namespace + "sym";

                XElement name = null; // the code of the waypoint
                XElement cmt = null;  // the description that is shown on the GPS
                XElement desc = null; // the title of the waypoint
                XElement sym = null;  // the symbol

                foreach (var el in wpt.Elements())
                {
                    if (el.Name == nname)
                        name = el;
                    else if (el.Name == ncmt)
                        cmt = el;
                    else if (el.Name == ndesc)
                        desc = el;
                    else if (el.Name == nsym)
                        sym = el;
                }

                // add the comment field as it will be required further on.
                if (cmt == null)
                    wpt.Add(cmt = new XElement(ncmt));

                // copy the name of the waypoint in the description
                if (desc != null && !string.IsNullOrWhiteSpace(desc.GetValue()))
                    cmt.SetValue(desc.GetValue() + Environment.NewLine + Environment.NewLine + cmt.GetValue());

                // copy the parent geocache reference in the description
                string code = null;
                if (name != null && !string.IsNullOrWhiteSpace(name.GetValue()))
                {
                    code = name.GetValue();
                    if (code.Length > 2)
                    {
                        code = code.Substring(2);
                        if (_caches.ContainsKey(code))
                            cmt.SetValue("Geocache: " + _caches[code] + Environment.NewLine + Environment.NewLine + cmt.GetValue());
                    }
                    else
                    {
                        code = null; // used for the symbol change
                    }
                }

                // change the symbol for trail head
                if (code != null && sym != null)
                {
                    var curSym = sym.GetValue();
                    if (string.Equals(curSym, "Trailhead", StringComparison.OrdinalIgnoreCase))
                        sym.SetValue("Trail Head");
                    else if (string.Equals(curSym, "Final Location", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(curSym, "Stages of a Multicache", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(curSym, "Question to Answer", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(curSym, "Reference Point", StringComparison.OrdinalIgnoreCase))
                    {
                        // randomize the color - use the GetHashCode so that the color persists during multiple
                        // updates.
                        sym.SetValue("Navaid, " + colors[Math.Abs(code.GetHashCode()) % colors.Count]);
                    }
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
