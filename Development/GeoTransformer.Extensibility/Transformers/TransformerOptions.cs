/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Transformers
{
    /// <summary>
    /// Specifies the context how the transformers should process.
    /// </summary>
    [Flags]
    public enum TransformerOptions
    {
        /// <summary>
        /// No special options apply to the transformer.
        /// </summary>
        None = 0,

        /// <summary>
        /// If this flag is set, the transformer should not perform any operation that requires user intervention.
        /// </summary>
        BackgroundOperation = 1,

        /// <summary>
        /// If this flag is set, the transformer has to load data only from the local storage (even expired copies are OK).
        /// If the transformer does not load data, the flag does not apply.
        /// </summary>
        UseLocalStorage = 2 | BackgroundOperation,

        /// <summary>
        /// This flag is set when the transformers are invoked to initialize the cache that is used by the user interface and
        /// not published to any kind of store.
        /// </summary>
        LoadingViewerCache = 4
    }
}
