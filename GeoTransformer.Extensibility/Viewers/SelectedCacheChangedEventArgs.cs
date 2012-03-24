/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Viewers
{
    /// <summary>
    /// The arguments for the event when the currently selected cache is changed in the <see cref="Extensions.ICacheListViewer"/>.
    /// </summary>
    public class SelectedCacheChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedCacheChangedEventArgs"/> class.
        /// </summary>
        /// <param name="cache">The XML element (WPT element) that contains the currently selected cache. This element will be passed to the individual cache viewers. Use <c>null</c> if the selection was cleared.</param>
        public SelectedCacheChangedEventArgs(System.Xml.Linq.XElement cache)
        {
            this.CacheXmlData = cache;

            if (cache != null)
                this.CacheCode = cache.Element(XmlExtensions.GpxSchema_1_1 + "name").GetValue();
        }

        /// <summary>
        /// Gets the XML element (WPT element) that contains the currently selected cache.
        /// </summary>
        public System.Xml.Linq.XElement CacheXmlData { get; private set; }

        /// <summary>
        /// Gets the GC code of the element that is represented in the data.
        /// </summary>
        public string CacheCode { get; private set; }
    }
}
