/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer
{
    static class ReleaseNotes
    {
        public static Dictionary<Version, string> Notes = new Dictionary<Version, string>()
        {
            { Version.Parse("4.1.0.0"), "Added an option to put geocache attributes in a log entry so it can be read on GPS." + Environment.NewLine +
                                        "Added an option to select which caches should be ignored when publishing." + Environment.NewLine +
                                        "Added support for loading GPX 1.1 files." + Environment.NewLine + 
                                        "Partial support for geocaching.com GPX 1.0.2 extensions (still under development by Groundspeak)." + Environment.NewLine +
                                        "Virtual caches are now displayed on the map with the correct symbol." },
            { Version.Parse("4.0.0.3"), "Fixed the 'Not available' problem in Pocket Query download." + Environment.NewLine +
                                        "Cached PQ downloads are now removed if unused (freeing up space)." },
            { Version.Parse("4.0.0.0"), "Created a welcome screen that is shown to the users." + Environment.NewLine +
                                        "Added support for geocaching.com Live API." + Environment.NewLine +
                                        "Removing customizations now work correctly (no need to press the button twice)." + Environment.NewLine +
                                        "Added status column to the table views." + Environment.NewLine +
                                        "Added waypoint symbol and description enhancer." + Environment.NewLine +
                                        "Bing maps displays tooltips and shows disabled caches with gray icons." + Environment.NewLine +
                                        "Configuration tab is now divided in categories." + Environment.NewLine + 
                                        "User can change the folder that contains locally downloaded GPX files." + Environment.NewLine +
                                        "Moving GeoTransformer folder around no longer crashes the application." },
            { Version.Parse("3.1.0.0"), "When the application crashes, the error message is written to a file to make the issue reporting easier." + Environment.NewLine +
                                        "Tooltips containing description for configuration options now stay open for 30 seconds." + Environment.NewLine + 
                                        "Bing Translator extension changed to make use of the new authentication model."},
            { Version.Parse("3.0.0.0"), "The application has been rewritten to support third-party extensions." + Environment.NewLine +
                                        "Added Bing maps view, option to view the cache details online without opening browser."},
            { Version.Parse("2.4.0.0"), "Added option to prefix disabled cache names." },
            { Version.Parse("2.3.0.0"), "Automatic download of pocket queries from geocaching.com." + Environment.NewLine +
                                        "Release notes are shown after automatic update." + Environment.NewLine +
                                        "Correct behavior when clicking Save directly after editing data." + Environment.NewLine +
                                        "Deletion of rows is much more straight-forward." }
        };

        private static void UpdateLastVersionShown()
        {
            var q = Program.Database.Settings.Update();
            q.Value(o => o.LastVersionShown, typeof(ReleaseNotes).Assembly.GetName().Version.ToString());
            var rows = q.Execute();

            if (rows == 0)
            {
                var qi = Program.Database.Settings.Insert();
                qi.Value(o => o.LastVersionShown, typeof(ReleaseNotes).Assembly.GetName().Version.ToString());
                qi.Execute();
            }
        }

        public static void ShowReleaseNotes()
        {
            var q = Program.Database.Settings.Select();
            q.Select(o => o.LastVersionShown);
            
            // this is checked so that the release notes are not shown if the application was not updated but it is a clean install.
            if (q.ExecuteScalar(o => o.RowId) == 0)
            {
                UpdateLastVersionShown();
                return;
            }

            var curVer = typeof(ReleaseNotes).Assembly.GetName().Version;
            var lastVer = Version.Parse(q.ExecuteScalar(o => o.LastVersionShown) ?? "2.2.2.0");

            bool any = false;
            var sb = new StringBuilder();
            sb.AppendLine("A new version (" + curVer + ") of GeoTransformer has been installed.");
            foreach (var x in Notes.Where(o => o.Key > lastVer).OrderByDescending(o => o.Key))
            {
                any = true;
                sb.AppendLine();
                sb.AppendLine("Changes new to version " + x.Key);
                foreach (var l in x.Value.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {
                    sb.Append("  * ");
                    sb.AppendLine(l);
                }
            }

            if (any)
                System.Windows.Forms.MessageBox.Show(sb.ToString(), "Release notes", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);

            if (curVer != lastVer)
                UpdateLastVersionShown();
        }
    }
}
