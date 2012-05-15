/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */

using System;
using System.IO.Compression;
using System.Web;

namespace FetchStatistics.Web
{
    /// <summary>
    /// The HTTP application class for this application.
    /// </summary>
    public class Global : System.Web.HttpApplication
    {
        /// <summary>
        /// Handles the PreSendRequestHeaders event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            // ensure that if GZip/Deflate Encoding is applied that headers are set
            // also works when error occurs if filters are still active
            HttpResponse response = HttpContext.Current.Response;
            if (response.Filter is GZipStream && response.Headers["Content-encoding"] != "gzip")
                response.AppendHeader("Content-encoding", "gzip");

            else if (response.Filter is DeflateStream && response.Headers["Content-encoding"] != "deflate")
                response.AppendHeader("Content-encoding", "deflate");
        }
    }
}