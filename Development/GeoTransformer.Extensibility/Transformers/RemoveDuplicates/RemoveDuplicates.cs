/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GeoTransformer.Extensions;

namespace GeoTransformer.Transformers.RemoveDuplicates
{
    public class RemoveDuplicates : TransformerBase, IConfigurable
    {
        private SimpleConfigurationControl Configuration;

        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Remove duplicates"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.PreProcess; }
        }

        /// <summary>
        /// Processes the specified GPX documents. If the method is not overriden in the derived class,
        /// calls <see cref="Process(Gpx.GpxDocument, Transformers.TransformerOptions)"/> for each document in the list.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
            this._issuedWarnings.Clear();

            var totalWpts = documents.Count == 0 ? 0 : documents.Sum(o => o.Waypoints.Count);

            // processed holds the currently known newest copy for each waypoint name.
            var processed = new Dictionary<string, Tuple<Gpx.GpxDocument, Gpx.GpxWaypoint>>(StringComparer.OrdinalIgnoreCase);
            int removedWaypoints = 0;
            int removedCaches = 0;
            int processedWaypoints = 0;

            foreach (var doc in documents)
            {
                for (int i = doc.Waypoints.Count - 1; i >= 0; i--)
                {
                    processedWaypoints++;
                    if (processedWaypoints % 25 == 0)
                        this.ReportProgress(processedWaypoints, totalWpts);

                    var wpt = doc.Waypoints[i];
                    var name = wpt.Name;
                    if (string.IsNullOrEmpty(name))
                        continue;

                    if (!processed.ContainsKey(name))
                    {
                        processed.Add(name, Tuple.Create(doc, wpt));
                        continue;
                    }

                    if (wpt.Geocache.IsDefined())
                        removedCaches++;
                    else
                        removedWaypoints++;

                    var current = processed[name];
                    var currentTime = ReadGpxTime(current.Item1, current.Item2);
                    var candidateTime = ReadGpxTime(doc, wpt);

                    if (candidateTime < currentTime)
                        doc.Waypoints.RemoveAt(i);
                    else
                    {
                        current.Item1.Waypoints.Remove(current.Item2);
                        processed[name] = Tuple.Create(doc, wpt);
                    }
                }
            }

            this.ReportStatus("Removed " + removedCaches + " duplicate cache" + (removedCaches % 10 == 1 && removedCaches != 11 ? string.Empty : "s")
                + " and " + removedWaypoints + " waypoint" + (removedWaypoints % 10 == 1 && removedWaypoints != 11 ? string.Empty : "s"));
        }

        /// <summary>
        /// Holds a list of file names (<see cref="GpxMetadata.OriginalFileName"/>) that already have been shown in the warning list
        /// because they do not have the creation time defined.
        /// </summary>
        private HashSet<string> _issuedWarnings = new HashSet<string>();

        /// <summary>
        /// Retrieves the last update time of the given <paramref name="waypoint"/> assuming that it is located in the <paramref name="document"/>.
        /// </summary>
        /// <param name="document">The GPX document that contains the waypoint.</param>
        /// <param name="waypoint">The GPX waypoint.</param>
        /// <returns>Date and time when the waypoint was updated (usually when the GPX file was initially created). Returns <see cref="DateTime.MinValue"/> if the document does not have a creation time.</returns>
        private DateTime ReadGpxTime(Gpx.GpxDocument document, Gpx.GpxWaypoint waypoint)
        {
            var val = document.Metadata.CreationTime;
            if (!val.HasValue)
            {
                if (!this._issuedWarnings.Contains(document.Metadata.OriginalFileName))
                    this.ReportStatus("GPX file '{0}' does not have a creation time defined so the removal might not produce correct results.", document.Metadata.OriginalFileName);
                else
                    this._issuedWarnings.Add(document.Metadata.OriginalFileName);

                return DateTime.MinValue;
            }

            return val.Value;
        }

        public System.Windows.Forms.Control Initialize(byte[] currentConfiguration)
        {
            this.Configuration = new SimpleConfigurationControl(this.Title,
@"Enables automatic deletion of duplicate waypoints and 
geocaches. This is useful if you have more than one source
of GPX files such as overlapping pocket queries, download
single cache separately to get the latest information etc.

This option will check the generation time of each file and
in cases of duplicates (the duplicates are checked by cache
codes) leaves only the newest copy.");

            this.Configuration.checkBoxEnabled.Checked = currentConfiguration == null || (currentConfiguration.Length > 0 && currentConfiguration[0] == 1);

            return this.Configuration;
        }

        public byte[] SerializeConfiguration()
        {
            return new byte[] { this.IsEnabled ? (byte)1 : (byte)0 };
        }

        public bool IsEnabled
        {
            get { return this.Configuration.checkBoxEnabled.Checked; }
        }

        Category IHasCategory.Category { get { return Category.Transformers; } }

    }
}
