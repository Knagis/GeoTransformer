namespace GeoTransformer.Transformers.ManualPublish
{
    partial class EditorControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
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
            GeoTransformer.UI.SingletonTooltip singletonTooltip;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorControl));
            this.label1 = new System.Windows.Forms.Label();
            this.publishSettings = new System.Windows.Forms.ComboBox();
            singletonTooltip = new GeoTransformer.UI.SingletonTooltip();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Control publish:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            singletonTooltip.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // publishSettings
            // 
            this.publishSettings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.publishSettings.FormattingEnabled = true;
            this.publishSettings.Items.AddRange(new object[] {
            "",
            "Always skip",
            "Always publish"});
            this.publishSettings.Location = new System.Drawing.Point(159, 3);
            this.publishSettings.Name = "publishSettings";
            this.publishSettings.Size = new System.Drawing.Size(200, 21);
            this.publishSettings.TabIndex = 2;
            this.publishSettings.SelectedIndexChanged += new System.EventHandler(this.publishSettings_SelectedIndexChanged);
            // 
            // EditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.publishSettings);
            this.Controls.Add(this.label1);
            this.Name = "EditorControl";
            this.Size = new System.Drawing.Size(362, 27);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox publishSettings;
    }
}
