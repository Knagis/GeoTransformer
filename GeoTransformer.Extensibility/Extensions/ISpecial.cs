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
    /// Marker interface used by special classes that do not need to be loaded automatically with other extensions.
    /// These extensions are not available in <see cref="ExtensionLoader.Extensions"/> and must be initialized by
    /// <see cref="IPublisher"/> or a similar extension.
    /// </summary>
    /// <remarks>Extensions marked with <see cref="ISpecial"/> are not required to have parameterless constructor.</remarks>
    public interface ISpecial
    {
    }
}
