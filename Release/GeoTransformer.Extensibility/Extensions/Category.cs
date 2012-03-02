/*
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
    /// Describes a category for the extensions. Categories are used to group extensions in user interface (for example, configuration).
    /// </summary>
    public class Category
    {
        #region [ Default categories ]

        private static Category _geocacheSources = new Category() { ConfigurationOrder = 1000, Title = "Geocache sources" };
        private static Category _miscellaneous = new Category() { ConfigurationOrder = 10000, Title = "Miscellaneous" };
        private static Category _viewers = new Category() { ConfigurationOrder = 2000, Title = "Geocache display" };
        private static Category _apiConfiguration = new Category() { ConfigurationOrder = 0, Title = "API configuration" };
        private static Category _transformers = new Category() { ConfigurationOrder = 2000, Title = "Data transformations" };

        /// <summary>
        /// Gets the category for geocache sources (transformers that load geocaches from files or network services).
        /// </summary>
        public static Category GeocacheSources { get { return _geocacheSources; } }

        /// <summary>
        /// Gets the category for miscellaneous extensions.
        /// </summary>
        public static Category Miscellaneous { get { return _miscellaneous; } }

        /// <summary>
        /// Gets the category for geocache data viewers (either list or individual cache).
        /// </summary>
        public static Category Viewers { get { return _viewers; } }

        /// <summary>
        /// Gets the category for global API configuration extensions.
        /// </summary>
        public static Category ApiConfiguration { get { return _apiConfiguration; } }

        /// <summary>
        /// Gets the category for geocache data transformers.
        /// </summary>
        public static Category Transformers { get { return _transformers; } }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Category"/> class.
        /// </summary>
        /// <param name="order">The configuration order of the category. The categories are sorted in ascending order.</param>
        /// <param name="title">The title (in English) for the category.</param>
        public Category(int order = 0, string title = "")
        {
            this.ConfigurationOrder = order;
            this.Title = title;
        }

        /// <summary>
        /// Gets or sets the configuration order of the category. The categories are sorted in ascending order.
        /// </summary>
        public int ConfigurationOrder { get; set; }

        /// <summary>
        /// Gets or sets the title (in English) for the category.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (this.Title ?? string.Empty).GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as Category;
            if (other == null)
                return false;

            return string.Equals(this.Title, other.Title, StringComparison.OrdinalIgnoreCase);
        }
    }
}
