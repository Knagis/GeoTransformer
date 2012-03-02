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

namespace GeoTransformer.Transformers.HtmlEntityDecoder
{
    public class HtmlEntityDecoder : TransformerBase, IConfigurable
    {
        private SimpleConfigurationControl Configuration;

        public override string Title
        {
            get { return "Decode HTML entities"; }
        }

        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.Process; }
        }

        public override void Process(XDocument xml)
        {
            foreach (var todecode in xml.CacheDescendants("short_description")
                                      .Union(xml.CacheDescendants("long_description"))
                                      .Union(xml.CacheDescendants("encoded_hints"))
                                      .Union(xml.CacheDescendants("text"))
                        )
            {
                if (string.IsNullOrWhiteSpace(todecode.Value))
                    continue;
                var newval = System.Net.WebUtility.HtmlDecode(todecode.Value);
                todecode.Value = newval;
            }
        }

        public System.Windows.Forms.Control Initialize(byte[] currentConfiguration)
        {
            this.Configuration = new SimpleConfigurationControl(this.Title,
@"Garmin GPS devices cannot display HTML entities (such as &#039;) 
but these entities are used by lots of people to write the cache 
descriptions and logs in their own language (such as Russian, 
Chinese etc.). 

This option enables decoding these entities so that the symbols 
are not missing on any device. 

Has no known side effects.");

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
