/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoTransformer.Gpx
{
    /// <summary>
    /// Base class for all data objects parsed from GPX file that can contain custom extension elements.
    /// </summary>
    public abstract class GpxExtendableElement : GpxElementBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpxExtendableElement"/> class.
        /// </summary>
        /// <param name="suspendObservation">if set to <c>true</c> suspends the observation until <see cref="ResumeObservation"/> method is called. Should be set to <c>true</c> when the constructor loads the data.</param>
        protected GpxExtendableElement(bool suspendObservation = false)
            : base(suspendObservation)
        {
        }

        /// <summary>
        /// Initializes the current instance from the given XML <paramref name="container"/>.
        /// </summary>
        /// <typeparam name="T">The type of the class that is derived from <see cref="GpxElementBase"/></typeparam>
        /// <param name="container">The XML element that contains the data.</param>
        /// <param name="attributeInitializers">The XML attribute to property mapping. Attributes that are not given are stored in <see cref="ExtensionAttributes"/>.</param>
        /// <param name="elementInitializers">The XML element to property mapping. Elements that are not given are stored in <see cref="ExtensionElements"/>.</param>
        protected override void Initialize<T>(XElement container, 
            IDictionary<XName, Action<T, XAttribute>> attributeInitializers, 
            IDictionary<XName, Action<T, XElement>> elementInitializers)
        {
            T obj = (T)(object)this;

            if (container == null)
                return;

            if (attributeInitializers == null)
                attributeInitializers = new Dictionary<XName, Action<T, XAttribute>>(0);
            if (elementInitializers == null)
                elementInitializers = new Dictionary<XName, Action<T, XElement>>(0);

            var extensionAttributes = this.ExtensionAttributes;
            var extensionElements = this.ExtensionElements;

            foreach (var attr in container.Attributes())
            {
                if (attr.IsNamespaceDeclaration)
                    continue;

                Action<T, XAttribute> initializer;
                if (!attributeInitializers.TryGetValue(attr.Name, out initializer))
                {
                    extensionAttributes.Add(new XAttribute(attr));
                }
                else
                {
                    try { initializer(obj, attr); }
                    catch { }
                }
            }

            foreach (var elem in container.Elements())
            {
                Action<T, XElement> initializer;
                if (!elementInitializers.TryGetValue(elem.Name, out initializer))
                {
                    extensionElements.Add(new XElement(elem));
                }
                else
                {
                    try { initializer(obj, elem); }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Gets a collection of any extension attributes that are not defined by the schema of the element.
        /// </summary>
        public IList<XAttribute> ExtensionAttributes
        {
            get { return this.GetValue<ObservableCollection<XAttribute>>("ExtensionAttributes", true); }
        }

        /// <summary>
        /// Gets a collection of any extension attributes that are not defined by the schema of the element.
        /// </summary>
        public IList<XElement> ExtensionElements
        {
            get { return this.GetValue<ObservableCollection<XElement>>("ExtensionElements", true); }
        }

        /// <summary>
        /// Finds the extension attribute with the given <paramref name="name"/> and returns the value.
        /// </summary>
        /// <param name="name">The name of the extension attribute.</param>
        /// <returns>The value of the attribute or <c>null</c> if the attribute with the given <paramref name="name"/> does not exist.</returns>
        public string FindExtensionAttributeValue(XName name)
        {
            var attr = this.ExtensionAttributes.FirstOrDefault(o => o.Name == name);
            if (attr == null)
                return null;

            return attr.Value;
        }

        /// <summary>
        /// Finds the extension attribute with the given <paramref name="name"/> (assuming <see cref="XmlExtensions.GeoTransformerSchema"/>) and returns the value.
        /// </summary>
        /// <param name="localName">Name of the local.</param>
        /// <returns>The value of the attribute or <c>null</c> if the attribute with the given <paramref name="name"/> does not exist.</returns>
        public string FindExtensionAttributeValue(string localName)
        {
            return this.FindExtensionAttributeValue(XmlExtensions.GeoTransformerSchema + localName);
        }

        /// <summary>
        /// Finds the extension element with the given <paramref name="name"/>. If <paramref name="create"/> is set to <c>true</c>
        /// will create a new element if one does not exist.
        /// </summary>
        /// <param name="name">The fully qualified name of the extension element.</param>
        /// <param name="create"><c>True</c> to create a new element if one cannot be found.</param>
        /// <returns>The extension element or <c>null</c> if one cannot be found and <paramref name="create"/> is <c>false</c>.</returns>
        public XElement FindExtensionElement(XName name, bool create = false)
        {
            var elem = this.ExtensionElements.FirstOrDefault(o => o.Name == name);
            if (elem == null && create)
                this.ExtensionElements.Add(elem = new XElement(name));

            return elem;
        }

        /// <summary>
        /// Finds the extension element with the given <paramref name="name"/> assuming <see cref="XmlExtensions.GeoTransformerSchema"/>.
        /// If <paramref name="create"/> is set to <c>true</c> will create a new element if one does not exist.
        /// </summary>
        /// <param name="localName">The local name of the extension element.</param>
        /// <param name="create"><c>True</c> to create a new element if one cannot be found.</param>
        /// <returns>
        /// The extension element or <c>null</c> if one cannot be found and <paramref name="create"/> is <c>false</c>.
        /// </returns>
        public XElement FindExtensionElement(string localName, bool create = false)
        {
            return FindExtensionElement(XmlExtensions.GeoTransformerSchema + localName, create);
        }

        /// <summary>
        /// Finds the extension element for the given <paramref name="type"/> class (uses <see cref="Type.FullName"/> as the element name).
        /// If <paramref name="create"/> is set to <c>true</c> will create a new element if one does not exist.
        /// </summary>
        /// <param name="type">The extension for which to find the element.</param>
        /// <param name="create"><c>True</c> to create a new element if one cannot be found.</param>
        /// <returns>
        /// The extension element or <c>null</c> if one cannot be found and <paramref name="create"/> is <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">when <paramref name="type"/> is <c>null</c></exception>
        public XElement FindExtensionElement(Type type, bool create = false)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return FindExtensionElement(XmlExtensions.GeoTransformerSchema + type.FullName, create);
        }
    }
}
