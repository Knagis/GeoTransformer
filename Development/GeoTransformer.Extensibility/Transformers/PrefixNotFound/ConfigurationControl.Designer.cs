namespace GeoTransformer.Transformers.PrefixNotFound
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
            this.checkBoxPrefix = new System.Windows.Forms.CheckBox();
            this.textBoxPrefix = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // checkBoxPrefix
            // 
            this.checkBoxPrefix.AutoSize = true;
            this.checkBoxPrefix.Checked = true;
            this.checkBoxPrefix.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPrefix.Location = new System.Drawing.Point(3, 3);
            this.checkBoxPrefix.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.checkBoxPrefix.Name = "checkBoxPrefix";
            this.checkBoxPrefix.Size = new System.Drawing.Size(149, 17);
            this.checkBoxPrefix.TabIndex = 7;
            this.checkBoxPrefix.Text = "Prefix DNFed caches with";
            this.configurationTooltip.SetToolTip(this.checkBoxPrefix, "Prefixes caches where last logs are DNFs and N/M with\r\nthe specified text. In the" +
        " text the question mark (?) is\r\nreplaced with the number of such logs after the " +
        "last positive\r\nlog.");
            this.checkBoxPrefix.UseVisualStyleBackColor = true;
            this.checkBoxPrefix.CheckedChanged += new System.EventHandler(this.checkBoxPrefixDisabled_CheckedChanged);
            // 
            // textBoxPrefix
            // 
            this.textBoxPrefix.Location = new System.Drawing.Point(158, 1);
            this.textBoxPrefix.Margin = new System.Windows.Forms.Padding(1);
            this.textBoxPrefix.Name = "textBoxPrefix";
            this.textBoxPrefix.Size = new System.Drawing.Size(77, 20);
            this.textBoxPrefix.TabIndex = 8;
            this.textBoxPrefix.Text = "[?DNF]";
            this.textBoxPrefix.TextChanged += new System.EventHandler(this.textBoxDisabledPrefix_TextChanged);
            // 
            // ConfigurationControl
            // 
            this.Controls.Add(this.checkBoxPrefix);
            this.Controls.Add(this.textBoxPrefix);
            this.Name = "ConfigurationControl";
            this.Size = new System.Drawing.Size(236, 23);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UI.SingletonTooltip configurationTooltip;
        public System.Windows.Forms.CheckBox checkBoxPrefix;
        public System.Windows.Forms.TextBox textBoxPrefix;

    }
}
