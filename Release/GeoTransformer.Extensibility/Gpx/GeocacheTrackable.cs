/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XmlConvert = System.Xml.XmlConvert;

namespace GeoTransformer.Gpx
{
    /// <summary>
    /// Contains information about a trackable.
    /// </summary>
    public class GeocacheTrackable : GpxElementBase
    {
        private static Dictionary<XName, Action<GeocacheTrackable, XAttribute>> _attributeInitializers = new Dictionary<XName, Action<GeocacheTrackable, XAttribute>>
        {
            { "id", (o, a) => o.Id = XmlConvert.ToInt32(a.Value) },
        };

        private static Dictionary<XName, Action<GeocacheTrackable, XElement>> _elementInitializers = new Dictionary<XName, Action<GeocacheTrackable, XElement>>
        {
            { XmlExtensions.GeocacheSchema_1_0_0 + "name", (o, e) => o.Name = e.Value },
            { XmlExtensions.GeocacheSchema_1_0_1 + "name", (o, e) => o.Name = e.Value },
            { XmlExtensions.GeocacheSchema_1_0_2 + "name", (o, e) => o.Name = e.Value },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheTrackable"/> class.
        /// </summary>
        public GeocacheTrackable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheTrackable"/> class.
        /// </summary>
        /// <param name="travelbug">The geocache travelbug XML element.</param>
        public GeocacheTrackable(XElement travelbug)
            : base(true, 2)
        {
            if (travelbug != null)
                this.Initialize(travelbug, _attributeInitializers, _elementInitializers);
            this.ResumeObservation();
        }

        /// <summary>
        /// Serializes the geocache trackable to XML format.
        /// </summary>
        /// <param name="options">The serialization options.</param>
        /// <returns>The serialized data or <c>null</c> if this instance is empty.</returns>
        public XElement Serialize(GpxSerializationOptions options)
        {
            var el = new XElement(options.GeocacheNamespace + "travelbug");

            // the schema specifies that the ID is mandatory but we will assume that the data provider will make sure of that and
            // also because most applications will probably not care about the ID, just the contents.
            if (this.Id.HasValue)
                el.Add(new XAttribute("id", this.Id.Value));

            if (!string.IsNullOrEmpty(this.Name))
                el.Add(new XElement(options.GeocacheNamespace + "travelbug", this.Name));

            if (el.IsEmpty)
                return null;

            return el;
        }

        /// <summary>
        /// Parses the contents of <c>groundspeak:travelbugs</c> element and returns a collection of trackables (one for each child element).
        /// </summary>
        /// <param name="travelbugs">The <c>groundspeak:travelbugs</c> XML element.</param>
        /// <returns>A collection with one <see cref="GeocacheTrackable"/> instance per XML element.</returns>
        public static ObservableCollection<GeocacheTrackable> Parse(XElement travelbugs)
        {
            var col = new ObservableCollection<GeocacheTrackable>();
            if (travelbugs == null)
                return col;

            foreach (var elem in travelbugs.Elements())
            {
                if (string.Equals(elem.Name.LocalName, "travelbug", StringComparison.Ordinal))
                    col.Add(new GeocacheTrackable(elem));
            }

            return col;
        }

        /// <summary>
        /// Gets or sets the ID of the trackable.
        /// </summary>
        public int? Id
        {
            get { return this.GetValue<int?>("Id"); }
            set { this.SetValue("Id", value); }
        }

        /// <summary>
        /// Gets or sets the name of the trackable.
        /// </summary>
        public string Name
        {
            get { return this.GetValue<string>("Name"); }
            set { this.SetValue("Name", value); }
        }
    }
}
