/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace GeoTransformer.Gpx
{
    /// <summary>
    /// Class that contains generic methods for loading GPX data from files and other standard structures.
    /// </summary>
    public static class Loader
    {
        #region [ Standard GPX file load ]

        public static XDocument Gpx(string fileName)
        {
            var xml = XDocument.Load(fileName, LoadOptions.PreserveWhitespace);
            xml.Root.SetAttributeValue("originalFileName", Path.GetFileName(fileName));
            PostProcess(xml);
            return xml;
        }

        public static XDocument Gpx(Stream stream, string originalFileName)
        {
            var xml = XDocument.Load(stream, LoadOptions.PreserveWhitespace);
            xml.Root.SetAttributeValue("originalFileName", originalFileName);
            PostProcess(xml);
            return xml;
        }

        #endregion

        #region [ ZIP archives ]

        public static IEnumerable<XDocument> Zip(string fileName)
        {
            using (var zip = new ICSharpCode.SharpZipLib.Zip.ZipFile(fileName))
                foreach (ICSharpCode.SharpZipLib.Zip.ZipEntry zipEntry in zip)
                    using (var zips = zip.GetInputStream(zipEntry))
                        yield return Gpx(zips, zipEntry.Name);
        }

        public static XDocument Zip(string fileName, string entry)
        {
            using (var zip = new ICSharpCode.SharpZipLib.Zip.ZipFile(fileName))
            using (var zips = zip.GetInputStream(zip.GetEntry(entry)))
                return Gpx(zips, entry);
        }

        #endregion

        #region [ Live API Geocache structure ]

        public static XElement Convert(GeocachingService.Geocache geocache)
        {
            if (geocache == null)
                throw new ArgumentNullException("geocache");

            var gpx = XmlExtensions.GpxSchema_1_0;

            var wpt = new XElement(gpx + "wpt");
            wpt.SetAttributeValue("lat", geocache.Latitude);
            wpt.SetAttributeValue("lon", geocache.Longitude);
            wpt.SetElementValue(gpx + "time", geocache.UTCPlaceDate);
            wpt.SetElementValue(gpx + "name", geocache.Code);
            var cacheType = geocache.CacheType == null ? null : geocache.CacheType.GeocacheTypeName;
            if (geocache.CacheType != null && geocache.CacheType.GeocacheTypeId == 8)
                cacheType = "Unknown Cache";
            wpt.SetElementValue(gpx + "desc", string.Format(System.Globalization.CultureInfo.InvariantCulture, 
                "{0} by {1}, {2} ({3}/{4})", geocache.Name, geocache.PlacedBy, cacheType, geocache.Difficulty, geocache.Terrain));
            wpt.SetElementValue(gpx + "url", geocache.Url);
            wpt.SetElementValue(gpx + "urlname", geocache.Name);
            wpt.SetElementValue(gpx + "sym", "Geocache");
            wpt.SetElementValue(gpx + "type", "Geocache|" + cacheType);

            var groundspeak = XmlExtensions.GeocacheSchema101;
            var cache = new XElement(groundspeak + "cache");
            // this line forces the output XML to include prefixes.
            cache.SetAttributeValue(XNamespace.Xmlns + "groundspeak", XmlExtensions.GeocacheSchema101Clean);
            wpt.Add(cache);
            cache.Add(new XAttribute("id", geocache.ID));
            cache.Add(new XAttribute("available", geocache.Available));
            cache.Add(new XAttribute("archived", geocache.Archived));
            cache.Add(new XElement(groundspeak + "name", geocache.Name));
            cache.Add(new XElement(groundspeak + "placed_by", geocache.PlacedBy));
            if (geocache.Owner != null)
                cache.Add(new XElement(groundspeak + "owner", new XAttribute("id", geocache.Owner.Id), geocache.Owner.UserName));
            cache.Add(new XElement(groundspeak + "type", cacheType));
            if (geocache.ContainerType != null)
                cache.Add(new XElement(groundspeak + "container", geocache.ContainerType.ContainerTypeName));

            var attributes = new XElement(groundspeak + "attributes");
            cache.Add(attributes);
            var attributeTypeCache = GeocachingService.LiveClient.Attributes;
            foreach (var a in geocache.Attributes ?? new GeocachingService.Attribute[0])
            {
                if (!attributeTypeCache.ContainsKey(a.AttributeTypeID))
                    continue;
                attributes.Add(new XElement(groundspeak + "attribute",
                                    new XAttribute("id", a.AttributeTypeID),
                                    new XAttribute("inc", a.IsOn ? "1" : "0"),
                                    attributeTypeCache[a.AttributeTypeID].Name
                                ));
            }

            cache.Add(new XElement(groundspeak + "difficulty", geocache.Difficulty));
            cache.Add(new XElement(groundspeak + "terrain", geocache.Terrain));

            // the Lite version does not include the name of the country, just the ID
            var country = geocache.Country;
            if (string.IsNullOrWhiteSpace(country))
                GeocachingService.LiveClient.Countries.TryGetValue(geocache.CountryID, out country);
            cache.Add(new XElement(groundspeak + "country", country));

            cache.Add(new XElement(groundspeak + "state", geocache.State));
            cache.Add(new XElement(groundspeak + "short_description", new XAttribute("html", geocache.ShortDescriptionIsHtml), geocache.ShortDescription));
            cache.Add(new XElement(groundspeak + "long_description", new XAttribute("html", geocache.LongDescriptionIsHtml), geocache.LongDescription));
            cache.Add(new XElement(groundspeak + "encoded_hints", geocache.EncodedHints));

            var logs = new XElement(groundspeak + "logs");
            cache.Add(logs);
            foreach (var l in geocache.GeocacheLogs ?? new GeocachingService.GeocacheLog[0])
            {
                logs.Add(new XElement(groundspeak + "log",
                        new XAttribute("id", l.ID),
                        new XElement(groundspeak + "date", l.VisitDate),
                        new XElement(groundspeak + "type", l.LogType.WptLogTypeName),
                        new XElement(groundspeak + "finder", new XAttribute("id", l.Finder.Id), l.Finder.UserName),
                        new XElement(groundspeak + "text", new XAttribute("encoded", l.LogIsEncoded), l.LogText)
                    ));
            }



            PostProcess(wpt);
            return wpt;
        }

        #endregion

        #region [ Common functionality ]

        /// <summary>
        /// Creates a GPX document without any waypoints with just the basic information.
        /// </summary>
        public static XDocument CreateEmptyDocument(string fileName)
        {
            fileName = System.IO.Path.GetFileName(fileName);

            var ns = XmlExtensions.GpxSchema_1_0;
            var xml = new XDocument(new XElement(ns + "gpx"));

            // set the namespace attributes so that the document is as close as possible to the default
            // gpx files by geocaching.com
            xml.Root.SetAttributeValue(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xml.Root.SetAttributeValue(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema");
            xml.Root.SetAttributeValue("{http://www.w3.org/2001/XMLSchema-instance}schemaLocation", "http://www.topografix.com/GPX/1/0 http://www.topografix.com/GPX/1/0/gpx.xsd http://www.groundspeak.com/cache/1/0/1 http://www.groundspeak.com/cache/1/0/1/cache.xsd");

            xml.Root.SetAttributeValue("version", "1.0");
            xml.Root.SetAttributeValue("creator", "GeoTransformer. http://knagis.miga.lv/GeoTransformer/");
            xml.Root.SetAttributeValue("originalFileName", Path.GetFileName(fileName));
            xml.Root.Add(new XElement(ns + "name", "Geocache listings generated by GeoTransformer"));
            xml.Root.Add(new XElement(ns + "time", DateTime.Now.ToUniversalTime()));
            xml.Root.Add(new XElement(ns + "keywords", "cache, geocache"));

            PostProcess(xml);

            return xml;
        }

        /// <summary>
        /// Performs post-processing of the loaded waypoints (such as creating the cached copy).
        /// </summary>
        /// <param name="waypoints">The waypoints to process.</param>
        private static void PostProcess(IEnumerable<XElement> waypoints)
        {
            if (waypoints == null)
                return;

            // .GetCachedCopy() creates the copy if it does not exist yet.
            foreach (var wpt in waypoints)
                wpt.GetCachedCopy();
        }

        private static void PostProcess(params XElement[] waypoints)
        {
            PostProcess((IEnumerable<XElement>)waypoints);
        }

        private static void PostProcess(XDocument document)
        {
            if (document == null)
                return;

            PostProcess(document.Root.WaypointElements("wpt"));
        }

        #endregion
    }
}
