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
    /// Specifies the severity of a status message.
    /// </summary>
    public enum StatusSeverity
    {
        /// <summary>
        /// The message is an informative message about the state of the transformer.
        /// </summary>
        Information = 0,

        /// <summary>
        /// The message is a warning about a suboptimal state of the transformer.
        /// </summary>
        Warning = 1,

        /// <summary>
        /// The message is an error message about an exception in the transformer.
        /// The user will be given a chance to ignore the error and proceed with the next transformer.
        /// This severity will cause the engine to stop the execution of the transformer.
        /// </summary>
        Error = 2,

        /// <summary>
        /// The message is a fatal error in the transformer that has to stop the whole process.
        /// The user is not given a chance to ignore it.
        /// All unhandled exception in the transformer execution are considered fatal errors.
        /// This severity will cause the engine to stop the execution of the transformer.
        /// </summary>
        FatalError = 3,
    }
}
