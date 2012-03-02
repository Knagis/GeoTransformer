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

namespace GeoTransformer.Transformers.MarkSolved
{
    internal partial class EditorControl : UI.UserControlBase
    {
        private System.Xml.Linq.XElement _boundElement;

        public EditorControl()
        {
            InitializeComponent();

            this.checkBox.CheckedChanged += checkBoxCheckedChanged;
        }

        void checkBoxCheckedChanged(object sender, EventArgs e)
        {
            if (this._boundElement != null)
            {
                var v = this.checkBox.Checked ? bool.TrueString : string.Empty;
                if (!string.Equals(v, this._boundElement.Value, StringComparison.OrdinalIgnoreCase))
                    this._boundElement.Value = v;
            }
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

                if (value == null)
                    this.checkBox.Checked = false;
                else
                {
                    bool b;
                    this.checkBox.Checked = bool.TryParse(value.Value, out b) & b;
                }
            }
        }
    }
}
