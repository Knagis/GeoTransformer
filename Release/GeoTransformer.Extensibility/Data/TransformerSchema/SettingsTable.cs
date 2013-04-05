/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Data.TransformerSchema
{
    internal class SettingsTable : DatabaseTable
    {
        public SettingsTable()
            : base("Settings", 1)
        {
            this.AddColumn<long>("CurrentVersion", 1);
            this.AddColumn<int>("MainFormWindowState", 1);
            this.AddColumn<int>("MainFormWindowLeft", 1);
            this.AddColumn<int>("MainFormWindowTop", 1);
            this.AddColumn<int>("MainFormWindowWidth", 1);
            this.AddColumn<int>("MainFormWindowHeight", 1);
            this.AddColumn<bool>("DisableSolvedIconChange", 4);
            this.AddColumn<string>("MainFormDefaultUrl", 4);
            this.AddColumn<bool>("EnableGpsAutoEject", 6);
            this.AddColumn<string>("MainFormColumnSettings", 7);
            this.AddColumn<string>("LastVersionShown", 11);
            this.AddColumn<bool>("DoNotShowWelcomeScreen", 17);
            this.AddColumn<int>("ListViewerHeight", 18);
            this.AddColumn<string>("RecentPublishers", 19);

            // obsolete columns
            this.AddColumn<byte[]>("PocketQueryDownload", 12);
            this.AddColumn<byte[]>("GpxTautai", 10);

            this.AddColumn<bool>("TranslatorEnabled", 8);
            this.AddColumn<bool>("TranslatorDisableHints", 8);
            this.AddColumn<bool>("TranslatorDisableDescriptions", 8);
            this.AddColumn<bool>("TranslatorEnableLogs", 8);
            this.AddColumn<string>("TranslatorTargetLanguage", 8);
            this.AddColumn<string>("TranslatorIgnoreLanguages", 8);
            this.AddColumn<string>("TranslatorLanguageNameCache", 8);

            this.AddColumn<bool>("DisableHtmlEntityDecode", 4);
            this.AddColumn<bool>("EnableFileMerge", 5);
            this.AddColumn<bool>("DisableDuplicateRemoval", 9);

            this.AddColumn<bool>("DisableDisabledCachePrefix", 13);
            this.AddColumn<string>("DisabledCachePrefix", 13);
        }

        public DatabaseColumn<long> CurrentVersion { get { return this.GetColumn<long>("CurrentVersion"); } }
        public DatabaseColumn<int> MainFormWindowState { get { return this.GetColumn<int>("MainFormWindowState"); } }
        public DatabaseColumn<int> MainFormWindowLeft { get { return this.GetColumn<int>("MainFormWindowLeft"); } }
        public DatabaseColumn<int> MainFormWindowTop { get { return this.GetColumn<int>("MainFormWindowTop"); } }
        public DatabaseColumn<int> MainFormWindowWidth { get { return this.GetColumn<int>("MainFormWindowWidth"); } }
        public DatabaseColumn<int> MainFormWindowHeight { get { return this.GetColumn<int>("MainFormWindowHeight"); } }
        public DatabaseColumn<bool> DisableSolvedIconChange { get { return this.GetColumn<bool>("DisableSolvedIconChange"); } }
        public DatabaseColumn<string> MainFormDefaultUrl { get { return this.GetColumn<string>("MainFormDefaultUrl"); } }
        public DatabaseColumn<bool> EnableGpsAutomaticEject { get { return this.GetColumn<bool>("EnableGpsAutoEject"); } }
        public DatabaseColumn<string> MainFormColumnSettings { get { return this.GetColumn<string>("MainFormColumnSettings"); } }
        public DatabaseColumn<string> LastVersionShown { get { return this.GetColumn<string>("LastVersionShown"); } }
        public DatabaseColumn<bool> DoNotShowWelcomeScreen { get { return this.GetColumn<bool>("DoNotShowWelcomeScreen"); } }

        /// <summary>
        /// Gets the most recently used publisher GUIDs.
        /// </summary>
        public DatabaseColumn<string> RecentPublishers { get { return this.GetColumn<string>("RecentPublishers"); } }
        
        /// <summary>
        /// Gets the column that stores the height of the list viewer on the main form.
        /// </summary>
        public DatabaseColumn<int> ListViewerHeight { get { return this.GetColumn<int>("ListViewerHeight"); } }

        /*
         * Obsolete columns
         * 
        public DatabaseColumn<bool> TranslatorEnabled { get { return this.GetColumn<bool>("TranslatorEnabled"); } }
        public DatabaseColumn<bool> TranslatorDisableHints { get { return this.GetColumn<bool>("TranslatorDisableHints"); } }
        public DatabaseColumn<bool> TranslatorDisableDescriptions { get { return this.GetColumn<bool>("TranslatorDisableDescriptions"); } }
        public DatabaseColumn<bool> TranslatorEnableLogs { get { return this.GetColumn<bool>("TranslatorEnableLogs"); } }
        public DatabaseColumn<string> TranslatorTargetLanguage { get { return this.GetColumn<string>("TranslatorTargetLanguage"); } }
        public DatabaseColumn<string> TranslatorIgnoreLanguages { get { return this.GetColumn<string>("TranslatorIgnoreLanguages"); } }
        public DatabaseColumn<string> TranslatorLanguageNameCache { get { return this.GetColumn<string>("TranslatorLanguageNameCache"); } }
        public DatabaseColumn<bool> DisableHtmlEntityDecode { get { return this.GetColumn<bool>("DisableHtmlEntityDecode"); } }
        public DatabaseColumn<bool> EnableFileMerge { get { return this.GetColumn<bool>("EnableFileMerge"); } }
        public DatabaseColumn<bool> DisableDuplicateRemoval { get { return this.GetColumn<bool>("DisableDuplicateRemoval"); } }
        public DatabaseColumn<string> DisabledCachePrefix { get { return this.GetColumn<string>("DisabledCachePrefix"); } }
        public DatabaseColumn<bool> DisableDisabledCachePrefix { get { return this.GetColumn<bool>("DisableDisabledCachePrefix"); } }
        public DatabaseColumn<byte[]> PocketQueryDownload { get { return this.GetColumn<byte[]>("PocketQueryDownload"); } }
        public DatabaseColumn<byte[]> GpxTautai { get { return this.GetColumn<byte[]>("GpxTautai"); } }
         */
    }
}
