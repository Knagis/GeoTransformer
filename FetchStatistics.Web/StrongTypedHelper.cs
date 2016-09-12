/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FetchStatistics.Web
{
    public static class StrongTypedHelper
    {
        private static Dictionary<Delegate, Tuple<object, object>> EdgeValueCache
        {
            get
            {
                var c = HttpContext.Current.Items["EdgeValueCache"] as Dictionary<Delegate, Tuple<object, object>>;
                if (c == null)
                    HttpContext.Current.Items["EdgeValueCache"] = c = new Dictionary<Delegate, Tuple<object, object>>();

                return c;
            }
        }

        public static string Value(this System.Web.UI.WebControls.RepeaterItem container, System.Linq.Expressions.Expression<Func<StatisticsData, DateTime>> selectorExpression)
        {
            return Value(container, selectorExpression, a => a.ToShortDateString(), false);
        }

        public static string Value(this System.Web.UI.WebControls.RepeaterItem container, System.Linq.Expressions.Expression<Func<StatisticsData, DateTime?>> selectorExpression)
        {
            return Value(container, selectorExpression, a => a.Value.ToShortDateString(), true);
        }

        public static string Value(this System.Web.UI.WebControls.RepeaterItem container, System.Linq.Expressions.Expression<Func<StatisticsData, decimal>> selectorExpression)
        {
            return Value(container, selectorExpression, a => a.ToString(System.Globalization.CultureInfo.InvariantCulture), false);
        }

        public static string Value(this System.Web.UI.WebControls.RepeaterItem container, System.Linq.Expressions.Expression<Func<StatisticsData, decimal?>> selectorExpression)
        {
            return Value(container, selectorExpression, a => a.Value.ToString(System.Globalization.CultureInfo.InvariantCulture), true);
        }

        public static string Value<TValue>(this System.Web.UI.WebControls.RepeaterItem container, System.Linq.Expressions.Expression<Func<StatisticsData, TValue>> selectorExpression)
        {
            return Value(container, selectorExpression, a => a.ToString(), !typeof(TValue).IsValueType);
        }

        public static string Value<TValue>(this System.Web.UI.WebControls.RepeaterItem container, System.Linq.Expressions.Expression<Func<StatisticsData, Nullable<TValue>>> selectorExpression)
            where TValue : struct
        {
            return Value(container, selectorExpression, a => a.Value.ToString(), true);
        }

        private static string Value<TValue>(this System.Web.UI.WebControls.RepeaterItem container, System.Linq.Expressions.Expression<Func<StatisticsData, TValue>> selectorExpression, Func<TValue, string> toString, bool isNullable, string cacheKey = null)
        {
            string name = selectorExpression.ToString();

            UISettingsAttribute attr = null;
            if (selectorExpression.Body is System.Linq.Expressions.MemberExpression)
            {
                var me = (System.Linq.Expressions.MemberExpression)selectorExpression.Body;
                attr = Default._uiSettingsCache[me.Member.Name];
            }


            Func<StatisticsData, TValue> selector;
            if (!Default._keySelectorCache.ContainsKey(name))
                Default._keySelectorCache[name] = selector = selectorExpression.Compile();
            else
                selector = (Func<StatisticsData, TValue>)Default._keySelectorCache[name];

            var obj = (StatisticsData)container.DataItem;
            var v = selector(obj);
            if (v == null && isNullable)
                return string.Empty;

            var edge = EdgeValueCache;
            if (!edge.ContainsKey(selector))
            {
                var rep = (System.Web.UI.WebControls.Repeater)container.Parent;
                var ds = (IEnumerable<StatisticsData>)rep.DataSource;
                var q = ds.Select(selector);
                if (isNullable)
                    q = q.Where(o => o != null);

                edge[selector] = Tuple.Create<object, object>(q.Min(), q.Max());
            }

            var minmax = edge[selector];
            int barWidth = -1;

            try
            {
                if (typeof(TValue).Equals(typeof(int)))
                    barWidth = 100 * ((int)(object)v - (int)minmax.Item1) / ((int)minmax.Item2 - (int)minmax.Item1);
                else if (typeof(TValue).Equals(typeof(int?)))
                    barWidth = 100 * (((int?)(object)v).Value - (int)minmax.Item1) / ((int)minmax.Item2 - (int)minmax.Item1);
                else if (typeof(TValue).Equals(typeof(decimal)))
                    barWidth = (int)Math.Round(100 * ((decimal)(object)v - (decimal)minmax.Item1) / ((decimal)minmax.Item2 - (decimal)minmax.Item1));
                else if (typeof(TValue).Equals(typeof(decimal?)))
                    barWidth = (int)Math.Round(100 * (((decimal?)(object)v).Value - (decimal)minmax.Item1) / ((decimal)minmax.Item2 - (decimal)minmax.Item1));
            }
            catch (DivideByZeroException)
            {
                barWidth = 100;
            }

            if (typeof(TValue).Equals(typeof(Guid)))
                return toString(v);

            if (barWidth == -1)
                return "<span>" + toString(v) + "</span>";

            if (attr != null && !attr.OrderByDescendingFirst) barWidth = 100 - barWidth;
            var color = MiddleColor(barWidth / 100M);

            return "<span style=\"background:" + System.Drawing.ColorTranslator.ToHtml(color) + ";width:" + barWidth + "px\">" + toString(v) + "</span>";
        }

        private static System.Drawing.Color MaximumColor = System.Drawing.ColorTranslator.FromHtml("#87e500");
        private static System.Drawing.Color MinimumColor = System.Drawing.ColorTranslator.FromHtml("#ebc2c2");

        private static System.Drawing.Color MiddleColor(decimal point, bool reverse)
        {
            if (reverse)
                return MiddleColor(MinimumColor, MaximumColor, 1m - point);
            else
                return MiddleColor(MinimumColor, MaximumColor, point);
        }
        private static System.Drawing.Color MiddleColor(decimal point)
        {
            return MiddleColor(MinimumColor, MaximumColor, point);
        }
        private static System.Drawing.Color MiddleColor(System.Drawing.Color c1, System.Drawing.Color c2, decimal point)
        {
            decimal h1, s1, v1;
            decimal h2, s2, v2;
            decimal h3, s3, v3;
            ColorToHSV(c1, out h1, out s1, out v1);
            ColorToHSV(c2, out h2, out s2, out v2);

            h3 = h2 * point + h1 * (1m - point);
            s3 = s2 * point + s1 * (1m - point);
            v3 = v2 * point + v1 * (1m - point);

            return ColorFromHSV(h3, s3, v3);
        }

        private static void ColorToHSV(System.Drawing.Color color, out decimal hue, out decimal saturation, out decimal value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = (decimal)color.GetHue();
            saturation = (max == 0) ? 0 : 1m - (1m * min / max);
            value = max / 255m;
        }

        private static System.Drawing.Color ColorFromHSV(decimal hue, decimal saturation, decimal value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            decimal f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return System.Drawing.Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return System.Drawing.Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return System.Drawing.Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return System.Drawing.Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return System.Drawing.Color.FromArgb(255, t, p, v);
            else
                return System.Drawing.Color.FromArgb(255, v, p, q);
        }

        public static TValue TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue @default) 
        {
            if (source == null)
                return @default;

            TValue res;
            if (!source.TryGetValue(key, out res))
                res = @default;

            return res;
        }

        public static Nullable<TValue> TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key)
            where TValue: struct
        {
            if (source == null)
                return null;

            TValue res;
            if (!source.TryGetValue(key, out res))
                return null;

            return res;
        }
    }
}