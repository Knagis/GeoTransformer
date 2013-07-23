/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Extensions.ExtensionManager
{
    /// <summary>
    /// Data structure that stores data about an extension available for download.
    /// </summary>
    public class ExtensionData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionData"/> class.
        /// </summary>
        /// <param name="xml">The XML data containing the information about the extension.</param>
        public ExtensionData(System.Xml.Linq.XElement xml)
        {
            var ns = new List<string>();
            foreach (var e in xml.Elements())
            {
                if (e.Name == "assembly")
                    this.AssemblyName = e.Value;
                else if (e.Name == "title")
                    this.Title = e.Value;
                else if (e.Name == "description")
                    this.Description = e.Value;
                else if (e.Name == "version")
                    this.Version = Version.Parse(e.Value);
                else if (e.Name == "href")
                    this.DownloadUri = new Uri(e.Value);
                else if (e.Name == "namespace")
                    ns.Add(e.Value);
            }

            this.ExtensionNamespaces = ns.AsReadOnly();

            var ass = ExtensionLoader.Extensions.Select(o => o.GetType().Assembly)
                .FirstOrDefault(o => string.Equals(this.AssemblyName, o.GetName().Name));

            if (ass != null)
            {
                this.IsInstalled = true;
                if (ass.GetName().Version.CompareTo(this.Version) < 0)
                    this.IsUpdated = false;
            }
        }

        /// <summary>
        /// Gets the name of the extension. This is the name of the subfolder in <c>Extensions</c> folder.
        /// </summary>
        public string AssemblyName { get; private set; }

        /// <summary>
        /// Gets the namespaces that the extension uses. These are used to determine which folders under <c>ExtensionData</c> belong to this extension.
        /// </summary>
        public System.Collections.ObjectModel.ReadOnlyCollection<string> ExtensionNamespaces { get; private set; }

        /// <summary>
        /// Gets the title of this extension.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the description of this extension.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the latest version number of this extension.
        /// </summary>
        public Version Version { get; private set; }

        /// <summary>
        /// Gets the URI where the extension can be downloaded.
        /// </summary>
        public Uri DownloadUri { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this extension is installed.
        /// </summary>
        public bool IsInstalled { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this extension is updated to the latest version.
        /// </summary>
        public bool IsUpdated { get; private set; }
    }
}
