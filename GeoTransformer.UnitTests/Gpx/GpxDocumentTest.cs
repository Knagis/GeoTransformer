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
<link href=""http://www.topografix.com/"">
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
            Assert.AreEqual(9, result.Root.Elements().Count());
            Assert.IsNotNull(result.Descendants(XmlExtensions.GpxSchema_1_0 + "urlname"));
        }

        /// <summary>
        /// Tests the parsing and serializing of Geocache extensions.
        /// </summary>
        [TestMethod]
        public void TestGeocache()
        {
            #region [ XML init ]
            var xdoc = XDocument.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?>
<gpx xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" version=""1.0"" creator=""Groundspeak, Inc. All Rights Reserved. http://www.groundspeak.com"" xsi:schemaLocation=""http://www.topografix.com/GPX/1/0 http://www.topografix.com/GPX/1/0/gpx.xsd http://www.groundspeak.com/cache/1/0/1 http://www.groundspeak.com/cache/1/0/1/cache.xsd"" xmlns=""http://www.topografix.com/GPX/1/0"">
  <name>Cache Listing Generated from Geocaching.com</name>
  <desc>This is an individual cache generated from Geocaching.com</desc>
  <author>Account ""Migelis"" From Geocaching.com</author>
  <email>contact@geocaching.com</email>
  <url>http://www.geocaching.com</url>
  <urlname>Geocaching - High Tech Treasure Hunting</urlname>
  <time>2012-02-24T10:01:56.1980413Z</time>
  <keywords>cache, geocache</keywords>
  <bounds minlat=""56.990978"" minlon=""23.522812"" maxlat=""56.992278"" maxlon=""23.525483"" />
  <wpt lat=""56.991433"" lon=""23.525483"">
    <time>2011-07-21T07:00:00Z</time>
    <name>GC30M8M</name>
    <desc>Kupskalnu dabas parks by Migelis, Traditional Cache (2/1.5)</desc>
    <url>http://www.geocaching.com/seek/cache_details.aspx?guid=3f3a373d-e166-4624-92a0-d414684630c1</url>
    <urlname>Kupskalnu dabas parks</urlname>
    <sym>Geocache Found</sym>
    <type>Geocache|Traditional Cache</type>
    <groundspeak:cache id=""2378931"" available=""False"" archived=""False"" xmlns:groundspeak=""http://www.groundspeak.com/cache/1/0/1"">
      <groundspeak:name>Kupskalnu dabas parks</groundspeak:name>
      <groundspeak:placed_by>Migelis</groundspeak:placed_by>
      <groundspeak:owner id=""4135725"">Migelis</groundspeak:owner>
      <groundspeak:type>Traditional Cache</groundspeak:type>
      <groundspeak:container>Micro</groundspeak:container>
      <groundspeak:attributes>
        <groundspeak:attribute id=""38"" inc=""0"">Campfires</groundspeak:attribute>
        <groundspeak:attribute id=""58"" inc=""1"">Fuel Nearby</groundspeak:attribute>
        <groundspeak:attribute id=""32"" inc=""1"">Bicycles</groundspeak:attribute>
        <groundspeak:attribute id=""13"" inc=""1"">Available at all times</groundspeak:attribute>
        <groundspeak:attribute id=""25"" inc=""1"">Parking available</groundspeak:attribute>
        <groundspeak:attribute id=""8"" inc=""1"">Scenic view</groundspeak:attribute>
        <groundspeak:attribute id=""7"" inc=""1"">Takes less than an hour</groundspeak:attribute>
        <groundspeak:attribute id=""6"" inc=""1"">Recommended for kids</groundspeak:attribute>
        <groundspeak:attribute id=""1"" inc=""1"">Dogs</groundspeak:attribute>
      </groundspeak:attributes>
      <groundspeak:difficulty>2</groundspeak:difficulty>
      <groundspeak:terrain>1.5</groundspeak:terrain>
      <groundspeak:country>Latvia</groundspeak:country>
      <groundspeak:state>
      </groundspeak:state>
      <groundspeak:short_description html=""True"">&lt;font face=""Times New Roman"" color=""#FF0000""&gt;LAT&lt;/font&gt; konteiners
ir 35mm fotofilmas karbina, kas satur viesugramatu un zimuli.&lt;br /&gt;
&lt;font face=""Times New Roman"" color=""#FF0000""&gt;ENG&lt;/font&gt; Container
is a 35.mm film canister, which contain logbook and pencil.</groundspeak:short_description>
      <groundspeak:long_description html=""True"">&lt;font face=""Times New Roman"" size=""+1"" color=""#FF0000""&gt;LAT&lt;/font&gt;
&lt;b&gt;Kupskalnu dabas parks&lt;/b&gt; izveidots tikai 2005. gada. Tas
atrodas uz Lapme&amp;#382;ciema un Bigaunciema robe&amp;#382;as. Parka ir
dabas taka gar Silinupi, kur kadreiz dzivoju&amp;scaron;i senie
zvejnieki. Izbuveta koka laipa un tiltin&amp;scaron;. Taka noved pie
juras, kur apskatams Lapme&amp;#382;ciema mols.&lt;br /&gt;
&lt;font face=""Times New Roman"" size=""+1"" color=""#FF0000""&gt;ENG&lt;/font&gt;
&lt;b&gt;Kupskalns National Park&lt;/b&gt; was founded in 2005. It is located
on the border of Lapme&amp;#382;ciems and Bigaunciems. There is a
nature path in the park along the river Silinupe, where some time
ago ancient fishermen were living. Wooden foot-bridges and a small
bridge were established. The path goes to the sea, where you can
look at Lapme&amp;#382;ciems breakwater
&lt;p&gt;&lt;font face=""Times New Roman"" color=""#FF0000""&gt;LAT&lt;/font&gt; Ta ka
nezinu cik kordinates ir precizas tad nu slepni iesaku meklet no
bri&amp;#382;a kad sakas laipa un skaitit pa&amp;scaron;us garakos delus,
cik? skatit hintu.&lt;br /&gt;
&lt;font face=""Times New Roman"" color=""#FF0000""&gt;ENG&lt;/font&gt; So I don't
know how correct is coordinates so I suggest you start counting the
longest planks at the moment when start foot- bridge. For correct
number look hint.&lt;/p&gt;&lt;p&gt;Additional Hidden Waypoints&lt;/p&gt;MC30M8M - Degviela  (Gas &amp; shop)&lt;br /&gt;N 56° 59.537 E 023° 31.369&lt;br /&gt;Šais kordinātēs atrodas degvielas uzpildes stacija un veiklas, tāpat arī plašs stāvlaukums.&lt;br /&gt;PK30M8M - Stāvlaukums&lt;br /&gt;N 56° 59.459 E 023° 31.420&lt;br /&gt;&lt;br /&gt;</groundspeak:long_description>
      <groundspeak:encoded_hints>[LAT]trisdesmit cetri
[ENG] thirty-four</groundspeak:encoded_hints>
      <groundspeak:logs>
        <groundspeak:log id=""209024215"">
          <groundspeak:date>2012-01-19T20:00:00Z</groundspeak:date>
          <groundspeak:type>Write note</groundspeak:type>
          <groundspeak:finder id=""4135725"">Migelis</groundspeak:finder>
          <groundspeak:text encoded=""False"">Cath will be back when snow  go away :) So please dont delete this</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""204646210"">
          <groundspeak:date>2011-12-27T20:00:00Z</groundspeak:date>
          <groundspeak:type>Post Reviewer Note</groundspeak:type>
          <groundspeak:finder id=""437962"">Pohatu Nuva</groundspeak:finder>
          <groundspeak:text encoded=""False"">I noticed that this cache has been temporarily disabled for a period of time well in excess of the period of ""a few weeks"" as contemplated by the cache guidelines published on Geocaching.com. While I feel that Geocaching.com should hold the location for you and block other caches from entering the area around this cache for a reasonable amount of time, we can't do so forever. Please either repair/replace this cache, or archive it (using the [i]archive listing[/i] link in the upper right) so that someone else can place a cache in the area, and geocachers can once again enjoy visiting this location.

If you plan on repairing this cache, please [b]log a note to the cache[/b] (not email) within the next 30 days so I don't archive the listing for non-communication.</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""197674786"">
          <groundspeak:date>2011-11-08T05:49:06Z</groundspeak:date>
          <groundspeak:type>Temporarily Disable Listing</groundspeak:type>
          <groundspeak:finder id=""4135725"">Migelis</groundspeak:finder>
          <groundspeak:text encoded=""False"">s;epnis nozudis, tuvakaja laika atjaunosu</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""197163525"">
          <groundspeak:date>2011-11-06T10:58:30Z</groundspeak:date>
          <groundspeak:type>Didn't find it</groundspeak:type>
          <groundspeak:finder id=""4911068"">Skrastins</groundspeak:finder>
          <groundspeak:text encoded=""False"">Ja vadaas peec deelju skaita, tad vinjsh tur nebija, varbuut vajadzeetu paarbaudiit!</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""197557152"">
          <groundspeak:date>2011-11-05T19:00:00Z</groundspeak:date>
          <groundspeak:type>Didn't find it</groundspeak:type>
          <groundspeak:finder id=""2458808"">jukis&amp;prusak</groundspeak:finder>
          <groundspeak:text encoded=""False"">Unfortunately no sign of a cache, but we did find some sort of a metal plate on the wooden boards. It looked as if the container used to be attached to this metal plate, at least we couldn’t think of any other reason for it being there.</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""197230615"">
          <groundspeak:date>2011-11-05T19:00:00Z</groundspeak:date>
          <groundspeak:type>Didn't find it</groundspeak:type>
          <groundspeak:finder id=""2712566"">niknais_kaamis</groundspeak:finder>
          <groundspeak:text encoded=""False"">No slepna palicis tikai stiprinajums... [:(]</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""195147285"">
          <groundspeak:date>2011-10-22T19:00:00Z</groundspeak:date>
          <groundspeak:type>Found it</groundspeak:type>
          <groundspeak:finder id=""3526438"">Sandiss</groundspeak:finder>
          <groundspeak:text encoded=""False"">Tika atrasts jau ar pedejo gaismu. Fotgrafešana bez stativa vairs pat nebija iespejama... TFTC!

Atrasts kopa ar Margaritello.</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""194339788"">
          <groundspeak:date>2011-10-22T19:00:00Z</groundspeak:date>
          <groundspeak:type>Found it</groundspeak:type>
          <groundspeak:finder id=""4729258"">Margaritello</groundspeak:finder>
          <groundspeak:text encoded=""False"">Patika piestiprinašanas veids. TFTC!
Kopa ar Sandiss.</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""193955960"">
          <groundspeak:date>2011-10-22T15:30:20Z</groundspeak:date>
          <groundspeak:type>Found it</groundspeak:type>
          <groundspeak:finder id=""4962896"">kirpis</groundspeak:finder>
          <groundspeak:text encoded=""False"">Atradam jau tumsa</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""193262053"">
          <groundspeak:date>2011-10-17T19:00:00Z</groundspeak:date>
          <groundspeak:type>Found it</groundspeak:type>
          <groundspeak:finder id=""4680580"">siimos</groundspeak:finder>
          <groundspeak:text encoded=""False"">TFTC.</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""192770036"">
          <groundspeak:date>2011-10-16T19:00:00Z</groundspeak:date>
          <groundspeak:type>Found it</groundspeak:type>
          <groundspeak:finder id=""5023377"">Leo&amp;Linda</groundspeak:finder>
          <groundspeak:text encoded=""False"">7 m nobiide! Bet hints bija preciizs!</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""192769955"">
          <groundspeak:date>2011-10-16T07:00:00Z</groundspeak:date>
          <groundspeak:type>Found it</groundspeak:type>
          <groundspeak:finder id=""2394154"">sprogis</groundspeak:finder>
          <groundspeak:text encoded=""False"">Hints ljoti precizs</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""192921407"">
          <groundspeak:date>2011-10-15T19:00:00Z</groundspeak:date>
          <groundspeak:type>Found it</groundspeak:type>
          <groundspeak:finder id=""4691983"">salaca09</groundspeak:finder>
          <groundspeak:text encoded=""False"">TFTC</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""192768250"">
          <groundspeak:date>2011-10-15T19:00:00Z</groundspeak:date>
          <groundspeak:type>Found it</groundspeak:type>
          <groundspeak:finder id=""2495063"">Karlittos</groundspeak:finder>
          <groundspeak:text encoded=""False"">Kad pielec hints, viss aiziet easy [;)] TFTC
Kopa ar Dzidzis</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""192752149"">
          <groundspeak:date>2011-10-15T19:00:00Z</groundspeak:date>
          <groundspeak:type>Found it</groundspeak:type>
          <groundspeak:finder id=""4884077"">dzidzis</groundspeak:finder>
          <groundspeak:text encoded=""False"">Kamer es vel petiju sava abola GPS, Karlittos jau atrada slepni. [:D]
TFTC!</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""191678103"">
          <groundspeak:date>2011-10-10T19:00:00Z</groundspeak:date>
          <groundspeak:type>Found it</groundspeak:type>
          <groundspeak:finder id=""3435155"">RGB.</groundspeak:finder>
          <groundspeak:text encoded=""False"">Romantiska vieta, kartigs slepnis.
Paldies!
_____

There is a romantic path, good cache.
TFTC!</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""189922610"">
          <groundspeak:date>2011-10-01T19:00:00Z</groundspeak:date>
          <groundspeak:type>Found it</groundspeak:type>
          <groundspeak:finder id=""4289231"">esprets</groundspeak:finder>
          <groundspeak:text encoded=""False"">Lidz jurai gan neaizgaju, bet vieta un slepnis patika.
TFTC</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""189792483"">
          <groundspeak:date>2011-10-01T19:00:00Z</groundspeak:date>
          <groundspeak:type>Found it</groundspeak:type>
          <groundspeak:finder id=""4456106"">roxiss</groundspeak:finder>
          <groundspeak:text encoded=""False"">Aizstaigajam ari lidz jurai, bet slepni pa fikso atradam.</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""189075058"">
          <groundspeak:date>2011-09-25T19:00:00Z</groundspeak:date>
          <groundspeak:type>Found it</groundspeak:type>
          <groundspeak:finder id=""1732496"">edupuika</groundspeak:finder>
          <groundspeak:text encoded=""False"">atradas atri un norades bij precizas. gandriz pie paša slepna sastapam tanti, kas uz celiem notupusies ložnaja pie laipas, likas jau ka slepnotaja, bet tad atklajas, ka damai palicis ""slikti"" un pec briža ieraudzijam rokas ari sliktas pašsajutas iemeslu - caurspidigas krasas stipro dzerienu! :)</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""188854830"">
          <groundspeak:date>2011-09-25T19:00:00Z</groundspeak:date>
          <groundspeak:type>Found it</groundspeak:type>
          <groundspeak:finder id=""2667484"">Pele_</groundspeak:finder>
          <groundspeak:text encoded=""False"">Bija viegli un aizstaigajam lidz jurai.. skaisti. Paldies!
Pele_ ( Elina &amp; Elviss )</groundspeak:text>
        </groundspeak:log>
        <groundspeak:log id=""185795376"">
          <groundspeak:date>2011-09-10T19:00:00Z</groundspeak:date>
          <groundspeak:type>Found it</groundspeak:type>
          <groundspeak:finder id=""3302173"">Knagis</groundspeak:finder>
          <groundspeak:text encoded=""False"">Man san&amp;#257;ca 35... :) PPS!</groundspeak:text>
        </groundspeak:log>
      </groundspeak:logs>
      <groundspeak:travelbugs />
    </groundspeak:cache>
  </wpt>
  <wpt lat=""56.992278"" lon=""23.522812"">
    <time>2011-07-22T01:07:35.527</time>
    <name>MC30M8M</name>
    <cmt>Šais kordinātēs atrodas degvielas uzpildes stacija un veiklas, tāpat arī plašs stāvlaukums.</cmt>
    <desc>Degviela  (Gas &amp; shop)</desc>
    <url>http://www.geocaching.com/seek/wpt.aspx?WID=81e69ad4-94a8-4af3-a70f-16de136204f6</url>
    <urlname>Degviela  (Gas &amp; shop)</urlname>
    <sym>Parking Area</sym>
    <type>Waypoint|Parking Area</type>
  </wpt>
  <wpt lat=""56.990978"" lon=""23.523668"">
    <time>2011-07-22T00:57:33.45</time>
    <name>PK30M8M</name>
    <cmt />
    <desc>Stāvlaukums</desc>
    <url>http://www.geocaching.com/seek/wpt.aspx?WID=d726c9ec-237e-4c10-b51b-72f3b3af17e8</url>
    <urlname>Stāvlaukums</urlname>
    <sym>Parking Area</sym>
    <type>Waypoint|Parking Area</type>
  </wpt>
</gpx>");
            #endregion

            var gdoc = new GeoTransformer.Gpx.GpxDocument(xdoc);

            Assert.AreEqual("Kupskalnu dabas parks", gdoc.Waypoints[0].Geocache.Name);
            Assert.AreEqual("Migelis", gdoc.Waypoints[0].Geocache.PlacedBy);
        }
    }
}
