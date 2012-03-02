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
    /// Contains two lat/lon pairs defining the extent of an element.
    /// </summary>
    public class GpxBounds : GpxElementBase
    {
        private static Dictionary<XName, Action<GpxBounds, XAttribute>> _attributeInitializers = new Dictionary<XName, Action<GpxBounds, XAttribute>>
        {
            { "minlat", (o, e) => o.MinLatitude = XmlConvert.ToDecimal(e.Value) },
            { "maxlat", (o, e) => o.MaxLatitude = XmlConvert.ToDecimal(e.Value) },
            { "minlon", (o, e) => o.MinLongitude = XmlConvert.ToDecimal(e.Value) },
            { "maxlon", (o, e) => o.MaxLongitude = XmlConvert.ToDecimal(e.Value) },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxBounds"/> class.
        /// </summary>
        public GpxBounds()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxBounds"/> class.
        /// </summary>
        /// <param name="bounds">The bounds element from GPX 1.0 or GPX 1.1 specification.</param>
        public GpxBounds(XElement bounds)
        {
            if (bounds == null)
                return;

            this.Initialize(bounds, _attributeInitializers, null);
        }

        /// <summary>
        /// Serializes the data to XML document.
        /// </summary>
        /// <param name="options">The GPX serialization options.</param>
        /// <returns>The bounds serialized as XML. Returns <c>null</c> if the object does not contain data (all coordinates are <c>0</c>).</returns>
        public XElement Serialize(GpxSerializationOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            bool containsExtensions = !options.DisableExtensions && options.EnableUnsupportedExtensions && (this.ExtensionAttributes.Count > 0 || this.ExtensionElements.Count > 0);

            if (!containsExtensions && this.MinLatitude == 0 && this.MaxLatitude == 0 && this.MinLongitude == 0 && this.MaxLongitude == 0)
                return null;

            var el = new XElement(options.GpxNamespace + "bounds",
                new XAttribute("minlat", this.MinLatitude),
                new XAttribute("minlon", this.MinLongitude),
                new XAttribute("maxlat", this.MaxLatitude),
                new XAttribute("maxlon", this.MaxLongitude)
            );

            if (containsExtensions)
            {
                foreach (var ext in this.ExtensionAttributes)
                    el.Add(new XAttribute(ext));
                foreach (var ext in this.ExtensionElements)
                    el.Add(new XElement(ext));
            }

            return el;
        }

        /// <summary>
        /// Gets or sets the minimum latitude.
        /// </summary>
        public decimal MinLatitude
        {
            get { return this.GetValue<decimal>("MinLatitude"); }
            set { this.SetValue("MinLatitude", value); }
        }

        /// <summary>
        /// Gets or sets the maximum latitude.
        /// </summary>
        public decimal MaxLatitude
        {
            get { return this.GetValue<decimal>("MaxLatitude"); }
            set { this.SetValue("MaxLatitude", value); }
        }

        /// <summary>
        /// Gets or sets the minimum longitude.
        /// </summary>
        public decimal MinLongitude
        {
            get { return this.GetValue<decimal>("MinLongitude"); }
            set { this.SetValue("MinLongitude", value); }
        }

        /// <summary>
        /// Gets or sets the maximum longitude.
        /// </summary>
        public decimal MaxLongitude
        {
            get { return this.GetValue<decimal>("MaxLongitude"); }
            set { this.SetValue("MaxLongitude", value); }
        }
    }
}
