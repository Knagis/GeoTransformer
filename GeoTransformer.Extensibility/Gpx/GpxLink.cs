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
    /// A link to an external resource (Web page, digital photo, video clip, etc) with additional information.
    /// </summary>
    public class GpxLink : GpxElementBase
    {
        private static Dictionary<XName, Action<GpxLink, XAttribute>> _attributeInitializers = new Dictionary<XName, Action<GpxLink, XAttribute>>
        {
            { "href", (o, e) => o.Href = new Uri(e.Value) }
        };
        private static Dictionary<XName, Action<GpxLink, XElement>> _elementInitializers = new Dictionary<XName, Action<GpxLink, XElement>>
        {
            { XmlExtensions.GpxSchema_1_1 + "text", (o, e) => o.Text = e.Value },
            { XmlExtensions.GpxSchema_1_1 + "type", (o, e) => o.MimeType = e.Value },
            { XmlExtensions.GpxSchema_1_1 + "href", (o, e) => o.Href = new Uri(e.Value) }
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxLink"/> class.
        /// </summary>
        public GpxLink()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxLink"/> class.
        /// </summary>
        /// <param name="link">The link element (GPX 1.1).</param>
        public GpxLink(XElement link)
            : base(true, 3)
        {
            this.Initialize(link, _attributeInitializers, _elementInitializers);
            this.ResumeObservation();
        }

        /// <summary>
        /// Serializes the data to XML document.
        /// </summary>
        /// <param name="options">The GPX serialization options.</param>
        /// <param name="localName">The name of the XML element that will be created.</param>
        /// <returns>The link as XML element. If the <see cref="Href"/> property is not set, returns <c>null</c> (unless <paramref name="options"/> specify otherwise).</returns>
        public XElement Serialize(GpxSerializationOptions options, string localName = "link")
        {
            if (options == null)
                throw new ArgumentNullException("options");

            if (options.GpxVersion != GpxVersion.Gpx_1_1)
                return null;

            var el = new XElement(options.GpxNamespace + localName);

            if (this.Href != null || options.EnableInvalidElements)
            {
                el.SetAttributeValue("href", this.Href);
                el.SetElementValue(options.GpxNamespace + "text", this.Text);
                el.SetElementValue(options.GpxNamespace + "type", this.MimeType);
            }

            if (el.IsEmpty)
                return null;

            return el;
        }

        /// <summary>
        /// Gets or sets the text of hyperlink.
        /// </summary>
        public string Text
        {
            get { return this.GetValue<string>("Text"); }
            set { this.SetValue("Text", value); }
        }

        /// <summary>
        /// Gets or sets the mime type of the content represented by the link.
        /// </summary>
        public string MimeType
        {
            get { return this.GetValue<string>("MimeType"); }
            set { this.SetValue("MimeType", value); }
        }

        /// <summary>
        /// Gets or sets the URI of the link. Required value for the link to be serialized.
        /// </summary>
        public Uri Href
        {
            get { return this.GetValue<Uri>("Href"); }
            set { this.SetValue("Href", value); }
        }
    }
}
