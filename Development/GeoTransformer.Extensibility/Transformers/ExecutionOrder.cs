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
    /// Specifies the main values that defines the order how transformers are executed. The order values are not limited
    /// to what is specified in this enumerator but other values should be used only in very specific cases.
    /// </summary>
    public enum ExecutionOrder
    {
        /// <summary>
        /// The transformer loads data - has to be executed before any processing is started.
        /// </summary>
        LoadSources = 0,

        /// <summary>
        /// First phase of transforming - usually transformers that reduce the amount of data such as duplicate removal.
        /// </summary>
        PreProcess = 1000,

        /// <summary>
        /// Any transformer that performs data manipulation.
        /// </summary>
        Process = 2000,

        /// <summary>
        /// Transformer that needs the data to be prepared for publishing but executes before saving.
        /// </summary>
        BeforePublish = 3000,

        /// <summary>
        /// The local in memory cache for viewers is created at this point. When doing refresh for viewers nothing past this point is executed.
        /// </summary>
        CreateViewerCache = 4000,

        /// <summary>
        /// Transformer publishes data to some location.
        /// </summary>
        Publish = 5000,

        /// <summary>
        /// Transformer that needs to perform some sort of cleanup after everything has been done.
        /// </summary>
        AfterPublish = 6000
    }
}
