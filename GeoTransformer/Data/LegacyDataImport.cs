/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace GeoTransformer.Data
{
    static class LegacyDataImport
    {
        private static void ConvertMovedCaches()
        {
            var newTable = Extensions.ExtensionLoader.RetrieveExtensions<Extensions.ExtensionEditorDataTable>().Single();

            var q = Program.Database.MovedCaches.Select();
            q.SelectAll();

            foreach (var x in q.Execute())
            {
                var code = x.Value(o => o.GeocacheCode);

                var sol = x.Value(o => o.Solved);
                if (sol)
                {
                    var xml = new System.Xml.Linq.XElement(XmlExtensions.GeoTransformerSchema + typeof(Transformers.MarkSolved.MarkSolved).FullName, bool.TrueString);

                    var iq = newTable.Insert();
                    iq.Value(o => o.CacheCode, code);
                    iq.Value(o => o.Data, xml.ToString());
                    iq.Execute();
                }

                var cx = x.Value(o => o.CoordinateX);
                var cy = x.Value(o => o.CoordinateY);
                if (cx.HasValue && cy.HasValue)
                {
                    Coordinates.Wgs84Point p = default(Coordinates.Wgs84Point);
                    bool err = false;
                    try
                    {
                        // perform re-parse so that the precision matches
                        p = new Coordinates.Wgs84Point((decimal)cy, (decimal)cx);
                        p = new Coordinates.Wgs84Point(p.ToString());
                    }
                    catch
                    {
                        err = true;
                    }

                    if (!err)
                    {
                        var xml = new System.Xml.Linq.XElement(XmlExtensions.GeoTransformerSchema + typeof(Transformers.UpdateCoordinates.UpdateCoordinates).FullName,
                                                                new System.Xml.Linq.XAttribute("latitude", p.Latitude),
                                                                new System.Xml.Linq.XAttribute("longitude", p.Longitude));

                        var iq = newTable.Insert();
                        iq.Value(o => o.CacheCode, code);
                        iq.Value(o => o.Data, xml.ToString());
                        iq.Execute();
                    }
                }


                var uq = Program.Database.MovedCaches.Delete();
                uq.Where(o => o.GeocacheCode, code);
                uq.Execute();
            }
        }

        /// <summary>
        /// Converts the extension configuration from the values stored in Settings table up to version 2.4. Also moves extension specific files to their new location.
        /// </summary>
        public static void ConvertExtensionConfiguration()
        {
            ConvertMovedCaches();

            var appDir = new System.IO.FileInfo(Application.ExecutablePath).DirectoryName;
            try
            {
                if (System.IO.File.Exists(System.IO.Path.Combine(appDir, "Translator.data")))
                {
                    string newPath = System.IO.Path.Combine(appDir, "ExtensionData", typeof(Transformers.Translator.Translator).Namespace);
                    System.IO.Directory.CreateDirectory(newPath);
                    System.IO.File.Move(System.IO.Path.Combine(appDir, "Translator.data"),
                                        System.IO.Path.Combine(newPath, "Translator.data"));
                }
            }
            catch (System.IO.IOException) { }
            try
            {
                if (System.IO.File.Exists(System.IO.Path.Combine(appDir, "GpxTautai.zip.cache")))
                {
                    string newPath = System.IO.Path.Combine(appDir, "ExtensionData", typeof(Transformers.LoadGpxTautai.LoadGpxTautai).Namespace);
                    System.IO.Directory.CreateDirectory(newPath);
                    System.IO.File.Move(System.IO.Path.Combine(appDir, "GpxTautai.zip.cache"),
                                        System.IO.Path.Combine(newPath, "GpxTautai.zip.cache"));
                    System.IO.File.Move(System.IO.Path.Combine(appDir, "GpxTautai.zip.cache.key"),
                                        System.IO.Path.Combine(newPath, "GpxTautai.zip.cache.key"));
                }
            }
            catch (System.IO.IOException) { }
            try
            {
                var files = System.IO.Directory.GetFiles(appDir, "PocketQuery-*", System.IO.SearchOption.TopDirectoryOnly);
                string newPath = System.IO.Path.Combine(appDir, "ExtensionData", typeof(Transformers.PocketQueryDownload.PocketQueryDownload).Namespace);
                System.IO.Directory.CreateDirectory(newPath);
                foreach (var oldFile in files)
                {
                    System.IO.File.Move(oldFile, System.IO.Path.Combine(newPath, System.IO.Path.GetFileName(oldFile)));
                }
            }
            catch (System.IO.IOException) { }

            var extensionConfigurationTable = Extensions.ExtensionLoader.Extensions.OfType<Extensions.ExtensionConfigurationTable>().Single();

            var EnableFileMerge = Program.Database.Settings.GetColumn("EnableFileMerge");
            var DisableHtmlEntityDecode = Program.Database.Settings.GetColumn("DisableHtmlEntityDecode");
            var DisableDuplicateRemoval = Program.Database.Settings.GetColumn("DisableDuplicateRemoval");
            var TranslatorEnabled = Program.Database.Settings.GetColumn("TranslatorEnabled");
            var TranslatorDisableHints = Program.Database.Settings.GetColumn("TranslatorDisableHints");
            var TranslatorDisableDescriptions = Program.Database.Settings.GetColumn("TranslatorDisableDescriptions");
            var TranslatorEnableLogs = Program.Database.Settings.GetColumn("TranslatorEnableLogs");
            var TranslatorTargetLanguage = Program.Database.Settings.GetColumn("TranslatorTargetLanguage");
            var TranslatorIgnoreLanguages = Program.Database.Settings.GetColumn("TranslatorIgnoreLanguages");
            var TranslatorLanguageNameCache = Program.Database.Settings.GetColumn("TranslatorLanguageNameCache");
            var DisableDisabledCachePrefix = Program.Database.Settings.GetColumn("DisableDisabledCachePrefix");
            var DisabledCachePrefix = Program.Database.Settings.GetColumn("DisabledCachePrefix");
            var GpxTautai = Program.Database.Settings.GetColumn("GpxTautai");
            var PocketQueryDownload = Program.Database.Settings.GetColumn("PocketQueryDownload");

            var legacyQuery = Program.Database.Settings.Select();
            legacyQuery.Select(o => EnableFileMerge);
            legacyQuery.Select(o => DisableHtmlEntityDecode);
            legacyQuery.Select(o => DisableDuplicateRemoval);
            legacyQuery.Select(o => TranslatorEnabled);
            legacyQuery.Select(o => TranslatorDisableHints);
            legacyQuery.Select(o => TranslatorDisableDescriptions);
            legacyQuery.Select(o => TranslatorEnableLogs);
            legacyQuery.Select(o => TranslatorTargetLanguage);
            legacyQuery.Select(o => TranslatorIgnoreLanguages);
            legacyQuery.Select(o => TranslatorLanguageNameCache);
            legacyQuery.Select(o => DisableDisabledCachePrefix);
            legacyQuery.Select(o => DisabledCachePrefix);
            legacyQuery.Select(o => GpxTautai);
            legacyQuery.Select(o => PocketQueryDownload);

            var legacyUpdate = Program.Database.Settings.Update();
            foreach (var result in legacyQuery.Execute())
            {
                if (result.Value(o => PocketQueryDownload) != null)
                {
                    var update = extensionConfigurationTable.Replace();
                    update.Value(o => o.ClassName, typeof(Transformers.PocketQueryDownload.PocketQueryDownload).FullName);
                    update.Value(o => o.Configuration, result.Value<byte[]>(o => PocketQueryDownload));
                    update.Execute();

                    legacyUpdate.Value(o => PocketQueryDownload, null);
                }

                if (result.Value(o => GpxTautai) != null)
                {
                    var update = extensionConfigurationTable.Replace();
                    update.Value(o => o.ClassName, typeof(Transformers.LoadGpxTautai.LoadGpxTautai).FullName);
                    update.Value(o => o.Configuration, result.Value<byte[]>(o => GpxTautai));
                    update.Execute();

                    legacyUpdate.Value(o => GpxTautai, null);
                }

                if (result.Value(o => EnableFileMerge) != null)
                {
                    var update = extensionConfigurationTable.Replace();
                    update.Value(o => o.ClassName, typeof(Transformers.FileMerger.FileMerger).FullName);
                    update.Value(o => o.Configuration, new byte[] { result.Value<bool>(o => EnableFileMerge) ? (byte)1 : (byte)0 });
                    update.Execute();

                    legacyUpdate.Value(o => EnableFileMerge, null);
                }

                if (result.Value(o => DisableHtmlEntityDecode) != null)
                {
                    var update = extensionConfigurationTable.Replace();
                    update.Value(o => o.ClassName, typeof(Transformers.HtmlEntityDecoder.HtmlEntityDecoder).FullName);
                    update.Value(o => o.Configuration, new byte[] { result.Value<bool>(o => DisableHtmlEntityDecode) ? (byte)0 : (byte)1 });
                    update.Execute();

                    legacyUpdate.Value(o => DisableHtmlEntityDecode, null);
                }

                if (result.Value(o => DisableDuplicateRemoval) != null)
                {
                    var update = extensionConfigurationTable.Replace();
                    update.Value(o => o.ClassName, typeof(Transformers.RemoveDuplicates.RemoveDuplicates).FullName);
                    update.Value(o => o.Configuration, new byte[] { result.Value<bool>(o => DisableDuplicateRemoval) ? (byte)0 : (byte)1 });
                    update.Execute();

                    legacyUpdate.Value(o => DisableDuplicateRemoval, null);
                }

                if (result.Value(o => DisableDisabledCachePrefix) != null)
                {
                    var update = extensionConfigurationTable.Replace();
                    update.Value(o => o.ClassName, typeof(Transformers.PrefixDisabled.PrefixDisabled).FullName);
                    var text = result.Value<string>(o => DisabledCachePrefix);
                    var b = new byte[1 + System.Text.Encoding.UTF8.GetByteCount(text)];
                    System.Text.Encoding.UTF8.GetBytes(text, 0, text.Length, b, 1);
                    b[0] = result.Value<bool>(o => DisableDisabledCachePrefix) ? (byte)0 : (byte)1;
                    update.Value(o => o.Configuration, b);
                    update.Execute();

                    legacyUpdate.Value(o => DisableDisabledCachePrefix, null);
                    legacyUpdate.Value(o => DisabledCachePrefix, null);
                }

                if (result.Value(o => TranslatorEnabled) != null)
                {
                    var update = extensionConfigurationTable.Replace();
                    update.Value(o => o.ClassName, typeof(Transformers.Translator.Translator).FullName);
                    update.Value(o => o.Configuration, Transformers.Translator.ConfigurationData.ConvertLegacyConfiguration(
                            result.Value<string>(o => TranslatorLanguageNameCache),
                            result.Value<string>(o => TranslatorTargetLanguage),
                            result.Value<string>(o => TranslatorIgnoreLanguages),
                            result.Value<bool>(o => TranslatorEnabled),
                            !result.Value<bool>(o => TranslatorDisableDescriptions),
                            !result.Value<bool>(o => TranslatorDisableHints),
                            result.Value<bool>(o => TranslatorEnableLogs)
                        ));
                    update.Execute();

                    legacyUpdate.Value(o => TranslatorEnabled, null);
                    legacyUpdate.Value(o => TranslatorDisableHints, null);
                    legacyUpdate.Value(o => TranslatorDisableDescriptions, null);
                    legacyUpdate.Value(o => TranslatorEnableLogs, null);
                    legacyUpdate.Value(o => TranslatorTargetLanguage, null);
                    legacyUpdate.Value(o => TranslatorIgnoreLanguages, null);
                    legacyUpdate.Value(o => TranslatorLanguageNameCache, null);
                }
            }

            legacyUpdate.Execute();
        }
    }
}
