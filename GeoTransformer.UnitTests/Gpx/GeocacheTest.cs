/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeoTransformer.Gpx;
using System.Xml.Linq;

namespace GeoTransformer.UnitTests.Gpx
{
    [TestClass]
    public class GeocacheTest
    {
        /// <summary>
        /// Tests the <see cref="Geocache.IsDefined"/> method.
        /// </summary>
        [TestMethod]
        public void TestIsDefined()
        {
            var g = new Geocache();

            Assert.IsFalse(g.IsDefined());

            g.ShortDescription.Text = "sample";

            Assert.IsTrue(g.IsDefined());
        }
    }
}
