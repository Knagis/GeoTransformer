namespace GeoTransformer.Transformers.PrefixDisabled
{
    partial class ConfigurationControl
    {
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.configurationTooltip = new GeoTransformer.UI.SingletonTooltip();
            this.checkBoxPrefixDisabled = new System.Windows.Forms.CheckBox();
            this.textBoxDisabledPrefix = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // checkBoxPrefixDisabled
            // 
            this.checkBoxPrefixDisabled.AutoSize = true;
            this.checkBoxPrefixDisabled.Checked = true;
            this.checkBoxPrefixDisabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPrefixDisabled.Location = new System.Drawing.Point(3, 3);
            this.checkBoxPrefixDisabled.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.checkBoxPrefixDisabled.Name = "checkBoxPrefixDisabled";
            this.checkBoxPrefixDisabled.Size = new System.Drawing.Size(199, 21);
            this.checkBoxPrefixDisabled.TabIndex = 7;
            this.checkBoxPrefixDisabled.Text = "Prefix disabled caches with";
            this.configurationTooltip.SetToolTip(this.checkBoxPrefixDisabled, "Prefixes disabled or archived caches with the specified text.\r\nThis is useful if " +
        "the GPS unit does not visually differentiate\r\nactive and disabled caches (for ex" +
        "ample, Garmin units does\r\nnot).");
            this.checkBoxPrefixDisabled.UseVisualStyleBackColor = true;
            this.checkBoxPrefixDisabled.CheckedChanged += new System.EventHandler(this.checkBoxPrefixDisabled_CheckedChanged);
            // 
            // textBoxDisabledPrefix
            // 
            this.textBoxDisabledPrefix.Location = new System.Drawing.Point(158, 1);
            this.textBoxDisabledPrefix.Margin = new System.Windows.Forms.Padding(1);
            this.textBoxDisabledPrefix.Name = "textBoxDisabledPrefix";
            this.textBoxDisabledPrefix.Size = new System.Drawing.Size(77, 22);
            this.textBoxDisabledPrefix.TabIndex = 8;
            this.textBoxDisabledPrefix.Text = "[Disabled]";
            this.textBoxDisabledPrefix.TextChanged += new System.EventHandler(this.textBoxDisabledPrefix_TextChanged);
            // 
            // ConfigurationControl
            // 
            this.Controls.Add(this.checkBoxPrefixDisabled);
            this.Controls.Add(this.textBoxDisabledPrefix);
            this.Name = "ConfigurationControl";
            this.Size = new System.Drawing.Size(236, 27);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UI.SingletonTooltip configurationTooltip;
        public System.Windows.Forms.CheckBox checkBoxPrefixDisabled;
        public System.Windows.Forms.TextBox textBoxDisabledPrefix;

    }
}
