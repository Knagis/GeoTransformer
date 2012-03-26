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
        #region [ GPX files ]

        /// <summary>
        /// Loads a <see cref="GpxDocument"/> from the given file name.
        /// This method sets <see cref="GpxMetadata.OriginalFileName"/> property.
        /// </summary>
        /// <param name="fileName">Path to the GPX file.</param>
        /// <returns>A GPX document with data from the file.</returns>
        public static Gpx.GpxDocument Gpx(string fileName)
        {
            var gpx = new GpxDocument(XDocument.Load(fileName));
            gpx.Metadata.OriginalFileName = System.IO.Path.GetFileName(fileName);
            return gpx;
        }

        #endregion

        #region [ ZIP archives ]

        /// <summary>
        /// Unzips all GPX files from the specified ZIP archive and loads them into <see cref="GpxDocument"/> instances.
        /// All files with extension other than <c>.gpx</c> are ignored.
        /// </summary>
        /// <param name="fileName">Path to the ZIP file.</param>
        /// <returns>GPX documents with data from the archive.</returns>
        public static IEnumerable<Gpx.GpxDocument> Zip(string fileName)
        {
            using (var zip = new ICSharpCode.SharpZipLib.Zip.ZipFile(fileName))
                foreach (ICSharpCode.SharpZipLib.Zip.ZipEntry zipEntry in zip)
                {
                    if (!zipEntry.IsFile || !string.Equals(System.IO.Path.GetExtension(zipEntry.Name), ".gpx", StringComparison.OrdinalIgnoreCase))
                        continue;

                    using (var zips = zip.GetInputStream(zipEntry))
                    {
                        var gpx = new GpxDocument(XDocument.Load(zips));
                        gpx.Metadata.OriginalFileName = System.IO.Path.GetFileName(zipEntry.Name);
                        yield return gpx;
                    }
                }
        }

        /// <summary>
        /// Unzips the specified <paramref name="entry"/> from the ZIP archive and loads it into a <see cref="GpxDocument"/>.
        /// </summary>
        /// <param name="fileName">Path to the ZIP file.</param>
        /// <param name="entry">The name of the ZIP archive entry.</param>
        /// <returns>GPX document with data from the archive entry.</returns>
        public static Gpx.GpxDocument Zip(string fileName, string entry)
        {
            using (var zip = new ICSharpCode.SharpZipLib.Zip.ZipFile(fileName))
            using (var zips = zip.GetInputStream(zip.GetEntry(entry)))
            {
                var gpx = new GpxDocument(XDocument.Load(zips));
                gpx.Metadata.OriginalFileName = System.IO.Path.GetFileName(entry);
                return gpx;
            }
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

            var groundspeak = XmlExtensions.GeocacheSchema_1_0_1;
            var cache = new XElement(groundspeak + "cache");
            // this line forces the output XML to include prefixes.
            cache.SetAttributeValue(XNamespace.Xmlns + "groundspeak", XmlExtensions.GeocacheSchema_1_0_1);
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

            return wpt;
        }

        #endregion
    }
}
