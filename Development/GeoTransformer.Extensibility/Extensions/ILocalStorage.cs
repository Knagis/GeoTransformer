/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Extensions
{
    /// <summary>
    /// An interface for extensions that require creation of local files. The presence of the interface will force the engine to create the local folder and set its path to the property.
    /// </summary>
    public interface ILocalStorage
    {
        /// <summary>
        /// Sets the local storage path where the extension can store its cache if needed. The value is set by the main engine once the extension instance is created.
        /// </summary>
        string LocalStoragePath { set; }
    }
}
