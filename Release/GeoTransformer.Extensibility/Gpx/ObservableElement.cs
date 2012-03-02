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
    public class ObservableElement : System.ComponentModel.INotifyPropertyChanged
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
                        this.Container.AttachListener(this, n);

                if (args.OldItems != null)
                    foreach (var o in args.OldItems)
                        this.Container.DetachListener(this, o);
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
            object oldVal;
            if (this._values.TryGetValue(key, out oldVal))
            {
                if (object.ReferenceEquals(value, oldVal))
                    return;

                this.DetachListener(this.GetChangeHandler(key), oldVal);
            }

            this._values[key] = value;
            this.AttachListener(this.GetChangeHandler(key), value);

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

        private void AttachListener(ChangeHandler handler, object val)
        {
            var collection = val as System.Collections.Specialized.INotifyCollectionChanged;
            if (collection != null)
            {
                collection.CollectionChanged += handler.OnCollectionChanged;

                var en = val as System.Collections.IEnumerable;
                if (en != null)
                    foreach (var x in en)
                        this.AttachListener(handler, x);

                return;
            }

            var propChanged = val as System.ComponentModel.INotifyPropertyChanged;
            if (propChanged != null)
            {
                propChanged.PropertyChanged += handler.OnPropertyChanged;
                return;
            }

            var xobject = val as System.Xml.Linq.XObject;
            if (xobject != null)
            {
                xobject.Changed += handler.OnXObjectChanged;
            }
        }

        private void DetachListener(ChangeHandler handler, object val)
        {
            var collection = val as System.Collections.Specialized.INotifyCollectionChanged;
            if (collection != null)
            {
                var en = val as System.Collections.IEnumerable;
                if (en != null)
                    foreach (var x in en)
                        this.DetachListener(handler, x);

                collection.CollectionChanged -= handler.OnCollectionChanged;
                return;
            }

            var propChanged = val as System.ComponentModel.INotifyPropertyChanged;
            if (propChanged != null)
            {
                propChanged.PropertyChanged -= handler.OnPropertyChanged;
                return;
            }

            var xobject = val as System.Xml.Linq.XObject;
            if (xobject != null)
            {
                xobject.Changed -= handler.OnXObjectChanged;
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
