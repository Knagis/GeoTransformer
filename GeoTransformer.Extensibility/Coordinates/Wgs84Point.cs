/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Coordinates
{
    /// <summary>
    /// Represents a coordinate point in WGS84 datum.
    /// </summary>
    [Serializable]
    public struct Wgs84Point
    {
        private decimal _latitude;
        private decimal _longitude;

        /// <summary>
        /// Gets or sets the latitude (North/South coordinate).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">when <paramref name="value"/> is out of the [-90; 90] range.</exception>
        public decimal Latitude 
        {
            get { return this._latitude; }
            set 
            {
                if (value > 90 || value < -90)
                    throw new ArgumentOutOfRangeException("value", "Latitude must be in range [-90; 90].");

                this._latitude = value; 
            }
        }

        /// <summary>
        /// Gets or sets the longitude (East/West coordinate).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">when <paramref name="value"/> is set out of the [-180; 180] range.</exception>
        public decimal Longitude 
        {
            get { return this._longitude; }
            set
            {
                if (value < -180 || value > 180)
                    throw new ArgumentOutOfRangeException("value", "Longitude must be in range [-180; 180].");

                this._longitude = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Wgs84Point"/> struct.
        /// </summary>
        /// <param name="latitude">The latitude (North/South coordinate).</param>
        /// <param name="longitude">The longitude (East/West coordinate).</param>
        /// <exception cref="ArgumentOutOfRangeException">when <paramref name="latitude"/> is out of [-90; 90] range or <paramref name="longitude"/> is out of [-180; 180] range.</exception>
        public Wgs84Point(decimal latitude, decimal longitude)
        {
            if (longitude < -180 || longitude > 180)
                throw new ArgumentOutOfRangeException("longitude", "Longitude must be in range [-180; 180].");

            if (latitude > 90 || latitude < -90)
                throw new ArgumentOutOfRangeException("latitude", "Latitude must be in range [-90; 90].");

            this._latitude = latitude;
            this._longitude = longitude;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Wgs84Point"/> struct.
        /// </summary>
        /// <param name="coordinates">The formatted coordinates to be parsed.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="coordinates"/> is <c>null</c></exception>
        /// <exception cref="ArgumentOutOfRangeException">when latitude is out of [-90; 90] range or longitude is out of [-180; 180] range.</exception>
        /// <exception cref="FormatException">when the <paramref name="coordinates"/> cannot be parsed.</exception>
        public Wgs84Point(string coordinates)
        {
            if (coordinates == null)
                throw new ArgumentNullException("coordinates");

            string lat;
            string lon;
            SplitCoordinates(coordinates, out lat, out lon);
            
            decimal latV;
            decimal lonV;
            ParseCoordinates(lat, lon, out latV, out lonV);

            this._latitude = latV;
            this._longitude = lonV;
        }

        /// <summary>
        /// Tries to parse the given coordinates. Returns <c>null</c> if the parsing could not be completed.
        /// </summary>
        /// <param name="coordinates">The coordinates to parse.</param>
        /// <returns>The parsed coordinates object or <c>null</c> if they could not be parsed.</returns>
        public static Wgs84Point? TryParse(string coordinates)
        {
            if (string.IsNullOrWhiteSpace(coordinates))
                return null;

            try
            {
                return new Wgs84Point(coordinates);
            }
            catch (Exception) { }

            return null;
        }

        private static void SplitCoordinates(string coordinates, out string latitude, out string longitude)
        {
            if (string.IsNullOrWhiteSpace(coordinates))
            {
                latitude = "0";
                longitude = "0";
                return;
            }

            coordinates = coordinates.Trim().ToUpperInvariant();

            var xi = coordinates.IndexOfAny(new char[] { 'E', 'W' });
            var yi = coordinates.IndexOfAny(new char[] { 'N', 'S' });
            string x, y;
            if (xi < yi && yi > 0)
            {
                if (xi == 0)
                {
                    // E 024 N 56
                    x = coordinates.Substring(0, yi);
                    y = coordinates.Substring(yi);
                }
                else
                {
                    // 24E 56N
                    x = coordinates.Substring(0, xi + 1);
                    y = coordinates.Substring(xi + 1);
                }
            }
            else if (yi < xi && xi > 0)
            {
                if (yi == 0)
                {
                    // N 56 E 024
                    x = coordinates.Substring(xi);
                    y = coordinates.Substring(0, xi);
                }
                else
                {
                    // 56N 24E
                    x = coordinates.Substring(yi + 1);
                    y = coordinates.Substring(0, yi + 1);
                }
            }
            else
            {
                var parts = coordinates.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length % 2 == 0)
                {
                    y = string.Empty;
                    x = string.Empty;
                    for (yi = 0; yi < parts.Length / 2; yi++)
                        y += ' ' + parts[yi];
                    for (xi = parts.Length / 2; xi < parts.Length; xi++)
                        x += ' ' + parts[xi];
                }
                else
                {
                    throw new FormatException("Unable to separate longitude from latitude. Either specify N/S and E/W or use the same format for both latitude and longitude.");
                }
            }

            latitude = y.Trim();
            longitude = x.Trim();
        }

        private static void ParseCoordinates(string latitude, string longitude, out decimal latitudeValue, out decimal longitudeValue)
        {
            var x = longitude.Trim();
            var y = latitude.Trim();
            x = x.Replace("E", "").Replace("e", "").Replace("W", "-").Replace("w", "-").Replace("- ", "-");
            y = y.Replace("N", "").Replace("n", "").Replace("S", "-").Replace("s", "-").Replace("- ", "-");
            x = x.Replace("°", " ").Replace("\"", "").Replace("'", "").Replace("`", "");
            y = y.Replace("°", " ").Replace("\"", "").Replace("'", "").Replace("`", "");
            x = x.Replace(",", ".");
            y = y.Replace(",", ".");
            x = x.Trim();
            y = y.Trim();
            if (!char.IsDigit(x[x.Length - 1])) x = x[x.Length - 1] + x.Substring(0, x.Length - 1);
            if (!char.IsDigit(y[y.Length - 1])) y = y[y.Length - 1] + y.Substring(0, y.Length - 1);

            decimal xc = 0;
            decimal yc = 0;
            Func<string, decimal> parse = a => decimal.Parse(a, System.Globalization.CultureInfo.InvariantCulture);

            try
            {
                var xa = x.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (xa.Length == 1)
                    xc = parse(xa[0]);
                else if (xa.Length == 2)
                {
                    xc = parse(xa[0]) + (xa[0][0] == '-' ? -1M : 1M) * parse(xa[1]) / 60M;
                }
                else if (xa.Length == 3)
                    xc = parse(xa[0]) + (xa[0][0] == '-' ? -1M : 1M) * (parse(xa[1]) + parse(xa[2]) / 60M) / 60M;
            }
            catch
            {
                throw new FormatException("The longitude cannot be parsed.");
            }
            try
            {
                var ya = y.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (ya.Length == 1)
                    yc = parse(ya[0]);
                else if (ya.Length == 2)
                    yc = parse(ya[0]) + (ya[0][0] == '-' ? -1M : 1M) * parse(ya[1]) / 60M;
                else if (ya.Length == 3)
                    yc = parse(ya[0]) + (ya[0][0] == '-' ? -1M : 1M) * (parse(ya[1]) + parse(ya[2]) / 60M) / 60M;
            }
            catch
            {
                throw new FormatException("The latitude cannot be parsed.");
            }

            if (xc < -180 || xc > 180)
                throw new ArgumentOutOfRangeException("longitude", "The longitude value is out of the [-180; 180] range.");
            if (yc < -90 || yc > 90)
                throw new ArgumentOutOfRangeException("latitude", "The latitude value is out of the [-90; 90] range.");

            latitudeValue = yc;
            longitudeValue = xc;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            decimal latDegrees = Math.Truncate(this.Latitude);
            decimal lonDegrees = Math.Truncate(this.Longitude);

            var sb = new StringBuilder();
            sb.Append(this.Latitude < 0 ? "S " : "N ");
            sb.Append(Math.Abs(latDegrees).ToString("00", System.Globalization.CultureInfo.InvariantCulture));
            sb.Append("° ");
            sb.Append(((this.Latitude < 0 ? -1 : 1) * 60 * (this.Latitude - latDegrees)).ToString("00.000", System.Globalization.CultureInfo.InvariantCulture));
            sb.Append(" ");
            sb.Append(this.Longitude < 0 ? "W " : "E ");
            sb.Append(Math.Abs(lonDegrees).ToString("000", System.Globalization.CultureInfo.InvariantCulture));
            sb.Append("° ");
            sb.Append(((this.Longitude < 0 ? -1 : 1) * 60 * (this.Longitude - lonDegrees)).ToString("00.000", System.Globalization.CultureInfo.InvariantCulture));

            return sb.ToString();
        }

        /// <summary>
        /// Implements the equality operator. 
        /// </summary>
        /// <param name="left">The left side operand.</param>
        /// <param name="right">The right side operand.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Wgs84Point left, Wgs84Point right)
        {
            return left._latitude == right._latitude && left._longitude == right._longitude;
        }

        /// <summary>
        /// Implements the inequality operator. 
        /// </summary>
        /// <param name="left">The left side operand.</param>
        /// <param name="right">The right side operand.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Wgs84Point left, Wgs84Point right)
        {
            return left._latitude != right._latitude || left._longitude != right._longitude;
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
            if (obj == null || !(obj is Wgs84Point))
                return false;

            var right = (Wgs84Point)obj;
            return this._latitude == right._latitude && this._longitude == right._longitude;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this._latitude.GetHashCode() % this._longitude.GetHashCode();
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Wgs84Point"/> to <see cref="GeoTransformer.GeocachingService.LatLngPoint"/>.
        /// </summary>
        public static explicit operator GeocachingService.LatLngPoint(Wgs84Point value)
        {
            return new GeocachingService.LatLngPoint()
            {
                Latitude = (double)value.Latitude,
                Longitude = (double)value.Longitude
            };
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="GeoTransformer.GeocachingService.LatLngPoint"/> to <see cref="GeoTransformer.Coordinates.Wgs84Point"/>.
        /// </summary>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator Wgs84Point(GeocachingService.LatLngPoint value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return new Wgs84Point((decimal)value.Latitude, (decimal)value.Longitude);
        }

        /// <summary>
        /// Determines if the two coordinates are approximately equal (uses the <see cref="ToString"/> result for comparison).
        /// </summary>
        /// <param name="first">The first coordinates.</param>
        /// <param name="second">The second coordinates.</param>
        /// <returns><c>True</c> if the coordinates are approximately the same.</returns>
        public static bool ApproximateEquals(Wgs84Point? first, Wgs84Point? second)
        {
            if (!first.HasValue)
                return !second.HasValue;
            if (!second.HasValue)
                return false;

            return string.Equals(first.ToString(), second.ToString());
        }
    }
}
