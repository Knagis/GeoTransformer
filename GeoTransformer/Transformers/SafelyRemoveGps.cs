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

namespace GeoTransformer.Transformers
{
    class SafelyRemoveGps : TransformerBase, Extensions.ISpecial
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

        public SafelyRemoveGps(string driveRoot)
        {
            if (string.IsNullOrWhiteSpace(driveRoot))
                throw new ArgumentNullException("driveRoot");
            this._driveRoot = driveRoot;
        }

        public override void Process(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
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
