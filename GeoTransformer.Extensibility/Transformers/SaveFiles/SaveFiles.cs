/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoTransformer.Transformers.SaveFiles
{
    /// <summary>
    /// Extension that persists the GPX files to a file system folder.
    /// </summary>
    public class SaveFiles : TransformerBase, Extensions.ISpecial
    {
        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Publish GPX files"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.Publish; }
        }

        private string _cacheDirectory;
        private string _waypointDirectory;
        private HashSet<string> _usedNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFiles"/> class.
        /// </summary>
        /// <param name="cacheDirectory">The file system path where the GPX files for geocaches are stored.</param>
        /// <param name="waypointDirectory">The file system path where the GPX files for geocache waypoints are stored.</param>
        public SaveFiles(string cacheDirectory, string waypointDirectory)
        {
            if (string.IsNullOrWhiteSpace(cacheDirectory))
                throw new ArgumentNullException("cacheDirectory");
            this._cacheDirectory = cacheDirectory;
            this._waypointDirectory = waypointDirectory;
        }

        /// <summary>
        /// Processes the specified GPX documents. Calls <see cref="Process(Gpx.GpxDocument, Transformers.TransformerOptions)"/> for each document in the list.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
            this._usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            System.IO.Directory.CreateDirectory(this._cacheDirectory);
            if (!string.Equals(this._cacheDirectory, this._waypointDirectory, StringComparison.OrdinalIgnoreCase))
                System.IO.Directory.CreateDirectory(this._waypointDirectory);

            base.Process(documents, options);
        }

        /// <summary>
        /// Processes the specified GPX document - stores it in the specified folder as XML file.
        /// </summary>
        /// <param name="document">The document that has to be processed.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
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

            string targetFile;
            if (origname.Name.EndsWith("-wpts.gpx", StringComparison.OrdinalIgnoreCase))
                targetFile = System.IO.Path.Combine(this._waypointDirectory ?? this._cacheDirectory, fname);
            else
                targetFile = System.IO.Path.Combine(this._cacheDirectory, fname);

            using (var fileStream = System.IO.File.Create(targetFile))
            using (var xmlWriter = System.Xml.XmlWriter.Create(fileStream, new System.Xml.XmlWriterSettings() { CheckCharacters = false }))
                document.Serialize(Gpx.GpxSerializationOptions.Compatibility).WriteTo(xmlWriter);
        }
    }
}
