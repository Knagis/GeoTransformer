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
    internal class TranslateData
    {
        public TranslateData(string text)
            : this(text, false)
        {
        }
        public TranslateData(string text, bool isHtml)
        {
            if (text == null)
                text = string.Empty;
            this.Text = text;
            this.IsHtml = isHtml;
        }

        public System.Xml.Linq.XElement SourceElement;
        public bool IsHtml;
        public string Text;
        private string _hash;
        public string Hash
        {
            get
            {
                if (_hash == null)
                    _hash = CalculateMd5(this.Text);
                return _hash;
            }
        }
        public string SourceLanguage;
        public bool SourceLanguageCacheTested;
        public bool SourceLanguageError;
        public string Translation;
        public bool TranslationCacheTested;
        public bool TranslationError;

        public static string CalculateMd5(string data)
        {
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var buffer = System.Text.Encoding.UTF8.GetBytes(data);

            var result = new StringBuilder();
            foreach (var b in md5.ComputeHash(buffer))
                result.Append(b.ToString("X2"));

            return result.ToString();
        }
    }
}
