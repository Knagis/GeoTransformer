/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Transformers.Translator
{
    internal sealed class ConfigurationData
    {
        public static byte[] ConvertLegacyConfiguration(string languageNameCache, string targetLanguage, string ignoreLanguages, bool enabled, bool descriptions, bool hints, bool logs)
        {
            var obj = new ConfigurationData();

            var lc = ((languageNameCache) ?? string.Empty).Split(new string[] { ";;;" }, StringSplitOptions.RemoveEmptyEntries);
            if (lc.Length == 2)
            {
                obj.LanguageCodes = lc[0].Split(';');
                obj.LanguageNames = lc[1].Split(';');
                obj.TargetLanguage = (!obj.LanguageCodes.Contains(targetLanguage)) ? "en" : targetLanguage;
                obj.IgnoreLanguages = ignoreLanguages.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
            obj.IsEnabled = enabled;
            obj.TranslateDescriptions = descriptions;
            obj.TranslateHints = hints;
            obj.TranslateLogs = logs;

            return obj.Serialize();
        }

        public static ConfigurationData Deserialize(byte[] data)
        {
            if (data == null || data.Length == 0)
                return new ConfigurationData();

            using (var ms = new System.IO.MemoryStream(data))
            {
                using (var reader = new System.IO.BinaryReader(ms))
                {
                    var obj = new ConfigurationData();

                    obj.IsEnabled = reader.ReadBoolean();
                    obj.TranslateDescriptions = reader.ReadBoolean();
                    obj.TranslateHints = reader.ReadBoolean();
                    obj.TranslateLogs = reader.ReadBoolean();

                    var i = reader.ReadInt32();
                    if (i > 0)
                    {
                        var list = new List<string>();
                        while (i-- > 0)
                            list.Add(reader.ReadString());
                        obj.LanguageCodes = list;
                    }

                    i = reader.ReadInt32();
                    if (i > 0)
                    {
                        var list = new List<string>();
                        while (i-- > 0)
                            list.Add(reader.ReadString());
                        obj.LanguageNames = list;
                    }

                    i = reader.ReadInt32();
                    if (i > 0)
                    {
                        var list = new List<string>();
                        while (i-- > 0)
                            list.Add(reader.ReadString());
                        obj.IgnoreLanguages = list;
                    }

                    obj.TargetLanguage = reader.ReadString();

                    if (reader.PeekChar() != -1) obj.ClientId = reader.ReadString();
                    if (reader.PeekChar() != -1) obj.ClientSecret = reader.ReadString();

                    return obj;
                }
            }
        }

        public byte[] Serialize()
        {
            using (var ms = new System.IO.MemoryStream())
            {
                using (var writer = new System.IO.BinaryWriter(ms))
                {
                    writer.Write(this.IsEnabled);
                    writer.Write(this.TranslateDescriptions);
                    writer.Write(this.TranslateHints);
                    writer.Write(this.TranslateLogs);

                    writer.Write(this.LanguageCodes == null ? 0 : this.LanguageCodes.Count());
                    if (this.LanguageCodes != null)
                        foreach (var x in this.LanguageCodes)
                            writer.Write(x);

                    writer.Write(this.LanguageNames == null ? 0 : this.LanguageNames.Count());
                    if (this.LanguageNames != null)
                        foreach (var x in this.LanguageNames)
                            writer.Write(x);

                    writer.Write(this.IgnoreLanguages == null ? 0 : this.IgnoreLanguages.Count());
                    if (this.IgnoreLanguages != null)
                        foreach (var x in this.IgnoreLanguages)
                            writer.Write(x);

                    writer.Write(this.TargetLanguage);

                    writer.Write(this.ClientId);
                    writer.Write(this.ClientSecret);
                }
                return ms.ToArray();
            }
        }

        public IEnumerable<string> LanguageCodes;
        public IEnumerable<string> LanguageNames;

        public string TargetLanguage = "en";
        public IEnumerable<string> IgnoreLanguages;

        public bool IsEnabled;
        public bool TranslateDescriptions = true;
        public bool TranslateHints = true;
        public bool TranslateLogs;

        public string ClientId;
        public string ClientSecret;

    }
}
