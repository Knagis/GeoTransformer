﻿/*
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
    public class GeocacheOwner : GpxElementBase
    {
        private static Dictionary<XName, Action<GeocacheOwner, XAttribute>> _attributeInitializers = new Dictionary<XName, Action<GeocacheOwner, XAttribute>>
        {
            { "id", (o, a) => o.ParseId(a.Value) },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheOwner"/> class.
        /// </summary>
        public GeocacheOwner()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocacheOwner"/> class.
        /// </summary>
        /// <param name="owner">The owner XML element.</param>
        public GeocacheOwner(XElement owner)
        {
            if (owner == null)
                return;

            this.Initialize(owner, _attributeInitializers, null);
            this.Name = owner.Value;
        }

        /// <summary>
        /// Serializes the owner to XML element.
        /// </summary>
        /// <param name="options">The serialization options.</param>
        /// <returns>The serialized object or <c>null</c> if this object is empty.</returns>
        public XElement Serialize(GpxSerializationOptions options)
        {
            var el = new XElement(options.GeocacheNamespace + "owner");

            // the schema specifies that the ID is mandatory but we will assume that the data provider will make sure of that and
            // also because most applications will probably not care about the ID, just the name.
            if (this.Id.HasValue)
                el.Add(new XAttribute("id", this.Id.Value));

            if (!string.IsNullOrEmpty(this.Name))
                el.Value = this.Name;

            if (el.IsEmpty)
                return null;

            return el;
        }

        /// <summary>
        /// Parses the value from the ID attribute. Versions 1.0 and 1.0.1 included numeric ID but version 1.0.2 contains the PR code instead.
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
        /// Gets or sets the numeric ID of the account that owns the geocache.
        /// </summary>
        public int? Id
        {
            get { return this.GetValue<int?>("Id"); }
            set { this.SetValue("Id", value); }
        }

        /// <summary>
        /// Gets or sets the name of the account that owns the geocache.
        /// </summary>
        public string Name
        {
            get { return this.GetValue<string>("Name"); }
            set { this.SetValue("Name", value); }
        }
    }
}
