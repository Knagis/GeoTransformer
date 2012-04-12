/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Gpx
{
    /// <summary>
    /// Lists different versions of Groundspeak GPX extensions supported by <see cref="GpxDocument"/> and <see cref="Geocache"/>.
    /// </summary>
    public enum GeocacheVersion
    {
        /// <summary>
        /// Specifies Groundspeak geocache extensions version 1.0.
        /// </summary>
        Geocache_1_0_0,

        /// <summary>
        /// Specifies Groundspeak geocache extensions version 1.0.1.
        /// </summary>
        Geocache_1_0_1,

        /// <summary>
        /// Specifies Groundspeak geocache extensions version 1.0.2.
        /// Note that this version is still under development by Groundspeak.
        /// </summary>
        Geocache_1_0_2,
    }
}
