/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeoTransformer.Coordinates;
using System.Xml.Linq;

namespace GeoTransformer.UnitTests.Gpx
{
    [TestClass]
    public class GpxDocumentTest
    {
        [TestMethod]
        public void TestEmptyDocument()
        {
            var g = new GeoTransformer.Gpx.GpxDocument();

            Assert.IsNull(g.Metadata.Author.Link.Text);
            Assert.IsNull(g.Metadata.Author.Email);
            Assert.AreEqual(0, g.Metadata.Links.Count);
            Assert.AreEqual(0, g.Waypoints.Count);

            g.Metadata.Name = "test";
            Assert.AreEqual("test", g.Metadata.Name);
        }

        /// <summary>
        /// Tests that the metada is parsed and serialized correctly.
        /// </summary>
        [TestMethod]
        public void TestMetadataGpx_1_1()
        {
            var xdoc = XDocument.Parse(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<gpx xmlns=""http://www.topografix.com/GPX/1/1"" version=""1.1"" creator=""ExpertGPS 1.9 a0"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd http://www.topografix.com/GPX/Private/TopoGrafix/0/3 http://www.topografix.com/GPX/Private/TopoGrafix/0/3/topografix.xsd"">
<metadata>
<name>Metadata Test</name>
<desc>This is a demonstration of GPX metadata</desc>
<author>
<name>Dan Foster</name>
<email id=""gpx2004"" domain=""topografix.com""/>
<link href=""http://www.topografix.com"">
<text>TopoGrafix</text>
</link>
</author>
<copyright author=""Dan Foster"">
<year>2004</year>
<license>http://creativecommons.org/licenses/by/2.0/</license>
</copyright>
<link href=""http://www.topografix.com/gpx.asp"">
<text>GPX site</text>
<type>text/html</type>
</link>
<time>2004-06-10T12:03:26Z</time>
<keywords>gpx, style, metadata</keywords>
<bounds minlat=""40.451925327"" minlon=""-74.266517"" maxlat=""41.451925327"" maxlon=""-71.266517""/>
<extensions>
<active_point xmlns=""http://www.topografix.com/GPX/Private/TopoGrafix/0/3"" lat=""40.4606481824082"" lon=""-74.2819017731053""/>
</extensions>
</metadata>
<wpt lat=""40.451925327"" lon=""-74.266517"">
<name>WAYPOINT</name>
<cmt>WAYPOINT</cmt>
<desc>Waypoint</desc>
<sym>Tall Tower</sym>
</wpt>
<extensions>
</extensions>
</gpx>");
            var gpxDoc = new GeoTransformer.Gpx.GpxDocument(xdoc);

            Assert.AreEqual("ExpertGPS 1.9 a0", gpxDoc.Metadata.Creator);
            Assert.AreEqual("Metadata Test", gpxDoc.Metadata.Name);
            Assert.AreEqual("This is a demonstration of GPX metadata", gpxDoc.Metadata.Description);
            Assert.AreEqual("Dan Foster", gpxDoc.Metadata.Author.Name);
            Assert.AreEqual("gpx2004@topografix.com", gpxDoc.Metadata.Author.Email);
            Assert.AreEqual("http://www.topografix.com/", gpxDoc.Metadata.Author.Link.Href.ToString());
            Assert.AreEqual("TopoGrafix", gpxDoc.Metadata.Author.Link.Text);
            Assert.AreEqual("Dan Foster", gpxDoc.Metadata.Copyright.Author);
            Assert.AreEqual(2004, gpxDoc.Metadata.Copyright.Year);
            Assert.AreEqual("http://creativecommons.org/licenses/by/2.0/", gpxDoc.Metadata.Copyright.License.ToString());
            Assert.AreEqual(1, gpxDoc.Metadata.Links.Count);
            Assert.AreEqual("http://www.topografix.com/gpx.asp", gpxDoc.Metadata.Links[0].Href.ToString());
            Assert.AreEqual("text/html", gpxDoc.Metadata.Links[0].MimeType);
            Assert.AreEqual(new DateTime(2004, 6, 10, 12, 3, 26, DateTimeKind.Utc).ToLocalTime(), gpxDoc.Metadata.CreationTime);
            Assert.AreEqual(40.451925327M, gpxDoc.Metadata.Bounds.MinLatitude);
            Assert.AreEqual(-74.266517M, gpxDoc.Metadata.Bounds.MinLongitude);
            Assert.AreEqual(-71.266517M, gpxDoc.Metadata.Bounds.MaxLongitude);
            Assert.AreEqual(41.451925327M, gpxDoc.Metadata.Bounds.MaxLatitude);
            Assert.AreEqual(1, gpxDoc.Metadata.ExtensionElements.Count);


            var result = gpxDoc.Serialize();

            var metadata = result.Root.Element(XmlExtensions.GpxSchema_1_1 + "metadata");
            Assert.IsNotNull(metadata);
            Assert.AreEqual("Metadata Test", metadata.Element(XmlExtensions.GpxSchema_1_1 + "name").GetValue());

            Assert.AreEqual(1, metadata.Elements(XmlExtensions.GpxSchema_1_1 + "extensions").Elements().Count());
        }

        /// <summary>
        /// Tests that metadata from GPX 1.0 document is parsed and serialized correctly;
        /// </summary>
        [TestMethod]
        public void TestMetadataGpx_1_0()
        {
            var xdoc = XDocument.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?>
<gpx xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" version=""1.0"" creator=""Groundspeak, Inc. All Rights Reserved. http://www.groundspeak.com"" xsi:schemaLocation=""http://www.topografix.com/GPX/1/0 http://www.topografix.com/GPX/1/0/gpx.xsd http://www.groundspeak.com/cache/1/0/1 http://www.groundspeak.com/cache/1/0/1/cache.xsd"" xmlns=""http://www.topografix.com/GPX/1/0"">
  <name>Cache Listing Generated from Geocaching.com</name>
  <desc>This is an individual cache generated from Geocaching.com</desc>
  <author>Account ""Migelis"" From Geocaching.com</author>
  <email>contact@geocaching.com</email>
  <url>http://www.geocaching.com</url>
  <urlname>Geocaching - High Tech Treasure Hunting</urlname>
  <time>2011-07-25T12:41:18.027Z</time>
  <keywords>cache, geocache</keywords>
  <bounds minlat=""56.990978"" minlon=""23.522812"" maxlat=""56.992278"" maxlon=""23.525483"" />
</gpx>");
            var gdoc = new GeoTransformer.Gpx.GpxDocument(xdoc);

            Assert.AreEqual("Groundspeak, Inc. All Rights Reserved. http://www.groundspeak.com", gdoc.Metadata.Creator);
            Assert.AreEqual("Cache Listing Generated from Geocaching.com", gdoc.Metadata.Name);
            Assert.AreEqual("This is an individual cache generated from Geocaching.com", gdoc.Metadata.Description);
            Assert.AreEqual(@"Account ""Migelis"" From Geocaching.com", gdoc.Metadata.Author.Name);
            Assert.AreEqual("contact@geocaching.com", gdoc.Metadata.Author.Email);
            Assert.AreEqual(1, gdoc.Metadata.Links.Count);
            Assert.AreEqual("http://www.geocaching.com/", gdoc.Metadata.Links[0].Href.ToString());
            Assert.AreEqual("Geocaching - High Tech Treasure Hunting", gdoc.Metadata.Links[0].Text);
            Assert.AreEqual(new DateTime(2011, 7, 25, 12, 41, 18, 27, DateTimeKind.Utc).ToLocalTime(), gdoc.Metadata.CreationTime.Value);
            Assert.AreEqual("cache, geocache", gdoc.Metadata.Keywords);
            Assert.AreEqual(56.990978M, gdoc.Metadata.Bounds.MinLatitude);
            Assert.AreEqual(23.522812M, gdoc.Metadata.Bounds.MinLongitude);
            Assert.AreEqual(56.992278M, gdoc.Metadata.Bounds.MaxLatitude);
            Assert.AreEqual(23.525483M, gdoc.Metadata.Bounds.MaxLongitude);

            var result = gdoc.Serialize(new GeoTransformer.Gpx.GpxSerializationOptions() { GpxVersion = GeoTransformer.Gpx.GpxVersion.Gpx_1_0 });
        }
    }
}
