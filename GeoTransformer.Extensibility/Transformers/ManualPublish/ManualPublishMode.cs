/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;

namespace GeoTransformer.Transformers.ManualPublish
{
    /// <summary>
    /// Lists the modes that <see cref="ManualPublish"/> editor provides.
    /// </summary>
    public enum ManualPublishMode
    {
        /// <summary>
        /// The default mode - waypoint is published only when it is loaded from a persistent source.
        /// </summary>
        Default,

        /// <summary>
        /// The user has chosen to always ignore the waypoint and not put it in the resulting file.
        /// </summary>
        AlwaysSkip,

        /// <summary>
        /// The user has chosen to always put the waypoint in the resulting file. If it is not loaded from a persistent source, a cached copy is published.
        /// </summary>
        AlwaysPublish
    }
}
