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
    /// <summary>
    /// Class that handles automatic updating of the application.
    /// </summary>
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
                    Directory.Delete("PendingUpdate", true);
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
        /// Backs up the GeoTransformer.data file.
        /// </summary>
        private static void BackupDataFile()
        {
            if (File.Exists(@"..\GeoTransformer.data"))
            {
                Directory.CreateDirectory(@"..\Backup");
                File.Copy(@"..\GeoTransformer.data", @"..\Backup\GeoTransformer.data." + DateTime.Now.ToString("yyyyMMddHHmmss"), true);
            }

            if (File.Exists(@"..\GeoTransformer.data.backup"))
            {
                Directory.CreateDirectory(@"..\Backup");
                File.Move(@"..\GeoTransformer.data.backup", @"..\Backup\GeoTransformer.data.backup." + DateTime.Now.ToString("yyyyMMddHHmmss"));
            }
        }

        /// <summary>
        /// Installs the update if any is available and already downloaded.
        /// </summary>
        private static void InstallUpdate()
        {
            try
            {
                BackupDataFile();

                var fz = new ICSharpCode.SharpZipLib.Zip.FastZip();

                if (System.IO.File.Exists("Update.zip"))
                    fz.ExtractZip("Update.zip", @"..\", ICSharpCode.SharpZipLib.Zip.FastZip.Overwrite.Always, null, null, null, false);

                foreach (var x in System.IO.Directory.EnumerateFiles(".", "Update.*.zip"))
                {
                    var ass = System.IO.Path.GetFileNameWithoutExtension(x).Substring(7);
                    var path = System.IO.Path.Combine("..", "Extensions", ass);
                    System.IO.Directory.Delete(path, true);
                    System.IO.Directory.CreateDirectory(path);
                    fz.ExtractZip(x, path, ICSharpCode.SharpZipLib.Zip.FastZip.Overwrite.Always, null, null, null, false);
                }

                // remove csExWB.dll and related files as they are only needed up to version 3.1
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

                    try
                    {
                        Directory.Delete("PendingUpdate", true);
                    }
                    catch (IOException)
                    {
                    }

                    var extensionsNeedUpdate = DownloadExtensionUpdates(wc);
                    bool applicationNeedsUpdate = false;

                    var parts = wc.DownloadString("http://knagis.miga.lv/GeoTransformer/version.txt").Split('|');
                    var newver = Version.Parse(parts[0]);
                    var link = parts[1];
                    var updater = parts[2];
                    if (force || newver.CompareTo(current) == 1)
                    {
                        Directory.CreateDirectory("PendingUpdate");
                        File.Delete(@"PendingUpdate\Update.Complete");
                        File.Delete(@"PendingUpdate\Update.zip");

                        wc.DownloadFile(link, @"PendingUpdate\Update.temp");
                        File.Move(@"PendingUpdate\Update.temp", @"PendingUpdate\Update.zip");

                        applicationNeedsUpdate = true;
                    }

                    if (applicationNeedsUpdate || extensionsNeedUpdate)
                    {
                        wc.DownloadFile(updater, @"PendingUpdate\Updater.zip");
                        new ICSharpCode.SharpZipLib.Zip.FastZip()
                            .ExtractZip(@"PendingUpdate\Updater.zip",
                                        @"PendingUpdate\",
                                        ICSharpCode.SharpZipLib.Zip.FastZip.Overwrite.Always, null, null, null, false);
                        System.IO.File.Delete(@"PendingUpdate\Updater.zip");

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

        /// <summary>
        /// Downloads any new extensions.
        /// </summary>
        /// <param name="wc">The webclient used to download data.</param>
        private static bool DownloadExtensionUpdates(System.Net.WebClient wc)
        {
            var fresh = Extensions.ExtensionManager.ExtensionManager.Extensions.ToDictionary(o => o.AssemblyName);
            var needsUpdate = Extensions.ExtensionLoader.Extensions
                    .Select(o => o.GetType().Assembly.GetName())
                    .Distinct()
                    .Select(o => new { o.Version, o.Name, Data = fresh.ContainsKey(o.Name) ? fresh[o.Name] : null })
                    .Where(o => o.Data != null && o.Version < o.Data.Version);

            int i = 0;
            foreach (var ext in needsUpdate)
            {
                if (i == 0)
                    System.IO.Directory.CreateDirectory(@"PendingUpdate");

                wc.DownloadFile(ext.Data.DownloadUri, @"PendingUpdate\Update.temp");
                File.Move(@"PendingUpdate\Update.temp", @"PendingUpdate\Update." + ext.Data.AssemblyName + @".zip");
                i++;
            }

            return i > 0;
        }
    }
}
