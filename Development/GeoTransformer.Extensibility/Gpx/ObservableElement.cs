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

            public void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
            {
                this.Container.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(this.PropertyName));

                if (args.NewItems != null)
                    foreach (var n in args.NewItems)
                        this.Container.AttachListener(this.PropertyName, n, this);

                if (args.OldItems != null)
                    foreach (var o in args.OldItems)
                        this.Container.DetachListener(this.PropertyName, o, this);
            }

            public void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
            {
                this.Container.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(this.PropertyName));
            }

            public void OnXObjectChanged(object sender, System.Xml.Linq.XObjectChangeEventArgs args)
            {
                this.Container.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(this.PropertyName));
            }
        }

        private Dictionary<string, object> _values = new Dictionary<string, object>(StringComparer.Ordinal);
        private Dictionary<string, ChangeHandler> _handlers = new Dictionary<string, ChangeHandler>(StringComparer.Ordinal);
        
        /// <summary>
        /// Holds a value indicating whether the observation is suspended. This is done to increase performance when performing the initial load
        /// of the data when there are no event subscribers.
        /// </summary>
        private bool _suspendObservation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableElement"/> class.
        /// </summary>
        /// <param name="suspendObservation">if set to <c>true</c> suspends the observation until <see cref="ResumeObservation"/> method is called. Should be set to <c>true</c> when the constructor loads the data.</param>
        protected ObservableElement(bool suspendObservation = false)
        {
            this._suspendObservation = suspendObservation;
        }

        protected T GetValue<T>(string key, bool create = false)
            where T : new()
        {
            object val;
            if (!this._values.TryGetValue(key, out val))
            {
                if (!create)
                    return default(T);

                var newVal = new T();
                this.SetValue(key, newVal);
                return newVal;
            }
            else
                return (T)val;
        }

        protected T GetValue<T>(string key)
        {
            object val;
            if (!this._values.TryGetValue(key, out val))
                return default(T);
            else
                return (T)val;
        }

        protected void SetValue<T>(string key, T value)
        {
            if (this._suspendObservation)
            {
                this._values[key] = value;
                return;
            }

            object oldVal;
            if (this._values.TryGetValue(key, out oldVal))
            {
                // use ReferenceEquals instead of Equals because the purpose is to remove the event handlers if the
                // object is changed in order to remove any potential memory leaks.
                if (object.ReferenceEquals(value, oldVal))
                    return;

                this.DetachListener(key, oldVal);
            }

            this._values[key] = value;
            this.AttachListener(key, value);

            if (!object.Equals(value, oldVal))
                this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(key));
        }

        private ChangeHandler GetChangeHandler(string key)
        {
            ChangeHandler handler;
            if (!this._handlers.TryGetValue(key, out handler))
            {
                handler = new ChangeHandler();
                handler.Container = this;
                handler.PropertyName = key;
                this._handlers.Add(key, handler);
            }
            return handler;
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

            foreach (var k in this._values)
                this.AttachListener(k.Key, k.Value);

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

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, args);
        }
    }
}
