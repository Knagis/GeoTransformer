/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoTransformer.Viewers.TableView
{
    /// <summary>
    /// Represents the cache data as parsed from the GPX file.
    /// </summary>
    internal class ParsedCacheData
    {
        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsedCacheData"/> class.
        /// </summary>
        /// <param name="sourceElement">The source element (the waypoint node).</param>
        /// <param name="mode">The mode how to fill the instance.</param>
        public ParsedCacheData(XElement sourceElement)
        {
            if (sourceElement == null)
                throw new ArgumentNullException("sourceElement");

            this.SourceElement = sourceElement;

            this.Initialize();
        }

        /// <summary>
        /// Initializes this instance from the XML data.
        /// </summary>
        private void Initialize()
        {
            decimal d;

            if (string.Equals(this.SourceElement.GetAttributeValue(XmlExtensions.GeoTransformerSchema + "EditorOnly"), bool.TrueString, StringComparison.OrdinalIgnoreCase))
                this.IsEditorOnly = true;

            foreach (var e in this.SourceElement.Elements())
            {
                var ns = e.Name.Namespace;
                if (ns == XmlExtensions.GpxSchema_1_0 || ns == XmlExtensions.GpxSchema_1_1)
                {
                    switch (e.Name.LocalName)
                    {
                        case "name":
                            this.CacheCode = e.Value;
                            break;
                        case "desc":
                            this.Description = e.Value;
                            break;
                    }
                }
                else if (ns == XmlExtensions.GeocacheSchema_1_0_0 || ns == XmlExtensions.GeocacheSchema_1_0_1 || ns == XmlExtensions.GeocacheSchema_1_0_2)
                {
                    if (string.Equals(e.Name.LocalName, "cache"))
                    {
                        this.IsGeocache = true;
                        if (string.Equals(e.GetAttributeValue("archived"), bool.TrueString, StringComparison.OrdinalIgnoreCase))
                            this.Status = "Archived";
                        else
                        {
                            var x = e.GetAttributeValue("available");
                            if (string.Equals(x, bool.FalseString, StringComparison.OrdinalIgnoreCase))
                                this.Status = "Disabled";
                            else if (string.Equals(x, bool.TrueString, StringComparison.OrdinalIgnoreCase))
                                this.Status = "Active";
                        }

                        foreach (var ce in e.Elements())
                        {
                            switch (ce.Name.LocalName)
                            {
                                case "type":
                                    this.CacheType = ce.Value;
                                    break;
                                case "name":
                                    this.Title = ce.Value;
                                    break;
                                case "container":
                                    this.ContainerSize = ce.Value;
                                    break;
                                case "country":
                                    this.Country = ce.Value;
                                    break;
                                case "state":
                                    this.State = (ce.Value ?? string.Empty).Trim();
                                    break;
                                case "placed_by":
                                    this.PlacedBy = ce.Value;
                                    break;
                                case "owner":
                                    this.Owner = ce.Value;
                                    break;
                                case "difficulty":
                                    if (decimal.TryParse(ce.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d))
                                        this.Difficulty = d;
                                    break;
                                case "terrain":
                                    if (decimal.TryParse(ce.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d))
                                        this.Terrain = d;
                                    break;
                                case "attributes":
                                    if (string.Equals(this.Status, "Active", StringComparison.OrdinalIgnoreCase))
                                        if (ce.Elements().Any(o => o.GetAttributeValue("id") == "42"))
                                            this.Status = "Needs maintenance";
                                    break;
                            }
                        }
                    }
                }
                else if (ns == XmlExtensions.GeoTransformerSchema
                            && !string.Equals(e.Name.LocalName, "CachedCopy", StringComparison.Ordinal) 
                            && e.ContainsSignificantInformation())
                {
                    this.ContainsExtensionElements = true;
                }
            }
        }

        #endregion

        #region [ InitializeColumns ]

        public static void InitializeListView(BrightIdeasSoftware.ObjectListView olv)
        {
            var imgList = new System.Windows.Forms.ImageList();
            imgList.ImageSize = new System.Drawing.Size(16, 16);
            foreach (var x in typeof(UI.CacheTypeIcons).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
            {
                if (typeof(System.Drawing.Image).IsAssignableFrom(x.PropertyType))
                    imgList.Images.Add(x.Name, (System.Drawing.Image)x.GetValue(null, null));
            }
            olv.SmallImageList = imgList;

            olv.FullRowSelect = true;
            olv.UseFiltering = true;
            olv.UseFilterIndicator = true;
            olv.ShowGroups = true;
            olv.ShowImagesOnSubItems = true;
            olv.ShowCommandMenuOnRightClick = true;
            olv.SortGroupItemsByPrimaryColumn = false;
            olv.MultiSelect = false;
            olv.AllowColumnReorder = true;
            olv.HeaderUsesThemes = false;
            olv.HideSelection = false;
            olv.BorderStyle = System.Windows.Forms.BorderStyle.None;


            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn("Cache type", "CacheType") { ImageAspectName = "CacheTypeImageKey", MinimumWidth = 22, MaximumWidth = 22 });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn("Cache code", "CacheCode") { UseFiltering = false, UseInitialLetterForGroup = true });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn("Title", "Title") { UseInitialLetterForGroup = true });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn("Status", "Status") { });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn("Container size", "ContainerSize") { });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn("Country", "Country") { });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn("State", "State") { });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn("PlacedBy", "PlacedBy") { });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn("Owner", "Owner") { });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn("Difficulty", "Difficulty") { });
            olv.AllColumns.Add(new BrightIdeasSoftware.OLVColumn("Terrain", "Terrain") { });

            olv.Columns.AddRange(olv.AllColumns.ToArray());
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the source element from which this instance was created.
        /// </summary>
        public XElement SourceElement { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this waypoint is loaded from the local data and should be only displayed in editors.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the waypoint is intended only for editors; otherwise, <c>false</c>.
        /// </value>
        public bool IsEditorOnly { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the waypoiny contains GeoTransformer extension elements - this would indicate that the cache data has been edited by the user.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the waypoint contains extension elements; otherwise, <c>false</c>.
        /// </value>
        public bool ContainsExtensionElements { get; private set; }

        /// <summary>
        /// Gets the cache type image.
        /// </summary>
        public string CacheTypeImageKey
        { 
            get 
            {
                if (string.IsNullOrEmpty(this.CacheType))
                    return null;

                return this.CacheType.Replace(" ", "").Replace("-", "");
            } 
        }

        /// <summary>
        /// Gets the type of the geocache. Returns <c>null</c> if the waypoint does not represent a geocache.
        /// </summary>
        public string CacheType { get; private set; }

        /// <summary>
        /// Gets the geocache code.
        /// </summary>
        public string CacheCode { get; private set; }

        /// <summary>
        /// Gets the status of the geocache.
        /// </summary>
        public string Status { get; private set; }

        /// <summary>
        /// Gets the description of the waypoint (for geocaches contains the title, difficulties, owner and location).
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the title of the cache.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the container size of the cache.
        /// </summary>
        public string ContainerSize { get; private set; }

        /// <summary>
        /// Gets the country where the cache is located.
        /// </summary>
        public string Country { get; private set; }

        /// <summary>
        /// Gets the state where the cache is located.
        /// </summary>
        public string State { get; private set; }

        /// <summary>
        /// Gets the persons who placed the cache.
        /// </summary>
        public string PlacedBy { get; private set; }

        /// <summary>
        /// Gets the owner of the cache.
        /// </summary>
        public string Owner { get; private set; }

        /// <summary>
        /// Gets the difficulty rating of the cache.
        /// </summary>
        public decimal? Difficulty { get; private set; }

        /// <summary>
        /// Gets the terrain rating of the cache.
        /// </summary>
        public decimal? Terrain { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the XML waypoint represents a geocache (contains the groundspeak extensions).
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is geocache; otherwise, <c>false</c>.
        /// </value>
        public bool IsGeocache { get; private set; }

        #endregion
    }
}
