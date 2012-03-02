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
    /// An interface intended for transformers that require a popup form to be displayed for user interaction. For example, to allow the user to enter CAPTCHA words.
    /// As the transformers are executed on a background thread, such operation requires the UI code to be executed using <see cref="System.Windows.Forms.Control.Invoke"/> method
    /// on the <see cref="ParentWindow"/> property. Also the property is used to display modal dialogs.
    /// </summary>
    public interface ITransformerWithPopup : ITransformer
    {
        /// <summary>
        /// Sets the parent window that should be used if the transformer has to show a dialog user interface.
        /// </summary>
        System.Windows.Forms.Form ParentWindow { set; }
    }
}
