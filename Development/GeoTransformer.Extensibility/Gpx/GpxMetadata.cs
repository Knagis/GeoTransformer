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
    /// Class that describes the GPX <c>metadata</c> element.
    /// </summary>
    public class GpxMetadata : GpxExtendableElement
    {
        private static Dictionary<XName, Action<GpxMetadata, XElement>> _elementInitializers = new Dictionary<XName, Action<GpxMetadata, XElement>>
        {
            { XmlExtensions.GpxSchema_1_1 + "name", (o, e) => o.Name = e.Value },
            { XmlExtensions.GpxSchema_1_1 + "desc", (o, e) => o.Description = e.Value },
            { XmlExtensions.GpxSchema_1_1 + "author", (o, e) => o.Author = new GpxPerson(e) },
            { XmlExtensions.GpxSchema_1_1 + "copyright", (o, e) => o.Copyright = new GpxCopyright(e) },
            { XmlExtensions.GpxSchema_1_1 + "link", (o, e) => o.Links.Add(new GpxLink(e)) },
            { XmlExtensions.GpxSchema_1_1 + "time", (o, e) => o.LastRefresh = XmlConvert.ToDateTime(e.Value, System.Xml.XmlDateTimeSerializationMode.Local) },
            { XmlExtensions.GpxSchema_1_1 + "keywords", (o, e) => o.Keywords = e.Value },
            { XmlExtensions.GpxSchema_1_1 + "bounds", (o, e) => o.Bounds = new GpxBounds(e) },
            { XmlExtensions.GpxSchema_1_1 + "extensions", (o, e) => o.Initialize<GpxMetadata>(e, null, null) },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxMetadata"/> class.
        /// </summary>
        public GpxMetadata()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The GPX version 1.1 metadata element.</param>
        public GpxMetadata(XElement metadata)
            : base(true)
        {
            this.Initialize(metadata);
            this.ResumeObservation();
        }

        /// <summary>
        /// Initializes the <see cref="GpxMetadata"/> object with data from the specified gpx:metadata (version 1.1) XML element.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        internal void Initialize(XElement metadata)
        {
            if (metadata == null)
                return;

            if (metadata.Name != XmlExtensions.GpxSchema_1_1 + "metadata")
                return;

            this.Initialize(metadata, null, _elementInitializers);
        }

        /// <summary>
        /// Serializes the GPX metadata to XML. Only works when <see cref="GpxSerializationOptions.GpxVersion"/> is <see cref="GpxVersion.Gpx_1_1"/>.
        /// </summary>
        /// <param name="options">The serialization options.</param>
        /// <returns>The GPX 1.1 element containing the metadata. If the options specify other version, returns <c>null</c>. 
        /// If there is no data to be serialized, returns <c>null</c>.</returns>
        public XElement Serialize(GpxSerializationOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            if (options.GpxVersion != GpxVersion.Gpx_1_1)
                return null;

            var el = new XElement(options.GpxNamespace + "metadata");

            if (!string.IsNullOrEmpty(this.Name))
                el.Add(new XElement(options.GpxNamespace + "name", this.Name));
            if (!string.IsNullOrEmpty(this.Description))
                el.Add(new XElement(options.GpxNamespace + "desc", this.Description));
            el.Add(this.Author.Serialize(options));
            el.Add(this.Copyright.Serialize(options));
            foreach (var link in this.Links)
                el.Add(link.Serialize(options));
            if (this.LastRefresh.HasValue)
                el.Add(new XElement(options.GpxNamespace + "time", this.LastRefresh.Value.ToUniversalTime()));
            if (!string.IsNullOrEmpty(this.Keywords))
                el.Add(new XElement(options.GpxNamespace + "keywords", this.Keywords));
            el.Add(this.Bounds.Serialize(options));

            if (!options.DisableExtensions)
            {
                if (options.EnableUnsupportedExtensions)
                    foreach (var attr in this.ExtensionAttributes)
                        el.Add(new XAttribute(attr));

                var extel = new XElement(options.GpxNamespace + "extensions");
                foreach (var elem in this.ExtensionElements)
                    extel.Add(new XElement(elem));

                if (!extel.IsEmpty)
                    el.Add(extel);
            }

            if (el.IsEmpty)
                return null;

            return el;
        }

        /// <summary>
        /// Gets or sets the filename of the file from which the <see cref="GpxDocument"/> as initialized.
        /// This value is used to save the document to a file when no specific file name is given.
        /// </summary>
        public string OriginalFileName
        {
            get { return this.GetValue<string>("OriginalFileName"); }
            set { this.SetValue("OriginalFileName", value); }
        }

        /// <summary>
        /// Gets or sets the name or URL of the software that created your GPX document. This allows others to inform the creator of a GPX instance document that fails to validate.
        /// </summary>
        public string Creator
        {
            get { return this.GetValue<string>("Creator"); }
            set { this.SetValue("Creator", value); }
        }

        /// <summary>
        /// Gets or sets the name of the GPX file.
        /// </summary>
        public string Name
        {
            get { return this.GetValue<string>("Name"); }
            set { this.SetValue("Name", value); }
        }

        /// <summary>
        /// Gets or sets a description of the contents of the GPX file.
        /// </summary>
        public string Description
        {
            get { return this.GetValue<string>("Description"); }
            set { this.SetValue("Description", value); }
        }

        /// <summary>
        /// Gets the person or organization who created the GPX file.
        /// </summary>
        public GpxPerson Author
        {
            get { return this.GetValue<GpxPerson>("Author", true); }
            private set { this.SetValue("Author", value); }
        }

        /// <summary>
        /// Gets the copyright and license information governing use of the document.
        /// </summary>
        public GpxCopyright Copyright
        {
            get { return this.GetValue<GpxCopyright>("Copyright", true); }
            private set { this.SetValue("Copyright", value); }
        }

        /// <summary>
        /// Gets the collection of URLs associated with the location described in the file.
        /// </summary>
        public IList<GpxLink> Links
        {
            get { return this.GetValue<ObservableCollection<GpxLink>>("Links", true); }
        }

        /// <summary>
        /// Gets or sets the time when the file data was last refreshed.
        /// </summary>
        /// <seealso cref="GpxWaypoint.LastRefresh"/>
        public DateTime? LastRefresh
        {
            get { return this.GetValue<DateTime?>("CreationTime"); }
            set { this.SetValue("CreationTime", value); }
        }

        /// <summary>
        /// Gets or sets keywords associated with the file. Search engines or databases can use this information to classify the data
        /// </summary>
        public string Keywords
        {
            get { return this.GetValue<string>("Keywords"); }
            set { this.SetValue("Keywords", value); }
        }

        /// <summary>
        /// Gets the minimum and maximum coordinates which describe the extent of the coordinates in the file.
        /// </summary>
        public GpxBounds Bounds
        {
            get { return this.GetValue<GpxBounds>("Bounds", true); }
            internal set { this.SetValue("Bounds", value); }
        }
    }
}
