/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.GeocachingService
{
    public partial class LiveClient
    {
        /// <summary>
        /// Gets a value indicating whether the the use of Groundspeak Geocaching Live service is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the service is enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool IsEnabled
        {
            get
            {
                var ext = GeoTransformer.Extensions.ExtensionLoader.RetrieveExtensions<Extensions.GeocachingService.GeocachingService>().FirstOrDefault();
                if (ext == null || ext.ConfigurationControl == null)
                    return false;

                return ext.ConfigurationControl.IsEnabled;
            }
        }

        /// <summary>
        /// Occurs when the value of <see cref="IsEnabled"/> has changed.
        /// </summary>
        public static event EventHandler IsEnabledChanged;

        private static Dictionary<int, string> _countries = new Dictionary<int, string>()
        {
            {12, "Afghanistan"},{272, "Aland Islands"},{244, "Albania"},{14, "Algeria"},{245, "American Samoa"},{16, "Andorra"},{17, "Angola"},{246, "Anguilla"},{18, "Antarctica"},{13, "Antigua and Barbuda"},{19, "Argentina"},{15, "Armenia"},{20, "Aruba"},{3, "Australia"},{227, "Austria"},{21, "Azerbaijan"},{23, "Bahamas"},{29, "Bahrain"},{24, "Bangladesh"},{25, "Barbados"},{40, "Belarus"},{4, "Belgium"},{31, "Belize"},{26, "Benin"},{27, "Bermuda"},{30, "Bhutan"},{32, "Bolivia"},{275, "Bonaire"},{234, "Bosnia and Herzegovina"},{33, "Botswana"},{247, "Bouvet Island"},{34, "Brazil"},{248, "British Indian Ocean Territories"},{39, "British Virgin Islands"},{36, "Brunei"},{37, "Bulgaria"},{216, "Burkina Faso"},{35, "Burundi"},{42, "Cambodia"},{43, "Cameroon"},{5, "Canada"},{239, "Cape Verde"},{44, "Cayman Islands"},{46, "Central African Republic"},{249, "Chad"},{6, "Chile"},{47, "China"},{250, "Christmas Island"},{251, "Cocos (Keeling) Islands"},{49, "Colombia"},{50, "Comoros"},{51, "Congo"},{48, "Cook Islands"},{52, "Costa Rica"},{53, "Croatia"},{238, "Cuba"},{54, "Curacao"},{55, "Cyprus"},{56, "Czech Republic"},{257, "Democratic Republic of the Congo"},{57, "Denmark"},{58, "Djibouti"},{59, "Dominica"},{60, "Dominican Republic"},{252, "East Timor"},{61, "Ecuador"},{63, "Egypt"},{64, "El Salvador"},{62, "Equatorial Guinea"},{65, "Eritrea"},{66, "Estonia"},{67, "Ethiopia"},{69, "Falkland Islands"},{68, "Faroe Islands"},{71, "Fiji"},{72, "Finland"},{73, "France"},{70, "French Guiana"},{74, "French Polynesia"},{253, "French Southern Territories"},{75, "Gabon"},{76, "Gambia"},{78, "Georgia"},{79, "Germany"},{254, "Ghana"},{80, "Gibraltar"},{82, "Greece"},{83, "Greenland"},{81, "Grenada"},{77, "Guadeloupe"},{229, "Guam"},{84, "Guatemala"},{86, "Guernsey"},{255, "Guinea"},{85, "Guinea-Bissau"},{87, "Guyana"},{89, "Haiti"},{256, "Heard Island And Mcdonald Islands"},{90, "Honduras"},{91, "Hong Kong"},{92, "Hungary"},{93, "Iceland"},{94, "India"},{95, "Indonesia"},{96, "Iran"},{97, "Iraq"},{7, "Ireland"},{243, "Isle of Man"},{98, "Israel"},{99, "Italy"},{100, "Ivory Coast"},{101, "Jamaica"},{104, "Japan"},{102, "Jersey"},{103, "Jordan"},{106, "Kazakhstan"},{107, "Kenya"},{109, "Kiribati"},{241, "Kuwait"},{108, "Kyrgyzstan"},{110, "Laos"},{111, "Latvia"},{113, "Lebanon"},{114, "Lesotho"},{115, "Liberia"},{112, "Libya"},{116, "Liechtenstein"},{117, "Lithuania"},{8, "Luxembourg"},{258, "Macau"},{125, "Macedonia"},{119, "Madagascar"},{129, "Malawi"},{121, "Malaysia"},{124, "Maldives"},{127, "Mali"},{128, "Malta"},{240, "Marshall Islands"},{122, "Martinique"},{123, "Mauritania"},{134, "Mauritius"},{259, "Mayotte"},{228, "Mexico"},{242, "Micronesia"},{237, "Moldova"},{130, "Monaco"},{131, "Mongolia"},{274, "Montenegro"},{135, "Montserrat"},{132, "Morocco"},{133, "Mozambique"},{136, "Myanmar"},{137, "Namibia"},{138, "Nauru"},{140, "Nepal"},{141, "Netherlands"},{41, "New Caledonia"},{9, "New Zealand"},{144, "Nicaragua"},{143, "Niger"},{145, "Nigeria"},{149, "Niue"},{260, "Norfolk Island"},{236, "Northern Mariana Islands"},{147, "Norway"},{150, "Oman"},{151, "Pakistan"},{261, "Palau"},{276, "Palestine"},{152, "Panama"},{156, "Papua New Guinea"},{262, "Paraguay"},{153, "Peru"},{154, "Philippines"},{155, "Pitcairn Islands"},{158, "Poland"},{159, "Portugal"},{226, "Puerto Rico"},{160, "Qatar"},{161, "Reunion"},{162, "Romania"},{163, "Russia"},{164, "Rwanda"},{277, "Saba"},{171, "Saint Helena"},{264, "Saint Kitts and Nevis"},{173, "Saint Lucia"},{217, "Samoa"},{183, "San Marino"},{176, "Sao Tome and Principe"},{166, "Saudi Arabia"},{167, "Senegal"},{222, "Serbia"},{168, "Seychelles"},{178, "Sierra Leone"},{179, "Singapore"},{182, "Slovakia"},{181, "Slovenia"},{184, "Solomon Islands"},{185, "Somalia"},{165, "South Africa"},{267, "South Georgia and Sandwich Islands"},{180, "South Korea"},{278, "South Sudan"},{186, "Spain"},{187, "Sri Lanka"},{169, "St Barthelemy"},{170, "St Eustatius"},{175, "St Pierre Miquelon"},{177, "St Vince Grenadines"},{174, "St. Martin"},{188, "Sudan"},{189, "Suriname"},{268, "Svalbard and Jan Mayen"},{190, "Swaziland"},{10, "Sweden"},{192, "Switzerland"},{193, "Syria"},{194, "Taiwan"},{195, "Tajikistan"},{196, "Tanzania"},{198, "Thailand"},{200, "Togo"},{269, "Tokelau"},{201, "Tonga"},{202, "Trinidad and Tobago"},{203, "Tunisia"},{204, "Turkey"},{199, "Turkmenistan"},{197, "Turks and Caicos Islands"},{205, "Tuvalu"},{208, "Uganda"},{207, "Ukraine"},{206, "United Arab Emirates"},{11, "United Kingdom"},{210, "Uruguay"},{270, "US Minor Outlying Islands"},{235, "US Virgin Islands"},{211, "Uzbekistan"},{212, "Vanuatu"},{213, "Vatican City State"},{214, "Venezuela"},{215, "Vietnam"},{218, "Wallis And Futuna Islands"},{271, "Western Sahara"},{220, "Yemen"},{224, "Zambia"},{225, "Zimbabwe"}
        };

        /// <summary>
        /// Gets a cached collection of all country ID's and names.
        /// </summary>
        public static Dictionary<int, string> Countries
        {
            get
            {
                return _countries;
            }
        }

        private static Dictionary<int, string> _attributes;
        /// <summary>
        /// Gets a cached collection of all geocache attributes. Returns an empty dictionary if <see cref="IsEnabled"/> is <c>false</c> or when
        /// the service call fails.
        /// </summary>
        public static Dictionary<int, string> Attributes
        {
            get
            {
                if (_attributes != null)
                    return _attributes;

                return _attributes = new Dictionary<int, string>
                {
                    {1, "Dogs"}, 
                    {2, "Access or parking fee"}, 
                    {3, "Climbing gear"}, 
                    {4, "Boat"}, 
                    {5, "Scuba gear"}, 
                    {6, "Recommended for kids"}, 
                    {7, "Takes less than an hour"}, 
                    {8, "Scenic view"}, 
                    {9, "Significant Hike"}, 
                    {10, "Difficult climbing"}, 
                    {11, "May require wading"}, 
                    {12, "May require swimming"}, 
                    {13, "Available at all times"}, 
                    {14, "Recommended at night"}, 
                    {15, "Available during winter"}, 
                    {16, "Cactus"}, 
                    {17, "Poison plants"}, 
                    {18, "Dangerous Animals"}, 
                    {19, "Ticks"},
                    {20, "Abandoned mines"},
                    {21, "Cliff / falling rocks"},
                    {22, "Hunting"},
                    {23, "Dangerous area"},
                    {24, "Wheelchair accessible"},
                    {25, "Parking available"},
                    {26, "Public transportation"},
                    {27, "Drinking water nearby"},
                    {28, "Public restrooms nearby"},
                    {29, "Telephone nearby"},
                    {30, "Picnic tables nearby"},
                    {31, "Camping available"},
                    {32, "Bicycles"},
                    {33, "Motorcycles"},
                    {34, "Quads"},
                    {35, "Off-road vehicles"}, 
                    {36, "Snowmobiles"}, 
                    {37, "Horses"}, 
                    {38, "Campfires"}, 
                    {39, "Thorns"}, 
                    {40, "Stealth required"}, 
                    {41, "Stroller accessible"}, 
                    {42, "Needs maintenance"}, 
                    {43, "Watch for livestock"}, 
                    {44, "Flashlight required"}, 
                    {45, "Lost And Found Tour"}, 
                    {46, "Truck Driver/RV"}, 
                    {47, "Field Puzzle"}, 
                    {48, "UV Light Required"}, 
                    {49, "Snowshoes"}, 
                    {50, "Cross Country Skis"},
                    {51, "Special Tool Required"}, 
                    {52, "Night Cache"}, 
                    {53, "Park and Grab"}, 
                    {54, "Abandoned Structure"}, 
                    {55, "Short hike (less than 1km)"}, 
                    {56, "Medium hike (1km-10km)"}, 
                    {57, "Long Hike (+10km)"}, 
                    {58, "Fuel Nearby"}, 
                    {59, "Food Nearby"}, 
                    {60, "Wireless Beacon"}, 
                    {61, "Partnership Cache"}, 
                    {62, "Seasonal Access"}, 
                    {63, "Tourist Friendly"}, 
                    {64, "Tree Climbing"}, 
                    {65, "Front Yard (Private Residence)"}, 
                    {66, "Teamwork Required"},
                };
            }
        }

        /// <summary>
        /// Raises the <see cref="IsEnabledChanged"/> event.
        /// </summary>
        internal static void OnIsEnabledChanged()
        {
            var h = IsEnabledChanged;
            if (h != null)
                h(null, EventArgs.Empty);
        }

        /// <summary>
        /// Gets the access token that can be used to call the Live service. Returns <c>null</c> if the extension is not enabled.
        /// </summary>
        public string AccessToken
        {
            get
            {
                var ext = Extensions.ExtensionLoader.RetrieveExtensions<Extensions.GeocachingService.GeocachingService>().FirstOrDefault();
                if (ext == null)
                    return null;

                return ext.ConfigurationControl.AccessToken;
            }
        }

        /// <summary>
        /// Creates the client proxy using the default configuration.
        /// </summary>
        /// <returns>An initialized client proxy instance.</returns>
        public static LiveClient CreateClientProxy()
        {
            var encoding = new System.ServiceModel.Channels.BinaryMessageEncodingBindingElement();
            encoding.MaxReadPoolSize = 64;
            encoding.MaxReadPoolSize = 16;
            encoding.MaxSessionSize = 2048;
            encoding.ReaderQuotas.MaxDepth = 32;
            encoding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            encoding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            encoding.ReaderQuotas.MaxBytesPerRead = 4096;
            encoding.ReaderQuotas.MaxNameTableCharCount = 16384;

            var transport = new System.ServiceModel.Channels.HttpsTransportBindingElement();
            transport.ManualAddressing = false;
            transport.MaxBufferPoolSize = 524288;
            transport.MaxReceivedMessageSize = int.MaxValue;
            transport.AllowCookies = false;
            transport.AuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous;
            transport.BypassProxyOnLocal = false;
            transport.DecompressionEnabled = true;
            transport.HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
            transport.KeepAliveEnabled = true;
            transport.MaxBufferSize = 65536;
            transport.ProxyAuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous;
            transport.Realm = string.Empty;
            transport.TransferMode = System.ServiceModel.TransferMode.StreamedResponse;
            transport.UnsafeConnectionNtlmAuthentication = false;
            transport.UseDefaultWebProxy = true;
            transport.RequireClientCertificate = false;

            var binding = new System.ServiceModel.Channels.CustomBinding(encoding, transport);
            binding.SendTimeout = new TimeSpan(0, 1, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 10, 0);

            var lc = new LiveClient(binding, new System.ServiceModel.EndpointAddress(GeoTransformer.Extensions.GeocachingService.Configuration.ServiceAddress));

            foreach (var current in lc.Endpoint.Contract.Operations)
                foreach (var current2 in current.Behaviors)
                {
                    var op = current2 as System.ServiceModel.Description.DataContractSerializerOperationBehavior;
                    if (op != null)
                        op.MaxItemsInObjectGraph = int.MaxValue;
                }

            return lc;
        }

        #region [ Easy to use overloads ]

        /// <summary>
        /// Gets the user profile of the currently logged in user. Will return the copy that is cached when the
        /// service is enabled (or when the application is launched). The response has all data populated.
        /// </summary>
        /// <remarks>If the method is called before the data is first cached then it calls the service to return the object.</remarks>
        /// <returns>The filled response object or <c>null</c> if the Live API is currently disabled.</returns>
        public GetUserProfileResponse GetYourUserProfileCached()
        {
            var ext = Extensions.ExtensionLoader.RetrieveExtensions<Extensions.GeocachingService.GeocachingService>().FirstOrDefault();
            if (ext == null)
                return null;

            if (!ext.ConfigurationControl.IsEnabled)
                return null;

            var x = ext.ConfigurationControl.UserProfile;
            if (x != null && x.Status.StatusCode == 0)
                return x;

            var result = this.GetYourUserProfile(true, true, true, true, true, true);
            ext.ConfigurationControl.UserProfile = result;
            return result;
        }

        /// <summary>
        /// Determines whether the current user is basic member.
        /// </summary>
        public bool? IsBasicMember()
        {
            var profile = this.GetYourUserProfileCached();
            if (profile.Status.StatusCode != 0 || profile.Profile.User == null || string.IsNullOrEmpty(profile.Profile.User.MemberType.MemberTypeName))
                return null;

            return string.Equals(this.GetYourUserProfileCached().Profile.User.MemberType.MemberTypeName, "Basic", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Retrieves a single geocache by its code.
        /// </summary>
        /// <param name="code, "The code of the geocache to search for.</param>
        /// <param name="liteVersion, "if set to <c>true</c> returns the lite version of the data. Default is to use lite version if the user is not a premium member.</param>
        /// <returns>The <see cref="Geocache"/> object or <c>null</c> if the service is disabled or the cache cannot be found.</returns>
        public Geocache GetGeocacheByCode(string code, bool? liteVersion = null)
        {
            if (!IsEnabled)
                return null;

            if (string.IsNullOrEmpty(code))
                return null;

            bool useLite;
            if (!liteVersion.HasValue)
                useLite = this.IsBasicMember() ?? true;
            else
                useLite = liteVersion.Value;

            var req = new SearchForGeocachesRequest();
            req.AccessToken = this.AccessToken;
            req.MaxPerPage = 1;
            req.CacheCode = new CacheCodeFilter() { CacheCodes = new string[] { code } };
            req.IsLite = useLite;
            var res = this.SearchForGeocaches(req);

            while (res.Status.StatusCode == 140)
            {
                // 140 - too many calls per minute
                System.Threading.Thread.Sleep(10000);
                res = this.SearchForGeocaches(req);
            }

            if (res.Status.StatusCode != 0)
                return null;

            if (res.Geocaches.Length == 0)
                return null;

            return res.Geocaches[0];
        }

        /// <summary>
        /// Retrieves a list of geocaches by their codes.
        /// </summary>
        /// <param name="codes, "The list of codes for the geocache to search for.</param>
        /// <param name="liteVersion, "if set to <c>true</c> returns the lite version of the data. Default is to use lite version if the user is not a premium member.</param>
        /// <param name="errorHandler, "specifies the delegate that will be called if the Live API returns an error.</param>
        /// <returns>The <see cref="Geocache"/> objects for all loaded caches. If the service is disabled returns an empty list. All caches that cannot be loaded are not returned in the result set.</returns>
        public IEnumerable<Geocache> GetGeocachesByCode(IEnumerable<string> codes, bool? liteVersion = null, Action<string> errorHandler = null)
        {
            if (!IsEnabled || codes == null)
                yield break;

            bool useLite;
            if (!liteVersion.HasValue)
                useLite = this.IsBasicMember() ?? true;
            else
                useLite = liteVersion.Value;

            var req = new SearchForGeocachesRequest();
            req.AccessToken = this.AccessToken;
            req.IsLite = useLite;

            var subset = new List<string>();
            foreach (var c in codes)
            {
                subset.Add(c);

                if (subset.Count == 50)
                {
                    req.MaxPerPage = subset.Count;
                    req.CacheCode = new CacheCodeFilter() { CacheCodes = subset.ToArray() };
                    var res = this.SearchForGeocaches(req);

                    while (res.Status.StatusCode == 140)
                    {
                        // 140 - too many calls per minute
                        System.Threading.Thread.Sleep(10000);
                        res = this.SearchForGeocaches(req);
                    }

                    if (res.Status.StatusCode != 0)
                    {
                        if (errorHandler != null)
                            errorHandler(res.Status.StatusMessage);
                        yield break;
                    }

                    foreach (var x in res.Geocaches)
                        yield return x;

                    subset.Clear();
                }
            }

            if (subset.Count > 0)
            {
                req.MaxPerPage = subset.Count;
                req.CacheCode = new CacheCodeFilter() { CacheCodes = subset.ToArray() };
                var res = this.SearchForGeocaches(req);

                if (res.Status.StatusCode != 0)
                    yield break;

                foreach (var x in res.Geocaches)
                    yield return x;
            }
        }

        /// <summary>
        /// Gets the user profile of the currently logged in user. Automatically populates the request with access token.
        /// </summary>
        /// <param name="challenges, "if set to <c>true</c>, downloads challenge data.</param>
        /// <param name="favoritePoints, "if set to <c>true</c>, downloads favorite points.</param>
        /// <param name="geocaches, "if set to <c>true</c>, downloads geocache data.</param>
        /// <param name="publicProfile, "if set to <c>true</c>, downloads public profile.</param>
        /// <param name="souvenirs, "if set to <c>true</c>, downloads souvenir data.</param>
        /// <param name="trackables, "if set to <c>true</c>, downloads trackable data.</param>
        public GetUserProfileResponse GetYourUserProfile(bool challenges = false, bool favoritePoints = false, bool geocaches = false, bool publicProfile = false, bool souvenirs = false, bool trackables = false)
        {
            var req = new GeoTransformer.GeocachingService.GetYourUserProfileRequest();
            req.AccessToken = this.AccessToken;

            var process = System.Diagnostics.Process.GetCurrentProcess();
            req.DeviceInfo = new DeviceData()
            {
                ApplicationCurrentMemoryUsage = (int)process.WorkingSet64,
                ApplicationPeakMemoryUsage = (int)process.PeakWorkingSet64,
                ApplicationSoftwareVersion = System.Windows.Forms.Application.ProductVersion,
                DeviceManufacturer = string.Empty,
                DeviceName = "Windows PC",
                DeviceOperatingSystem = "Windows " + Environment.OSVersion.VersionString,
                DeviceTotalMemoryInMB = Environment.SystemPageSize / 1024 / 1024,
                DeviceUniqueId = Environment.MachineName,
                MobileHardwareVersion = string.Empty,
                WebBrowserVersion = string.Empty
            };

            req.ProfileOptions = new UserProfileOptions()
            {
                ChallengesData = challenges,
                FavoritePointsData = favoritePoints,
                GeocacheData = geocaches,
                PublicProfileData = publicProfile,
                SouvenirData = souvenirs,
                TrackableData = trackables
            };

            return this.GetYourUserProfile(req);
        }

        #endregion
    }
}
