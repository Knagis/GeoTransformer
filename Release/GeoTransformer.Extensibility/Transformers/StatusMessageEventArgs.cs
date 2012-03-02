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
    /// Contains the status message from the transformer.
    /// </summary>
    public class StatusMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusReportEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message text.</param>
        public StatusMessageEventArgs(string message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusReportEventArgs"/> class.
        /// </summary>
        /// <param name="isError">if set to <c>true</c> an error occurred in the transformer.</param>
        /// <param name="message">The error text.</param>
        public StatusMessageEventArgs(bool isError, string message)
        {
            this.IsError = isError;
            this.Message = message;
        }

        /// <summary>
        /// Gets a value indicating whether the message describes an error in the transformer process.
        /// </summary>
        public bool IsError { get; private set; }

        /// <summary>
        /// Gets the message text.
        /// </summary>
        public string Message { get; private set; }
    }
}
