﻿/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Extensions
{
    /// <summary>
    /// The interface that defines services the GeoTransformer main application provides for the extensions.
    /// </summary>
    public interface IApplicationService
    {
        /// <summary>
        /// Retrieves the caches that are currently being displayed in the main form.
        /// </summary>
        /// <returns>The GPX documents that are currently displayed or <c>null</c> if the loading of caches is not yet completed.</returns>
        IEnumerable<System.Xml.Linq.XDocument> RetrieveDisplayedCaches();

        /// <summary>
        /// Refreshes the whole displayed cache list. Should be called when the list contents are changed (a cache is removed or added to it).
        /// Note that this method does not refresh the cache list from the sources.
        /// </summary>
        void RefreshCacheList();

        /// <summary>
        /// Instructs the cache list and individual view to select a cache with a particular code (the value of the GPX <c>name</c> element).
        /// If the code cannot be found, the selection will be cleared or a new blank placeholder will be created depending on the <paramref name="selectForEditing"/> parameter.
        /// </summary>
        /// <param name="code">The code of the cache that will be selected.</param>
        /// <param name="selectForEditing">If set to <c>true</c> then an empty placeholder cache will be created for the editor.</param>
        void SelectCache(string code, bool selectForEditing = false);
    }
}
