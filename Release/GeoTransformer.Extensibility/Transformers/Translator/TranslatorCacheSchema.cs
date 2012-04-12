/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoTransformer.Data;

namespace GeoTransformer.Transformers.Translator
{

    /*
     * Schema history:
     *  1 - base table structure, Translations and LanguageDetect tables
     */

    public class TranslatorCacheSchema : DatabaseSchema
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslatorCacheSchema"/> class.
        /// </summary>
        /// <param name="databaseFileName">The path to the database file.</param>
        public TranslatorCacheSchema(string databaseFileName)
            : base(databaseFileName)
        {
            this.AddTable(new TranslationsTable());
            this.AddTable(new LanguageDetectTable());
        }

        public TranslationsTable Translations { get { return this.GetTable<TranslationsTable>(); } }
        public LanguageDetectTable LanguageDetects { get { return this.GetTable<LanguageDetectTable>(); } }
    }

    public class TranslationsTable : DatabaseTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationsTable"/> class.
        /// </summary>
        public TranslationsTable()
            : base("Translations", 1)
        {
            this.AddColumn<string>("HashCode", 1);
            this.AddColumn<string>("SourceLanguage", 1);
            this.AddColumn<string>("TargetLanguage", 1);
            this.AddColumn<string>("Translation", 1);

            this.AddIndex(new DatabaseIndex(this, "HashCode", false, 1, this.HashCode));
            this.AddIndex(new DatabaseIndex(this, "HashCodeWithLanguages", true, 1, this.HashCode, this.SourceLanguage, this.TargetLanguage));
        }

        public DatabaseColumn<string> HashCode { get { return this.GetColumn<string>("HashCode"); } }
        public DatabaseColumn<string> SourceLanguage { get { return this.GetColumn<string>("SourceLanguage"); } }
        public DatabaseColumn<string> TargetLanguage { get { return this.GetColumn<string>("TargetLanguage"); } }
        public DatabaseColumn<string> Translation { get { return this.GetColumn<string>("Translation"); } }
    }

    public class LanguageDetectTable : DatabaseTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageDetectTable"/> class.
        /// </summary>
        public LanguageDetectTable()
            : base("LanguageDetect", 1)
        {
            this.AddColumn<string>("HashCode", 1);
            this.AddColumn<string>("Language", 1);

            this.AddIndex(new DatabaseIndex(this, "HashCode", true, 1, this.HashCode));
        }

        public DatabaseColumn<string> HashCode { get { return this.GetColumn<string>("HashCode"); } }
        public DatabaseColumn<string> Language { get { return this.GetColumn<string>("Language"); } }
    }

}
