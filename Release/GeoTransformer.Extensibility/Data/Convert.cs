/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Data
{
    /// <summary>
    /// Class contains static methods for generic type conversion.
    /// </summary>
    public static class Convert
    {
        /// <summary>
        /// Changes the type for the given value.
        /// </summary>
        /// <typeparam name="TConversion">The target type for the conversion.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <param name="provider">The format provider.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static TConversion ChangeType<TConversion>(object value, IFormatProvider provider)
        {
            return (TConversion)ChangeType(value, typeof(TConversion), provider);
        }

        /// <summary>
        /// Changes the type for the given value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="conversionType">The target type for the conversion.</param>
        /// <param name="provider">The format provider.</param>
        public static object ChangeType(object value, Type conversionType, IFormatProvider provider)
        {
            if (conversionType == null)
                throw new ArgumentNullException("conversionType");

            if (value is DBNull)
                value = null;

            if (conversionType.IsValueType && value == null)
                return System.Activator.CreateInstance(conversionType);

            if (value == null)
                return null;

            if (conversionType.IsAssignableFrom(value.GetType()))
                return value;

            if (conversionType.IsGenericType && !conversionType.IsGenericTypeDefinition && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (value.ToString().Trim().Length == 0)
                    return null;
                conversionType = conversionType.GetGenericArguments()[0];
            }

            if (conversionType.IsEnum)
            {
                string v = value as string;
                if (v != null)
                {
                    v = v.Trim();
                    if (v.Length == 0)
                        return null;
                    return Enum.Parse(conversionType, v, true);
                }

                value = ChangeType(value, Enum.GetUnderlyingType(conversionType), provider);
                return Enum.ToObject(conversionType, value);
            }

            if (conversionType.IsPrimitive || conversionType.Equals(typeof(decimal)))
            {
                if (!conversionType.Equals(typeof(bool)) && !conversionType.Equals(typeof(char)))
                {
                    string v = value as string;
                    if (v != null)
                        value = "0" + v;
                }
            }

            if (!(value is IConvertible) && conversionType.Equals(typeof(string)))
                return value.ToString();

            return System.Convert.ChangeType(value, conversionType, provider);
        }
    }
}
