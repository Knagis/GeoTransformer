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
    /// Contains information about an attribute assigned to the geocache.
    /// </summary>
    public class GeocacheAttribute : GpxElementBase
    {
        private static Dictionary<XName, Action<GeocacheAttribute, XAttribute>> _attributeInitializers = new Dictionary<XName, Action<GeocacheAttribute, XAttribute>>
        {
            { "id", (o, a) => o.Id = XmlConvert.ToInt32(a.Value) },
            { "inc", (o, a) => o.IsInclusive = string.Equals(a.Value, "1", StringComparison.Ordinal) }
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheAttribute"/> class.
        /// </summary>
        public GeocacheAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheAttribute"/> class.
        /// </summary>
        /// <param name="type">The attribute XML element.</param>
        public GeocacheAttribute(XElement attribute)
        {
            if (attribute == null)
                return;

            this.Initialize(attribute, _attributeInitializers, null);
            this.Name = attribute.Value;
        }

        /// <summary>
        /// Parses the contents of <c>groundspeak:attributes</c> element and returns a collection of attributes (one for each child element).
        /// </summary>
        /// <param name="attributes">The <c>groundspeak:attributes</c> XML element.</param>
        /// <returns>A collection with one <see cref="GeocacheAttribute"/> instance per XML element.</returns>
        public static ObservableCollection<GeocacheAttribute> Parse(XElement attributes)
        {
            var col = new ObservableCollection<GeocacheAttribute>();
            if (attributes == null)
                return col;

            foreach (var elem in attributes.Elements())
            {
                if (string.Equals(elem.Name.LocalName, "attribute", StringComparison.Ordinal))
                    col.Add(new GeocacheAttribute(elem));
            }

            return col;
        }

        /// <summary>
        /// Serializes the attribute to XML element.
        /// </summary>
        /// <param name="options">The serialization options.</param>
        /// <returns>The serialized object or <c>null</c> if this object is empty.</returns>
        public XElement Serialize(GpxSerializationOptions options)
        {
            var el = new XElement(options.GeocacheNamespace + "attribute");

            // the schema specifies that the ID is mandatory but we will assume that the data provider will make sure of that and
            // also because many applications will probably not care about the ID, just the name.
            if (this.Id.HasValue)
                el.Add(new XAttribute("id", this.Id.Value));

            if (!string.IsNullOrEmpty(this.Name))
                el.Value = this.Name;

            if (el.IsEmpty)
                return null;

            if (options.GeocacheVersion == GeocacheVersion.Geocache_1_0_2)
            {
                if (!this.IsInclusive)
                {
                    if (!options.EnableInvalidElements)
                        return null;

                    el.Add(new XAttribute("inc", "0"));
                }
            }
            else
            {
                el.Add(new XAttribute("inc", this.IsInclusive ? "1" : "0"));
            }

            return el;
        }

        /// <summary>
        /// Gets or sets the ID of the cache type.
        /// </summary>
        public int? Id
        {
            get { return this.GetValue<int?>("Id"); }
            set { this.SetValue("Id", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute is inclusive (e.g. available in night) or exclusive (e.g. not available in night).
        /// Note that version 1.0.2 of the schema no longer supports this attribute and only inclusive attributes are allowed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the attribute is inclusive; otherwise, <c>false</c>.
        /// </value>
        public bool IsInclusive
        {
            get { return this.GetValue<bool?>("IsInclusive") ?? true; }
            set { this.SetValue("IsInclusive", value); }
        }

        /// <summary>
        /// Gets or sets the name of the cache type.
        /// </summary>
        public string Name
        {
            get { return this.GetValue<string>("Name"); }
            set { this.SetValue("Name", value); }
        }
    }
}
