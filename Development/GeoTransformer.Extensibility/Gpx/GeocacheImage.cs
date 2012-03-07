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
    /// Contains information about a image attached to a geocache.
    /// </summary>
    public class GeocacheImage : GpxElementBase
    {
        private static Dictionary<XName, Action<GeocacheImage, XElement>> _elementInitializers = new Dictionary<XName, Action<GeocacheImage, XElement>>
        {
            { XmlExtensions.GeocacheSchema_1_0_2 + "name", (o, e) => o.Title = e.Value },
            { XmlExtensions.GeocacheSchema_1_0_2 + "url", (o, e) => o.Address = new Uri(e.Value) },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheImage"/> class.
        /// </summary>
        public GeocacheImage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheImage"/> class.
        /// </summary>
        /// <param name="type">The geocache image XML element.</param>
        public GeocacheImage(XElement image)
        {
            if (image == null)
                return;

            this.Initialize(image, null, _elementInitializers);
        }

        /// <summary>
        /// Serializes the geocache image to XML format.
        /// </summary>
        /// <param name="options">The serialization options.</param>
        /// <returns>The serialized data or <c>null</c> if this instance is empty.</returns>
        public XElement Serialize(GpxSerializationOptions options)
        {
            if (options.GeocacheVersion != GeocacheVersion.Geocache_1_0_2 && !options.EnableUnsupportedExtensions)
                return null;

            var el = new XElement(XmlExtensions.GeocacheSchema_1_0_2 + "image");
            if (!string.IsNullOrWhiteSpace(this.Title))
                el.Add(new XElement(XmlExtensions.GeocacheSchema_1_0_2 + "name", this.Title));
            if (this.Address != null)
                el.Add(new XElement(XmlExtensions.GeocacheSchema_1_0_2 + "url", this.Address));

            if (el.IsEmpty)
                return null;

            return el;
        }

        /// <summary>
        /// Parses the contents of <c>groundspeak:images</c> element and returns a collection of images (one for each child element).
        /// </summary>
        /// <param name="attributes">The <c>groundspeak:images</c> XML element.</param>
        /// <returns>A collection with one <see cref="GeocacheImage"/> instance per XML element.</returns>
        public static ObservableCollection<GeocacheImage> Parse(XElement images)
        {
            var col = new ObservableCollection<GeocacheImage>();
            if (images == null)
                return col;

            foreach (var elem in images.Elements())
            {
                if (string.Equals(elem.Name.LocalName, "image", StringComparison.Ordinal))
                    col.Add(new GeocacheImage(elem));
            }

            return col;
        }

        /// <summary>
        /// Gets or sets the title of the image.
        /// </summary>
        public string Title
        {
            get { return this.GetValue<string>("Title"); }
            set { this.SetValue("Title", value); }
        }

        /// <summary>
        /// Gets or sets the URL address of the image.
        /// </summary>
        public Uri Address
        {
            get { return this.GetValue<Uri>("Address"); }
            set { this.SetValue("Address", value); }
        }
    }
}
