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
    /// Lists different GPX versions supported by <see cref="GpxDocument"/>.
    /// </summary>
    public enum GpxVersion
    {
        /// <summary>
        /// Specifies GPX 1.0 version. 
        /// Note that some information (mostly extension elements) cannot be serialized in this version so it should not be used when 
        /// it is important to persist all details.
        /// </summary>
        Gpx_1_0,

        /// <summary>
        /// Specifies GPX 1.1 version.
        /// </summary>
        Gpx_1_1
    }
}
