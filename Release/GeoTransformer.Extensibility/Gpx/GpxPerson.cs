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
    /// Describes a person or organization in a GPX element.
    /// </summary>
    public class GpxPerson : GpxElementBase
    {
        private static Dictionary<XName, Action<GpxPerson, XElement>> _elementInitializers = new Dictionary<XName, Action<GpxPerson, XElement>>
        {
            { XmlExtensions.GpxSchema_1_1 + "name", (o, e) => o.Name = e.Value },
            { XmlExtensions.GpxSchema_1_1 + "email", (o, e) => o.Email = e.GetAttributeValue("id") + "@" + e.GetAttributeValue("domain") },
            { XmlExtensions.GpxSchema_1_1 + "link", (o, e) => o.Link = new GpxLink(e) },
        };
        /// <summary>
        /// Initializes a new instance of the <see cref="GpxPerson"/> class.
        /// </summary>
        public GpxPerson()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxPerson"/> class.
        /// </summary>
        /// <param name="person">The XML element containing the person description (GPX 1.1).</param>
        public GpxPerson(XElement person)
            :base(true, 3)
        {
            if (person != null)
                this.Initialize(person, null, _elementInitializers);

            this.ResumeObservation();
        }

        /// <summary>
        /// Serializes the data to XML document.
        /// </summary>
        /// <param name="options">The GPX serialization options.</param>
        /// <param name="localName">The name of the XML element that will be created.</param>
        /// <returns>The person as XML element.</returns>
        public XElement Serialize(GpxSerializationOptions options, string localName = "author")
        {
            if (options == null)
                throw new ArgumentNullException("options");

            if (options.GpxVersion != GpxVersion.Gpx_1_1)
                return null;

            var el = new XElement(options.GpxNamespace + localName);

            if (!string.IsNullOrEmpty(this.Name))
                el.Add(new XElement(options.GpxNamespace + "name", this.Name));
            if (!string.IsNullOrEmpty(this.Email))
            {
                var at = this.Email.IndexOf('@');
                if (at != -1)
                    el.Add(new XElement(options.GpxNamespace + "email", 
                                new XAttribute("id", this.Email.Substring(0, at)), 
                                new XAttribute("domain", this.Email.Substring(at + 1))
                            ));
            }
            el.Add(this.Link.Serialize(options));

            if (el.IsEmpty)
                return null;

            return el;
        }

        /// <summary>
        /// Gets or sets the name of the person or organization.
        /// </summary>
        public string Name
        {
            get { return this.GetValue<string>("Name"); }
            set { this.SetValue("Name", value); }
        }

        /// <summary>
        /// Gets or sets the name of the person or organization.
        /// </summary>
        public string Email
        {
            get { return this.GetValue<string>("Email"); }
            set 
            {
                if (value != null && !value.Contains('@'))
                    throw new ArgumentException("The value must be a valid e-mail address.", "value");
                this.SetValue("Email", value); 
            }
        }

        /// <summary>
        /// Gets the link to Web site or other external information about person.
        /// </summary>
        public GpxLink Link
        {
            get { return this.GetValue<GpxLink>("Link", true); }
            private set { this.SetValue("Link", value); }
        }
    }
}
