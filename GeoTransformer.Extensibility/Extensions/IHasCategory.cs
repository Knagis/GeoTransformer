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
    /// An interface that marks the extension as one that has a category specified.
    /// </summary>
    public interface IHasCategory
    {
        /// <summary>
        /// Gets the category of the extension.
        /// </summary>
        Category Category { get; }
    }
}
