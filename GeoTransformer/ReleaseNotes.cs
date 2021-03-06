﻿/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer
{
    /// <summary>
    /// Class contains all release notes since version 2.3 and method to display them to the user.
    /// </summary>
    internal static class ReleaseNotes
    {
        /// <summary>
        /// Dictionary containing all release notes for each version.
        /// </summary>
        private static Dictionary<Version, string> Notes = new Dictionary<Version, string>()
        {
            { Version.Parse("4.6.2.0"), "Fixed Live API integration." }, 
            { Version.Parse("4.6.1.0"), "Waypoint enhancement now recognizes the new geocaching.com waypoint types and uses more symbols on GPS." }, 
            { Version.Parse("4.6.0.0"), "'Loading edited caches from Live API' now works much faster." + Environment.NewLine + 
                                        "Personal notes are now added as a log entry ('Refresh from Live API' must be enabled for this to work)." + Environment.NewLine +
                                        "Bugfix for refreshing found caches from Live API." + Environment.NewLine +
                                        "Table view now shows if the cache has been found." + Environment.NewLine +
                                        "Fixed Wherigo and Mega-Event cache icons." },
            { Version.Parse("4.5.0.0"), "Extensions can now be installed and uninstalled from the application." + Environment.NewLine + 
                                        "Added ability to search for caches by pressing CTRL+F in the table views." + Environment.NewLine +
                                        "Added ability to remove edited customizations for multiple caches at once (use SHIFT or CTRL to select multiple lines in the table)." },
            { Version.Parse("4.4.0.0"), "It is now possible to download pocket query data with favorite points, images etc." + Environment.NewLine +
                                        "Added transformer that adds [DNF] in the cache title when the last logs are negative." + Environment.NewLine +
                                        "Updated coordinates automatically show up in the maps view." + Environment.NewLine +
                                        "Added GeoHack tool quick view for individual cache listing." + Environment.NewLine +
                                        "Most recent publisher (e.g. a Garmin device) show up in the bottom toolbar by themselves." },
            { Version.Parse("4.3.0.0"), "Transformation status window now displays progress indicator for long running tasks." + Environment.NewLine +
                                        "Transformation status window displays warnings and errors separate from other messages." + Environment.NewLine + 
                                        "Pocket query download now warns if the query is not available or is too old." + Environment.NewLine +
                                        "Pocket query download uses local copies when network connection is not available." + Environment.NewLine +
                                        "Pocket query download no longer remove fresh copies of pocket queries that are unselected." + Environment.NewLine +
                                        "User can now choose to ignore even fatal errors in the transformation process." + Environment.NewLine +
                                        "Refresh Images will no longer duplicate images if it is used together with Refresh Data." + Environment.NewLine +
                                        "Previously downloaded geocache images will be removed from disk one month after last use." } ,
            { Version.Parse("4.2.1.0"), "Fixed Bing Translator extension according to the Bing service changes." } ,
            { Version.Parse("4.2.0.2"), "Map view now automatically picks up changes made to a cache." + Environment.NewLine +
                                        "Added ability to publish images to Garmin GPS devices (configurable)." + Environment.NewLine +
                                        "Added an option to load geocache images from Live API (works for basic members)." + Environment.NewLine +
                                        "Added an option to load all advanced data (such as favorite points and images) from Live API." + Environment.NewLine +
                                        "Fixed some bugs related to removing edited data." + Environment.NewLine +
                                        "Added ability to import XML files with invalid characters in them." + Environment.NewLine +
                                        "Edited geocaches that are not currently loaded are refreshed every week." },
            { Version.Parse("4.1.0.0"), "Added an option to put geocache attributes in a log entry so it can be read on GPS." + Environment.NewLine +
                                        "Added an option to select which caches should be ignored when publishing." + Environment.NewLine +
                                        "Added support for loading GPX 1.1 files." + Environment.NewLine + 
                                        "Partial support for geocaching.com GPX 1.0.2 extensions (still under development by Groundspeak)." + Environment.NewLine +
                                        "Virtual caches are now displayed on the map with the correct symbol." + Environment.NewLine +
                                        "Geocache list tables now remember sort, column widths and hidden columns." + Environment.NewLine +
                                        "List view / editor size proportion is now saved between restarts." },
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

        /// <summary>
        /// Updates the database by storing the last version for which the release notes were shown to the user.
        /// </summary>
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

        /// <summary>
        /// Checks if there are any release notes that need to be shown to the user and displays them.
        /// </summary>
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
