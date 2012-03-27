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

namespace GeoTransformer.Transformers.ManualPublish
{
    /// <summary>
    /// Editor control for the <see cref="ManualPublish"/> extension.
    /// </summary>
    public partial class EditorControl : UI.UserControlBase
    {
        private System.Xml.Linq.XElement _boundElement;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorControl"/> class.
        /// </summary>
        public EditorControl()
        {
            InitializeComponent();
            this.publishSettings.SelectedIndex = 0;
        }

        /// <summary>
        /// Gets or sets the XML element that is bound to the input element.
        /// </summary>
        public System.Xml.Linq.XElement BoundElement
        {
            get
            {
                return this._boundElement;
            }

            set
            {
                if (this._boundElement == value)
                    return;

                this._boundElement = value;
                var parsed = ManualPublishMode.Default;

                if (value != null)
                    Enum.TryParse(value.Value, out parsed);

                this.publishSettings.SelectedIndex = (int)parsed;
            }
        }

        private void publishSettings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._boundElement == null)
                return;

            var val = (ManualPublishMode)this.publishSettings.SelectedIndex;
            var sval = val == ManualPublishMode.Default ? string.Empty : val.ToString();

            if (!string.Equals(this._boundElement.Value, sval, StringComparison.Ordinal))
                this._boundElement.Value = sval;
        }
    }
}
