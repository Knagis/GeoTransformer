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
    public class GpxDocument : GpxElementBase
    {
        private static Dictionary<XName, Action<GpxDocument, XAttribute>> _attributeInitializers = new Dictionary<XName, Action<GpxDocument, XAttribute>>
        {
            { "version", (o, a) => { } },
            { "creator", (o, a) => o.Metadata.Creator = a.Value },
        };

        private static Dictionary<XName, Action<GpxDocument, XElement>> _elementInitializers = new Dictionary<XName, Action<GpxDocument, XElement>>
        {
            { XmlExtensions.GpxSchema_1_0 + "name", (o, e) => o.Metadata.Name = e.Value },
            { XmlExtensions.GpxSchema_1_0 + "desc", (o, e) => o.Metadata.Description = e.Value },
            { XmlExtensions.GpxSchema_1_0 + "author", (o, e) => o.Metadata.Author.Name = e.Value },
            { XmlExtensions.GpxSchema_1_0 + "email", (o, e) => o.Metadata.Author.Email = e.Value },
            { XmlExtensions.GpxSchema_1_0 + "url", (o, e) => { var v = new Uri(e.Value); var l = o.Metadata.Links; if (l.Count == 0) l.Add(new GpxLink() { Href = v }); else l[0].Href = v; } },
            { XmlExtensions.GpxSchema_1_0 + "urlname", (o, e) => { var l = o.Metadata.Links; if (l.Count == 0) l.Add(new GpxLink() { Text = e.Value }); else l[0].Text = e.Value; } },
            { XmlExtensions.GpxSchema_1_0 + "time", (o, e) => o.Metadata.CreationTime = XmlConvert.ToDateTime(e.Value, System.Xml.XmlDateTimeSerializationMode.Local) },
            { XmlExtensions.GpxSchema_1_0 + "keywords", (o, e) => o.Metadata.Keywords = e.Value },
            { XmlExtensions.GpxSchema_1_0 + "bounds", (o, e) => o.Metadata.Bounds = new GpxBounds(e) },

            { XmlExtensions.GpxSchema_1_0 + "wpt", (o, e) => o.Waypoints.Add(new GpxWaypoint(e)) },
            { XmlExtensions.GpxSchema_1_1 + "wpt", (o, e) => o.Waypoints.Add(new GpxWaypoint(e)) },

            { XmlExtensions.GpxSchema_1_0 + "rte", (o, e) => o.Routes.Add(new XElement(e)) },
            { XmlExtensions.GpxSchema_1_1 + "rte", (o, e) => o.Routes.Add(new XElement(e)) },

            { XmlExtensions.GpxSchema_1_0 + "trk", (o, e) => o.Tracks.Add(new XElement(e)) },
            { XmlExtensions.GpxSchema_1_1 + "trk", (o, e) => o.Tracks.Add(new XElement(e)) },

            { XmlExtensions.GpxSchema_1_1 + "metadata", (o, e) => o.Metadata.Initialize(e) },
            { XmlExtensions.GpxSchema_1_1 + "extensions", (o, e) => o.Initialize<GpxDocument>(e, null, null) },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxDocument"/> class.
        /// </summary>
        public GpxDocument()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxDocument"/> class.
        /// </summary>
        /// <param name="document">The document containing GPX 1.0 or 1.1 data.</param>
        public GpxDocument(XDocument document)
        {
            if (document == null || document.Root == null)
                return;

            this.Initialize(document.Root, _attributeInitializers, _elementInitializers);
        }

        /// <summary>
        /// Serializes this GPX document as XML document.
        /// </summary>
        /// <param name="options">The serialization options. By default (when <c>null</c> is specified) uses <see cref="GpxSerializationOptions.Default"/>.</param>
        /// <returns>The data in this object as XML.</returns>
        public XDocument Serialize(GpxSerializationOptions options = null)
        {
            if (options == null)
                options = GpxSerializationOptions.Default;

            var schema = options.GpxNamespace;

            var root = new XElement(schema + "gpx",
                          new XAttribute("version", options.GpxVersion == GpxVersion.Gpx_1_0 ? "1.0" : "1.1"),
                          new XAttribute("creator", string.IsNullOrWhiteSpace(this.Metadata.Creator) ? ("GeoTransformer " + System.Windows.Forms.Application.ProductVersion) : this.Metadata.Creator)
                        );

            if (options.GpxVersion == GpxVersion.Gpx_1_0)
            {
                if (!string.IsNullOrWhiteSpace(this.Metadata.Name))
                    root.Add(new XElement(schema + "name", this.Metadata.Name));

                if (!string.IsNullOrWhiteSpace(this.Metadata.Description))
                    root.Add(new XElement(schema + "desc", this.Metadata.Description));

                if (!string.IsNullOrWhiteSpace(this.Metadata.Author.Name))
                    root.Add(new XElement(schema + "author", this.Metadata.Author.Name));

                if (!string.IsNullOrWhiteSpace(this.Metadata.Author.Email))
                    root.Add(new XElement(schema + "email", this.Metadata.Author.Email));

                if (this.Metadata.Links.Count > 0)
                {
                    var link = this.Metadata.Links[0];
                    if (link.Href != null)
                        root.Add(new XElement(schema + "url", link.Href.ToString()));
                    if (!string.IsNullOrWhiteSpace(link.Text))
                        root.Add(new XElement(schema + "urlname", link.Text));
                }

                if (this.Metadata.CreationTime.HasValue && this.Metadata.CreationTime.Value != DateTime.MinValue)
                    root.Add(new XElement(schema + "time", this.Metadata.CreationTime.Value.ToUniversalTime()));

                if (!string.IsNullOrWhiteSpace(this.Metadata.Keywords))
                    root.Add(new XElement(schema + "keywords", this.Metadata.Keywords));

                root.Add(this.Metadata.Bounds.Serialize(options));
            }
            else
            {
                root.Add(this.Metadata.Serialize(options));
            }

            foreach (var waypoint in this.Waypoints)
                root.Add(waypoint.Serialize(options));

            foreach (var route in this.Routes)
                root.Add(new XElement(route));

            foreach (var track in this.Tracks)
                root.Add(new XElement(track));

            if (!options.DisableExtensions)
            {
                if (options.GpxVersion == GpxVersion.Gpx_1_0)
                {
                    if (options.EnableUnsupportedExtensions)
                        foreach (var ext in this.Metadata.ExtensionAttributes)
                            root.Add(new XAttribute(ext));

                    foreach (var ext in this.Metadata.ExtensionElements)
                        root.Add(new XElement(ext));

                    foreach (var ext in this.ExtensionAttributes)
                        root.Add(new XAttribute(ext));

                    foreach (var ext in this.ExtensionElements)
                        root.Add(new XElement(ext));
                }
                else
                {
                    foreach (var ext in this.ExtensionAttributes)
                        root.Add(new XAttribute(ext));

                    var extel = new XElement(options.GpxNamespace + "extensions");
                    foreach (var ext in this.ExtensionElements)
                        extel.Add(new XElement(ext));
                    if (!extel.IsEmpty)
                        root.Add(extel);
                }
            }

            return new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
        }

        /// <summary>
        /// Gets the metadata information of the GPX document.
        /// </summary>
        public GpxMetadata Metadata
        {
            get { return this.GetValue<GpxMetadata>("Metadata", true); }
        }

        /// <summary>
        /// Gets a collection of GPX waypoints defined in the document.
        /// </summary>
        public IList<GpxWaypoint> Waypoints
        {
            get { return this.GetValue<ObservableCollection<GpxWaypoint>>("Waypoints", true); }
        }

        /// <summary>
        /// Gets the list of routes in this document.
        /// </summary>
        public IList<XElement> Routes
        {
            get { return this.GetValue<ObservableCollection<XElement>>("Routes", true); }
        }

        /// <summary>
        /// Gets the list of tracks in this document.
        /// </summary>
        public IList<XElement> Tracks
        {
            get { return this.GetValue<ObservableCollection<XElement>>("Tracks", true); }
        }
    }
}
