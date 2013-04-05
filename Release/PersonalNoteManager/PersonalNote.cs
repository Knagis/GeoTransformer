/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersonalNoteManager
{
    internal class PersonalNote
    {
        public string CacheCode { get; set; }

        public string CacheTitle { get; set; }

        public bool IsFound { get; set; }

        public bool IsArchived { get; set; }

        public string NoteText { get; set; }
    }
}
