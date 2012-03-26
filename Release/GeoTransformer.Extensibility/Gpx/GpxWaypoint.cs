﻿/*
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
    /// Represents a waypoint, point of interest, or named feature on a map.
    /// </summary>
    public class GpxWaypoint : GpxElementBase
    {
        private static Dictionary<XName, Action<GpxWaypoint, XAttribute>> _attributeInitializers = new Dictionary<XName, Action<GpxWaypoint, XAttribute>>
        {
            { "lat", (o, a) => o.Coordinates = new Coordinates.Wgs84Point(XmlConvert.ToDecimal(a.Value), o.Coordinates.Longitude) },
            { "lon", (o, a) => o.Coordinates = new Coordinates.Wgs84Point(o.Coordinates.Latitude, XmlConvert.ToDecimal(a.Value)) },
        };

        private static Dictionary<XName, Action<GpxWaypoint, XElement>> _elementInitializers = new Dictionary<XName, Action<GpxWaypoint, XElement>>
        {
            { XmlExtensions.GpxSchema_1_0 + "ele", (o, e) => o.Elevation = XmlConvert.ToDecimal(e.Value) },
            { XmlExtensions.GpxSchema_1_1 + "ele", (o, e) => o.Elevation = XmlConvert.ToDecimal(e.Value) },
            { XmlExtensions.GpxSchema_1_0 + "time", (o, e) => o.CreationTime = XmlConvert.ToDateTime(e.Value, System.Xml.XmlDateTimeSerializationMode.Local) },
            { XmlExtensions.GpxSchema_1_1 + "time", (o, e) => o.CreationTime = XmlConvert.ToDateTime(e.Value, System.Xml.XmlDateTimeSerializationMode.Local) },
            { XmlExtensions.GpxSchema_1_0 + "magvar", (o, e) => o.MagneticVariation = XmlConvert.ToDecimal(e.Value) },
            { XmlExtensions.GpxSchema_1_1 + "magvar", (o, e) => o.MagneticVariation = XmlConvert.ToDecimal(e.Value) },
            { XmlExtensions.GpxSchema_1_0 + "geoidheight", (o, e) => o.GeoidHeight = XmlConvert.ToDecimal(e.Value) },
            { XmlExtensions.GpxSchema_1_1 + "geoidheight", (o, e) => o.GeoidHeight = XmlConvert.ToDecimal(e.Value) },
            { XmlExtensions.GpxSchema_1_0 + "name", (o, e) => o.Name = e.Value },
            { XmlExtensions.GpxSchema_1_1 + "name", (o, e) => o.Name = e.Value },
            { XmlExtensions.GpxSchema_1_0 + "cmt", (o, e) => o.Comment = e.Value },
            { XmlExtensions.GpxSchema_1_1 + "cmt", (o, e) => o.Comment = e.Value },
            { XmlExtensions.GpxSchema_1_0 + "desc", (o, e) => o.Description = e.Value },
            { XmlExtensions.GpxSchema_1_1 + "desc", (o, e) => o.Description = e.Value },
            { XmlExtensions.GpxSchema_1_0 + "src", (o, e) => o.Source = e.Value },
            { XmlExtensions.GpxSchema_1_1 + "src", (o, e) => o.Source = e.Value },
            { XmlExtensions.GpxSchema_1_0 + "url", (o, e) => { var v = new Uri(e.Value); var l = o.Links; if (l.Count == 0) l.Add(new GpxLink() { Href = v }); else l[0].Href = v; } },
            { XmlExtensions.GpxSchema_1_0 + "urlname", (o, e) => { var l = o.Links; if (l.Count == 0) l.Add(new GpxLink() { Text = e.Value }); else l[0].Text = e.Value; } },
            { XmlExtensions.GpxSchema_1_1 + "link", (o, e) => o.Links.Add(new GpxLink(e)) },
            { XmlExtensions.GpxSchema_1_0 + "sym", (o, e) => o.Symbol = e.Value },
            { XmlExtensions.GpxSchema_1_1 + "sym", (o, e) => o.Symbol = e.Value },
            { XmlExtensions.GpxSchema_1_0 + "type", (o, e) => o.WaypointType = e.Value },
            { XmlExtensions.GpxSchema_1_1 + "type", (o, e) => o.WaypointType = e.Value },
            { XmlExtensions.GpxSchema_1_0 + "fix", (o, e) => o.FixType = e.Value },
            { XmlExtensions.GpxSchema_1_1 + "fix", (o, e) => o.FixType = e.Value },
            { XmlExtensions.GpxSchema_1_0 + "sat", (o, e) => o.Satellites = XmlConvert.ToByte(e.Value) },
            { XmlExtensions.GpxSchema_1_1 + "sat", (o, e) => o.Satellites = XmlConvert.ToByte(e.Value) },
            { XmlExtensions.GpxSchema_1_0 + "hdop", (o, e) => o.HorizontalDilution = XmlConvert.ToDecimal(e.Value) },
            { XmlExtensions.GpxSchema_1_1 + "hdop", (o, e) => o.HorizontalDilution = XmlConvert.ToDecimal(e.Value) },
            { XmlExtensions.GpxSchema_1_0 + "vdop", (o, e) => o.VerticalDilution = XmlConvert.ToDecimal(e.Value) },
            { XmlExtensions.GpxSchema_1_1 + "vdop", (o, e) => o.VerticalDilution = XmlConvert.ToDecimal(e.Value) },
            { XmlExtensions.GpxSchema_1_0 + "pdop", (o, e) => o.PositionDilution = XmlConvert.ToDecimal(e.Value) },
            { XmlExtensions.GpxSchema_1_1 + "pdop", (o, e) => o.PositionDilution = XmlConvert.ToDecimal(e.Value) },
            { XmlExtensions.GpxSchema_1_0 + "ageofdgpsdata", (o, e) => o.DgpsDataAge = XmlConvert.ToDecimal(e.Value) },
            { XmlExtensions.GpxSchema_1_1 + "ageofdgpsdata", (o, e) => o.DgpsDataAge = XmlConvert.ToDecimal(e.Value) },
            { XmlExtensions.GpxSchema_1_0 + "dgpsid", (o, e) => o.DgpsDataAge = XmlConvert.ToUInt16(e.Value) },
            { XmlExtensions.GpxSchema_1_1 + "dgpsid", (o, e) => o.DgpsDataAge = XmlConvert.ToUInt16(e.Value) },
            { XmlExtensions.GpxSchema_1_1 + "extensions", (o, e) => o.Initialize<GpxWaypoint>(e, null, null) }
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxWaypoint"/> class.
        /// </summary>
        public GpxWaypoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxWaypoint"/> class.
        /// </summary>
        /// <param name="waypoint">The waypoint XML element.</param>
        public GpxWaypoint(XElement waypoint)
        {
            if (waypoint == null)
                return;

            this.Initialize(waypoint, _attributeInitializers, _elementInitializers);
        }

        /// <summary>
        /// Gets or sets the coordinates of the waypoint.
        /// </summary>
        public Coordinates.Wgs84Point Coordinates
        {
            get { return this.GetValue<Coordinates.Wgs84Point>("Coordinates"); }
            set { this.SetValue<Coordinates.Wgs84Point>("Coordinates", value); }
        }

        /// <summary>
        /// Gets or sets the elevation (in meters) of the point.
        /// </summary>
        public decimal? Elevation
        {
            get { return this.GetValue<decimal?>("Elevation"); }
            set { this.SetValue("Elevation", value); }
        }

        /// <summary>
        /// Gets or sets the creation/modification timestamp for element.
        /// </summary>
        public DateTime? CreationTime
        {
            get { return this.GetValue<DateTime?>("CreationTime"); }
            set { this.SetValue("CreationTime", value); }
        }

        /// <summary>
        /// Gets or sets the magnetic variation (in degrees) at the point.
        /// </summary>
        public decimal? MagneticVariation
        {
            get { return this.GetValue<decimal?>("MagneticVariation"); }
            set { this.SetValue("MagneticVariation", value); }
        }

        /// <summary>
        /// Gets or sets the height (in meters) of geoid (mean sea level) above WGS84 earth ellipsoid. As defined in NMEA GGA message.
        /// </summary>
        public decimal? GeoidHeight
        {
            get { return this.GetValue<decimal?>("GeoidHeight"); }
            set { this.SetValue("GeoidHeight", value); }
        }

        /// <summary>
        /// Gets or sets the GPS name of the waypoint. This field will be transferred to and from the GPS.
        /// </summary>
        public string Name
        {
            get { return this.GetValue<string>("Name"); }
            set { this.SetValue("Name", value); }
        }

        /// <summary>
        /// Gets or sets the GPS waypoint comment. Sent to GPS as comment.
        /// </summary>
        public string Comment
        {
            get { return this.GetValue<string>("Comment"); }
            set { this.SetValue("Comment", value); }
        }

        /// <summary>
        /// Gets or sets a text description of the element. Holds additional information about the element intended for the user, not the GPS.
        /// </summary>
        public string Description
        {
            get { return this.GetValue<string>("Description"); }
            set { this.SetValue("Description", value); }
        }

        /// <summary>
        /// Gets or sets the source of data. Included to give user some idea of reliability and accuracy of data. "Garmin eTrex", "USGS quad Boston North", e.g.
        /// </summary>
        public string Source
        {
            get { return this.GetValue<string>("Source"); }
            set { this.SetValue("Source", value); }
        }

        /// <summary>
        /// Gets the collection of links to additional information about the waypoint.
        /// </summary>
        public IList<GpxLink> Links
        {
            get { return this.GetValue<ObservableCollection<GpxLink>>("Links", true); }
        }

        /// <summary>
        /// Gets or sets the GPS symbol name.
        /// </summary>
        public string Symbol
        {
            get { return this.GetValue<string>("Symbol"); }
            set { this.SetValue("Symbol", value); }
        }

        /// <summary>
        /// Gets or sets the type (classification) of the waypoint.
        /// </summary>
        public string WaypointType
        {
            get { return this.GetValue<string>("Type"); }
            set { this.SetValue("Type", value); }
        }

        /// <summary>
        /// Gets or sets the type of GPS fix.
        /// </summary>
        public string FixType
        {
            get { return this.GetValue<string>("FixType"); }
            set { this.SetValue("FixType", value); }
        }

        /// <summary>
        /// Gets or sets the number of satellites used to calculate the GPS fix.
        /// </summary>
        public byte? Satellites
        {
            get { return this.GetValue<byte?>("Satellites"); }
            set { this.SetValue("Satellites", value); }
        }

        /// <summary>
        /// Gets or sets the horizonal dilution of precision.
        /// </summary>
        public decimal? HorizontalDilution
        {
            get { return this.GetValue<decimal?>("hdop"); }
            set { this.SetValue("hdop", value); }
        }

        /// <summary>
        /// Gets or sets the vertical dilution of precision.
        /// </summary>
        public decimal? VerticalDilution
        {
            get { return this.GetValue<decimal?>("vdop"); }
            set { this.SetValue("vdop", value); }
        }

        /// <summary>
        /// Gets or sets the position dilution of precision.
        /// </summary>
        public decimal? PositionDilution
        {
            get { return this.GetValue<decimal?>("pdop"); }
            set { this.SetValue("pdop", value); }
        }

        /// <summary>
        /// Gets or sets the number of seconds since last DGPS update.
        /// </summary>
        public decimal? DgpsDataAge
        {
            get { return this.GetValue<decimal?>("DgpsDataAge"); }
            set { this.SetValue("DgpsDataAge", value); }
        }

        /// <summary>
        /// Gets or sets the ID of DGPS station used in differential correction.
        /// </summary>
        public UInt16? DgpsStationId
        {
            get { return this.GetValue<UInt16?>("DgpsStationId"); }
            set { this.SetValue("DgpsStationId", value); }
        }
    }
}