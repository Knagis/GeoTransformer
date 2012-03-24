/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace GeoTransformer
{
    internal static class AutoUpdater
    {
        /// <summary>
        /// Performs the update process.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns><c>True</c> if the application has to stop running.</returns>
        public static bool PerformUpdate(string[] args)
        {
            if (args.Contains("update"))
            {
                var parentExe = Path.GetFullPath(@"..\GeoTransformer.exe");
                if (!File.Exists(parentExe))
                {
                    MessageBox.Show("Autoupdate process started incorrectly.");
                    return true;
                }

                // get any process that is running the application and wait for them to exit.
                while (true)
                {
                    try
                    {
                        var processes = System.Diagnostics.Process.GetProcessesByName("GeoTransformer").Where(o => string.Equals(parentExe, o.MainModule.FileName, StringComparison.OrdinalIgnoreCase));
                        if (processes.Count() == 0)
                            break;

                        foreach (var pr in processes)
                        {
                            pr.WaitForExit();
                        }
                    }
                    catch { }
                }

                InstallUpdate();
                return true;
            }

            if (args.Contains("cleanupdate"))
            {
                var updateExe = Path.GetFullPath(@"PendingUpdate\GeoTransformer.exe");

                // get any process that is running the application and wait for them to exit.
                while (true)
                {
                    try
                    {
                        var processes = System.Diagnostics.Process.GetProcessesByName("GeoTransformer").Where(o => string.Equals(updateExe, o.MainModule.FileName, StringComparison.OrdinalIgnoreCase));
                        if (processes.Count() == 0)
                            break;

                        foreach (var pr in processes)
                        {
                            pr.WaitForExit();
                        }
                    }
                    catch { }
                }

                try
                {
                    foreach (var f in Directory.EnumerateFiles("PendingUpdate"))
                        File.Delete(f);
                    Directory.Delete("PendingUpdate");
                }
                catch
                {
                }

                return true;
            }

            StartUpdateDownload(args.Contains("forceupdate"));

            return false;
        }

        /// <summary>
        /// Installs the update if any is available and already downloaded.
        /// </summary>
        private static void InstallUpdate()
        {
            try
            {
                if (File.Exists(@"..\GeoTransformer.data"))
                    File.Copy(@"..\GeoTransformer.data", @"..\GeoTransformer.data.backup", true);

                var fz = new ICSharpCode.SharpZipLib.Zip.FastZip();
                fz.ExtractZip("Update.zip", @"..\", ICSharpCode.SharpZipLib.Zip.FastZip.Overwrite.Always, null, null, null, false);

                // remove csExWB.dll and related files as it is only needed up to version 3.1
                if (File.Exists(@"..\csExWB.dll"))
                {
                    File.Delete(@"..\csExWB.dll");
                    File.Delete(@"..\GeoTransformer.exe.manifest");
                    File.Delete(@"..\ComUtilities.dll");
                    File.Delete(@"..\Interop.ComUtilitiesLib.dll");
                }

                File.WriteAllText("Update.Complete", string.Empty);

                var psi = new ProcessStartInfo(@"GeoTransformer.exe", "cleanupdate");
                psi.WorkingDirectory = @"..\";
                Process.Start(psi);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Starts the update download in a separate thread.
        /// </summary>
        /// <param name="force">if set to <c>true</c> forces the installation of the newest version available even if the current is newer.</param>
        private static void StartUpdateDownload(bool force)
        {
            var thread = new System.Threading.Thread(DownloadUpdateInner);
            thread.Name = "AutoUpdater";
            thread.Start(force);
        }

        /// <summary>
        /// Downloads the update if any is available.
        /// </summary>
        private static void DownloadUpdateInner(object state)
        {
            bool force = (bool)state;
            try
            {
                var current = System.Reflection.Assembly.GetAssembly(typeof(Program)).GetName().Version;
                using (var wc = new System.Net.WebClient())
                {
                    wc.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                    var parts = wc.DownloadString("http://knagis.miga.lv/GeoTransformer/version.txt").Split('|');
                    var newver = Version.Parse(parts[0]);
                    var link = parts[1];
                    var updater = parts[2];

                    if (force || newver.CompareTo(current) == 1)
                    {
                        Directory.CreateDirectory("PendingUpdate");
                        File.Delete(@"PendingUpdate\Update.Complete");
                        File.Delete(@"PendingUpdate\Update.zip");

                        wc.DownloadFile(updater, @"PendingUpdate\Updater.zip");
                        new ICSharpCode.SharpZipLib.Zip.FastZip()
                            .ExtractZip(@"PendingUpdate\Updater.zip",
                                        @"PendingUpdate\", 
                                        ICSharpCode.SharpZipLib.Zip.FastZip.Overwrite.Always, null, null, null, false);
                        System.IO.File.Delete(@"PendingUpdate\Updater.zip");

                        ////The old style updater
                        //File.Copy("GeoTransformer.exe", @"PendingUpdate\GeoTransformer.exe", true);
                        //File.Copy("ICSharpCode.SharpZipLib.dll", @"PendingUpdate\ICSharpCode.SharpZipLib.dll", true);
                        //File.Copy("GeoTransformer.Extensibility.dll", @"PendingUpdate\GeoTransformer.Extensibility.dll", true);

                        wc.DownloadFile(link, @"PendingUpdate\Update.temp");
                        File.Move(@"PendingUpdate\Update.temp", @"PendingUpdate\Update.zip");

                        var psi = new ProcessStartInfo(@"GeoTransformer.exe", "update");
                        psi.WorkingDirectory = @"PendingUpdate\";
                        Process.Start(psi);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
