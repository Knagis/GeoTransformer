/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoTransformer.Publishers.GarminGps
{
    /// <summary>
    /// The configuration control for <see cref="GarminGps"/> publisher.
    /// </summary>
    internal partial class Configuration : UI.UserControlBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Configuration(byte[] configuration)
        {
            InitializeComponent();

            if (configuration == null || configuration.Length == 0)
                return;

            using (var ms = new System.IO.MemoryStream(configuration))
            using (var reader = new System.IO.BinaryReader(ms))
            {
                // The version field.
                reader.ReadByte();

                this.checkBoxEnableImages.Checked = reader.ReadBoolean();
                this.txtMaximumSize.Value = reader.ReadInt16();
                this.chkPublishLogImages.Checked = reader.ReadBoolean();
                this.chkRemoveOtherImages.Checked = reader.ReadBoolean();
            }
        }

        /// <summary>
        /// Gets a value indicating whether to publish images.
        /// </summary>
        public bool PublishImages { get { return this.checkBoxEnableImages.Checked; } }

        /// <summary>
        /// Gets the maximum size of the published images.
        /// </summary>
        public System.Drawing.Size PublishImageSize { get { return new Size((int)this.txtMaximumSize.Value, (int)this.txtMaximumSize.Value); } }

        /// <summary>
        /// Gets a value indicating whether to publish images from logs.
        /// </summary>
        public bool PublishLogImages { get { return this.chkPublishLogImages.Checked; } }

        /// <summary>
        /// Gets a value indicating whether to remove existing images from the GPS.
        /// </summary>
        public bool RemoveExistingImages { get { return this.chkRemoveOtherImages.Checked; } }

        /// <summary>
        /// Serializes the configuration.
        /// </summary>
        /// <returns>The serialized configuration</returns>
        public byte[] Serialize()
        {
            using (var ms = new System.IO.MemoryStream())
            using (var writer = new System.IO.BinaryWriter(ms))
            {
                // The version field.
                writer.Write((byte)0);

                writer.Write(this.checkBoxEnableImages.Checked);
                writer.Write((short)this.txtMaximumSize.Value);
                writer.Write(this.chkPublishLogImages.Checked);
                writer.Write(this.chkRemoveOtherImages.Checked);

                writer.Flush();
                return ms.ToArray();
            }
        }
    }
}
