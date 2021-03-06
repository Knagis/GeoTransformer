﻿/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GeoTransformer.Publishers.MagellanGps
{
    /// <summary>
    /// An extension for publishing GPX files directly to Magellan GPS devices.
    /// </summary>
    public class MagellanGps : Extensions.IPublisher
    {
        IList<PublisherTarget> _currentTargets;

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
            var path = target.Tag as Tuple<string, string>;

            cancel = false;
            return new List<Extensions.ITransformer> 
            {
                new Transformers.SaveFiles.SaveFiles(path.Item1, path.Item2),
                new Transformers.SafelyRemoveGps.SafelyRemoveGps(System.IO.Directory.GetDirectoryRoot(path.Item1))
            };
        }

        /// <summary>
        /// Occurs when the publisher targets have been changed (for example, due to removable device being removed).
        /// The previous target list for the publisher is overwritten with the new one.
        /// </summary>
        public event EventHandler<PublisherTargetsChangedEventArgs> TargetsChanged;

        /// <summary>
        /// Called by the application to notify the extension that some of the removable devices have been changed.
        /// If the extension targets removable drives this method should check if the previously given targets are
        /// still actual and raise <see cref="TargetsChanged"/> event if they have changed.
        /// </summary>
        public void NotifyDevicesChanged()
        {
            // just for safety
            if (this._currentTargets == null)
                this._currentTargets = new List<PublisherTarget>();

            var newTargets = this.Initialize();

            if (!Enumerable.SequenceEqual(this._currentTargets.Select(o => o.Text).OrderBy(o => o),
                                          newTargets.Select(o => o.Text).OrderBy(o => o)))
            {
                this._currentTargets = newTargets.ToList();
                var handler = this.TargetsChanged;
                if (handler != null)
                    handler(this, new PublisherTargetsChangedEventArgs(this._currentTargets));
            }
        }

        /// <summary>
        /// Initializes the publisher and returns the initially available publisher targets.
        /// </summary>
        /// <returns>
        /// The publisher targets that are available at this moment.
        /// </returns>
        public IEnumerable<PublisherTarget> Initialize()
        {
            var result = new List<PublisherTarget>();
            foreach (var drive in System.IO.DriveInfo.GetDrives())
            {
                try
                {
                    if (!drive.IsReady || drive.DriveType != System.IO.DriveType.Removable)
                        continue;

                    Image img = null;

                    // last fallback - if no other name can be detected, use drive label
                    var label = drive.VolumeLabel;

                    // read additional data from autorun.inf (icon and drive label)
                    if (System.IO.File.Exists(System.IO.Path.Combine(drive.RootDirectory.FullName, "autorun.inf")))
                    {
                        var lines = System.IO.File.ReadAllLines(System.IO.Path.Combine(drive.RootDirectory.FullName, "autorun.inf"))
                            .SkipWhile(o => !string.Equals(o.Trim(), "[autorun]", StringComparison.OrdinalIgnoreCase))
                            .Skip(1)
                            .TakeWhile(o => !o.Trim().StartsWith("[", StringComparison.Ordinal));

                        var imgFile = lines.Where(o => o.StartsWith("icon", StringComparison.OrdinalIgnoreCase))
                                           .Select(o => System.IO.Path.Combine(drive.RootDirectory.FullName, o.Substring(o.IndexOf("=") + 1)))
                                           .FirstOrDefault();

                        if (imgFile != null && imgFile.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))
                        {
                            // note that this image will not be properly disposed but it should not be a problem since 
                            // GC will eventually reclaim it and there should not be more than few of them lingering.
                            using (var icon = new Icon(imgFile, 24, 24))
                                img = icon.ToBitmap();
                        }

                        label = lines.Where(o => o.StartsWith("label", StringComparison.OrdinalIgnoreCase))
                                      .Select(o => o.Substring(o.IndexOf("=") + 1))
                                      .FirstOrDefault() ?? label;
                    }

                    if (!drive.VolumeLabel.StartsWith("MAGELLAN", StringComparison.OrdinalIgnoreCase)
                        && !label.StartsWith("MAGELLAN", StringComparison.OrdinalIgnoreCase))
                        continue;

                    // The id is a number unique for each device so we are using it to create the unique key for it
                    // the first 12 bytes are just random numbers that identify this extension - please change it if
                    // you are copying the code for another extension.
                    byte[] key = new byte[] { 11, 12, 13, 14, 23, 24, 25, 26, 134, 135, 136, 137, 0, 0, 0, 0 };
                    Array.Copy(BitConverter.GetBytes(label.GetHashCode()), 0, key, 12, 4);

                    var dir = System.IO.Path.Combine(drive.RootDirectory.FullName, "Geocaches");
                    var wptdir = System.IO.Path.Combine(drive.RootDirectory.FullName, "Waypoints");
                    result.Add(new PublisherTarget(this, label + " (" + drive.RootDirectory.FullName + ")", img, new Guid(key), Tuple.Create(dir, wptdir)));
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("GarminGps.Initialize: " + ex.Message);
                    continue;
                }
            }

            // _currentTargets will be null when the Initialize is called first time, not when NotifyDeviceChange is called.
            if (this._currentTargets == null)
                this._currentTargets = result;
            return result;
        }
    }
}
