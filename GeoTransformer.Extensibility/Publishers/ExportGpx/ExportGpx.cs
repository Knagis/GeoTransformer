/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoTransformer.Publishers.ExportGpx
{
    /// <summary>
    /// Extension for publishing GPX files to a file system folder.
    /// </summary>
    public class ExportGpx : Extensions.IPublisher
    {
        /// <summary>
        /// Gets the needed transformers marked with <see cref="ISpecial"/> interface that will publish the data
        /// for the given <paramref name="target"/>. The transformers are added to the list of standard transformers.
        /// </summary>
        /// <param name="target">The target that the user has chosen.</param>
        /// <param name="cancel">If set to <c>true</c> the publishing is cancelled without running transformers.</param>
        /// <returns>
        /// List of transformers.
        /// </returns>
        public IEnumerable<Extensions.ITransformer> GetSpecialTransformers(PublisherTarget target, out bool cancel)
        {
            string path = target.Tag as string;
            if (path == null)
            {
                using (var folderBrowser = new System.Windows.Forms.FolderBrowserDialog())
                {
                    folderBrowser.SelectedPath = Data.TransformerSchema.TransformerSchema.Instance.RecentPublishFolders.ReadPaths().FirstOrDefault() 
                                                    ?? System.Windows.Forms.Application.StartupPath;
                    var res = folderBrowser.ShowDialog();
                    if (res != System.Windows.Forms.DialogResult.OK)
                    {
                        cancel = true;
                        return null;
                    }
                    path = folderBrowser.SelectedPath;
                }
            }

            Data.TransformerSchema.TransformerSchema.Instance.RecentPublishFolders.SaveRecentFolder(path);

            cancel = false;
            return new List<Extensions.ITransformer> 
            {
                new Transformers.SaveFiles.SaveFiles(path, path)
            };
        }

        /// <summary>
        /// Occurs when the publisher targets have been changed (for example, due to removable device being removed).
        /// The previous target list for the publisher is overwritten with the new one.
        /// </summary>
        public event EventHandler<PublisherTargetsChangedEventArgs> TargetsChanged
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Called by the application to notify the extension that some of the removable devices have been changed.
        /// If the extension targets removable drives this method should check if the previously given targets are
        /// still actual and raise <see cref="TargetsChanged"/> event if they have changed.
        /// </summary>
        public void NotifyDevicesChanged()
        {
        }

        /// <summary>
        /// Initializes the publisher and returns the initially available publisher targets.
        /// </summary>
        /// <returns>
        /// The publisher targets that are available at this moment.
        /// </returns>
        public IEnumerable<PublisherTarget> Initialize()
        {
            var targets = new List<PublisherTarget>();

            targets.Add(new PublisherTarget(this, "Export GPX (Browse...)", Resources.Icon));

            var paths = Data.TransformerSchema.TransformerSchema.Instance.RecentPublishFolders.ReadPaths().ToList();
            if (paths.Count > 0)
            {
                var recentFolders = new PublisherTarget(this, "Export GPX (Recent Folders)", Resources.Icon);
                targets.Add(recentFolders);

                foreach (var p in paths)
                    recentFolders.Children.Add(new PublisherTarget(this, p, Resources.Icon, tag: p));
            }

            return targets;
        }
    }
}
