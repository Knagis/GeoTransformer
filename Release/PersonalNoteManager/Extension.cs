/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersonalNoteManager
{
    /// <summary>
    /// The extension class - used by GeoTransformer to initialize the extension.
    /// </summary>
    public class Extension : GeoTransformer.Extensions.ITopLevelTabPage
    {
        /// <summary>
        /// Gets the image to be displayed on the button.
        /// </summary>
        public System.Drawing.Image TabPageImage
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the text to be displayed on the button that opens the tab page.
        /// </summary>
        public string TabPageTitle
        {
            get { return "Personal notes"; }
        }

        /// <summary>
        /// Creates the control that will be displayed in the tab page. The method is called only once and after that the control is reused.
        /// </summary>
        /// <returns>
        /// An initialized control that is displayed in the tab page.
        /// </returns>
        public System.Windows.Forms.Control Initialize()
        {
            return new Window();
        }
    }
}
