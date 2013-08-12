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
    /// Extension that persists the GPX files to a file system folder as index Garmin ZIP archive (.GGZ).
    /// </summary>
    public class SaveGarminZip : TransformerBase, Extensions.ISpecial
    {
        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Publish GGZ file"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.Publish; }
        }

        /// <summary>
        /// The file system path where the GGZ file will be stored.
        /// </summary>
        private string _fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveGarminZip"/> class.
        /// </summary>
        /// <param name="fileName">The file system path where the GGZ file will be stored.</param>
        public SaveGarminZip(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException("fileName");

            this._fileName = fileName;
        }

        /// <summary>
        /// Processes the specified GPX documents. Calls <see cref="Process(Gpx.GpxDocument, Transformers.TransformerOptions)"/> for each document in the list.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
            var directory = System.IO.Path.GetDirectoryName(this._fileName);
            System.IO.Directory.CreateDirectory(directory);

            using (var file = System.IO.File.Create(this._fileName))
            {
                Gpx.GarminZipWriter.Create(documents, file);
            }
        }
    }
}
