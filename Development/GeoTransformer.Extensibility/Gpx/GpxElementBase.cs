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
    /// Base class common for all data objects parsed from GPX file.
    /// </summary>
    public abstract class GpxElementBase : ObservableElement
    {
        /// <summary>
        /// Initializes the current instance from the given XML <paramref name="container"/>.
        /// </summary>
        /// <typeparam name="T">The type of the class that is derived from <see cref="GpxElementBase"/></typeparam>
        /// <param name="container">The XML element that contains the data.</param>
        /// <param name="attributeInitializers">The XML attribute to property mapping. Attributes that are not given are not persisted.</param>
        /// <param name="elementInitializers">The XML element to property mapping. Elements that are not given are not persisted.</param>
        protected virtual void Initialize<T>(XElement container, 
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

            foreach (var attr in container.Attributes())
            {
                if (attr.IsNamespaceDeclaration)
                    continue;

                Action<T, XAttribute> initializer;
                if (attributeInitializers.TryGetValue(attr.Name, out initializer))
                {
                    try { initializer(obj, attr); }
                    catch { }
                }
            }

            foreach (var elem in container.Elements())
            {
                Action<T, XElement> initializer;
                if (elementInitializers.TryGetValue(elem.Name, out initializer))
                {
                    try { initializer(obj, elem); }
                    catch { }
                }
            }
        }
    }
}
