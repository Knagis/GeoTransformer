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
    public class GeocacheType : GpxElementBase
    {
        private static Dictionary<XName, Action<GeocacheType, XAttribute>> _attributeInitializers = new Dictionary<XName, Action<GeocacheType, XAttribute>>
        {
            { "id", (o, a) => o.Id = XmlConvert.ToInt32(a.Value) },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheType"/> class.
        /// </summary>
        public GeocacheType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheType"/> class.
        /// </summary>
        /// <param name="type">The cache type XML element.</param>
        public GeocacheType(XElement type)
        {
            if (type == null)
                return;

            this.Initialize(type, _attributeInitializers, null);
            this.Name = type.Value;
        }

        /// <summary>
        /// Serializes the cache type to XML element.
        /// </summary>
        /// <param name="options">The serialization options.</param>
        /// <returns>The serialized object or <c>null</c> if this object is empty.</returns>
        public XElement Serialize(GpxSerializationOptions options)
        {
            var el = new XElement(options.GeocacheNamespace + "type");

            // the schema specifies that the ID is mandatory but we will assume that the data provider will make sure of that and
            // also because most applications will probably not care about the ID, just the name.
            if (this.Id.HasValue && (options.EnableInvalidElements || options.GeocacheVersion == GeocacheVersion.Geocache_1_0_2))
                el.Add(new XAttribute("id", this.Id.Value));

            if (!string.IsNullOrEmpty(this.Name))
                el.Value = this.Name;

            if (el.IsEmpty)
                return null;

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
        /// Gets or sets the name of the cache type.
        /// </summary>
        public string Name
        {
            get { return this.GetValue<string>("Name"); }
            set { this.SetValue("Name", value); }
        }
    }
}
