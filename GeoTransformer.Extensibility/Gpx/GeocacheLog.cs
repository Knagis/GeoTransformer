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
    /// Contains information about a single geocache log (visit).
    /// </summary>
    public class GeocacheLog : GpxElementBase
    {
        private static Dictionary<XName, Action<GeocacheLog, XAttribute>> _attributeInitializers = new Dictionary<XName, Action<GeocacheLog, XAttribute>>
        {
            { "id", (o, a) => o.Id = XmlConvert.ToInt32(a.Value) },
        };

        private static Dictionary<XName, Action<GeocacheLog, XElement>> _elementInitializers = new Dictionary<XName, Action<GeocacheLog, XElement>>
        {
            { XmlExtensions.GeocacheSchema_1_0_0 + "date", (o, e) => o.Date = XmlConvert.ToDateTime(e.Value, System.Xml.XmlDateTimeSerializationMode.Utc) },
            { XmlExtensions.GeocacheSchema_1_0_1 + "date", (o, e) => o.Date = XmlConvert.ToDateTime(e.Value, System.Xml.XmlDateTimeSerializationMode.Utc) },
            { XmlExtensions.GeocacheSchema_1_0_2 + "date", (o, e) => o.Date = XmlConvert.ToDateTime(e.Value, System.Xml.XmlDateTimeSerializationMode.Utc) },

            { XmlExtensions.GeocacheSchema_1_0_0 + "type", (o, e) => o.LogType = new GeocacheLogType(e) },
            { XmlExtensions.GeocacheSchema_1_0_1 + "type", (o, e) => o.LogType = new GeocacheLogType(e) },
            { XmlExtensions.GeocacheSchema_1_0_2 + "type", (o, e) => o.LogType = new GeocacheLogType(e) },
            
            { XmlExtensions.GeocacheSchema_1_0_0 + "finder", (o, e) => o.Finder = new GeocacheAccount(e) },
            { XmlExtensions.GeocacheSchema_1_0_1 + "finder", (o, e) => o.Finder = new GeocacheAccount(e) },
            { XmlExtensions.GeocacheSchema_1_0_2 + "finder", (o, e) => o.Finder = new GeocacheAccount(e) },

            { XmlExtensions.GeocacheSchema_1_0_0 + "text", (o, e) => o.Text = new GeocacheLogText(e) },
            { XmlExtensions.GeocacheSchema_1_0_1 + "text", (o, e) => o.Text = new GeocacheLogText(e) },
            { XmlExtensions.GeocacheSchema_1_0_2 + "text", (o, e) => o.Text = new GeocacheLogText(e) },

            { XmlExtensions.GeocacheSchema_1_0_2 + "log_wpt", (o, e) => o.Waypoint = new Coordinates.Wgs84Point(XmlConvert.ToDecimal(e.GetAttributeValue("lat")), XmlConvert.ToDecimal(e.GetAttributeValue("lon"))) },

            { XmlExtensions.GeocacheSchema_1_0_2 + "images", (o, e) => o.Images = GeocacheImage.Parse(e) },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheLog"/> class.
        /// </summary>
        public GeocacheLog()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheLog"/> class.
        /// </summary>
        /// <param name="log">The geocache log XML element.</param>
        public GeocacheLog(XElement log)
            : base(true) // the propertyCount is not passed as it is 5 before 1.0.2 version and 7 after (6 is the magic number that changes implementation).
        {
            if (log != null)
                this.Initialize(log, _attributeInitializers, _elementInitializers);

            this.ResumeObservation();
        }

        /// <summary>
        /// Serializes the geocache log to XML format.
        /// </summary>
        /// <param name="options">The serialization options.</param>
        /// <returns>The serialized data or <c>null</c> if this instance is empty.</returns>
        public XElement Serialize(GpxSerializationOptions options)
        {
            var el = new XElement(options.GeocacheNamespace + "log");

            // the schema specifies that the ID is mandatory but we will assume that the data provider will make sure of that and
            // also because most applications will probably not care about the ID, just the contents.
            if (this.Id.HasValue)
                el.Add(new XAttribute("id", this.Id.Value));

            if (this.Date.HasValue)
                el.Add(new XElement(options.GeocacheNamespace + "date", this.Date.Value.ToUniversalTime()));

            el.Add(this.LogType.Serialize(options));
            el.Add(this.Finder.Serialize(options, "finder"));
            el.Add(this.Text.Serialize(options));

            if (this.Waypoint.HasValue && (options.GeocacheVersion == GeocacheVersion.Geocache_1_0_2 || options.EnableUnsupportedExtensions))
                el.Add(new XElement(XmlExtensions.GeocacheSchema_1_0_2 + "log_wpt", 
                                        new XAttribute("lat", this.Waypoint.Value.Latitude),
                                        new XAttribute("lon", this.Waypoint.Value.Longitude)));

            if (this.Images.Count > 0 && (options.GeocacheVersion == GeocacheVersion.Geocache_1_0_2 || options.EnableUnsupportedExtensions))
            {
                var images = new XElement(XmlExtensions.GeocacheSchema_1_0_2 + "images");
                foreach (var img in this.Images)
                    images.Add(img.Serialize(options));

                if (!images.IsEmpty)
                    el.Add(images);
            }

            if (el.IsEmpty)
                return null;

            return el;
        }

        /// <summary>
        /// Parses the contents of <c>groundspeak:logs</c> element and returns a collection of logs (one for each child element).
        /// </summary>
        /// <param name="logs">The <c>groundspeak:logs</c> XML element.</param>
        /// <returns>A collection with one <see cref="GeocacheLog"/> instance per XML element.</returns>
        public static ObservableCollection<GeocacheLog> Parse(XElement logs)
        {
            var col = new ObservableCollection<GeocacheLog>();
            if (logs == null)
                return col;

            foreach (var elem in logs.Elements())
            {
                if (string.Equals(elem.Name.LocalName, "log", StringComparison.Ordinal))
                    col.Add(new GeocacheLog(elem));
            }

            return col;
        }

        /// <summary>
        /// Gets or sets the ID of the geocache log (visit).
        /// </summary>
        public int? Id
        {
            get { return this.GetValue<int?>("Id"); }
            set { this.SetValue("Id", value); }
        }

        /// <summary>
        /// Gets or sets the date when the visit was done.
        /// </summary>
        public DateTime? Date
        {
            get { return this.GetValue<DateTime?>("Date"); }
            set { this.SetValue("Date", value); }
        }

        /// <summary>
        /// Gets the type of the log.
        /// </summary>
        public GeocacheLogType LogType
        {
            get { return this.GetValue<GeocacheLogType>("LogType", true); }
            private set { this.SetValue("LogType", value); }
        }

        /// <summary>
        /// Gets the account that made the log/visit.
        /// </summary>
        public GeocacheAccount Finder
        {
            get { return this.GetValue<GeocacheAccount>("Finder", true); }
            private set { this.SetValue("Finder", value); }
        }

        /// <summary>
        /// Gets the text of the log/visit.
        /// </summary>
        public GeocacheLogText Text
        {
            get { return this.GetValue<GeocacheLogText>("Text", true); }
            private set { this.SetValue("Text", value); }
        }

        /// <summary>
        /// Gets or sets the waypoint coordinates that are associated with the log.
        /// </summary>
        public Coordinates.Wgs84Point? Waypoint
        {
            get { return this.GetValue<Coordinates.Wgs84Point?>("Waypoint"); }
            set { this.SetValue("Waypoint", value); }
        }

        /// <summary>
        /// Gets the images associated with this log.
        /// </summary>
        public IList<GeocacheImage> Images
        {
            get { return this.GetValue<ObservableCollection<GeocacheImage>>("Images", true); }
            private set { this.SetValue("Images", (ObservableCollection<GeocacheImage>)value); }
        }
    }
}
