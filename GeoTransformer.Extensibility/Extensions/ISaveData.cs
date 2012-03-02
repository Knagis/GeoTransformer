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
    /// An extension that is called when the application saves the locally edited data (for example, when the user clicks the Save button).
    /// </summary>
    public interface ISaveData : IExtension
    {
        /// <summary>
        /// Saves the data that the extension needs in the specified XML files.
        /// </summary>
        /// <param name="xmlFiles">The XML files.</param>
        void Save(IEnumerable<System.Xml.Linq.XDocument> xmlFiles);
    }
}
