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
    public class GeocacheLogText : GpxElementBase
    {
        private static Dictionary<XName, Action<GeocacheLogText, XAttribute>> _attributeInitializers = new Dictionary<XName, Action<GeocacheLogText, XAttribute>>
        {
            { "encoded", (o, a) => o.IsEncoded = XmlConvert.ToBoolean(a.Value) },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheLogText"/> class.
        /// </summary>
        public GeocacheLogText()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheLogText"/> class.
        /// </summary>
        /// <param name="description">The log text XML element.</param>
        public GeocacheLogText(XElement description)
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
        public XElement Serialize(GpxSerializationOptions options)
        {
            if (string.IsNullOrWhiteSpace(this.Text))
                return null;

            var el = new XElement(options.GeocacheNamespace + "text");
            el.Value = this.Text;

            el.Add(new XAttribute("encoded", this.IsEncoded));

            return el;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text in this instance is encoded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the text is encoded; otherwise, <c>false</c>.
        /// </value>
        public bool IsEncoded
        {
            get { return this.GetValue<bool>("IsEncoded"); }
            set { this.SetValue("IsEncoded", value); }
        }

        /// <summary>
        /// Gets or sets the text of the log/visit.
        /// </summary>
        public string Text
        {
            get { return this.GetValue<string>("Text"); }
            set { this.SetValue("Text", value); }
        }
    }
}