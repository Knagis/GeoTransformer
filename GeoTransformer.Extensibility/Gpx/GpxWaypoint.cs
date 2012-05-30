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
    /// Represents a waypoint, point of interest, or named feature on a map.
    /// </summary>
    public class GpxWaypoint : GpxExtendableElement
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
            { XmlExtensions.GpxSchema_1_1 + "extensions", (o, e) => o.Initialize<GpxWaypoint>(e, null, null) },

            { XmlExtensions.GeocacheSchema_1_0_0 + "cache", (o, e) => o.Geocache = new Geocache(e) },
            { XmlExtensions.GeocacheSchema_1_0_1 + "cache", (o, e) => o.Geocache = new Geocache(e) },
            { XmlExtensions.GeocacheSchema_1_0_2 + "cache", (o, e) => o.Geocache = new Geocache(e) },

            { XmlExtensions.GeoTransformerSchema + "lastRefresh", (o, e) => o.LastRefresh = XmlConvert.ToDateTime(e.Value, System.Xml.XmlDateTimeSerializationMode.Local) },
        };

        /// <summary>
        /// Prevents a default instance of the <see cref="GpxWaypoint"/> class from being created.
        /// </summary>
        /// <param name="isCopy">if set to <c>true</c> indicates that the instance represents a copy of a waypoint.</param>
        private GpxWaypoint(bool isCopy)
        {
            if (!isCopy)
            {
                this._originalValues = new GpxWaypoint(true);
                this._originalValues.PropertyChanged += (a, b) => { throw new InvalidOperationException("Cannot modify the OriginalValues property."); };
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxWaypoint"/> class.
        /// </summary>
        public GpxWaypoint()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxWaypoint"/> class.
        /// </summary>
        /// <param name="waypoint">The waypoint XML element.</param>
        public GpxWaypoint(XElement waypoint)
            : this(waypoint, false)
        {
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="GpxWaypoint"/> class from being created.
        /// </summary>
        /// <param name="waypoint">The waypoint XML element.</param>
        /// <param name="isCopy">if set to <c>true</c> indicates that the instance represents a copy of a waypoint.</param>
        private GpxWaypoint(XElement waypoint, bool isCopy)
            : base(true, 21)
        {
            if (!isCopy)
                this._originalValuesXml = waypoint;

            if (waypoint != null)
                this.Initialize(waypoint, _attributeInitializers, _elementInitializers);
    
            this.ResumeObservation();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxWaypoint"/> class.
        /// </summary>
        /// <param name="cache">The geocache object from the Live API service.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="cache"/> is <c>null</c></exception>
        public GpxWaypoint(GeoTransformer.GeocachingService.Geocache cache)
            : this(cache, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxWaypoint"/> class.
        /// </summary>
        /// <param name="cache">The geocache object from the Live API service.</param>
        /// <param name="isCopy">if set to <c>true</c> indicates that the instance represents a copy of a waypoint.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="cache"/> is <c>null</c></exception>
        private GpxWaypoint(GeoTransformer.GeocachingService.Geocache cache, bool isCopy)
            : base(true, 21)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");

            if (!isCopy)
                this._originalValuesGeocache = cache;

            var gc = this.Geocache;

            #region gc.CacheType
            if (cache.CacheType != null)
            {
                gc.CacheType.Id = (int)cache.CacheType.GeocacheTypeId;
                gc.CacheType.Name = (gc.CacheType.Id == 8) ? "Unknown Cache" : cache.CacheType.GeocacheTypeName;
            }
            #endregion

            this.Coordinates = new Coordinates.Wgs84Point(cache.Latitude.GetValueOrDefault(), cache.Longitude.GetValueOrDefault());
            this.CreationTime = cache.UTCPlaceDate;
            this.Description = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "{0} by {1}, {2} ({3}/{4})", cache.Name, cache.PlacedBy, gc.CacheType.Name, cache.Difficulty, cache.Terrain);
            this.Links.Add(new GpxLink() { Text = cache.Name, Href = new Uri(cache.Url) });
            this.Name = cache.Code;
            this.Symbol = "Geocache";
            this.WaypointType = "Geocache|" + gc.CacheType.Name;

            gc.Archived = cache.Archived.GetValueOrDefault();
            #region gc.Attributes
            if (cache.Attributes != null)
            {
                var attributeTypeCache = GeocachingService.LiveClient.Attributes;
                foreach (var a in cache.Attributes)
                {
                    if (!attributeTypeCache.ContainsKey(a.AttributeTypeID))
                        continue;

                    gc.Attributes.Add(new GeocacheAttribute()
                                        {
                                            Id = a.AttributeTypeID,
                                            Name = attributeTypeCache[a.AttributeTypeID].Name,
                                            IsInclusive = a.IsOn
                                        });
                }
            }
            #endregion
            gc.Available = cache.Available.GetValueOrDefault(true);
            #region gc.Container
            if (cache.ContainerType != null)
            {
                gc.Container.Id = (int)cache.ContainerType.ContainerTypeId;
                gc.Container.Name = cache.ContainerType.ContainerTypeName;
            }
            #endregion
            #region gc.Country
            // the Lite version does not include the name of the country, just the ID
            var country = cache.Country;
            if (string.IsNullOrWhiteSpace(country))
                GeocachingService.LiveClient.Countries.TryGetValue(cache.CountryID, out country);
            gc.Country = country;
            #endregion
            gc.CountryState = cache.State;
            gc.Difficulty = (decimal)cache.Difficulty;
            gc.FavoritePoints = cache.FavoritePoints;
            gc.Hints = cache.EncodedHints;
            gc.Id = (int)cache.ID;
            #region gc.Images
            if (cache.Images != null)
                foreach (var i in cache.Images)
                {
                    gc.Images.Add(new GeocacheImage()
                                        {
                                            Address = new Uri(i.Url),
                                            Title = i.Name
                                        });
                }
            #endregion
            #region gc.Logs
            if (cache.GeocacheLogs != null)
                foreach (var l in cache.GeocacheLogs)
                {
                    var gclog = new GeocacheLog();
                    gclog.Date = l.VisitDate;
                    gclog.Finder.Id = (int?)l.Finder.Id;
                    gclog.Finder.Name = l.Finder.UserName;
                    gclog.Id = l.ID;

                    if (l.Images != null)
                        foreach (var i in l.Images)
                        {
                            gclog.Images.Add(new GeocacheImage()
                            {
                                Address = new Uri(i.Url),
                                Title = i.Name
                            });
                        }

                    gclog.LogType.Id = (int)l.LogType.WptLogTypeId;
                    gclog.LogType.Name = l.LogType.WptLogTypeName;
                    gclog.Text.IsEncoded = l.LogIsEncoded;
                    gclog.Text.Text = l.LogText;
                    if (l.UpdatedLatitude.HasValue)
                        gclog.Waypoint = new Coordinates.Wgs84Point(l.UpdatedLatitude.GetValueOrDefault(), l.UpdatedLongitude.GetValueOrDefault());

                    gc.Logs.Add(gclog);
                }
            #endregion
            gc.LongDescription.Text = cache.LongDescription;
            gc.LongDescription.IsHtml = cache.LongDescriptionIsHtml;
            gc.MemberOnly = cache.IsPremium;
            gc.Name = cache.Name;
            #region gc.Owner
            if (cache.Owner != null)
            {
                gc.Owner.Id = (int?)cache.Owner.Id;
                gc.Owner.Name = cache.Owner.UserName;
            }
            #endregion
            gc.PersonalNote = cache.GeocacheNote;
            gc.PlacedBy = cache.PlacedBy;
            gc.ShortDescription.Text = cache.ShortDescription;
            gc.ShortDescription.IsHtml = cache.ShortDescriptionIsHtml;
            gc.Terrain = (decimal)cache.Terrain;
            #region gc.Trackables
            if (cache.Trackables != null)
            {
                foreach (var t in cache.Trackables)
                {
                    gc.Trackables.Add(new GeocacheTrackable()
                                        {
                                            Id = (int)t.Id,
                                            Name = t.Name
                                        });
                }
            }
            #endregion

            this.ResumeObservation();
        }

        /// <summary>
        /// Serializes the data to XML format.
        /// </summary>
        /// <param name="options">The serialization options.</param>
        /// <param name="localName">The name (without namespace) of the element that will be created.</param>
        /// <returns>A serialized XML object or <c>null</c> if this object does not contain any data.</returns>
        public XElement Serialize(GpxSerializationOptions options, string localName = "wpt")
        {
            var el = new XElement(options.GpxNamespace + localName);

            if (this.Elevation.HasValue)
                el.Add(new XElement(options.GpxNamespace + "ele", this.Elevation));
            if (this.CreationTime.HasValue)
                el.Add(new XElement(options.GpxNamespace + "time", this.CreationTime.Value.ToUniversalTime()));
            if (this.MagneticVariation.HasValue)
                el.Add(new XElement(options.GpxNamespace + "magvar", this.MagneticVariation));
            if (this.GeoidHeight.HasValue)
                el.Add(new XElement(options.GpxNamespace + "geoidheight", this.GeoidHeight));
            if (!string.IsNullOrEmpty(this.Name))
                el.Add(new XElement(options.GpxNamespace + "name", this.Name));
            if (!string.IsNullOrEmpty(this.Comment))
                el.Add(new XElement(options.GpxNamespace + "cmt", this.Comment));
            if (!string.IsNullOrEmpty(this.Description))
                el.Add(new XElement(options.GpxNamespace + "desc", this.Description));
            if (!string.IsNullOrEmpty(this.Source))
                el.Add(new XElement(options.GpxNamespace + "src", this.Source));

            if (options.GpxVersion == GpxVersion.Gpx_1_0)
            {
                if (this.Links.Count > 0)
                {
                    var link = this.Links[0];
                    if (link.Href != null)
                        el.Add(new XElement(options.GpxNamespace + "url", link.Href));
                    if (!string.IsNullOrEmpty(link.Text))
                        el.Add(new XElement(options.GpxNamespace + "urlname", link.Text));
                }
            }
            else
            {
                foreach (var link in this.Links)
                    el.Add(link.Serialize(options));
            }

            if (!string.IsNullOrEmpty(this.Symbol))
                el.Add(new XElement(options.GpxNamespace + "sym", this.Symbol));
            if (!string.IsNullOrEmpty(this.WaypointType))
                el.Add(new XElement(options.GpxNamespace + "type", this.WaypointType));
            if (!string.IsNullOrEmpty(this.FixType))
                el.Add(new XElement(options.GpxNamespace + "fix", this.FixType));
            if (this.Satellites.HasValue)
                el.Add(new XElement(options.GpxNamespace + "sat", this.Satellites));
            if (this.HorizontalDilution.HasValue)
                el.Add(new XElement(options.GpxNamespace + "hdop", this.HorizontalDilution));
            if (this.VerticalDilution.HasValue)
                el.Add(new XElement(options.GpxNamespace + "vdop", this.VerticalDilution));
            if (this.PositionDilution.HasValue)
                el.Add(new XElement(options.GpxNamespace + "pdop", this.PositionDilution));
            if (this.DgpsDataAge.HasValue)
                el.Add(new XElement(options.GpxNamespace + "ageofdgpsdata", this.DgpsDataAge));
            if (this.DgpsStationId.HasValue)
                el.Add(new XElement(options.GpxNamespace + "dgpsid", this.DgpsStationId));

            if (!options.DisableExtensions)
            {
                if (options.GpxVersion == GpxVersion.Gpx_1_0)
                {
                    el.Add(this.Geocache.Serialize(options));
                    if (this.CreationTime.HasValue)
                        el.Add(new XElement(XmlExtensions.GeoTransformerSchema + "lastRefresh", this.CreationTime.Value));

                    if (options.EnableUnsupportedExtensions)
                        foreach (var ext in this.ExtensionAttributes)
                            el.Add(new XAttribute(ext));

                    foreach (var ext in this.ExtensionElements)
                        el.Add(new XElement(ext));
                }
                else
                {
                    if (options.EnableUnsupportedExtensions)
                        foreach (var attr in this.ExtensionAttributes)
                            el.Add(new XAttribute(attr));

                    var extel = new XElement(options.GpxNamespace + "extensions");

                    el.Add(this.Geocache.Serialize(options));
                    if (this.CreationTime.HasValue)
                        el.Add(new XElement(XmlExtensions.GeoTransformerSchema + "lastRefresh", this.CreationTime.Value));

                    foreach (var elem in this.ExtensionElements)
                        extel.Add(new XElement(elem));

                    if (!extel.IsEmpty)
                        el.Add(extel);
                }
            }
            else
            {
                if (options.GpxVersion == GpxVersion.Gpx_1_0)
                {
                    el.Add(this.Geocache.Serialize(options));
                }
                else
                {
                    var extel = new XElement(options.GpxNamespace + "extensions");
                    extel.Add(this.Geocache.Serialize(options));
                    if (!extel.IsEmpty)
                        el.Add(extel);
                }
            }

            if (!el.IsEmpty || this.Coordinates.Latitude != 0 || this.Coordinates.Longitude != 0)
            {
                if (!options.EnableUnsupportedExtensions)
                {
                    // only put full precision values if this is a roundtrip serialization.
                    // this is done to ensure maximum application compatibility.
                    el.Add(new XAttribute("lat", this.Coordinates.Latitude));
                    el.Add(new XAttribute("lon", this.Coordinates.Longitude));
                }
                else
                {
                    // usually the coordinates are given with 6 decimal places. Assuming that 7 will not break any application
                    el.Add(new XAttribute("lat", decimal.Round(this.Coordinates.Latitude, 7)));
                    el.Add(new XAttribute("lon", decimal.Round(this.Coordinates.Longitude, 7)));
                }
            }

            if (el.IsEmpty)
                return null;

            return el;
        }

        /// <summary>
        /// Holds the value for <see cref="OriginalValues"/> property.
        /// </summary>
        private GpxWaypoint _originalValues;

        /// <summary>
        /// Holds the XML element from which the waypoint was initialized. This is used by <see cref="OriginalValues"/> to initialize the in-memory
        /// representation only when needed and not always (as the original values are not often needed, this results in significant reduce in memory usage).
        /// </summary>
        private XElement _originalValuesXml;

        /// <summary>
        /// Holds the <see cref="GeocachingService.Geocache"/> object from which the waypoint was initialized.
        /// This is used by <see cref="OriginalValues"/> to initialize the in-memory representation only when 
        /// needed and not always (as the original values are not often needed, this results in significant reduce 
        /// in memory usage).
        /// </summary>
        private GeocachingService.Geocache _originalValuesGeocache;

        /// <summary>
        /// Gets the original values for this GPX waypoint. The object is read-only (any modification will result in an exception).
        /// </summary>
        public GpxWaypoint OriginalValues
        {
            get
            {
                if (this._originalValues != null)
                    return this._originalValues;

                if (this._originalValuesXml != null)
                {
                    this._originalValues = new GpxWaypoint(this._originalValuesXml, true);
                    this._originalValuesXml = null;
                }
                else if (this._originalValuesGeocache != null)
                {
                    this._originalValues = new GpxWaypoint(this._originalValuesGeocache, true);
                    this._originalValuesGeocache = null;
                }
                else
                {
                    // this check will trigger when the property is requested for the instance that itself is a copy for another waypoint.
                    return this;
                }

                this._originalValues.PropertyChanged += (a, b) => { throw new InvalidOperationException("Cannot modify the OriginalValues property."); };
                return this._originalValues;
            }
        }

        /// <summary>
        /// Gets or sets the last time the waypoint data was refreshed. If this is not specified the caller should default to <see cref="GpxMetadata.LastRefresh"/>.
        /// </summary>
        /// <remarks>Stored in <c>geotransformer:lastRefresh</c> extension element.</remarks>
        public DateTime? LastRefresh
        {
            get { return this.GetValue<DateTime?>("LastRefresh"); }
            set { this.SetValue("LastRefresh", value); }
        }

        /// <summary>
        /// Gets or sets the coordinates of the waypoint.
        /// </summary>
        public Coordinates.Wgs84Point Coordinates
        {
            get { return this.GetValue<Coordinates.Wgs84Point>("Coordinates"); }
            set { this.SetValue("Coordinates", value); }
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
        /// Gets or sets the creation/modification timestamp for element. For geocaches this is the date when it was placed.
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

        /// <summary>
        /// Gets the Groundspeak geocache extensions for this waypoint.
        /// </summary>
        public Geocache Geocache
        {
            get { return this.GetValue<Geocache>("Geocache", true); }
            private set { this.SetValue("Geocache", value); }
        }
    }
}
