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
    /// An interface for extensions that need to display their own top level tab-page. This interface allows the extension to provide 
    /// a completely separate user interface.
    /// </summary>
    public interface ITopLevelTabPage : IExtension
    {
        /// <summary>
        /// Gets the image to be displayed on the button.
        /// </summary>
        /// <remarks>The image should be 24x24 pixels.</remarks>
        System.Drawing.Image TabPageImage { get; }

        /// <summary>
        /// Gets the text to be displayed on the button that opens the tab page.
        /// </summary>
        string TabPageTitle { get; }

        /// <summary>
        /// Creates the control that will be displayed in the tab page. The method is called only once and after that the control is reused.
        /// </summary>
        /// <returns>An initialized control that is displayed in the tab page.</returns>
        System.Windows.Forms.Control Initialize();
    }
}
