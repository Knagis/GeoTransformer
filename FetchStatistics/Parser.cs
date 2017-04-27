/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FetchStatistics
{
    public static class Parser
    {
        private static bool TextAfter(ref string text, string before)
        {
            if (text == null)
                return false;

            var i = text.IndexOf(before, StringComparison.OrdinalIgnoreCase);
            if (i == -1)
                return false;

            i += before.Length;

            // remove the text before the first delimiter
            text = text.Substring(i);

            return true;
        }

        private static string TextBetween(this string text, string before, string after)
        {
            return TextBetween(ref text, before, after);
        }
        private static string TextBetween(ref string text, string before, string after)
        {
            if (text == null)
                return null;

            var i = text.IndexOf(before, StringComparison.OrdinalIgnoreCase);
            if (i == -1)
            {
                System.Diagnostics.Debugger.Launch();
                return null;
            }

            i += before.Length;

            // remove the text before the first delimiter
            text = text.Substring(i);

            if (after == null)
                return text;

            i = text.IndexOf(after, StringComparison.OrdinalIgnoreCase);
            if (i == -1)
                return null;

            // find the actual substring
            var substring = text.Substring(0, i);

            // remove the after part
            i += after.Length;
            text = text.Substring(i);

            return substring;
        }

        private static IEnumerable<HtmlElement> GetCells(this HtmlElement row)
        {
            return row.GetElementsByTagName("td").Cast<HtmlElement>()
                .Union(row.GetElementsByTagName("th").Cast<HtmlElement>());
        }

        public static void ParseMainPage(HtmlDocument doc, StatisticsData data)
        {
            if (doc == null)
                throw new ArgumentNullException("doc");

            if (data == null)
                throw new ArgumentNullException("data");

            data.UpdatedBy = doc.GetElementsByClassName("a", "SignedInProfileLink")
                .First()
                .GetElementsByTagName("span").OfType<HtmlElement>().Skip(2).First().InnerText;

            data.UserName = doc.GetElementById("ctl00_ContentBody_ProfilePanel1_lblMemberName").InnerText;

            try
            {
                var box = doc.GetElementById("ctl00_ContentBody_ProfilePanel1_Panel_CachesFound");
                var t = box.InnerText.Trim();
                t = t.Substring(0, t.IndexOf(' '));
                data.CacheFinds = int.Parse(t, System.Globalization.NumberStyles.Integer | System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            }
            catch { }

            try
            {
                var box = doc.GetElementById("ctl00_ContentBody_ProfilePanel1_Panel_TrackableStats");
                var t = box.InnerText.Trim();
                t = t.Substring(0, t.IndexOf(' '));
                data.TrackablesLogged = int.Parse(t, System.Globalization.NumberStyles.Integer | System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            }
            catch { }

            data.MemberSince = DateTime.ParseExact(doc.GetElementById("ctl00_ContentBody_ProfilePanel1_lblMemberSinceDate").InnerText,
                                    new string[] { "dddd, MMMM dd, yyyy", "dddd, dd MMMM yyyy" }, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.AllowWhiteSpaces);
            data.LastVisit = DateTime.ParseExact(doc.GetElementById("ctl00_ContentBody_ProfilePanel1_lblLastVisitDate").InnerText,
                                    new string[] { "dddd, MMMM dd, yyyy", "dddd, dd MMMM yyyy" }, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.AllowWhiteSpaces);
        }

        public static void ParseStatPage(HtmlDocument doc, StatisticsData data)
        {
            var tables = doc.GetElementById("ctl00_ContentBody_ProfilePanel1_pnlStats").GetElementsByTagName("table");
            var found = tables.Cast<HtmlElement>().First();
            var hidden = tables.Cast<HtmlElement>().Last();
            try
            {
                foreach (var row in hidden.GetElementsByTagName("tr").Cast<HtmlElement>())
                {
                    var cells = row.GetCells().Cast<HtmlElement>();
                    var label = cells.First().InnerText.Trim();
                    var value = cells.Last().InnerText.Trim();
                    if (string.Equals(label, "Total Caches Owned", StringComparison.Ordinal) && !string.IsNullOrWhiteSpace(value))
                        data.CachesHidden = int.Parse(value.Trim(), System.Globalization.NumberStyles.Integer | System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                }
            }
            catch { }
        }

        public static void ParseStatistics(HtmlDocument doc, StatisticsData data)
        {
            if (doc.GetElementById("ctl00_ContentBody_ProfilePanel1_uxStatisticsWarningPanel") != null)
            {
                data.DetailedStatisticsAvailable = false;
                return;
            }

            data.DetailedStatisticsAvailable = true;
            data.CacheFindsByCountry = new Dictionary<string, int>();

            try
            {
                var basicInfo = doc.GetElementById("BasicFinds").InnerHtml;
                TextAfter(ref basicInfo, "</strong>");
                //data.DistinctCacheFinds = int.Parse(TextBetween(ref basicInfo, "(", " "), System.Globalization.CultureInfo.InvariantCulture);
                data.FirstCacheDate = DateTime.ParseExact(TextBetween(ref basicInfo, "<strong>", "</strong>"), new[] { "dd/MMM/yyyy", "yyyy-MM-dd", "MM/dd/yyyy" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
            }
            catch
            {
#if DEBUG
                throw;
#endif
            }

            try
            {
                var simpleStats = doc.GetElementById("ctl00_ContentBody_ProfilePanel1_StatsChronologyControl1_FindStatistics").GetElementsByTagName("dd").Cast<HtmlElement>().GetEnumerator();

                simpleStats.MoveNext();
                data.FindRate = decimal.Parse(TextBetween(simpleStats.Current.InnerHtml, "<strong>", "</strong>"), System.Globalization.CultureInfo.InvariantCulture);

                simpleStats.MoveNext();
                data.LongestStreak = int.Parse(TextBetween(simpleStats.Current.InnerHtml, "<strong>", "</strong>"), System.Globalization.CultureInfo.InvariantCulture);

                simpleStats.MoveNext();
                data.LongestSlump = int.Parse(TextBetween(simpleStats.Current.InnerHtml, "<strong>", "</strong>"), System.Globalization.CultureInfo.InvariantCulture);

                // current streak
                simpleStats.MoveNext();

                // current slump
                simpleStats.MoveNext();

                simpleStats.MoveNext();
                data.BestDay = int.Parse(TextBetween(simpleStats.Current.InnerHtml, "<strong>", "</strong>"), System.Globalization.CultureInfo.InvariantCulture);

                simpleStats.MoveNext();
                data.BestMonth = int.Parse(TextBetween(simpleStats.Current.InnerHtml, "<strong>", "</strong>"), System.Globalization.CultureInfo.InvariantCulture);

                simpleStats.MoveNext();
                data.BestYear = int.Parse(TextBetween(simpleStats.Current.InnerHtml, "<strong>", "</strong>"), System.Globalization.CultureInfo.InvariantCulture);

                simpleStats.Dispose();
            }
            catch
            {
#if DEBUG
                throw;
#endif
            }

            try
            {
                var table = doc.GetElementById("ctl00_ContentBody_ProfilePanel1_StatsChronologyControl1_YearlyBreakdown").GetElementsByTagName("table").Cast<HtmlElement>().First();
                var lastRow = table.GetElementsByTagName("tr").Cast<HtmlElement>().Last();
                var lastCell = lastRow.GetCells().Cast<HtmlElement>().Last();
                data.CurrentYearFindRate = decimal.Parse(TextBetween(lastCell.InnerHtml, "<strong>", "</strong>"), System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
#if DEBUG
                throw;
#endif
            }

            try
            {
                var days = doc.GetElementById("ctl00_ContentBody_ProfilePanel1_StatsChronologyControl1_FindsForEachDayOfYear").GetElementsByTagName("p").Cast<HtmlElement>().First();
                data.DaysInYearCached = int.Parse(TextBetween(days.InnerHtml, "<strong>", "</strong>"), System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
#if DEBUG
                throw;
#endif
            }

            //cache types 
            try
            {
                var typeElem = doc.GetElementsByClassName("ul", "CacheTypeDataList").Single().GetElementsByTagName("li").Cast<HtmlElement>().ToList();
                var countElem = doc.GetElementsByClassName("ul", "CacheTypeCountList").Single().GetElementsByTagName("li").Cast<HtmlElement>().ToList();

                data.CacheFindsByType = new Dictionary<string, int>(typeElem.Count);
                for (var i = 0; i < typeElem.Count; i++)
                {
                    var cntText = countElem[i].InnerText;
                    var cnt = int.Parse(cntText.Substring(0, cntText.IndexOf(' ')), System.Globalization.CultureInfo.InvariantCulture);

                    data.CacheFindsByType[typeElem[i].InnerText.Trim()] = cnt;
                }
            }
            catch
            {
#if DEBUG
                throw;
#endif
            }

            //cache sizes 
            try
            {
                var typeElem = doc.GetElementsByClassName("ul", "ContainerTypeDataList").Single().GetElementsByTagName("li").Cast<HtmlElement>().ToList();
                var countElem = doc.GetElementsByClassName("ul", "ContainerTypeCountList").Single().GetElementsByTagName("li").Cast<HtmlElement>().ToList();

                data.CacheFindsBySize = new Dictionary<string, int>(typeElem.Count);
                for (var i = 0; i < typeElem.Count; i++)
                {
                    var cntText = countElem[i].InnerText;
                    var cnt = int.Parse(cntText.Substring(0, cntText.IndexOf(' ')), System.Globalization.CultureInfo.InvariantCulture);

                    data.CacheFindsBySize[typeElem[i].InnerText.Trim()] = cnt;
                }
            }
            catch
            {
#if DEBUG
                throw;
#endif
            }

            try
            {
                var difElem = doc.GetElementById("ctl00_ContentBody_ProfilePanel1_StatsDifficultyTerrainControl1_uxAverageDifficultyValueLabel");
                data.AverageDifficulty = decimal.Parse(difElem.InnerHtml, System.Globalization.CultureInfo.InvariantCulture);

                var terElem = doc.GetElementById("ctl00_ContentBody_ProfilePanel1_StatsDifficultyTerrainControl1_uxAverageTerrainValueLabel");
                data.AverageTerrain = decimal.Parse(terElem.InnerHtml, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
#if DEBUG
                throw;
#endif
            }

            try
            {
                var table = doc.GetElementById("ctl00_ContentBody_ProfilePanel1_StatsDifficultyTerrainControl1_uxDifficultyTerrainTable");
                data.DifficultyTerrainCombinations = 81;
                var cells = table.GetCells();
                foreach (var cell in cells)
                    if (cell.GetAttribute("className") != null && cell.GetAttribute("className").Contains("stats_cellzero")) data.DifficultyTerrainCombinations--;

                var lastRow = table.GetElementsByTagName("tr").Cast<HtmlElement>().Last();
                var t5 = lastRow.GetCells().Reverse().Skip(1).First();
                data.Terrain5Caches = int.Parse(t5.InnerText, System.Globalization.CultureInfo.InvariantCulture);

                var d5row = table.GetElementsByTagName("tr").Cast<HtmlElement>().Reverse().Skip(1).First();
                var d5 = d5row.GetCells().Last();
                data.Difficulty5Caches = int.Parse(d5.InnerText, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
#if DEBUG
                throw;
#endif
            }

            try
            {
                var disabled = !string.IsNullOrWhiteSpace(doc.GetElementById("ctl00_ContentBody_ProfilePanel1_uxMapsMessagePanel").InnerText);
                if (!disabled)
                {
                    // parse coutry list
                    var table = doc.GetElementById("StatsFlagLists").GetElementsByTagName("table").Cast<HtmlElement>().Last();
                    foreach (var row in table.GetElementsByTagName("tr").Cast<HtmlElement>())
                    {
                        var country = row.GetCells().First().InnerText;
                        var count = int.Parse(row.GetCells().Last().InnerText, System.Globalization.CultureInfo.InvariantCulture);

                        data.CacheFindsByCountry[country] = count;
                    }
                }

            }
            catch
            {
#if DEBUG
                throw;
#endif
            }
        }
    }
}
