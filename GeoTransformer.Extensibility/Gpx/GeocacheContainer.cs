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
    public class GeocacheContainer : GpxElementBase
    {
        private static Dictionary<XName, Action<GeocacheContainer, XAttribute>> _attributeInitializers = new Dictionary<XName, Action<GeocacheContainer, XAttribute>>
        {
            { "id", (o, a) => o.Id = XmlConvert.ToInt32(a.Value) },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheContainer"/> class.
        /// </summary>
        public GeocacheContainer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheContainer"/> class.
        /// </summary>
        /// <param name="container">The cache container XML element.</param>
        public GeocacheContainer(XElement container)
            : base(true, 2)
        {
            if (container != null)
            {
                this.Initialize(container, _attributeInitializers, null);
                this.Name = container.Value;
            }

            this.ResumeObservation();
        }

        /// <summary>
        /// Serializes the owner to XML element.
        /// </summary>
        /// <param name="options">The serialization options.</param>
        /// <returns>The serialized object or <c>null</c> if this object is empty.</returns>
        public XElement Serialize(GpxSerializationOptions options)
        {
            var el = new XElement(options.GeocacheNamespace + "container");

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
        /// Gets or sets the ID of the container size.
        /// </summary>
        public int? Id
        {
            get { return this.GetValue<int?>("Id"); }
            set { this.SetValue("Id", value); }
        }

        /// <summary>
        /// Gets or sets the name of the container size.
        /// </summary>
        public string Name
        {
            get { return this.GetValue<string>("Name"); }
            set { this.SetValue("Name", value); }
        }

        /// <summary>
        /// Gets the numeric value of this container size.
        /// Used for OpenCaching and Garmin compatibility (http://www.opencaching.com/api_doc/concepts/ratings.html).
        /// </summary>
        public decimal? NumericValue
        {
            get
            {
                if (string.IsNullOrEmpty(this.Name))
                    return null;

                switch (this.Name.ToUpperInvariant())
                {
                    case "MICRO":
                        return 2;

                    case "SMALL":
                        return 3;

                    case "REGULAR":
                        return 4;

                    case "LARGE":
                        return 5;

                    default:
                        return null;
                }
            }
        }
    }
}
