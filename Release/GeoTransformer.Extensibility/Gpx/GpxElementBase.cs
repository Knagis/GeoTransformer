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
    public class GpxElementBase : ObservableElement
    {
        protected void Initialize<T>(XElement container, 
            IDictionary<XName, Action<T, XAttribute>> attributeInitializers, 
            IDictionary<XName, Action<T, XElement>> elementInitializers)
            where T : GpxElementBase
        {
            T obj = (T)this;

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
    }
}
