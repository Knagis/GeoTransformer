/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Web;

namespace GeocachingService.OAuth
{
    /// <summary>
    /// Simple HTTP handler that redirects the user to geocaching.com for authentication and stops the execution once the OAuth
    /// process returns to it.
    /// </summary>
    public class Login : IHttpHandler
    {
        /// <summary>
        /// The end-point for the OAuth service.
        /// </summary>
        private static DotNetOpenAuth.Messaging.MessageReceivingEndpoint OAuthEndPoint =
            new DotNetOpenAuth.Messaging.MessageReceivingEndpoint("https://www.geocaching.com/OAuth/mobileoauth.ashx", DotNetOpenAuth.Messaging.HttpDeliveryMethods.PostRequest);

        /// <summary>
        /// Description for OAuth service provider.
        /// </summary>
        private static DotNetOpenAuth.OAuth.ServiceProviderDescription OAuthService =
            new DotNetOpenAuth.OAuth.ServiceProviderDescription()
            {
                AccessTokenEndpoint = OAuthEndPoint,
                RequestTokenEndpoint = OAuthEndPoint,
                UserAuthorizationEndpoint = OAuthEndPoint,
                TamperProtectionElements = new DotNetOpenAuth.Messaging.ITamperProtectionChannelBindingElement[]
                {
                    new DotNetOpenAuth.OAuth.ChannelElements.HmacSha1SigningBindingElement()
                }
            };

        /// <summary>
        /// The token manager for the production service.
        /// </summary>
        private static TokenManager TokenManager = new TokenManager(LiveApiKeys.ProductionKey, LiveApiKeys.ProductionSecret);

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
            AuthHandler.HandleRequest(context, TokenManager, OAuthService);
        }

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}