/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GeoTransformer.Data.TransformerSchema
{
    /*
     * Schema history:
     *  1 - base table structure, LogCache table
     *  2 - some new columns
     *  3 - added RecentPublishFolders table
     *  4 - new columns in Settings table
     *  5 - new columns in Settings table
     *  6 - new columns in Settings table
     *  7 - new columns in Settings table
     *  8 - new columns in Settings table regarding translation
     *  9 - added Settings.DisableDuplicateRemoval column
     *  10 - added Settings.GpxTautai column
     *  11 - added Settings.LastVersionShown column
     *  12 - added Settings.PocketQueryDownload column
     *  13 - added Settings.DisableDisabledCachePrefix and Settings.DisabledCachePrefix columns
     *  14 - unused
     *  15 - unused
     *  16 - added ExtensionTableVersions table
     *  17 - added Settings.DoNotShowWelcomeScreen column
     *  18 - added Settings.ListViewerHeight column
     *  19 - added Settings.RecentPublishers column
     */

    internal class TransformerSchema : DatabaseSchema
    {
        private static TransformerSchema _instance;
        private static object locker = new object();

        /// <summary>
        /// Gets the global instance of the GeoTransformer main database.
        /// </summary>
        internal static TransformerSchema Instance
        {
            get
            {
                if (_instance == null)
                    lock (locker)
                        if (_instance == null)
                        {
                            if (!string.IsNullOrEmpty(System.Windows.Forms.Application.ExecutablePath))
                                _instance = new TransformerSchema(System.IO.Path.Combine(new System.IO.FileInfo(System.Windows.Forms.Application.ExecutablePath).DirectoryName, "GeoTransformer.data"));
                            else
                                _instance = new TransformerSchema(System.IO.Path.Combine(Environment.CurrentDirectory, "GeoTransformer.data"));
                        }

                return _instance;
            }
        }

        public TransformerSchema(string databaseFileName)
            : base(databaseFileName)
        {
            this.AddTable(new MovedCacheTable());
            this.AddTable(new RecentPublishFoldersTable());
            this.AddTable(new SettingsTable());

            foreach (var table in Extensions.ExtensionLoader.RetrieveExtensions<IExtensionTable>())
                this.AddTable(table);
        }

        public SettingsTable Settings { get { return this.GetTable<SettingsTable>(); } }
        public MovedCacheTable MovedCaches { get { return this.GetTable<MovedCacheTable>(); } }
        public RecentPublishFoldersTable RecentPublishFolders { get { return this.GetTable<RecentPublishFoldersTable>(); } }
    }
}
