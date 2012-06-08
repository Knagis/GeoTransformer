/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */

using System.IO;

namespace GeoTransformer.Publishers.GarminGps
{
    internal sealed class PathGenerator
    {
        /// <summary>
        /// The root folder for the drive where the images will be stored.
        /// </summary>
        private string _driveRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathGenerator"/> class.
        /// </summary>
        /// <param name="driveRoot">The root folder for the drive where the images will be stored.</param>
        public PathGenerator(string driveRoot)
        {
            this._driveRoot = driveRoot;
        }

        /// <summary>
        /// Creates the path where the image should be stored.
        /// </summary>
        /// <param name="waypoint">The waypoint that contains the image.</param>
        /// <param name="image">The image data.</param>
        /// <returns>The path where the image should be stored.</returns>
        public string CreateImagePath(Gpx.GpxWaypoint waypoint, Gpx.GeocacheImage image)
        {
            var code = waypoint.Name;

            var lastChar = code.Length > 2 ? code[code.Length - 1].ToString() : "0";
            var lastChar2 = code.Length > 3 ? code[code.Length - 2].ToString() : "0";
            return Path.Combine(this._driveRoot, "Garmin", "GeocachePhotos", lastChar, lastChar2, code, "{0}.jpg");
        }
    }
}
