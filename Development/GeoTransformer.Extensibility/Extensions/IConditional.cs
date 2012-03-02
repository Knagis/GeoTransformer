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
    /// An interface that allows the extension to inform the engine that it does not want to be executed.
    /// </summary>
    public interface IConditional
    {
        /// <summary>
        /// Gets a value indicating whether the this extension should be executed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this extension is enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsEnabled { get; }
    }
}
