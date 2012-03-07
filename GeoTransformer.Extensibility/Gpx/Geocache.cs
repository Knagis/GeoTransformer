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
    /// Contains the Groundspeak geocache extensions as stored inside a GPX waypoint.
    /// </summary>
    public class Geocache : GpxExtendableElement
    {
        private static Dictionary<XName, Action<Geocache, XAttribute>> _attributeInitializers = new Dictionary<XName, Action<Geocache, XAttribute>>
        {
            { "id", (o, a) => o.ParseId(a.Value) },
            { "available", (o, a) => o.Available = XmlConvert.ToBoolean(a.Value) },
            { "archived", (o, a) => o.Archived = XmlConvert.ToBoolean(a.Value) },
            { "memberonly", (o, a) => o.MemberOnly = XmlConvert.ToBoolean(a.Value) },
            { "customcoords", (o, a) => o.CustomCoordinates = XmlConvert.ToBoolean(a.Value) },
            
            // the attributes introduced in 1.0.2 can be serialized in earlier versions with qualified names
            { XmlExtensions.GeocacheSchema_1_0_2 + "memberonly", (o, a) => o.MemberOnly = XmlConvert.ToBoolean(a.Value) },
            { XmlExtensions.GeocacheSchema_1_0_2 + "customcoords", (o, a) => o.CustomCoordinates = XmlConvert.ToBoolean(a.Value) },
        };

        private static Dictionary<XName, Action<Geocache, XElement>> _elementInitializers = new Dictionary<XName, Action<Geocache, XElement>>
        {
            { XmlExtensions.GeocacheSchema_1_0_0 + "name", (o, e) => o.Name = e.Value },
            { XmlExtensions.GeocacheSchema_1_0_1 + "name", (o, e) => o.Name = e.Value },
            { XmlExtensions.GeocacheSchema_1_0_2 + "name", (o, e) => o.Name = e.Value },

            { XmlExtensions.GeocacheSchema_1_0_0 + "placed_by", (o, e) => o.PlacedBy = e.Value },
            { XmlExtensions.GeocacheSchema_1_0_1 + "placed_by", (o, e) => o.PlacedBy = e.Value },
            { XmlExtensions.GeocacheSchema_1_0_2 + "placed_by", (o, e) => o.PlacedBy = e.Value },

            { XmlExtensions.GeocacheSchema_1_0_0 + "owner", (o, e) => o.Owner = new GeocacheAccount(e) },
            { XmlExtensions.GeocacheSchema_1_0_1 + "owner", (o, e) => o.Owner = new GeocacheAccount(e) },
            { XmlExtensions.GeocacheSchema_1_0_2 + "owner", (o, e) => o.Owner = new GeocacheAccount(e) },

            { XmlExtensions.GeocacheSchema_1_0_0 + "type", (o, e) => o.CacheType = new GeocacheType(e) },
            { XmlExtensions.GeocacheSchema_1_0_1 + "type", (o, e) => o.CacheType = new GeocacheType(e) },
            { XmlExtensions.GeocacheSchema_1_0_2 + "type", (o, e) => o.CacheType = new GeocacheType(e) },

            { XmlExtensions.GeocacheSchema_1_0_0 + "container", (o, e) => o.Container = new GeocacheContainer(e) },
            { XmlExtensions.GeocacheSchema_1_0_1 + "container", (o, e) => o.Container = new GeocacheContainer(e) },
            { XmlExtensions.GeocacheSchema_1_0_2 + "container", (o, e) => o.Container = new GeocacheContainer(e) },

            { XmlExtensions.GeocacheSchema_1_0_0 + "difficulty", (o, e) => o.Difficulty = XmlConvert.ToDecimal(e.Value) },
            { XmlExtensions.GeocacheSchema_1_0_1 + "difficulty", (o, e) => o.Difficulty = XmlConvert.ToDecimal(e.Value) },
            { XmlExtensions.GeocacheSchema_1_0_2 + "difficulty", (o, e) => o.Difficulty = XmlConvert.ToDecimal(e.Value) },
        
            { XmlExtensions.GeocacheSchema_1_0_0 + "terrain", (o, e) => o.Terrain = XmlConvert.ToDecimal(e.Value) },
            { XmlExtensions.GeocacheSchema_1_0_1 + "terrain", (o, e) => o.Terrain = XmlConvert.ToDecimal(e.Value) },
            { XmlExtensions.GeocacheSchema_1_0_2 + "terrain", (o, e) => o.Terrain = XmlConvert.ToDecimal(e.Value) },

            { XmlExtensions.GeocacheSchema_1_0_0 + "country", (o, e) => o.Country = e.Value },
            { XmlExtensions.GeocacheSchema_1_0_1 + "country", (o, e) => o.Country = e.Value },
            { XmlExtensions.GeocacheSchema_1_0_2 + "country", (o, e) => o.Country = e.Value },

            { XmlExtensions.GeocacheSchema_1_0_0 + "state", (o, e) => o.CountryState = e.Value.Trim() },
            { XmlExtensions.GeocacheSchema_1_0_1 + "state", (o, e) => o.CountryState = e.Value.Trim() },
            { XmlExtensions.GeocacheSchema_1_0_2 + "state", (o, e) => o.CountryState = e.Value.Trim() },

            { XmlExtensions.GeocacheSchema_1_0_0 + "encoded_hints", (o, e) => o.Hints = e.Value },
            { XmlExtensions.GeocacheSchema_1_0_1 + "encoded_hints", (o, e) => o.Hints = e.Value },
            { XmlExtensions.GeocacheSchema_1_0_2 + "encoded_hints", (o, e) => o.Hints = e.Value },

            { XmlExtensions.GeocacheSchema_1_0_0 + "short_description", (o, e) => o.ShortDescription = new GeocacheDescription(e) },
            { XmlExtensions.GeocacheSchema_1_0_1 + "short_description", (o, e) => o.ShortDescription = new GeocacheDescription(e) },
            { XmlExtensions.GeocacheSchema_1_0_2 + "short_description", (o, e) => o.ShortDescription = new GeocacheDescription(e) },

            { XmlExtensions.GeocacheSchema_1_0_0 + "long_description", (o, e) => o.LongDescription = new GeocacheDescription(e) },
            { XmlExtensions.GeocacheSchema_1_0_1 + "long_description", (o, e) => o.LongDescription = new GeocacheDescription(e) },
            { XmlExtensions.GeocacheSchema_1_0_2 + "long_description", (o, e) => o.LongDescription = new GeocacheDescription(e) },

            { XmlExtensions.GeocacheSchema_1_0_2 + "personal_note", (o, e) => o.PersonalNote = e.Value },

            { XmlExtensions.GeocacheSchema_1_0_2 + "favorite_points", (o, e) => o.FavoritePoints = XmlConvert.ToInt32(e.Value) },

            { XmlExtensions.GeocacheSchema_1_0_1 + "attributes", (o, e) => o.Attributes = GeocacheAttribute.Parse(e) },
            { XmlExtensions.GeocacheSchema_1_0_2 + "attributes", (o, e) => o.Attributes = GeocacheAttribute.Parse(e) },

            { XmlExtensions.GeocacheSchema_1_0_0 + "logs", (o, e) => o.Logs = GeocacheLog.Parse(e) },
            { XmlExtensions.GeocacheSchema_1_0_1 + "logs", (o, e) => o.Logs = GeocacheLog.Parse(e) },
            { XmlExtensions.GeocacheSchema_1_0_2 + "logs", (o, e) => o.Logs = GeocacheLog.Parse(e) },

            { XmlExtensions.GeocacheSchema_1_0_2 + "images", (o, e) => o.Images = GeocacheImage.Parse(e) },

            // travelbugs, travelbugs-travelbug
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Geocache"/> class.
        /// </summary>
        public Geocache()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Geocache"/> class.
        /// </summary>
        /// <param name="cache">The XML element (groundspeak:cache) that contains the cache extensions.</param>
        public Geocache(XElement cache)
        {
            if (cache == null)
                return;

            this.Initialize(cache, _attributeInitializers, _elementInitializers);
        }

        /// <summary>
        /// Serializes the geocache information in XML element.
        /// </summary>
        /// <param name="options">The serialization options.</param>
        /// <returns>The serialized XML object or <c>null</c> if there is no data in this object.</returns>
        public XElement Serialize(GpxSerializationOptions options)
        {
            return null;
        }

        /// <summary>
        /// Parses the value from the ID attribute. Versions 1.0 and 1.0.1 included numeric ID but version 1.0.2 contains the GC code instead.
        /// </summary>
        /// <param name="value">The value of the ID attribute.</param>
        private void ParseId(string value)
        {
            int numeric;
            if (!int.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out numeric))
            {
                //TODO: implement GC code -> numeric ID conversion
                numeric = -1;
            }

            this.Id = numeric;
        }

        /// <summary>
        /// Gets or sets the numeric ID of the geocache.
        /// </summary>
        public int? Id
        {
            get { return this.GetValue<int?>("Id"); }
            set { this.SetValue("Id", value); }
        }

        /// <summary>
        /// Gets or sets if the geocache is available (not disabled).
        /// </summary>
        public bool Available
        {
            get { return this.GetValue<bool?>("Available") ?? true; }
            set { this.SetValue("Available", value); }
        }

        /// <summary>
        /// Gets or sets if the geocache is archived.
        /// </summary>
        public bool Archived
        {
            get { return this.GetValue<bool?>("Archived") ?? false; }
            set { this.SetValue("Archived", value); }
        }

        /// <summary>
        /// Gets or sets if the geocache is visible only by Premium Members.
        /// </summary>
        /// <remarks>Attribute is only available in 1.0.2 version.</remarks>
        public bool? MemberOnly
        {
            get { return this.GetValue<bool?>("MemberOnly"); }
            set { this.SetValue("MemberOnly", value); }
        }

        /// <summary>
        /// Gets or sets if the waypoint coordinates have been modified by the user.
        /// </summary>
        /// <remarks>Attribute is only available in 1.0.2 version.</remarks>
        public bool? CustomCoordinates
        {
            get { return this.GetValue<bool?>("CustomCoordinates"); }
            set { this.SetValue("CustomCoordinates", value); }
        }

        /// <summary>
        /// Gets or sets the name of the geocache.
        /// </summary>
        public string Name
        {
            get { return this.GetValue<string>("Name"); }
            set { this.SetValue("Name", value); }
        }

        /// <summary>
        /// Gets or sets the free format text that describes who placed the geocache.
        /// </summary>
        public string PlacedBy
        {
            get { return this.GetValue<string>("PlacedBy"); }
            set { this.SetValue("PlacedBy", value); }
        }

        /// <summary>
        /// Gets the account that owns the geocache.
        /// </summary>
        public GeocacheAccount Owner
        {
            get { return this.GetValue<GeocacheAccount>("Owner", true); }
            private set { this.SetValue("Owner", value); }
        }

        /// <summary>
        /// Gets the geocache type.
        /// </summary>
        public GeocacheType CacheType
        {
            get { return this.GetValue<GeocacheType>("CacheType", true); }
            private set { this.SetValue("CacheType", value); }
        }

        /// <summary>
        /// Gets the size of geocache container.
        /// </summary>
        public GeocacheContainer Container
        {
            get { return this.GetValue<GeocacheContainer>("Container", true); }
            private set { this.SetValue("Container", value); }
        }

        /// <summary>
        /// Gets or sets the difficulty rating of the cache.
        /// </summary>
        public decimal? Difficulty
        {
            get { return this.GetValue<decimal?>("Difficulty"); }
            set { this.SetValue("Difficulty", value); }
        }

        /// <summary>
        /// Gets or sets the terrain rating of the cache.
        /// </summary>
        public decimal? Terrain
        {
            get { return this.GetValue<decimal?>("Terrain"); }
            set { this.SetValue("Terrain", value); }
        }

        /// <summary>
        /// Gets or sets the name of the country the geocache resides in.
        /// </summary>
        public string Country
        {
            get { return this.GetValue<string>("Country"); }
            set { this.SetValue("Country", value); }
        }

        /// <summary>
        /// Gets or sets the name of the state the geocache resides in.
        /// </summary>
        public string CountryState
        {
            get { return this.GetValue<string>("CountryState"); }
            set { this.SetValue("CountryState", value); }
        }

        /// <summary>
        /// Gets or sets the hints for the cache.
        /// </summary>
        public string Hints
        {
            get { return this.GetValue<string>("Hints"); }
            set { this.SetValue("Hints", value); }
        }

        /// <summary>
        /// Gets or sets the personal note for the cache.
        /// Note that this element is only supported in 1.0.2 version.
        /// </summary>
        public string PersonalNote
        {
            get { return this.GetValue<string>("PersonalNote"); }
            set { this.SetValue("PersonalNote", value); }
        }

        /// <summary>
        /// Gets the short description for the cache.
        /// </summary>
        public GeocacheDescription ShortDescription
        {
            get { return this.GetValue<GeocacheDescription>("ShortDescription", true); }
            private set { this.SetValue("ShortDescription", value); }
        }

        /// <summary>
        /// Gets the long description for the cache.
        /// </summary>
        public GeocacheDescription LongDescription
        {
            get { return this.GetValue<GeocacheDescription>("LongDescription", true); }
            private set { this.SetValue("LongDescription", value); }
        }

        /// <summary>
        /// Gets or sets the number of favorite points for the cache.
        /// Note that this element is only supported in 1.0.2 version.
        /// </summary>
        public int? FavoritePoints
        {
            get { return this.GetValue<int?>("FavoritePoints"); }
            set { this.SetValue("FavoritePoints", value); }
        }

        /// <summary>
        /// Gets the attributes assigned to the geocache.
        /// Note that attributes are not supported on version 1.0 schema.
        /// </summary>
        public IList<GeocacheAttribute> Attributes
        {
            get { return this.GetValue<ObservableCollection<GeocacheAttribute>>("Attributes", true); }
            private set { this.SetValue("Attributes", (ObservableCollection<GeocacheAttribute>)value); }
        }

        /// <summary>
        /// Gets the images associated with this geocache.
        /// </summary>
        public IList<GeocacheImage> Images
        {
            get { return this.GetValue<ObservableCollection<GeocacheImage>>("Images", true); }
            private set { this.SetValue("Images", (ObservableCollection<GeocacheImage>)value); }
        }

        /// <summary>
        /// Gets the logs (visits) associated with this geocache.
        /// </summary>
        public IList<GeocacheLog> Logs
        {
            get { return this.GetValue<ObservableCollection<GeocacheLog>>("Logs", true); }
            private set { this.SetValue("Logs", (ObservableCollection<GeocacheLog>)value); }
        }

    }
}
