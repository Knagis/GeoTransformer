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

namespace GeoTransformer.Transformers.AdditionalHints
{
    public partial class EditorControl : UI.UserControlBase
    {
        private System.Xml.Linq.XElement _boundElement;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorControl"/> class.
        /// </summary>
        public EditorControl()
        {
            InitializeComponent();
        }

        void _textbox_TextChanged(object sender, EventArgs e)
        {
            if (this._boundElement != null && !string.Equals(this._boundElement.Value, this.textBox.Text, StringComparison.Ordinal))
                this._boundElement.Value = this.textBox.Text;
        }

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
                this.textBox.Text = value == null ? null : value.Value;
            }
        }
    }
}
