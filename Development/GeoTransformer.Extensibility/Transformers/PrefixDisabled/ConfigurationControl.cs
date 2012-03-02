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

namespace GeoTransformer.Transformers.PrefixDisabled
{
    internal partial class ConfigurationControl : UI.UserControlBase
    {
        public ConfigurationControl()
        {
            InitializeComponent();

            if (this.textBoxDisabledPrefix.Left < this.checkBoxPrefixDisabled.Right)
                this.textBoxDisabledPrefix.Left = this.checkBoxPrefixDisabled.Right;
        }

        private void checkBoxPrefixDisabled_CheckedChanged(object sender, EventArgs e)
        {
            this.textBoxDisabledPrefix.Enabled = this.checkBoxPrefixDisabled.Checked;
        }

        private void textBoxDisabledPrefix_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.textBoxDisabledPrefix.Text))
                this.textBoxDisabledPrefix.Text = "[Disabled]";
            else
                this.textBoxDisabledPrefix.Text = this.textBoxDisabledPrefix.Text.Trim();
        }
    }
}
