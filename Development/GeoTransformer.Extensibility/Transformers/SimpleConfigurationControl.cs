/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */

using System.Windows.Forms;

namespace GeoTransformer.Transformers
{
    public partial class SimpleConfigurationControl : UI.UserControlBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleConfigurationControl"/> class.
        /// </summary>
        protected SimpleConfigurationControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleConfigurationControl"/> class.
        /// </summary>
        /// <param name="checkBoxText">The text displayed on the <see cref="CheckBox"/> control.</param>
        /// <param name="checkBoxToolTip">The tool tip displayed on the <see cref="CheckBox"/> control.</param>
        public SimpleConfigurationControl(string checkBoxText, string checkBoxToolTip)
            : this()
        {
            this.checkBoxEnabled.Text = checkBoxText;
            this.configurationTooltip.SetToolTip(this.checkBoxEnabled, checkBoxToolTip);
        }
    }
}
