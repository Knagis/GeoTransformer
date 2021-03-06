﻿/*
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
        #region [ Xml self-cleaning reader ]

        /// <summary>
        /// <see cref="TextReader"/> implementation for reading streams and skipping all characters that cause XML
        /// parsing to fail.
        /// </summary>
        /// <remarks>The implementation is not optimized for performance and should only be used if standard parsing does not work.</remarks>
        private class SafeXmlTextReader : TextReader
        {
            private Stream _stream;
            private StreamReader _reader;
            private Queue<int> _pending;

            public SafeXmlTextReader(Stream stream)
            {
                if (stream == null)
                    throw new ArgumentNullException("stream");

                this._stream = stream;
                this._reader = new StreamReader(stream);
                this._pending = new Queue<int>();
            }

            private bool MoveForward()
            {
                while (this._pending.Count == 0)
                {
                    var c1 = this._reader.Read();
                    if (c1 == -1)
                        return false;

                    if (System.Xml.XmlConvert.IsXmlChar((char)c1))
                    {
                        this._pending.Enqueue(c1);
                    }
                    else
                    {
                        var c2 = this._reader.Read();
                        if (c2 == -1)
                            return false;

                        if (System.Xml.XmlConvert.IsXmlSurrogatePair((char)c1, (char)c2))
                        {
                            this._pending.Enqueue(c1);
                            this._pending.Enqueue(c2);
                        }
                        else if (System.Xml.XmlConvert.IsXmlChar((char)c2))
                        {
                            this._pending.Enqueue(c2);
                        }
                    }
                }

                return true;
            }

            public override int Peek()
            {
                if (!this.MoveForward())
                    return -1;
                    
                return this._pending.Peek();
            }

            public override int Read()
            {
                if (!this.MoveForward())
                    return -1;

                return this._pending.Dequeue();
            }
        }

        #endregion

        #region [ GPX files ]

        /// <summary>
        /// Loads a <see cref="XDocument"/> from the given <paramref name="stream"/>. If the loading fails
        /// asks the user if he allows to try to work around the issue. Second parsing is done with all non-XML
        /// characters removed.
        /// </summary>
        /// <param name="stream">A delegate to create the stream from which to load the XML document.</param>
        /// <returns>The <see cref="XDocument"/> loaded from the stream</returns>
        private static XDocument LoadXDocument(Func<Stream> stream)
        {
            long pos = 0;
            Stream str = null;
            try
            {
                str = stream();
                pos = str.Position;
                return XDocument.Load(str);
            }
            catch (System.Xml.XmlException ex)
            {
                try
                {
                    if (str != null)
                        str.Dispose();

                    // recreate the stream to avoid non-seekable streams
                    str = stream();

                    var res = System.Windows.Forms.MessageBox.Show("Unable to read the GPX file:" + Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine + "GeoTransformer can try to work around the problem but the data might not be loaded correctly. Do you want GeoTransformer to try the workaround?", "Invalid GPX data", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question, System.Windows.Forms.MessageBoxDefaultButton.Button2);
                    if (res != System.Windows.Forms.DialogResult.Yes)
                        throw;

                    var settings = new System.Xml.XmlReaderSettings();
                    settings.CheckCharacters = false;
                    using (var safeReader = new SafeXmlTextReader(str))
                    using (var xmlReader = System.Xml.XmlReader.Create(safeReader, settings))
                    {
                        return XDocument.Load(xmlReader);
                    }
                }
                catch
                {
                    throw new System.Xml.XmlException("The GPX document cannot be parsed and error recovery is not possible for this file." + Environment.NewLine + ex.Message, ex);
                }
            }
            finally
            {
                if (str != null)
                    str.Dispose();
            }
        }

        /// <summary>
        /// Loads a <see cref="GpxDocument"/> from the given file name.
        /// This method sets <see cref="GpxMetadata.OriginalFileName"/> property.
        /// </summary>
        /// <param name="fileName">Path to the GPX file.</param>
        /// <returns>A GPX document with data from the file.</returns>
        public static Gpx.GpxDocument Gpx(string fileName)
        {
            var gpx = new GpxDocument(LoadXDocument(() => System.IO.File.OpenRead(fileName)));
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
                    if (!zipEntry.IsFile)
                        continue;

                    var ext = System.IO.Path.GetExtension(zipEntry.Name);
                    GpxDocument gpx;
                    if (string.Equals(ext, ".gpx", StringComparison.OrdinalIgnoreCase))
                    {
                        gpx = new GpxDocument(LoadXDocument(() => zip.GetInputStream(zipEntry)));

                    }
                    else if (string.Equals(ext, ".gclive", StringComparison.OrdinalIgnoreCase))
                    {
                        using (var stream = zip.GetInputStream(zipEntry))
                            gpx = GcLive(stream);
                    }
                    else
                    {
                        continue;
                    }

                    gpx.Metadata.OriginalFileName = System.IO.Path.GetFileName(zipEntry.Name);
                    yield return gpx;
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
            {
                var zipEntry = zip.GetEntry(entry);
                if (!zipEntry.IsFile)
                    throw new ArgumentException("Only file entries are supported.", "entry");

                var ext = System.IO.Path.GetExtension(zipEntry.Name);
                GpxDocument gpx;
                if (string.Equals(ext, ".gpx", StringComparison.OrdinalIgnoreCase))
                {
                    gpx = new GpxDocument(LoadXDocument(() => zip.GetInputStream(zipEntry)));
                }
                else if (string.Equals(ext, ".gclive", StringComparison.OrdinalIgnoreCase))
                {
                    using (var stream = zip.GetInputStream(zipEntry))
                        gpx = GcLive(stream);
                }
                else
                {
                    throw new ArgumentException("The format " + ext + " is not recognized.");
                }

                gpx.Metadata.OriginalFileName = System.IO.Path.GetFileName(zipEntry.Name);
                return gpx;
            }
        }

        #endregion

        #region [ GCLIVE files - serialized data from geocaching.com ]

        /// <summary>
        /// Serializes the given <paramref name="data" /> in <paramref name="fileName" />.
        /// </summary>
        /// <param name="fileName">The path to the file where the data will be serialized.</param>
        /// <param name="useZip">If <c>true</c> then the created file is a ZIP archive with <c>.gclive</c> file in it. Otherwise it is just a <c>.gclive</c> file.</param>
        /// <param name="data">The geocache information that will be updated.</param>
        /// <param name="metadata">Additional document level metadata to be stored with the geocaches.</param>
        /// <exception cref="System.ArgumentNullException">when <paramref name="data"/> is <c>null</c></exception>
        public static void Serialize(string fileName, bool useZip, IEnumerable<GeocachingService.Geocache> data, GpxMetadata metadata = null)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (metadata == null)
                metadata = new GpxMetadata();

            Stream stream = null;
            ICSharpCode.SharpZipLib.Zip.ZipOutputStream zip = null;
            try
            {
                stream = System.IO.File.Create(fileName);

                if (useZip)
                {
                    zip = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(stream);
                    zip.SetLevel(8);
                    zip.UseZip64 = ICSharpCode.SharpZipLib.Zip.UseZip64.Off;
                    zip.PutNextEntry(new ICSharpCode.SharpZipLib.Zip.ZipEntry(System.IO.Path.GetFileNameWithoutExtension(fileName) + ".gclive"));
                    
                    stream = zip;
                }

                // write the version.
                var header = System.Text.Encoding.ASCII.GetBytes("GCLIVE.01");
                stream.Write(header, 0, header.Length);

                var serializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                serializer.Serialize(stream, metadata.Serialize(GpxSerializationOptions.Roundtrip).ToString());
                serializer.Serialize(stream, data.ToArray());

                if (useZip)
                {
                    zip.CloseEntry();
                }
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        /// <summary>
        /// Loads the serialized data from Live API. The data should be serialized using 
        /// </summary>
        /// <param name="stream">The stream containing the serialized geocache data.</param>
        /// <returns>A GPX document with the geocache information parsed.</returns>
        public static GpxDocument GcLive(System.IO.Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            var serializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var header = new byte[System.Text.Encoding.ASCII.GetByteCount("GCLIVE.XX")];
            stream.Read(header, 0, header.Length);

            var headerText = System.Text.Encoding.ASCII.GetString(header);
            if (!string.Equals(headerText, "GCLIVE.01"))
                throw new System.IO.InvalidDataException("The stream does not contain valid header for the Live API serialized data.");

            var metadataXml = (string)serializer.Deserialize(stream);
            var metadata = new Gpx.GpxMetadata(XElement.Parse(metadataXml));
            var data = (GeocachingService.Geocache[])serializer.Deserialize(stream);

            return new GpxDocument(data, metadata);
        }

        #endregion

        #region [ Merge ]

        /// <summary>
        /// Merges the data not present in <paramref name="target"/> from <paramref name="source"/>.
        /// <see cref="String.Empty"/> is not considered missing information, only <c>null</c> counts as missing.
        /// It is in general considered that <paramref name="target"/> has newer data than <paramref name="source"/>.
        /// </summary>
        /// <param name="target">The waypoint that will be modified.</param>
        /// <param name="source">The waypoint that contains missing data.</param>
        /// <exception cref="ArgumentNullException">when either <paramref name="target"/> or <paramref name="source"/> are <c>null</c></exception>
        /// <remarks>The method does not create copies of elements in collections, such as <see cref="GpxLink"/> - instead the same instance is added to <paramref name="target"/> collection.</remarks>
        public static void Merge(GpxWaypoint target, GpxWaypoint source)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (source == null)
                throw new ArgumentNullException("source");

            if (target.Comment == null && source.Comment != null) target.Comment = source.Comment;
            if (target.Description == null && source.Description != null) target.Description = source.Description;
            if (target.DgpsDataAge == null && source.DgpsDataAge != null) target.DgpsDataAge = source.DgpsDataAge;
            if (target.DgpsStationId == null && source.DgpsStationId != null) target.DgpsStationId = source.DgpsStationId;
            if (target.Elevation == null && source.Elevation != null) target.Elevation = source.Elevation;
            if (target.FixType == null && source.FixType != null) target.FixType = source.FixType;
            if (target.GeoidHeight == null && source.GeoidHeight != null) target.GeoidHeight = source.GeoidHeight;
            if (target.HorizontalDilution == null && source.HorizontalDilution != null) target.HorizontalDilution = source.HorizontalDilution;
            if (target.Links.Count == 0 && source.Links.Count > 0) foreach (var l in source.Links) target.Links.Add(l); // note - it would be better to create a copy but currently the Merge method is used in a way that does not require it
            if (target.MagneticVariation == null && source.MagneticVariation != null) target.MagneticVariation = source.MagneticVariation;
            if (target.Name == null && source.Name != null) target.Name = source.Name;
            if (target.PositionDilution == null && source.PositionDilution != null) target.PositionDilution = source.PositionDilution;
            if (target.Satellites == null && source.Satellites != null) target.Satellites = source.Satellites;
            if (target.Source == null && source.Source != null) target.Source = source.Source;
            if (target.Symbol == null && source.Symbol != null) target.Symbol = source.Symbol;
            if (target.VerticalDilution == null && source.VerticalDilution != null) target.VerticalDilution = source.VerticalDilution;
            if (target.WaypointType == null && source.WaypointType != null) target.WaypointType = source.WaypointType;

            var gc1 = target.Geocache;
            var gc2 = source.Geocache;
            if (gc1.Attributes.Count == 0 && gc2.Attributes.Count > 0) foreach (var a in gc2.Attributes) gc1.Attributes.Add(a);
            if ((gc1.CacheType.Id == null && gc2.CacheType.Id != null) || (gc1.CacheType.Name == null && gc2.CacheType.Name != null))
            {
                gc1.CacheType.Id = gc2.CacheType.Id;
                gc1.CacheType.Name = gc2.CacheType.Name;
            }
            if ((gc1.Container.Id == null && gc2.Container.Id != null) || (gc1.Container.Name == null && gc2.Container.Name != null))
            {
                gc1.Container.Id = gc2.Container.Id;
                gc1.Container.Name = gc2.Container.Name;
            }
            if (gc1.Country == null && gc2.Country != null) gc1.Country = gc2.Country;
            if (gc1.CountryState == null && gc2.CountryState != null) gc1.CountryState = gc2.CountryState;
            if (gc1.CustomCoordinates == null && gc2.CustomCoordinates != null)
            {
                gc1.CustomCoordinates = gc2.CustomCoordinates;
                target.Coordinates = source.Coordinates;
            }
            if (gc1.Difficulty == null && gc2.Difficulty != null) gc1.Difficulty = gc2.Difficulty;
            if (gc1.FavoritePoints == null && gc2.FavoritePoints != null) gc1.FavoritePoints = gc2.FavoritePoints;
            if (gc1.Hints == null && gc2.Hints != null) gc1.Hints = gc2.Hints;
            if (gc1.Id == null && gc2.Id != null) gc1.Id = gc2.Id;
            if (gc1.Images.Count == 0 && gc2.Images.Count > 0) foreach (var i in gc2.Images) gc1.Images.Add(i);
            if ((gc1.LongDescription.Text == null && gc2.LongDescription.Text != null)
                || (gc1.ShortDescription.Text == null && gc2.ShortDescription.Text != null))
            {
                gc1.LongDescription.Text = gc2.LongDescription.Text;
                gc1.LongDescription.IsHtml = gc2.LongDescription.IsHtml;
                gc1.ShortDescription.Text = gc2.ShortDescription.Text;
                gc1.ShortDescription.IsHtml = gc2.ShortDescription.IsHtml;
            }
            if (gc1.MemberOnly == null && gc2.MemberOnly != null) gc1.MemberOnly = gc2.MemberOnly;
            if (gc1.Name == null && gc2.Name != null) gc1.Name = gc2.Name;
            if ((gc1.Owner.Id == null && gc2.Owner.Id != null) || (gc1.Owner.Name == null && gc2.Owner.Name != null))
            {
                gc1.Owner.Id = gc2.Owner.Id;
                gc1.Owner.Name = gc2.Owner.Name;
            }
            if (gc1.PersonalNote == null && gc2.PersonalNote != null) gc1.PersonalNote = gc2.PersonalNote;
            if (gc1.PlacedBy == null && gc2.PlacedBy != null) gc1.PlacedBy = gc2.PlacedBy;
            if (gc1.Terrain == null && gc2.Terrain != null) gc1.Terrain = gc2.Terrain;
            // not syncing trackables since it is very likely scenario that the trackables were removed from
            // the cache after the source data was loaded (since source can be rather old).

            Func<GeocacheLog, long> getId = o => o.Id.GetValueOrDefault((o.Text.Text ?? string.Empty).GetHashCode());
            var allLogs = gc1.Logs.Select(o => getId(o))
                               .Union(gc2.Logs.Select(o => getId(o)))
                               .Select(o => new
                               {
                                   Target = gc1.Logs.FirstOrDefault(l => getId(l) == o),
                                   Source = gc2.Logs.FirstOrDefault(l => getId(l) == o)
                               }).OrderBy(o => (o.Target == null) ? o.Source.Date : o.Target.Date);

            int targetIndex = 0;
            foreach (var logs in allLogs)
            {
                if (logs.Target == null)
                {
                    gc1.Logs.Insert(targetIndex, logs.Source);
                }
                else if (logs.Source != null)
                {
                    var l1 = logs.Target;
                    var l2 = logs.Source;
                    if (l1.Date == null && l2.Date != null) l1.Date = l2.Date;
                    if (l1.Finder.Id == null && l2.Finder.Id != null) l1.Finder.Id = l2.Finder.Id;
                    if (l1.Finder.Name == null && l2.Finder.Name != null) l1.Finder.Name = l2.Finder.Name;
                    if (l1.Images.Count == 0 && l2.Images.Count > 0) foreach (var i in l2.Images) l1.Images.Add(i);
                    if (l1.LogType.Id == null && l2.LogType.Id != null) l1.LogType.Id = l2.LogType.Id;
                    if (l1.LogType.Name == null && l2.LogType.Name != null) l1.LogType.Name = l2.LogType.Name;
                    if (l1.Text.Text == null && l2.Text.Text != null)
                    {
                        l1.Text.Text = l2.Text.Text;
                        l1.Text.IsEncoded = l2.Text.IsEncoded;
                    }
                    if (l1.Waypoint == null && l2.Waypoint != null)
                        l1.Waypoint = l2.Waypoint;
                }

                targetIndex++;
            }
        }

        #endregion
    }
}
