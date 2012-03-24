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
        /// <param name="severity">The severity of the message.</param>
        /// <param name="message">The message text.</param>
        /// <param name="args">The arguments to pass to <see cref="String.Format"/> function; if specified, the <paramref name="message"/> is considered
        /// to contain the formatting template.</param>
        public StatusMessageEventArgs(StatusSeverity severity, string message, params object[] args)
        {
            this.Severity = severity;

            if (args != null && args.Length > 0)
                this.Message = string.Format(System.Globalization.CultureInfo.CurrentCulture, message, args);
            else
                this.Message = message;
        }

        /// <summary>
        /// Gets the severity of the status message.
        /// </summary>
        public StatusSeverity Severity { get; private set; }

        /// <summary>
        /// Gets the message text.
        /// </summary>
        public string Message { get; private set; }
    }
}
