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
    public class GeocacheDescription : GpxElementBase
    {
        private static Dictionary<XName, Action<GeocacheDescription, XAttribute>> _attributeInitializers = new Dictionary<XName, Action<GeocacheDescription, XAttribute>>
        {
            { "html", (o, a) => o.IsHtml = XmlConvert.ToBoolean(a.Value) },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheDescription"/> class.
        /// </summary>
        public GeocacheDescription()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheDescription"/> class.
        /// </summary>
        /// <param name="description">The description XML element.</param>
        public GeocacheDescription(XElement description)
        {
            if (description == null)
                return;

            this.Initialize(description, _attributeInitializers, null);
            this.Text = description.Value;
        }

        /// <summary>
        /// Serializes this object in XML format.
        /// </summary>
        /// <param name="options">The serialization options.</param>
        /// <param name="localName">Local name of the XML element to be created.</param>
        /// <returns></returns>
        public XElement Serialize(GpxSerializationOptions options, string localName)
        {
            if (string.IsNullOrEmpty(localName))
                throw new ArgumentNullException("localName");

            if (string.IsNullOrWhiteSpace(this.Text))
                return null;

            var el = new XElement(options.GeocacheNamespace + localName);
            el.Value = this.Text;

            el.Add(new XAttribute("html", this.IsHtml));

            return el;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text in this instance is HTML.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is HTML; otherwise, <c>false</c>.
        /// </value>
        public bool IsHtml
        {
            get { return this.GetValue<bool>("IsHtml"); }
            set { this.SetValue("IsHtml", value); }
        }

        /// <summary>
        /// Gets or sets the text of the description.
        /// </summary>
        public string Text
        {
            get { return this.GetValue<string>("Text"); }
            set { this.SetValue("Text", value); }
        }
    }
}