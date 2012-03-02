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
    /// An interface that allows the transformer to check for related transformers and decide if it is enabled from those relations.
    /// </summary>
    public interface IRelatedConditional
    {
        /// <summary>
        /// Determines whether the transformer is enabled by reviewing the current list of transformers that will be executed for any relations.
        /// The transformer list includes also the transformers that have <see cref="IConditional.IsEnabled"/> set to <c>false</c>.
        /// </summary>
        /// <param name="transformers">The transformers that are pending execution.</param>
        /// <returns>
        ///   <c>true</c> if the current transformer is enabled; otherwise, <c>false</c>.
        /// </returns>
        bool IsEnabled(IEnumerable<ITransformer> transformers);
    }
}
