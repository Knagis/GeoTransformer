/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeoTransformer.Coordinates;

namespace GeoTransformer.UnitTests.Gpx
{
    [TestClass]
    public class ObservableElementTest
    {
        private class SimpleImplementation : GeoTransformer.Gpx.ObservableElement
        {
            public SimpleImplementation(bool suspendObservation = false)
                : base(suspendObservation)
            {
            }

            public new void ResumeObservation()
            {
                base.ResumeObservation();
            }

            public void Set<T>(string key, T value)
            {
                this.SetValue<T>(key, value);
            }

            public T Get<T>(string key)
            {
                return this.GetValue<T>(key);
            }
        }

        private class SimpleNotifier : System.ComponentModel.INotifyPropertyChanged
        {
            public void Notify()
            {
                var h = this.PropertyChanged;
                if (h != null)
                    h(this, new System.ComponentModel.PropertyChangedEventArgs("simplenotifier"));
            }

            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        }

        /// <summary>
        /// Tests the collection itself.
        /// </summary>
        [TestMethod]
        public void TestCollection()
        {
            var dict = new SimpleImplementation();
            var key1 = "Id";
            var key2 = "ID";
            var key3 = "Name";

            dict.Set(key1, 123);
            dict.Set(key2, 234);
            dict.Set(key3, "geo");
            dict.Set(key3, "karlis");

            Assert.AreEqual(123, dict.Get<int>(key1));
            Assert.AreEqual(234, dict.Get<int>(key2));
            Assert.AreEqual("karlis", dict.Get<string>(key3));

            dict.Set<string>(key3, null);
            Assert.AreEqual(null, dict.Get<string>(key3));

            dict.Set(key3, "geo");
            Assert.AreEqual("geo", dict.Get<string>(key3));
        }

        /// <summary>
        /// Tests the constructors that take decimal values.
        /// </summary>
        [TestMethod]
        public void TestSimpleNotification()
        {
            var dict = new SimpleImplementation();
            int eventsRaised = 0;
            dict.PropertyChanged += (a, b) => { eventsRaised++; };

            string key1 = "id";

            // initialize a new value
            eventsRaised = 0;
            dict.Set(key1, 123);
            Assert.AreEqual(1, eventsRaised);

            // set non-existing to null (nothing changes)
            eventsRaised = 0;
            dict.Set<string>("asd", null);
            Assert.AreEqual(0, eventsRaised);

            // set existing key to the same value
            eventsRaised = 0;
            dict.Set(key1, 123);
            Assert.AreEqual(0, eventsRaised);

            // set existing key to a new value
            eventsRaised = 0;
            dict.Set(key1, 234);
            Assert.AreEqual(1, eventsRaised);

            // set new value to a default value (value-type) - test null/default(int)
            eventsRaised = 0;
            dict.Set("asdasda", 0);
            Assert.AreEqual(1, eventsRaised);
        }

        [TestMethod]
        public void TestXObjectNotification()
        {
            var dict = new SimpleImplementation();
            int eventsRaised = 0;
            string lastProperty = null;
            dict.PropertyChanged += (a, b) => { eventsRaised++; lastProperty = b.PropertyName; };

            var obj = new System.Xml.Linq.XElement("elem");
            dict.Set("key", obj);
            Assert.AreEqual(1, eventsRaised);
            Assert.AreEqual("key", lastProperty);

            eventsRaised = 0;
            lastProperty = null;
            obj.SetElementValue("inner", DateTime.Now);
            Assert.AreEqual(1, eventsRaised);
            Assert.AreEqual("key", lastProperty);

            eventsRaised = 0;
            dict.Set<object>("key", null);
            Assert.AreEqual(1, eventsRaised);
            obj.SetElementValue("inner", 123);
            Assert.AreEqual(1, eventsRaised);
        }

        /// <summary>
        /// Tests that the notifications are called correctly when children are changed
        /// </summary>
        [TestMethod]
        public void TestAdvancedNotification()
        {
            var dict = new SimpleImplementation();
            int eventsRaised = 0;
            dict.PropertyChanged += (a, b) => { eventsRaised++; };

            var collection = new GeoTransformer.Gpx.ObservableCollection<SimpleNotifier>();

            var n1 = new SimpleNotifier();
            var n2 = new SimpleNotifier();

            collection.Add(n1);

            // add a new object and force it to notify the dictionary
            eventsRaised = 0;
            dict.Set("1", n2);
            n2.Notify();
            Assert.AreEqual(2, eventsRaised);

            // remove it and force it to notify.
            eventsRaised = 0;
            dict.Set<SimpleNotifier>("1", null);
            n2.Notify();
            Assert.AreEqual(1, eventsRaised);

            // add a collection to the dict
            eventsRaised = 0;
            dict.Set("2", collection);
            Assert.AreEqual(1, eventsRaised);

            // add something to the dictionary
            eventsRaised = 0;
            collection.Add(n2);
            Assert.AreEqual(1, eventsRaised);

            // verify that the events from the list members are caught
            eventsRaised = 0;
            n2.Notify();
            Assert.AreEqual(1, eventsRaised);

            // verify that dict unsubscribes from items removed from the list.
            eventsRaised = 0;
            collection.Remove(n2);
            Assert.AreEqual(1, eventsRaised);
            n2.Notify();
            Assert.AreEqual(1, eventsRaised);

            // verify that .Clear is handler properly
            collection.Add(n2);
            eventsRaised = 0;
            collection.Clear();
            Assert.AreNotEqual(0, eventsRaised);
            eventsRaised = 0;
            n1.Notify();
            n2.Notify();
            Assert.AreEqual(0, eventsRaised);
        }

        /// <summary>
        /// Tests if the observation suspension and resume functions properly.
        /// </summary>
        [TestMethod]
        public void TestSuspendObservation()
        {
            var dict = new SimpleImplementation(true);
            int eventsRaised = 0;
            dict.PropertyChanged += (a, b) => { eventsRaised++; };

            string key1 = "id";

            // initialize a new value
            eventsRaised = 0;
            dict.Set(key1, 123);
            Assert.AreEqual(0, eventsRaised);

            // resume the observation
            dict.ResumeObservation();

            // set existing key to the same value
            eventsRaised = 0;
            dict.Set(key1, 123);
            Assert.AreEqual(0, eventsRaised);

            // set existing key to a new value
            eventsRaised = 0;
            dict.Set(key1, 234);
            Assert.AreEqual(1, eventsRaised);
        }

        /// <summary>
        /// Tests if the event arguments correctly contain recursive arguments.
        /// </summary>
        [TestMethod]
        public void TestEventArgsDepth()
        {
            var parent = new SimpleImplementation();
            var child = new SimpleImplementation();
            var xml = new System.Xml.Linq.XElement(XmlExtensions.GeoTransformerSchema + "test");

            GeoTransformer.Gpx.ObservableElementChangedEventArgs pargs = null;
            GeoTransformer.Gpx.ObservableElementChangedEventArgs cargs = null;
            parent.PropertyChanged += (a, b) => { pargs = b; };
            child.PropertyChanged += (a, b) => { cargs = b; };

            parent.Set("c", child);
            Assert.IsNull(cargs);
            Assert.AreEqual("c", pargs.PropertyName);
            Assert.IsNull(pargs.InnerChange);
            Assert.AreEqual(pargs, pargs.FindFirstChange());
            pargs = null;

            child.Set("xml", xml);
            Assert.IsNotNull(cargs);
            Assert.IsNotNull(pargs);
            Assert.AreEqual("c", pargs.PropertyName);
            Assert.AreSame(cargs, pargs.InnerChange);
            Assert.AreSame(cargs, pargs.FindFirstChange());
            Assert.AreEqual("xml", cargs.PropertyName);
            Assert.AreSame(child, cargs.Target);
            cargs = null;
            pargs = null;

            xml.Value = "data";
            Assert.IsNotNull(cargs);
            Assert.IsNotNull(pargs);
            Assert.AreSame(cargs, pargs.InnerChange);
            Assert.IsNotNull(pargs.FindFirstChange().XObjectChange);
        }
    }
}
