/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GeoTransformer.Publishers.GarminGps
{
    /// <summary>
    /// An extension for publishing GPX files directly to Garmin GPS devices.
    /// </summary>
    public class GarminGps : Extensions.IPublisher, Extensions.IConfigurable
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
            string path = target.Tag as string;

            cancel = false;
            var pathRoot = System.IO.Path.GetPathRoot(path);

#if DEBUG
            if (target.Key == new Guid("20F1C26E-7C4E-41D5-8214-93C0D22A6026"))
                pathRoot = path;
#endif

            var transformers = new List<Extensions.ITransformer> 
            {
                new Transformers.SaveFiles.SaveFiles(path, path),
            };

            if (this._configurationControl.PublishImages)
            {
                transformers.Add(
                    new Transformers.SaveFiles.SaveImages(new PathGenerator(pathRoot).CreateImagePath)
                        {
                            RemoveObsoleteImages = this._configurationControl.RemoveExistingImages,
                            ImageRootPath = System.IO.Path.Combine(pathRoot, "Garmin", "GeocachePhotos"),
                            EncodeEverything = true,
                            PublishLogImages = this._configurationControl.PublishLogImages,
                            MaximumSize = this._configurationControl.PublishImageSize
                        }
                    );
            }

#if DEBUG
            if (target.Key != new Guid("20F1C26E-7C4E-41D5-8214-93C0D22A6026"))
                transformers.Add(new Transformers.SafelyRemoveGps.SafelyRemoveGps(System.IO.Directory.GetDirectoryRoot(path)));
#else
            transformers.Add(new Transformers.SafelyRemoveGps.SafelyRemoveGps(System.IO.Directory.GetDirectoryRoot(path)));
#endif

            return transformers;
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

#if DEBUG
            result.Add(new PublisherTarget(this, "Debug Garmin to Folder", null, new Guid("20F1C26E-7C4E-41D5-8214-93C0D22A6026"), System.IO.Path.Combine(Environment.CurrentDirectory, "DebugGarminPublish")));
#endif

            foreach (var drive in System.IO.DriveInfo.GetDrives())
            {
                try
                {
                    if (!drive.IsReady || drive.DriveType != System.IO.DriveType.Removable)
                        continue;

                    // Assume that all Garmin devices will have the .xml file present.
                    var deviceXmlFile = System.IO.Path.Combine(drive.RootDirectory.FullName, "Garmin", "GarminDevice.xml");
                    if (!System.IO.File.Exists(deviceXmlFile))
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

                    var deviceInfo = System.Xml.Linq.XDocument.Load(deviceXmlFile);
                    var ns = deviceInfo.Root.Name.Namespace;
                    var model = deviceInfo.Root.Element(ns + "Model");
                    if (model != null)
                    {
                        var desc = model.Element(ns + "Description");
                        if (desc != null && !string.IsNullOrEmpty(desc.Value))
                            label = "Garmin " + desc.Value;
                    }

                    // The id is a number unique for each device so we are using it to create the unique key for it
                    // the first 12 bytes are just random numbers that identify this extension - please change it if
                    // you are copying the code for another extension.
                    var id = System.Xml.XmlConvert.ToInt32(deviceInfo.Root.Element("Id").GetValue() ?? "0");
                    if (id == 0) id = label.GetHashCode();
                    byte[] key = new byte[] { 1, 2, 3, 4, 123, 124, 125, 126, 234, 235, 236, 237, 0, 0, 0, 0 };
                    Array.Copy(BitConverter.GetBytes(id), 0, key, 12, 4);

                    var dir = System.IO.Path.Combine(drive.RootDirectory.FullName, "Garmin", "GPX");
                    
                    result.Add(new PublisherTarget(this, label + " (" + drive.RootDirectory.FullName + ")", img, new Guid(key), dir));
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

        private Configuration _configurationControl;

        /// <summary>
        /// Initializes the extension with the specified current configuration (can be <c>null</c> if the extension is initialized for the very first time) and
        /// returns the configuration UI control (can return <c>null</c> if the user interface is not needed).
        /// </summary>
        /// <param name="currentConfiguration">The current configuration.</param>
        /// <returns>
        /// The configuration UI control.
        /// </returns>
        public System.Windows.Forms.Control Initialize(byte[] currentConfiguration)
        {
            return this._configurationControl = new Configuration(currentConfiguration);
        }

        /// <summary>
        /// Retrieves the configuration from the extension's configuration UI control.
        /// </summary>
        /// <returns>
        /// The serialized configuration data.
        /// </returns>
        public byte[] SerializeConfiguration()
        {
            return this._configurationControl.Serialize();
        }

        /// <summary>
        /// Gets a value indicating whether the this extension should be executed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this extension is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the category of the extension.
        /// </summary>
        public Extensions.Category Category
        {
            get { return new Extensions.Category(3000, "Publish to Garmin GPS"); }
        }
    }
}
