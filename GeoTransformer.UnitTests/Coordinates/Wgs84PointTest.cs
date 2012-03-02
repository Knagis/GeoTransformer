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

namespace GeoTransformer.UnitTests.Coordinates
{
    [TestClass]
    public class Wgs84PointTest
    {
        /// <summary>
        /// Gets or sets the test context which provides information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Tests the constructors that take decimal values.
        /// </summary>
        [TestMethod]
        public void TestSimpleConstructors()
        {
            decimal[,] validData = new decimal[,]
            { 
                { 0, 0 },
                {-54.123M, 24.3455M},
                {-87.2344M, 132.00001M},
                {-90,-180},
                {90,-180},
                {90,180},
                {-90,-180}
            };

            decimal[,] outOfRange = new decimal[,]
            {
                {0, -180.0001M},
                {0, 180.00001M},
                {-90.00001M, 0},
                {90.000001M, 0}
            };

            var p1 = new Wgs84Point();
            Assert.AreEqual(p1.Latitude, 0);
            Assert.AreEqual(p1.Longitude, 0);

            for (int i = 0; i < validData.GetLength(0); i++)
            {
                p1 = new Wgs84Point(validData[i, 0], validData[i, 1]);
                Assert.AreEqual(validData[i, 0], p1.Latitude);
                Assert.AreEqual(validData[i, 1], p1.Longitude);
            }

            for (int i = 0; i < outOfRange.GetLength(0); i++)
            {
                try
                {
                    p1 = new Wgs84Point(outOfRange[i, 0], outOfRange[i, 1]);
                    Assert.Fail("ArgumentOutOfRangeException expected.");
                }
                catch (ArgumentOutOfRangeException)
                {
                }
            }
        }

        /// <summary>
        /// Tests the coordinate parser.
        /// </summary>
        [TestMethod]
        public void TestParser()
        {
            object[,] data = new object[,]
            {
                { "N 56° 41.180 E 023° 47.904", 56.68633M, 23.7984M },
                { "N   56°   41.180   E   023°   47.904  ", 56.68633M, 23.7984M },
                { "N 56° 41' 10.788\" E 23° 47' 54.240\"", 56.68633M, 23.7984M },
                { "56.68633 23.7984", 56.68633M, 23.7984M },
                { "56.68633N 23.7984E", 56.68633M, 23.7984M },
                { "23.7984E 56.68633N", 56.68633M, 23.7984M },
                { "56° 41.180N 023° 47.904E", 56.68633M, 23.7984M },

                { "S 56° 41.180 W 023° 47.904", -56.68633M, -23.7984M },
                { "S 56° 41' 10.788\" W 23° 47' 54.240\"", -56.68633M, -23.7984M },
                { "-56.68633 -23.7984", -56.68633M, -23.7984M },
                { "-  56.68633 -  23.7984", -56.68633M, -23.7984M },
                { "56.68633S 23.7984W", -56.68633M, -23.7984M },
                { "23.7984W 56.68633S", -56.68633M, -23.7984M },
                { "56° 41.180S 023° 47.904W", -56.68633M, -23.7984M },

                { "N 43° 33.650 W 079° 33.455", 43.56083M, -79.55758M },
                { "   N 43 33.65    W 79°   33,455 ", 43.56083M, -79.55758M },

                { "56°41.180S 023°47.904E", -56.68633M, 23.7984M },

                { "N 00° 00.001 E 000° 00.001", 0.00002M, 0.00002M },
                { "S 00° 00.001 E 000° 00.001", -0.00002M, 0.00002M },
                { "S 00° 00.001 W 000° 00.001", -0.00002M, -0.00002M },

                { "N 00° 01.001 E 000° 01.001", 0.01668M, 0.01668M },
                { "S 00° 01.001 W 000° 01.001", -0.01668M, -0.01668M },
                { "N 01° 01.001 E 001° 01.001", 1.01668M, 1.01668M },
                { "S 01° 01.001 W 001° 01.001", -1.01668M, -1.01668M },

            };

            for (int i = 0; i < data.GetLength(0); i++)
            {
                var p1 = new Wgs84Point((string)data[i, 0]);
                Assert.AreEqual((decimal)data[i, 1], Decimal.Round(p1.Latitude, 5));

                var p = (decimal)data[i, 2];
                if (Decimal.Round(p, 5) == Decimal.Round(p, 4))
                    Assert.AreEqual(p, Decimal.Round(p1.Longitude, 4));
                else
                    Assert.AreEqual(p, Decimal.Round(p1.Longitude, 5));
            }
        }

        /// <summary>
        /// Tests the coordinate formatter.
        /// </summary>
        [TestMethod]
        public void TestFormatter()
        {
            object[,] data = new object[,]
            {
                { "N 56° 41.180 E 023° 47.904", 56.68633M, 23.7984M },
                { "N 43° 33.650 W 079° 33.455", 43.56083M, -79.55758M },
                { "S 43° 33.650 W 079° 33.455", -43.56083M, -79.55758M },
                { "N 03° 33.650 E 079° 33.455", 3.56083M, 79.55758M },
                { "N 00° 00.001 E 000° 00.001", 0.00002M, 0.00002M },
                { "S 00° 00.001 E 000° 00.001", -0.00002M, 0.00002M },
                { "S 00° 00.001 W 000° 00.001", -0.00002M, -0.00002M },
            };

            for (int i = 0; i < data.GetLength(0); i++)
            {
                var p1 = new Wgs84Point((decimal)data[i, 1], (decimal)data[i, 2]);
                Assert.AreEqual((string)data[i, 0], p1.ToString());

                var p2 = new Wgs84Point(p1.ToString());
                Assert.AreEqual(p1.ToString(), p2.ToString());
            }
        }
    }
}
