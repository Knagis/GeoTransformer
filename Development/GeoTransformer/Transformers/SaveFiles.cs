/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoTransformer.Transformers
{
    class SaveFiles : TransformerBase, Extensions.ISpecial
    {
        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Publish GPX files"; }
        }

        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.Publish; }
        }

        private string _cacheDirectory;
        private string _waypointDirectory;
        private HashSet<string> _usedNames;

        public SaveFiles(string cacheDirectory, string waypointDirectory)
        {
            if (string.IsNullOrWhiteSpace(cacheDirectory))
                throw new ArgumentNullException("cacheDirectory");
            this._cacheDirectory = cacheDirectory;
            this._waypointDirectory = waypointDirectory;
        }

        public override void Process(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
            this._usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            base.Process(documents, options);
        }

        protected override void Process(Gpx.GpxDocument document, TransformerOptions options)
        {
            var fname = document.Metadata.OriginalFileName;
            if (string.IsNullOrWhiteSpace(fname))
                fname = "GeoTransformer-Unnamed.gpx";

            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                fname = fname.Replace(c, '_');

            var origname = new System.IO.FileInfo(fname);
            int i = 1;
            while (this._usedNames.Contains(fname))
            {
                fname = origname.Name.Substring(0, origname.Name.Length - origname.Extension.Length) + "-" + i.ToString(System.Globalization.CultureInfo.InvariantCulture) + origname.Extension;
                i++;
            }

            this._usedNames.Add(fname);

            if (origname.Name.EndsWith("-wpts.gpx", StringComparison.OrdinalIgnoreCase))
                document.Serialize(Gpx.GpxSerializationOptions.Compatibility).Save(System.IO.Path.Combine(this._waypointDirectory ?? this._cacheDirectory, fname));
            else
                document.Serialize(Gpx.GpxSerializationOptions.Compatibility).Save(System.IO.Path.Combine(this._cacheDirectory, fname));
        }
    }
}
