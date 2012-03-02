/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;

namespace GeoTransformer.Extensions
{
    /// <summary>
    /// An interface that enables the extension to provide controls for the cache editor.
    /// </summary>
    public interface IEditor
    {
        /// <summary>
        /// Creates the control that is used to edit the data for the caches. Note that the same control is reused for all caches.
        /// </summary>
        /// <returns>The user interface editor control.</returns>
        System.Windows.Forms.Control CreateControl();

        /// <summary>
        /// Binds the control to the given XML <paramref name="data"/> object. For consequent calls the method removes previous bindings and sets up new ones.
        /// </summary>
        /// <param name="data">The data object.</param>
        void BindControl(System.Xml.Linq.XElement data);
    }
}
