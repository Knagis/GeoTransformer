/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Extensions.GeocachingService
{
    internal static class Configuration
    {
//#if DEBUG
//        public const string ServiceAddress = "https://staging.api.groundspeak.com/Live/V6Beta/geocaching.svc/Silverlightsoap";

//        public const string AuthenticationAddress = "http://knagis.miga.lv/GeoTransformer/LiveApiAuth/LoginStaging.ashx";
//#else
        public const string ServiceAddress = "https://api.groundspeak.com/LiveV6/geocaching.svc/Silverlightsoap";

        public const string AuthenticationAddress = "http://knagis.miga.lv/GeoTransformer/LiveApiAuth/Login.ashx";
//#endif
    }
}
