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
    /// Base class for data objects that need to provide events when property values are changed.
    /// The class will automatically raise the events for nested objects as well.
    /// </summary>
    public abstract class ObservableElement : System.ComponentModel.INotifyPropertyChanged
    {
        private class ChangeHandler
        {
            public ObservableElement Container;
            public string PropertyName;

            public void OnObservableElementChange(object sender, ObservableElementChangedEventArgs args)
            {
                this.Container.OnPropertyChanged(new ObservableElementChangedEventArgs(sender, this.PropertyName, null, args));
            }

            public void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
            {
                this.Container.OnPropertyChanged(new ObservableElementChangedEventArgs(sender, this.PropertyName, collectionChange: args));

                if (args.NewItems != null)
                    foreach (var n in args.NewItems)
                        this.Container.AttachListener(this.PropertyName, n, this);

                if (args.OldItems != null)
                    foreach (var o in args.OldItems)
                        this.Container.DetachListener(this.PropertyName, o, this);
            }

            public void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
            {
                this.Container.OnPropertyChanged(new ObservableElementChangedEventArgs(sender, this.PropertyName));
            }

            public void OnXObjectChanged(object sender, System.Xml.Linq.XObjectChangeEventArgs args)
            {
                this.Container.OnPropertyChanged(new ObservableElementChangedEventArgs(sender, this.PropertyName, args.ObjectChange));
            }
        }

        private System.Collections.IDictionary _values;
        private System.Collections.IDictionary _handlers;
        
        /// <summary>
        /// Holds a value indicating whether the observation is suspended. This is done to increase performance when performing the initial load
        /// of the data when there are no event subscribers.
        /// </summary>
        private bool _suspendObservation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableElement"/> class.
        /// </summary>
        /// <param name="suspendObservation">if set to <c>true</c> suspends the observation until <see cref="ResumeObservation"/> method is called. Should be set to <c>true</c> when the constructor loads the data.</param>
        /// <param name="propertyCount">Number of properties that will be stored using <see cref="SetValue"/>. This should be specified when the known number is large to optimize performance.</param>
        protected ObservableElement(bool suspendObservation = false, int propertyCount = -1)
        {
            this._suspendObservation = suspendObservation;

            if (propertyCount == -1)
            {
                this._values = new System.Collections.Specialized.HybridDictionary();
                this._handlers = new System.Collections.Specialized.HybridDictionary();
            }
            else if (propertyCount < 6)
            {
                this._values = new System.Collections.Specialized.ListDictionary();
                this._handlers = new System.Collections.Specialized.ListDictionary();
            }
            else
            {
                this._values = new System.Collections.Hashtable(propertyCount);
                // since usually only part of the properties are observable, assume that hybrid dict will be better
                this._handlers = new System.Collections.Specialized.HybridDictionary(); 
            }
        }

        protected T GetValue<T>(string key, bool create = false)
            where T : new()
        {
            if (!this._values.Contains(key))
            {
                if (!create)
                    return default(T);

                var newVal = new T(); 

                // setting the default value - from the consumer perspective there is no change happening
                this.SetValue(key, newVal, true);
                return newVal;
            }
            else
                return (T)this._values[key];
        }

        protected T GetValue<T>(string key)
        {
            if (!this._values.Contains(key))
                return default(T);
            else
                return (T)this._values[key];
        }

        /// <summary>
        /// Sets a value in this element.
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="key">The key by which the value will be retrieved.</param>
        /// <param name="value">The value that will be stored.</param>
        /// <param name="suppressChangeEvent">if set to <c>true</c> suppresses the <see cref="PropertyChanged"/> event. Should only be used in very special scenarios.</param>
        protected void SetValue<T>(string key, T value, bool suppressChangeEvent = false)
        {
            if (this._suspendObservation)
            {
                this._values[key] = value;
                return;
            }

            object oldVal = null;
            if (this._values.Contains(key))
            {
                oldVal = this._values[key];

                // use ReferenceEquals instead of Equals because the purpose is to remove the event handlers if the
                // object is changed in order to remove any potential memory leaks.
                if (object.ReferenceEquals(value, oldVal))
                    return;

                this.DetachListener(key, oldVal);
            }

            this._values[key] = value;
            this.AttachListener(key, value);

            if (!suppressChangeEvent && !object.Equals(value, oldVal))
                this.OnPropertyChanged(new ObservableElementChangedEventArgs(this, key));
        }

        private ChangeHandler GetChangeHandler(string key)
        {
            if (!this._handlers.Contains(key))
            {
                var handler = new ChangeHandler();
                handler.Container = this;
                handler.PropertyName = key;
                this._handlers.Add(key, handler);
                return handler;
            }
            return (ChangeHandler)this._handlers[key];
        }

        /// <summary>
        /// Resumes the observation and starts raising <see cref="PropertyChanged"/> events.
        /// </summary>
        /// <exception cref="InvalidOperationException">when the observation is not suspended</exception>
        /// <seealso cref="ObservableElement(bool)"/>
        protected void ResumeObservation()
        {
            if (!this._suspendObservation)
                throw new InvalidOperationException("ResumeObservation can only be called once and if the ObservableElement constructor is called with argument suspendObservation=true.");

            foreach (System.Collections.DictionaryEntry k in this._values)
                this.AttachListener((string)k.Key, k.Value);

            this._suspendObservation = false;
        }

        private void AttachListener(string key, object val, ChangeHandler handler = null)
        {
            var collection = val as System.Collections.Specialized.INotifyCollectionChanged;
            if (collection != null)
            {
                if (handler == null) handler = this.GetChangeHandler(key);
                collection.CollectionChanged += handler.OnCollectionChanged;

                var en = val as System.Collections.IEnumerable;
                if (en != null)
                    foreach (var x in en)
                        this.AttachListener(key, x, handler);

                return;
            }

            var observable = val as ObservableElement;
            if (observable != null)
            {
                if (handler == null) handler = this.GetChangeHandler(key);
                observable.PropertyChanged += handler.OnObservableElementChange;
                return;
            }

            var propChanged = val as System.ComponentModel.INotifyPropertyChanged;
            if (propChanged != null)
            {
                if (handler == null) handler = this.GetChangeHandler(key);
                propChanged.PropertyChanged += handler.OnPropertyChanged;
                return;
            }

            var xobject = val as System.Xml.Linq.XObject;
            if (xobject != null)
            {
                if (handler == null) handler = this.GetChangeHandler(key);
                xobject.Changed += handler.OnXObjectChanged;
            }
        }

        private void DetachListener(string key, object val, ChangeHandler handler = null)
        {
            var collection = val as System.Collections.Specialized.INotifyCollectionChanged;
            if (collection != null)
            {
                if (handler == null) handler = this.GetChangeHandler(key);
                var en = val as System.Collections.IEnumerable;
                if (en != null)
                    foreach (var x in en)
                        this.DetachListener(key, x, handler);

                collection.CollectionChanged -= handler.OnCollectionChanged;
                return;
            }

            var observable = val as ObservableElement;
            if (observable != null)
            {
                if (handler == null) handler = this.GetChangeHandler(key);
                observable.PropertyChanged -= handler.OnObservableElementChange;
                return;
            }

            var propChanged = val as System.ComponentModel.INotifyPropertyChanged;
            if (propChanged != null)
            {
                if (handler == null) handler = this.GetChangeHandler(key);
                propChanged.PropertyChanged -= handler.OnPropertyChanged;
                return;
            }

            var xobject = val as System.Xml.Linq.XObject;
            if (xobject != null)
            {
                if (handler == null) handler = this.GetChangeHandler(key);
                xobject.Changed -= handler.OnXObjectChanged;
                return;
            }
        }

        /// <summary>
        /// Occurs when a property value changes or a property value issues a property changed notification of its own.
        /// </summary>
        public event EventHandler<ObservableElementChangedEventArgs> PropertyChanged;
        
        /// <summary>
        /// Holds the handler for the explicit implementation of <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/>.
        /// </summary>
        private event System.ComponentModel.PropertyChangedEventHandler _notifyPropertyChangeEvent;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        event System.ComponentModel.PropertyChangedEventHandler System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        {
            add { this._notifyPropertyChangeEvent += value; }
            remove { this._notifyPropertyChangeEvent -= value; }
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="ObservableElementChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(ObservableElementChangedEventArgs args)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, args);

            var handler2 = this._notifyPropertyChangeEvent;
            if (handler2 != null)
                handler(this, args);
        }
    }
}
