﻿/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Gpx
{
    /// <summary>
    /// An observable collection that will remove items one by one instead of calling Reset (that does not include the 
    /// items removed in the notification event).
    /// </summary>
    /// <typeparam name="T">Type of objects in the collection</typeparam>
    public class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollection&lt;T&gt;"/> class.
        /// </summary>
        public ObservableCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="data">The data that is the initial contents of this collection.</param>
        public ObservableCollection(IEnumerable<T> data)
            : base(data)
        {
        }

        /// <summary>
        /// Clears the items.
        /// </summary>
        protected override void ClearItems()
        {
            while (this.Count > 0)
                this.RemoveAt(0);
        }
    }
}
