/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FieldNoteManager
{
    /// <summary>
    /// Data object describing a single field note
    /// </summary>
    internal class FieldNote
    {
        public string ErrorMessage { get; set; }

        public DateTime LogTime { get; set; }

        public string CacheCode { get; set; }

        public string CacheTitle { get; set; }

        public string LogType { get; set; }

        public string Text { get; set; }

        public bool ShouldPublish { get; set; }

        public string Result { get; set; }
    }
}
