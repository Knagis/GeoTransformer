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

        public override void Process(IList<System.Xml.Linq.XDocument> xmlFiles, TransformerOptions options)
        {
            this._usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            base.Process(xmlFiles, options);
        }

        public override void Process(XDocument xml)
        {
            var fname = xml.Root.Attribute("originalFileName").GetValue();
            xml.Root.Attribute("originalFileName").Remove();
            if (string.IsNullOrWhiteSpace(fname))
                throw new InvalidOperationException("Error in GeoTransformer - no file name attached to the GPX file.");

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

            if (origname.Extension.EndsWith("-wpts.gpx", StringComparison.OrdinalIgnoreCase))
                xml.Save(System.IO.Path.Combine(this._waypointDirectory ?? this._cacheDirectory, fname), System.Xml.Linq.SaveOptions.DisableFormatting);
            else
                xml.Save(System.IO.Path.Combine(this._cacheDirectory, fname), System.Xml.Linq.SaveOptions.DisableFormatting);
        }
    }
}
