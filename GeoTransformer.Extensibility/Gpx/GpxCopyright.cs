/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoTransformer.Gpx
{
    /// <summary>
    /// Information about the copyright holder and any license governing use of this file. By linking to an appropriate license, you may place your data into the public domain or grant additional usage rights.
    /// </summary>
    public class GpxCopyright : GpxElementBase
    {
        private static Dictionary<XName, Action<GpxCopyright, XAttribute>> _attributeInitializers = new Dictionary<XName, Action<GpxCopyright, XAttribute>>
        {
            { "author", (o, e) => o.Author = e.Value },
        };
        private static Dictionary<XName, Action<GpxCopyright, XElement>> _elementInitializers = new Dictionary<XName, Action<GpxCopyright, XElement>>
        {
            { XmlExtensions.GpxSchema_1_1 + "year", (o, e) => o.Year = System.Runtime.Remoting.Metadata.W3cXsd2001.SoapYear.Parse(e.Value).Value.Year },
            { XmlExtensions.GpxSchema_1_1 + "license", (o, e) => o.License = new Uri(e.Value) },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxCopyright"/> class.
        /// </summary>
        public GpxCopyright()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxCopyright"/> class.
        /// </summary>
        /// <param name="copyright">The copyright element from GPX 1.1 schema.</param>
        public GpxCopyright(XElement copyright)
            : base(true)
        {
            this.Initialize(copyright, _attributeInitializers, _elementInitializers);
            this.ResumeObservation();
        }

        /// <summary>
        /// Serializes the data to XML document.
        /// </summary>
        /// <param name="options">The GPX serialization options.</param>
        /// <returns>The copyright serialized as XML. Returns <c>null</c> if the object does not contain data (all coordinates are <c>0</c>).</returns>
        public XElement Serialize(GpxSerializationOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            if (options.GpxVersion != GpxVersion.Gpx_1_1)
                return null;

            var el = new XElement(options.GpxNamespace + "copyright");

            if (!string.IsNullOrWhiteSpace(this.Author) || options.EnableInvalidElements)
            {
                el.SetAttributeValue("author", this.Author);
                if (this.Year.HasValue)
                    el.Add(new XElement(options.GpxNamespace + "year", this.Year)); // no need to use SoapYear as it is very unlikely that anyone would use something different than 2002 here.
                if (this.License != null)
                    el.Add(new XElement(options.GpxNamespace + "license", this.License));
            }

            if (el.IsEmpty)
                return null;

            return el;
        }

        /// <summary>
        /// Gets or sets the year of copyright.
        /// </summary>
        public int? Year
        {
            get { return this.GetValue<int?>("Year"); }
            set { this.SetValue("Year", value); }
        }

        /// <summary>
        /// Gets or sets the link to external file containing license text.
        /// </summary>
        public Uri License
        {
            get { return this.GetValue<Uri>("License"); }
            set { this.SetValue<Uri>("License", value); }
        }

        /// <summary>
        /// Gets or sets the copyright holder (e.g. "TopoSoft, Inc."). This is a required element.
        /// </summary>
        public string Author
        {
            get { return this.GetValue<string>("Author"); }
            set { this.SetValue<string>("Author", value); }
        }
    }
}
