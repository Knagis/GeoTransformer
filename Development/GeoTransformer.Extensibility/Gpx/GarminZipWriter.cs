/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoTransformer.Gpx
{
    /// <summary>
    /// Class for creating indexed Garmin GGZ files. The GGZ file is generated to contain only geocaches, additional waypoints are excluded.
    /// </summary>
    public static class GarminZipWriter
    {
        /// <summary>
        /// The namespace used in the index.xml file.
        /// </summary>
        private static XNamespace GGZ = "http://www.opencaching.com/xmlschemas/ggz/1/0";

        /// <summary>
        /// Writes the geocaches from the given GPX documents to the <paramref name="target"/> stream.
        /// </summary>
        /// <param name="data">Geocache information to be written.</param>
        /// <param name="target">Target stream.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="data"/> or <paramref name="target"/> are <c>null</c></exception>
        public static void Create(IEnumerable<GpxDocument> data, Stream target)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (target == null)
                throw new ArgumentNullException("target");

            var options = GpxSerializationOptions.Compatibility;

            var index = new XDocument(new XElement(GGZ + "ggz"));
            index.Root.Add(new XElement(GGZ + "time", DateTime.UtcNow));

            using (var zip = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(target))
            {
                zip.SetLevel(8);
                zip.UseZip64 = ICSharpCode.SharpZipLib.Zip.UseZip64.Off;
                zip.IsStreamOwner = false;

                using (var enumerator = data.SelectMany(o => o.Waypoints).Where(o => o.Geocache.IsDefined()).GetEnumerator())
                {
                    int i = 1;
                    bool finished = false;

                    while (!finished)
                    {
                        var name = "Default_" + (i++).ToString(System.Globalization.CultureInfo.InvariantCulture) + ".gpx";

                        zip.PutNextEntry(new ICSharpCode.SharpZipLib.Zip.ZipEntry("data/" + name));
                        var ms = CreateFile(index, name, enumerator, options, out finished);
                        ms.CopyTo(zip);
                        zip.CloseEntry();
                    }
                }

                zip.PutNextEntry(new ICSharpCode.SharpZipLib.Zip.ZipEntry("index/com/garmin/geocaches/v0/index.xml"));
                index.Save(zip);
                zip.CloseEntry();
            }
        }

        private static Stream CreateFile(XDocument index, string name, IEnumerator<GpxWaypoint> waypoints, GpxSerializationOptions options, out bool finished)
        {
            finished = true;

            var ms = new MemoryStream();
            var header = new GpxDocument().Serialize(options);
            header.Save(ms);
            ms.Position -= header.Root.Name.LocalName.Length + 3;

            var indexFile = new XElement(GGZ + "file");
            index.Root.Add(indexFile);
            indexFile.Add(new XElement(GGZ + "name", name));
            var indexCrc = new XElement(GGZ + "crc");
            indexFile.Add(indexCrc);
            indexFile.Add(new XElement(GGZ + "time", DateTime.UtcNow));

            while (waypoints.MoveNext())
            {
                var info = waypoints.Current;
                var startPos = ms.Position;
                using (var xmlWriter = System.Xml.XmlWriter.Create(ms, new System.Xml.XmlWriterSettings() { OmitXmlDeclaration = true }))
                    info.Serialize(options).Save(xmlWriter);

                var endPos = ms.Position;

                indexFile.Add(new XElement(
                    GGZ + "gch",
                    new XElement(GGZ + "code", info.Name),
                    new XElement(GGZ + "name", info.Geocache.Name),
                    new XElement(GGZ + "type", info.Geocache.CacheType.Name),
                    new XElement(GGZ + "lat", info.Coordinates.Latitude),
                    new XElement(GGZ + "lon", info.Coordinates.Longitude),
                    new XElement(GGZ + "file_pos", startPos),
                    new XElement(GGZ + "file_len", endPos - startPos),
                    new XElement(
                        GGZ + "ratings",
                        new XElement(GGZ + "awesomeness", 3.0m),
                        new XElement(GGZ + "difficulty", info.Geocache.Difficulty),
                        new XElement(GGZ + "size", info.Geocache.Container.NumericValue),
                        new XElement(GGZ + "terrain", info.Geocache.Terrain)),
                    new XElement(GGZ + "found", string.Equals(info.Symbol, "Geocache Found", StringComparison.OrdinalIgnoreCase))
                    ));

                // create a new file when this one gets past 4MB
                if (endPos > 4194304)
                {
                    finished = false;
                    break;
                }
            }

            var closingTag = Encoding.UTF8.GetBytes(@"</" + header.Root.Name.LocalName + ">");
            ms.Write(closingTag, 0, closingTag.Length);
            ms.Position = 0;

            var crc = new ICSharpCode.SharpZipLib.Checksums.Crc32();
            crc.Update(ms.GetBuffer(), 0, (int)ms.Length);
            indexCrc.Value = crc.Value.ToString("X2");

            return ms;
        }
    }
}
