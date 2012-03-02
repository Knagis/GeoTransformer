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
    /// An interface for extensions that is displayed when the user has selected a single cache.
    /// </summary>
    public interface ICacheViewer : IExtension
    {
        /// <summary>
        /// Gets the icon to be displayed on the button.
        /// </summary>
        /// <remarks>The image should be 24x24 pixels.</remarks>
        System.Drawing.Image ButtonImage { get; }

        /// <summary>
        /// Gets the text to be displayed on the button.
        /// </summary>
        string ButtonText { get; }

        /// <summary>
        /// Creates the control that will display the detailed cache information. The method is called only once and after that the control is reused.
        /// </summary>
        /// <returns>An initialized control that displays a cache.</returns>
        System.Windows.Forms.Control Initialize();

        /// <summary>
        /// Called by the main application to determine if the viewer is enabled for the particular waypoint.
        /// </summary>
        /// <param name="data">The GPX element containing the cache information (can be <c>null</c>).</param>
        /// <returns>
        ///   <c>true</c> if the viewer is enabled for the specified waypoint; otherwise, <c>false</c>.
        /// </returns>
        bool IsEnabled(System.Xml.Linq.XElement data);

        /// <summary>
        /// Called to display the cache details in the UI control. The method is only called for waypoint after <see cref="IsEnabled"/> returns <c>true</c>.
        /// </summary>
        /// <param name="data">The GPX element containing the cache information (can be <c>null</c>).</param>
        void DisplayCache(System.Xml.Linq.XElement data);
    }
}
