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
    /// Describes the options for serializing <see cref="GpxDocument"/>.
    /// </summary>
    public class GpxSerializationOptions
    {
        /// <summary>
        /// Gets the default serialization options.
        /// </summary>
        public static GpxSerializationOptions Default
        {
            get
            {
                return new GpxSerializationOptions();
            }
        }

        /// <summary>
        /// Gets the serialization options that enable the serialization of all available data so that the object can be returned to the previous state from the
        /// serialized XML.
        /// </summary>
        public static GpxSerializationOptions Roundtrip
        {
            get
            {
                return new GpxSerializationOptions()
                {
                    EnableUnsupportedExtensions = true,
                    EnableInvalidElements = true
                };
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxSerializationOptions"/> class.
        /// </summary>
        public GpxSerializationOptions()
        {
            this.GpxVersion = Gpx.GpxVersion.Gpx_1_1;
        }

        /// <summary>
        /// Gets or sets the GPX version for the resulting XML document. Default is <see cref="GpxVersion.Gpx_1_1"/>.
        /// </summary>
        public GpxVersion GpxVersion { get; set; }

        /// <summary>
        /// Gets or sets whether the resulting XML document will include all extension attributes and elements even on XML elements where the schema does not
        /// allow it. Should be <c>false</c> for the resulting document to be interchangeable with other applications and hardware and <c>true</c> if the saved
        /// data must contain all details from the <see cref="GpxDocument"/>.
        /// Note that if setting this to <c>true</c>, <see cref="GpxVersion.Gpx_1_1"/> should be used.
        /// Default is <c>false</c>.
        /// </summary>
        public bool EnableUnsupportedExtensions { get; set; }

        /// <summary>
        /// Gets or sets whether the resulting XML document will include elements that are invalid from the schema perspective - such as required attributes
        /// missing. This does NOT control <see cref="EnableUnsupportedExtensions"/> - note that unsupported extensions will also invalidate the element.
        /// </summary>
        public bool EnableInvalidElements { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disable all unrecognized extension attributes and elements.
        /// Default is <c>true</c>.
        /// </summary>
        public bool DisableExtensions { get; set; }

        /// <summary>
        /// Gets the GPX namespace that corresponds to <see cref="GpxVersion"/>.
        /// </summary>
        public XNamespace GpxNamespace
        {
            get
            {
                return this.GpxVersion == Gpx.GpxVersion.Gpx_1_0 ? XmlExtensions.GpxSchema_1_0 : XmlExtensions.GpxSchema_1_1;
            }
        }
    }
}
