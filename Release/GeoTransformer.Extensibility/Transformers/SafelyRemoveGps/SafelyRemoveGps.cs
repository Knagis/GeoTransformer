/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UsbEject.Library;

namespace GeoTransformer.Transformers.SafelyRemoveGps
{
    /// <summary>
    /// A transformer that should be run after an USB device has been updated. It will execute "Safely Remove" command
    /// for the USB device.
    /// </summary>
    public class SafelyRemoveGps : TransformerBase, Extensions.ISpecial
    {
        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Eject USB storage device"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.AfterPublish; }
        }

        private string _driveRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="SafelyRemoveGps"/> class.
        /// </summary>
        /// <param name="driveRoot">The path to the drive root.</param>
        public SafelyRemoveGps(string driveRoot)
        {
            if (string.IsNullOrWhiteSpace(driveRoot))
                throw new ArgumentNullException("driveRoot");

            this._driveRoot = driveRoot;
        }

        /// <summary>
        /// Ejects the specified USB device.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
#if DEBUG
            var res = System.Windows.Forms.MessageBox.Show("Do you want to eject " + this._driveRoot + "?", "Safely remove", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
            if (res != System.Windows.Forms.DialogResult.Yes)
                return;
#endif

            using (var vd = new VolumeDeviceClass())
            {
                var dr = this._driveRoot.Replace("\\", "");
                var volumes = vd.Devices.Cast<Volume>().Where(o => string.Equals(o.LogicalDrive, dr, StringComparison.OrdinalIgnoreCase));
                foreach (var x in volumes.SelectMany(o => o.RemovableDevices).Distinct())
                {
                    x.Eject(true);
                }
            }
        }
    }
}
