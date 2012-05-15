/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO.Compression;

namespace FetchStatistics.Web
{
    public partial class Default : System.Web.UI.Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Default"/> class.
        /// </summary>
        public Default()
        {
            if (_propertyCache == null)
                _propertyCache = typeof(StatisticsData)
                                    .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                                    .ToDictionary(o => o.Name, o => o);

            if (_uiSettingsCache == null)
                _uiSettingsCache = typeof(StatisticsData)
                                    .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                                    .ToDictionary(o => o.Name, o => o.GetCustomAttributes(typeof(UISettingsAttribute), true).Cast<UISettingsAttribute>().DefaultIfEmpty(new UISettingsAttribute(o.Name)).First());
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // apply manual compression since IIS 7.0 module requires server level configuration to
            // enable compression when the page size is over 256Kb.
            string compression = HttpContext.Current.Request.Headers["Accept-Encoding"];

            if (string.IsNullOrEmpty(compression))
            {
                return;
            }
            else if (compression.Contains("gzip"))
            {
                Response.Filter = new GZipStream(Response.Filter, CompressionMode.Compress);
                Response.Headers["Content-Encoding"] = "gzip";
            }
            else if (compression.Contains("deflate"))
            {
                Response.Filter = new DeflateStream(Response.Filter, CompressionMode.Compress);
                Response.Headers["Content-Encoding"] = "deflate";
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            StatisticsData.CurrentCountryFunction = () => this.Request["country"];

            var data = this.RetrieveData();
            this.Repeater.DataSource = data;
            this.RepeaterLeft.DataSource = data;
            
            this.DataBind();
        }

        private static Dictionary<string, System.Reflection.PropertyInfo> _propertyCache;
        public static Dictionary<string, UISettingsAttribute> _uiSettingsCache;
        public static Dictionary<string, Delegate> _keySelectorCache = new Dictionary<string, Delegate>();
        private IEnumerable<StatisticsData> RetrieveData()
        {
            IEnumerable<StatisticsData> full = new Service().RetrieveData();

            var sort = this.Request.QueryString["sort"] ?? "CacheFinds";
            var rev = this.Request.QueryString["rev"] == "1";
            try
            {
                Delegate keySelector;
                if (!_keySelectorCache.TryGetValue(sort, out keySelector))
                {
                    var p = System.Linq.Expressions.Expression.Parameter(typeof(StatisticsData));
                    var expr = System.Linq.Expressions.Expression.Property(p, sort);
                    var lambda = System.Linq.Expressions.Expression.Lambda(expr, p);
                    keySelector = lambda.Compile();
                    _keySelectorCache[sort] = keySelector;
                }

                var attr = _uiSettingsCache[sort];
                if (attr.OrderByDescendingFirst) rev = !rev;
                
                var type = typeof(QueryableCaller<>).MakeGenericType(_propertyCache[sort].PropertyType);
                full = (IEnumerable<StatisticsData>)type.InvokeMember(rev ? "OrderByDescending" : "OrderBy", System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, null, new object[] { full, keySelector });
            }
            catch
            {
                this.Response.Redirect("~/");
            }

            return full;
        }

        /// <summary>
        /// Returns a string that represents the HTML of the header for the specified column.
        /// </summary>
        /// <returns>The HTML of the header cell contents</returns>
        protected string CreateHeader<TValue>(System.Linq.Expressions.Expression<Func<StatisticsData, TValue>> member)
        {
            var me = (System.Linq.Expressions.MemberExpression)member.Body;

            var attr = _uiSettingsCache[me.Member.Name];

            var current = this.Request["country"] ?? "Latvia";
            var sort = this.Request.QueryString["sort"] ?? "CacheFinds";
            var rev = this.Request.QueryString["rev"] == "1";
            var desc = attr.OrderByDescendingFirst ? !rev : rev;

            var sb = new System.Text.StringBuilder();
            sb.Append("<th");
            if (sort == me.Member.Name)
                sb.Append(" class=\"" + (desc ? "sort-desc" : "sort-asc") + "\"");
            sb.Append(">");                
            sb.Append("<a onclick=\"sortTable(this, ");
            sb.Append((typeof(TValue) == typeof(int) || typeof(TValue) == typeof(int?) || typeof(TValue) == typeof(decimal) || typeof(TValue) == typeof(decimal?)) ? "true" : "false");
            sb.Append(", ");
            sb.Append(attr.OrderByDescendingFirst ? "true" : "false");
            sb.Append(");return false;\" href=\"");
            sb.Append(this.ResolveClientUrl("~/"));
            bool first = true;
            if (me.Member.Name != "CacheFinds" || (!rev && sort == me.Member.Name))
            {
                first = false;
                sb.Append("?sort=");
                sb.Append(me.Member.Name);
                if (!rev && sort == me.Member.Name)
                    sb.Append("&rev=1");
            }
            if (current != "Latvia")
            {
                sb.Append(first ? "?" : "&");
                sb.Append("country=");
                sb.Append(current);
            }
            sb.Append("\">");
            sb.Append(attr.HeaderText);
            sb.Append("</a>");

            if (me.Member.Name == "CacheFindsInCountry")
            {
                var source = (IEnumerable<StatisticsData>)this.Repeater.DataSource;
                var countries = source.Where(o => o.CacheFindsByCountry != null).SelectMany(o => o.CacheFindsByCountry.Keys).Distinct().OrderBy(o => o);
                sb.Append("<select onchange=\"changeCountry(this)\"");
                foreach (var c in countries)
                {
                    sb.Append("<option");
                    if (c == current)
                        sb.Append(" selected");
                    sb.Append(">");
                    sb.Append(c);
                }
                sb.Append("</select>");
            }

            sb.Append("</th>");

            return sb.ToString();
        }

        private static class QueryableCaller<TValue>
        {
            public static IEnumerable<StatisticsData> OrderBy(IEnumerable<StatisticsData> source, Func<StatisticsData, TValue> keySelector)
            {
                return source.OrderBy(keySelector);
            }
            public static IEnumerable<StatisticsData> OrderByDescending(IEnumerable<StatisticsData> source, Func<StatisticsData, TValue> keySelector)
            {
                return source.OrderByDescending(keySelector);
            }
        }
    }
}