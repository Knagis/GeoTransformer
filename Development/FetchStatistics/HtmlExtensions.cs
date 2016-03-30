using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FetchStatistics
{
    internal static class HtmlExtensions
    {
        public static IEnumerable<HtmlElement> GetElementsByClassName(this HtmlDocument doc, string tagName, string className)
        {
            return doc.GetElementsByTagName(tagName)
                .OfType<HtmlElement>()
                .Where(o => string.Equals(o.GetAttribute("className"), className, StringComparison.Ordinal));
        }
    }

}
