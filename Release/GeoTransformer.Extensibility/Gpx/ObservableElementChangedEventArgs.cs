/*
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
    /// Event arguments for <see cref="ObservableElement.PropertyChanged"/> event that allows walking up the property change chain.
    /// </summary>
    public class ObservableElementChangedEventArgs : System.ComponentModel.PropertyChangedEventArgs
    {
        private ObservableElementChangedEventArgs _innerChange;
        private object _target;
        private System.Xml.Linq.XObjectChange? _xObjectChange;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableElementChangedEventArgs"/> class.
        /// </summary>
        /// <param name="target">The instance that was changed.</param>
        /// <param name="propertyName">Name of the property that was changed.</param>
        /// <param name="xObjectChange">The the <see cref="System.Xml.Linq.XObjectChange"/> describing the change if the current arguments describe a change in an XML object.</param>
        /// <param name="innerChange">The <see cref="GeoTransformer.Gpx.ObservableElementChangedEventArgs"/> instance containing the nested event data.</param>
        public ObservableElementChangedEventArgs(object target, string propertyName, System.Xml.Linq.XObjectChange? xObjectChange = null, ObservableElementChangedEventArgs innerChange = null)
            : base(propertyName)
        {
            this._target = target;
            this._xObjectChange = xObjectChange;
            this._innerChange = innerChange;
        }

        /// <summary>
        /// Finds the deepest change that was intercepted.
        /// </summary>
        /// <returns>The deepest change, or this instance if it is already the deepest.</returns>
        public ObservableElementChangedEventArgs FindFirstChange()
        {
            if (this._innerChange != null)
                return this._innerChange.FindFirstChange();
            else
                return this;
        }

        /// <summary>
        /// Gets the instance that was changed.
        /// </summary>
        public object Target
        {
            get { return this._target; }
        }

        /// <summary>
        /// Gets the arguments of the change that caused this instance to be created. Can be <c>null</c>.
        /// </summary>
        public ObservableElementChangedEventArgs InnerChange
        {
            get { return this._innerChange; }
        }

        /// <summary>
        /// Gets the <see cref="System.Xml.Linq.XObjectChange"/> describing the change if the current arguments describe a change in an XML object.
        /// </summary>
        public System.Xml.Linq.XObjectChange? XObjectChange
        {
            get { return this._xObjectChange; }
        }
    }
}
