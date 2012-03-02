/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeocachingService.OAuth
{
    public static class AuthHandler
    {
        public static void HandleRequest(HttpContext context, TokenManager tokenManager, DotNetOpenAuth.OAuth.ServiceProviderDescription provider)
        {
            // this line could be used to prevent certificate errors when using Fiddler
            ////System.Net.ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            // create the OAuth consumer object
            var consumer = new DotNetOpenAuth.OAuth.WebConsumer(provider, tokenManager);

            if (context.Request.QueryString.Count > 0)
            {
                // if the actual access token is in the URL, return an empty response.
                if (context.Request.QueryString["accessToken"] != null)
                    return;

                // if the query string contains any other parameters, it is not a request for new authentication - most probably it contains the oauth_token
                var token = consumer.ProcessUserAuthorization();
                context.Response.Redirect(context.Request.Url.LocalPath + "?accessToken=" + Uri.EscapeDataString(token.AccessToken));
                return;
            }

            // create the message that requests user to be authenticated (this calls the OAuth service to get some tokens)
            var response = consumer.PrepareRequestUserAuthorization();

            // send the message - this will redirect the user.
            consumer.Channel.Send(response);
        }
    }
}