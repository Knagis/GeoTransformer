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
    /// An interface that markes the extension as one that contains user interface for the configuration data.
    /// </summary>
    public interface IConfigurable : IConditional, IHasCategory
    {
        /// <summary>
        /// Initializes the extension with the specified current configuration (can be <c>null</c> if the extension is initialized for the very first time) and
        /// returns the configuration UI control (can return <c>null</c> if the user interface is not needed).
        /// </summary>
        /// <param name="currentConfiguration">The current configuration.</param>
        /// <returns>The configuration UI control.</returns>
        System.Windows.Forms.Control Initialize(byte[] currentConfiguration);

        /// <summary>
        /// Retrieves the configuration from the extension's configuration UI control.
        /// </summary>
        /// <returns>The serialized configuration data.</returns>
        byte[] SerializeConfiguration();

    }
}
