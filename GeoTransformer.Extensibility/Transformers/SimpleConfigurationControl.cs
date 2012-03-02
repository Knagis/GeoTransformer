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

namespace GeoTransformer.Transformers
{
    public partial class SimpleConfigurationControl : UI.UserControlBase
    {
        protected SimpleConfigurationControl()
        {
            InitializeComponent();
        }

        public SimpleConfigurationControl(string checkBoxText, string checkBoxToolTip)
            : this()
        {
            this.checkBoxEnabled.Text = checkBoxText;
            this.configurationTooltip.SetToolTip(this.checkBoxEnabled, checkBoxToolTip);
        }
    }
}
