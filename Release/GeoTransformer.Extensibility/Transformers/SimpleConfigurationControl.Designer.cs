namespace GeoTransformer.Transformers
{
    partial class SimpleConfigurationControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleConfigurationControl));
            this.checkBoxEnabled = new System.Windows.Forms.CheckBox();
            this.configurationTooltip = new GeoTransformer.UI.SingletonTooltip();
            this.SuspendLayout();
            // 
            // checkBoxEnabled
            // 
            this.checkBoxEnabled.AutoSize = true;
            this.checkBoxEnabled.Checked = true;
            this.checkBoxEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxEnabled.Location = new System.Drawing.Point(3, 3);
            this.checkBoxEnabled.Name = "checkBoxEnabled";
            this.checkBoxEnabled.Size = new System.Drawing.Size(100, 17);
            this.checkBoxEnabled.TabIndex = 2;
            this.checkBoxEnabled.Text = "Merge .gpx files";
            this.configurationTooltip.SetToolTip(this.checkBoxEnabled, resources.GetString("checkBoxEnabled.ToolTip"));
            this.checkBoxEnabled.UseVisualStyleBackColor = true;
            // 
            // SimpleConfigurationControl
            // 
            this.AutoSize = true;
            this.Controls.Add(this.checkBoxEnabled);
            this.Name = "SimpleConfigurationControl";
            this.Size = new System.Drawing.Size(106, 23);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UI.SingletonTooltip configurationTooltip;
        public System.Windows.Forms.CheckBox checkBoxEnabled;

    }
}
