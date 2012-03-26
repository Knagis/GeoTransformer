/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;

namespace GeoTransformer.Coordinates
{
    /// <summary>
    /// Static utility class containing code to call service on <c>geonames.org</c>.
    /// </summary>
    public static class GeoNames
    {
        /// <summary>
        /// Invokes the specified method on GeoNames web service and returns the resulting XML document.
        /// </summary>
        /// <param name="method">The name of the method that will be invoked.</param>
        /// <param name="parameters">The parameters to the method.</param>
        /// <returns>The <see cref="XDocument"/> containing the result from GeoNames web service.</returns>
        public static XDocument Invoke(string method, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var url = "http://api.geonames.org/" + method + "?username=" + GeoNamesKeys.Username;
            if (parameters != null)
                foreach (var p in parameters)
                    url += "&" + System.Uri.EscapeDataString(p.Key) + "=" + System.Uri.EscapeDataString(p.Value);

            return XDocument.Load(url);
        }

        /// <summary>
        /// Invokes the specified method on GeoNames web service and returns the resulting XML document.
        /// </summary>
        /// <param name="method">The name of the method that will be invoked.</param>
        /// <param name="parameters">The parameters to the method. The array must contain even number of values - pairs of parameter names and values.</param>
        /// <returns>The <see cref="XDocument"/> containing the result from GeoNames web service.</returns>
        public static XDocument Invoke(string method, params string[] parameters)
        {
            var url = "http://api.geonames.org/" + method + "?username=" + GeoNamesKeys.Username;
            if (parameters != null)
            {
                if (parameters.Length % 2 != 0)
                    throw new ArgumentException("The parameters must contain an even number of values - pairs of parameter names and values.", "parameters");

                for (int i = 0; i < parameters.Length; i += 2)
                    url += "&" + System.Uri.EscapeDataString(parameters[i]) + "=" + System.Uri.EscapeDataString(parameters[i + 1]);
            }

            return XDocument.Load(url);
        }

        private static string FormatPlaceName(XElement gn)
        {
            if (gn == null)
                return null;

            var placeName = gn.Element("name").GetValue();
            var adminName = gn.Element("adminName1").GetValue();
            var countryName = gn.Element("countryName").GetValue();

            bool close = false;
            var fullName = placeName;
            if (!string.Equals(placeName, adminName, StringComparison.Ordinal))
            {
                fullName += " (" + adminName;
                close = true;
            }

            if (!string.Equals(countryName, adminName ?? placeName, StringComparison.Ordinal))
            {
                if (close)
                    fullName += ", ";
                else
                    fullName += " (";
                fullName += countryName;
                close = true;
            }

            if (close)
                fullName += ")";

            return fullName;
        }

        /// <summary>
        /// Finds the name of the nearest populated place for the given coordinates.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <returns>The name of the place or <c>null</c> if GeoNames service does not return a result.</returns>
        /// <remarks>This method will not throw an exception if the service is not available.</remarks>
        public static string FindNearbyPlaceName(Wgs84Point coordinates)
        {
            try
            {
                var result = Invoke("findNearbyPlaceName",
                                    "lat", coordinates.Latitude.ToString(CultureInfo.InvariantCulture),
                                    "lng", coordinates.Longitude.ToString(CultureInfo.InvariantCulture),
                                    "maxRows", "1",
                                    "style", "FULL");

                var gn = result.Root.Element("geoname");
                if (gn == null)
                    return null;

                return FormatPlaceName(gn);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Searches for the specified place name and returns the coordinates of the best result.
        /// </summary>
        /// <param name="name">The name of the place.</param>
        /// <param name="fullName">The full name of the place as returned by the search results.</param>
        /// <returns>The coordinates of the best result.</returns>
        public static Wgs84Point? Search(string name, out string fullName)
        {
            fullName = null;
            try
            {
                var result = Invoke("search",
                                    "q", name,
                                    "maxRows", "10",
                                    "isNameRequired", "true",
                                    "style", "FULL");

                var geonames = result.Root.Elements("geoname")
                                          .ToLookup(o => o.Element("fcl").Value);

                XElement gn = null;
                if (geonames.Contains("A"))
                    gn = geonames["A"].First();
                else if (geonames.Contains("P"))
                    gn = geonames["P"].First();
                else
                    gn = result.Root.Elements("geoname").FirstOrDefault();

                if (gn == null)
                    return null;

                fullName = FormatPlaceName(gn);

                return new Wgs84Point(decimal.Parse(gn.Element("lat").Value, CultureInfo.InvariantCulture), decimal.Parse(gn.Element("lng").Value, CultureInfo.InvariantCulture));
            }
            catch
            {
                return null;
            }
        }
    }
}
