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
        /// Returns the XML schema URI for the Groundspeak GPX extensions, version 1.0
        /// </summary>
        public static readonly XNamespace GeocacheSchema_1_0_0 = XNamespace.Get("http://www.groundspeak.com/cache/1/0");

        /// <summary>
        /// Returns the XML schema URI for the Groundspeak GPX extensions, version 1.0.1
        /// </summary>
        public static readonly XNamespace GeocacheSchema_1_0_1 = XNamespace.Get("http://www.groundspeak.com/cache/1/0/1");

        /// <summary>
        /// Returns the XML schema URI for the Groundspeak GPX extensions, version 1.0.2
        /// </summary>
        public static readonly XNamespace GeocacheSchema_1_0_2 = XNamespace.Get("http://www.groundspeak.com/cache/1/0/2");

        /// <summary>
        /// Returns the XML schema URI for GeoTransformer's XML extensions.
        /// </summary>
        public static readonly XNamespace GeoTransformerSchema = XNamespace.Get("http://www.geotransformer.com/xml/");

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
        /// Creates the name for the extension element for a given <paramref name="extensionType"/>.
        /// </summary>
        /// <param name="extensionType">The type of the extension for which to create the name.</param>
        /// <returns>The <see cref="XName"/> of the element.</returns>
        public static XName CreateExtensionName(Type extensionType)
        {
            if (extensionType == null)
                throw new ArgumentNullException("extensionType");

            return GeoTransformerSchema + extensionType.FullName;
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
