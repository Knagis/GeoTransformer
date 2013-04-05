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

namespace GeoTransformer.Transformers.PrefixNotFound
{
    internal partial class ConfigurationControl : UI.UserControlBase
    {
        public ConfigurationControl()
        {
            InitializeComponent();

            if (this.textBoxPrefix.Left < this.checkBoxPrefix.Right)
                this.textBoxPrefix.Left = this.checkBoxPrefix.Right;
        }

        private void checkBoxPrefixDisabled_CheckedChanged(object sender, EventArgs e)
        {
            this.textBoxPrefix.Enabled = this.checkBoxPrefix.Checked;
        }

        private void textBoxDisabledPrefix_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.textBoxPrefix.Text))
                this.textBoxPrefix.Text = "[Disabled]";
            else
                this.textBoxPrefix.Text = this.textBoxPrefix.Text.Trim();
        }
    }
}
