/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Extensions.GeocachingService
{
    /// <summary>
    /// The extension that provides global configuration for the geocaching.com Live API.
    /// </summary>
    public class GeocachingService : IExtension, IConfigurable
    {
        internal ConfigurationControl ConfigurationControl;

        System.Windows.Forms.Control IConfigurable.Initialize(byte[] currentConfiguration)
        {
            this.ConfigurationControl = new ConfigurationControl();

            if (currentConfiguration == null || currentConfiguration.Length == 0)
                return this.ConfigurationControl;

            try
            {
                using (var stream = new System.IO.MemoryStream(currentConfiguration))
                using (var reader = new System.IO.BinaryReader(stream))
                {
                    var isEnabled = reader.ReadBoolean();
                    this.ConfigurationControl.AccessToken = reader.ReadString();

                    // set this at the end as it will invoke some UI if needed.
                    this.ConfigurationControl.IsEnabled = isEnabled;
                }
            }
            catch
            {
                this.ConfigurationControl.IsEnabled = false;
            }

            return this.ConfigurationControl;
        }

        byte[] IConfigurable.SerializeConfiguration()
        {
            using (var stream = new System.IO.MemoryStream())
            using (var writer = new System.IO.BinaryWriter(stream))
            {
                writer.Write(this.ConfigurationControl.IsEnabled);
                writer.Write(this.ConfigurationControl.AccessToken ?? string.Empty);
                writer.Flush();
                return stream.ToArray();
            }
        }

        bool IConditional.IsEnabled
        {
            get { return this.ConfigurationControl.IsEnabled; }
        }

        Category IHasCategory.Category { get { return Category.ApiConfiguration; } }
    }
}
