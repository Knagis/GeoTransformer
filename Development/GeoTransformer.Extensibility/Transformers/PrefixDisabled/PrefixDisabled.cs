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

namespace GeoTransformer.Transformers.PrefixDisabled
{
    public class PrefixDisabled : TransformerBase, IConfigurable
    {
        private int _count;

        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Prefix disabled caches"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.Process; }
        }

        public override void Process(IList<System.Xml.Linq.XDocument> xmlFiles, TransformerOptions options)
        {
            this._count = 0;
            base.Process(xmlFiles, options);

            this.ReportStatus(this._count + " disabled cache" + ((this._count % 10 == 1 && this._count != 11) ? string.Empty : "s") + " prefixed.");
        }

        public override void Process(XDocument xml)
        {
            var prefix = this.Configuration.textBoxDisabledPrefix.Text + " ";
            foreach (var c in xml.CacheDescendants("cache"))
            {
                var available = bool.Parse(c.Attribute("available").GetValue() ?? "true");
                var archived = bool.Parse(c.Attribute("archived").GetValue() ?? "false");

                if (!available || archived)
                {
                    this._count++;
                    var n = c.CacheElement("name");
                    if (n != null && (n.Value == null || !n.Value.StartsWith(prefix, StringComparison.Ordinal)))
                        n.Value = prefix + n.Value;
                }
            }
        }

        #region [ IConfigurable ]

        private ConfigurationControl Configuration;

        public System.Windows.Forms.Control Initialize(byte[] currentConfiguration)
        {
            this.Configuration = new ConfigurationControl();

            this.Configuration.checkBoxPrefixDisabled.Checked = currentConfiguration == null || (currentConfiguration.Length > 0 && currentConfiguration[0] == 1);
            this.Configuration.textBoxDisabledPrefix.Text = currentConfiguration != null ? System.Text.Encoding.UTF8.GetString(currentConfiguration, 1, currentConfiguration.Length - 1) : null;

            return this.Configuration;
        }

        public byte[] SerializeConfiguration()
        {
            var text = this.Configuration.textBoxDisabledPrefix.Text;
            var b = new byte[1 + System.Text.Encoding.UTF8.GetByteCount(text)];

            System.Text.Encoding.UTF8.GetBytes(text, 0, text.Length, b, 1);
            b[0] = this.Configuration.checkBoxPrefixDisabled.Checked ? (byte)1 : (byte)0;

            return b;
        }

        public bool IsEnabled
        {
            get { return this.Configuration.checkBoxPrefixDisabled.Checked; }
        }

        Category IHasCategory.Category { get { return Category.Transformers; } }

        #endregion
    }
}
