/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoTransformer
{
    /// <summary>
    /// Static class containing extension methods to work with XML documents.
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// Returns the XML schema URI for the Topografix GPX 1.0.
        /// </summary>
        public static readonly XNamespace GpxSchema_1_0 = XNamespace.Get("http://www.topografix.com/GPX/1/0");

        /// <summary>
        /// Returns the XML schema URI for the Topografix GPX 1.1.
        /// </summary>
        public static readonly XNamespace GpxSchema_1_1 = XNamespace.Get("http://www.topografix.com/GPX/1/1");

        /// <summary>
        /// Returns the XML schema URI for the Groundspeak GPX 1.0. Includes the { } brackets used by Xml.Linq.
        /// </summary>
        public const string GeocacheSchema10 = "{http://www.groundspeak.com/cache/1/0}";

        /// <summary>
        /// Returns the XML schema URI for the Groundspeak GPX 1.0.1. Includes the { } brackets used by Xml.Linq.
        /// </summary>
        public const string GeocacheSchema101 = "{http://www.groundspeak.com/cache/1/0/1}";

        /// <summary>
        /// Returns the XML schema URI for GeoTransformer's XML extensions. Includes the { } brackets used by Xml.Linq.
        /// </summary>
        public const string GeoTransformerSchema = "{http://www.geotransformer.com/xml/}";

        /// <summary>
        /// Returns the XML schema URI for the Groundspeak GPX 1.0. Does NOT include the { } brackets used by Xml.Linq.
        /// </summary>
        public const string GeocacheSchema10Clean = "http://www.groundspeak.com/cache/1/0";

        /// <summary>
        /// Returns the XML schema URI for the Groundspeak GPX 1.0.1. Does NOT include the { } brackets used by Xml.Linq.
        /// </summary>
        public const string GeocacheSchema101Clean = "http://www.groundspeak.com/cache/1/0/1";

        /// <summary>
        /// Returns the XML schema URI for GeoTransformer's XML extensions. Does NOT include the { } brackets used by Xml.Linq.
        /// </summary>
        public const string GeoTransformerSchemaClean = "http://www.geotransformer.com/xml/";

        /// <summary>
        /// Retrieves the value of the element. If the element is not set (is <c>null</c>), does not fail but return <c>null</c>.
        /// </summary>
        /// <param name="node">The element from which the value should be read.</param>
        /// <returns>The value of the <paramref name="node"/> or <c>null</c> if the parameter is not set.</returns>
        public static string GetValue(this XElement node)
        {
            if (node == null)
                return null;

            return node.Value;
        }

        /// <summary>
        /// Retrieves the value of the attribute. If the attribute is not set (is <c>null</c>), does not fail but return <c>null</c>.
        /// </summary>
        /// <param name="node">The attribute from which the value should be read.</param>
        /// <returns>The value of the <paramref name="node"/> or <c>null</c> if the parameter is not set.</returns>
        public static string GetValue(this XAttribute node)
        {
            if (node == null)
                return null;

            return node.Value;
        }

        /// <summary>
        /// Retrieves the value of the attribute with the given <paramref name="attributeName"/> on the <paramref name="element"/>. If the element
        /// does not have such attribute, <c>null</c> is returned. If <paramref name="element"/> is not set, returns <c>null</c>.
        /// </summary>
        /// <param name="element">The element on which to look for the attribute.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>The value of the attribute or <c>null</c> if the <paramref name="element"/> is <c>null</c> or if the element does not contain such attribute.</returns>
        public static string GetAttributeValue(this XElement element, XName attributeName)
        {
            if (element == null)
                return null;

            var a = element.Attribute(attributeName);
            if (a == null)
                return null;

            return a.Value;
        }

        /// <summary>
        /// Retrieves the XML element by looking for the Groundspeak cache GPX schemas and the <paramref name="localName"/>. Tries both schemas and returns <c>null</c>
        /// if the element cannot be found.
        /// </summary>
        /// <param name="container">The container object.</param>
        /// <param name="localName">The unqualified name of the element.</param>
        /// <returns>The <see cref="XElement"/> or <c>null</c> if it cannot be found.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="container"/> is <c>null</c></exception>
        public static XElement CacheElement(this XContainer container, string localName)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            return container.Element(XmlExtensions.GeocacheSchema101 + localName) ?? container.Element(XmlExtensions.GeocacheSchema10 + localName);
        }

        /// <summary>
        /// Retrieves the XML elements that are direct descendants of the <paramref name="container"/> by looking for both Groundspeak cache GPX schemas and the <paramref name="localName"/>.
        /// </summary>
        /// <param name="container">The container object.</param>
        /// <param name="localName">The unqualified name of the element.</param>
        /// <returns>The enumeration of all matching elements. Document order is not guaranteed.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="container"/> is <c>null</c></exception>
        public static IEnumerable<XElement> CacheElements(this XContainer container, string localName)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            return container.Elements(XmlExtensions.GeocacheSchema101 + localName).Union(container.Elements(XmlExtensions.GeocacheSchema10 + localName));
        }

        /// <summary>
        /// Retrieves the XML elements that are descendants of the <paramref name="container"/> by looking for both Groundspeak cache GPX schemas and the <paramref name="localName"/>.
        /// </summary>
        /// <param name="container">The container object.</param>
        /// <param name="localName">The unqualified name of the element.</param>
        /// <returns>The enumeration of all matching elements. Document order is not guaranteed.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="container"/> is <c>null</c></exception>
        public static IEnumerable<XElement> CacheDescendants(this XContainer container, string localName)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            return container.Descendants(XmlExtensions.GeocacheSchema101 + localName).Union(container.Descendants(XmlExtensions.GeocacheSchema10 + localName));
        }

        /// <summary>
        /// Retrieves the XML element by looking for the Topografix GPX schema and the <paramref name="localName"/>. Returns <c>null</c>
        /// if the element cannot be found.
        /// </summary>
        /// <param name="container">The container object.</param>
        /// <param name="localName">The unqualified name of the element.</param>
        /// <returns>The <see cref="XElement"/> or <c>null</c> if it cannot be found.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="container"/> is <c>null</c></exception>
        public static XElement WaypointElement(this XContainer container, string localName)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            return container.Element(XmlExtensions.GpxSchema_1_0 + localName);
        }

        /// <summary>
        /// Retrieves the XML elements that are direct descendants of the <paramref name="container"/> by looking for Topografix GPX schema and the <paramref name="localName"/>.
        /// </summary>
        /// <param name="container">The container object.</param>
        /// <param name="localName">The unqualified name of the element.</param>
        /// <returns>The enumeration of all matching elements. Document order is not guaranteed.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="container"/> is <c>null</c></exception>
        public static IEnumerable<XElement> WaypointElements(this XContainer container, string localName)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            return container.Elements(XmlExtensions.GpxSchema_1_0 + localName);
        }

        /// <summary>
        /// Retrieves the XML elements that are descendants of the <paramref name="container"/> by looking for Topografix GPX schema and the <paramref name="localName"/>.
        /// </summary>
        /// <param name="container">The container object.</param>
        /// <param name="localName">The unqualified name of the element.</param>
        /// <returns>The enumeration of all matching elements. Document order is not guaranteed.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="container"/> is <c>null</c></exception>
        public static IEnumerable<XElement> WaypointDescendants(this XContainer container, string localName)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            return container.Descendants(XmlExtensions.GpxSchema_1_0 + localName);
        }

        /// <summary>
        /// Returns the XML element for the given <paramref name="extensionType"/> within the current element.
        /// </summary>
        /// <param name="container">The element within which the extension is stored. Must be the <c>wpt</c> element to retrieve a cache specific extension element.</param>
        /// <param name="extensionType">Type of the extension for which the element is retrieved for.</param>
        /// <returns>
        /// The extension element (returns <c>null</c> if it does not exist).
        /// </returns>
        /// <exception cref="ArgumentNullException">when <paramref name="container"/> or <paramref name="extensionType"/> are <c>null</c></exception>
        /// <exception cref="ArgumentException">when <paramref name="container"/> is not the <c>wpt</c> element</exception>
        public static XElement ExtensionElement(this XElement container, Type extensionType)
        {
            return ExtensionElement(container, extensionType.FullName, false);
        }

        /// <summary>
        /// Returns the XML element for the given <paramref name="extensionType"/> within the current element.
        /// </summary>
        /// <param name="container">The element within which the extension is stored. Must be the <c>wpt</c> element to retrieve a cache specific extension element.</param>
        /// <param name="extensionType">Type of the extension for which the element is retrieved for.</param>
        /// <param name="createIfNeeded">Determines if a new element is created if one does not already exist.</param>
        /// <returns>
        /// The extension element (creates a new one if it does not yet exist or returns <c>null</c> depending on the <paramref name="createIfNeeded"/> parameter).
        /// </returns>
        /// <exception cref="ArgumentNullException">when <paramref name="container"/> or <paramref name="extensionType"/> are <c>null</c></exception>
        /// <exception cref="ArgumentException">when <paramref name="container"/> is not the <c>wpt</c> element</exception>
        public static XElement ExtensionElement(this XElement container, Type extensionType, bool createIfNeeded)
        {
            return ExtensionElement(container, extensionType.FullName, createIfNeeded);
        }

        /// <summary>
        /// Returns the XML element for the given <paramref name="extensionType"/> within the current element.
        /// </summary>
        /// <param name="container">The element within which the extension is stored. Must be the <c>wpt</c> element to retrieve a cache specific extension element.</param>
        /// <param name="extensionType">The <see cref="Type.FullName"/> of the type of the extension for which the element is retrieved for.</param>
        /// <param name="createIfNeeded">Determines if a new element is created if one does not already exist.</param>
        /// <returns>
        /// The extension element (creates a new one if it does not yet exist or returns <c>null</c> depending on the <paramref name="createIfNeeded"/> parameter).
        /// </returns>
        /// <exception cref="ArgumentNullException">when <paramref name="container"/> or <paramref name="extensionType"/> are <c>null</c></exception>
        /// <exception cref="ArgumentException">when <paramref name="container"/> is not the <c>wpt</c> element</exception>
        internal static XElement ExtensionElement(this XElement container, string extensionType, bool createIfNeeded)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            if (extensionType == null)
                throw new ArgumentNullException("extensionType");

            if (container.Name != GpxSchema_1_0 + "wpt")
                throw new ArgumentException("The container must be the " + GpxSchema_1_0 + "wpt element.", "container");

            var name = GeoTransformerSchema + extensionType;

            var el = container.Elements(name).LastOrDefault();
            if (el == null && createIfNeeded)
                container.Add(el = new XElement(name));

            return el;
        }

        /// <summary>
        /// Retrieves all extension elements within the <paramref name="container"/> for the specified <paramref name="extensionType"/>.
        /// </summary>
        /// <param name="container">The container object.</param>
        /// <param name="extensionType">Type of the extension.</param>
        /// <returns>The list of the elements. The elements are returned in no particular order.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="container"/> or <paramref name="extensionType"/> are <c>null</c></exception>
        public static IEnumerable<XElement> ExtensionElements(this XContainer container, Type extensionType)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            if (extensionType == null)
                throw new ArgumentNullException("extensionType");

            return container.Descendants(GeoTransformerSchema + extensionType.FullName);
        }

        /// <summary>
        /// Gets the cached copy of the waypoint element.
        /// </summary>
        /// <param name="waypoint">The waypoint element.</param>
        /// <returns>The cached copy as stored in the element. If it does not exist, a cleaned up copy is created.</returns>
        /// <exception cref="ArgumentException">when <paramref name="waypoint"/> is not <c>gpx:wpt</c> element</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="waypoint"/> is <c>null</c></exception>
        public static XElement GetCachedCopy(this XElement waypoint)
        {
            if (waypoint == null)
                throw new ArgumentNullException("waypoint");

            if (waypoint.Name != GpxSchema_1_0 + "wpt")
                throw new ArgumentException("The waypoint must be a gpx:wpt element.", "waypoint");

            var copy = waypoint.ExtensionElement("CachedCopy", false);
            XElement wpt = null;

            if (copy != null)
            {
                wpt = copy.WaypointElement("wpt");
                if (wpt == null)
                {
                    // clean out incorrect cached copies.
                    copy.Remove();
                    copy = null;
                }
            }

            if (copy == null)
            {
                copy = XElement.Parse(waypoint.ToString(SaveOptions.DisableFormatting), LoadOptions.PreserveWhitespace);
                // ToList() is required as otherwise the .Remove() is modifying the current iterator.
                copy.Elements().Where(o => string.Equals(o.Name.NamespaceName, XmlExtensions.GeoTransformerSchemaClean, StringComparison.Ordinal)).ToList().Remove();
                copy.Attributes().Where(o => string.Equals(o.Name.NamespaceName, XmlExtensions.GeoTransformerSchemaClean, StringComparison.Ordinal)).ToList().Remove();
                
                wpt = copy;
                copy = new XElement(GeoTransformerSchema + "CachedCopy", copy);
                waypoint.Add(copy);
            }

            return wpt;
        }

        /// <summary>
        /// Checks if the given element contains any significant information (empty elements and attributes are not counted as well as namespace declaration attributes).
        /// </summary>
        /// <param name="element">The element that has to be checked.</param>
        /// <returns><c>False</c> if the element does not contain significant information; otherwise <c>True</c>.</returns>
        public static bool ContainsSignificantInformation(this XElement element)
        {
            if (element == null)
                return false;

            if (!string.IsNullOrWhiteSpace(element.Value))
                return true;

            foreach (var a in element.Attributes())
                if (!a.IsNamespaceDeclaration && !string.IsNullOrWhiteSpace(a.Value))
                    return true;

            foreach (var e in element.Elements())
                if (ContainsSignificantInformation(e))
                    return true;

            return false;
        }
    }
}
