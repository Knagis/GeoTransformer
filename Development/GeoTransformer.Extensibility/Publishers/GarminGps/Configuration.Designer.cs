namespace GeoTransformer.Publishers.GarminGps
{
    partial class Configuration
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
            System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Configuration));
            this.checkBoxEnableImages = new System.Windows.Forms.CheckBox();
            this.labelMaximumSize = new System.Windows.Forms.Label();
            this.txtMaximumSize = new System.Windows.Forms.NumericUpDown();
            this.chkPublishLogImages = new System.Windows.Forms.CheckBox();
            this.chkRemoveOtherImages = new System.Windows.Forms.CheckBox();
            this.singletonTooltip = new GeoTransformer.UI.SingletonTooltip();
            flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaximumSize)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.AutoScroll = true;
            flowLayoutPanel2.AutoSize = true;
            flowLayoutPanel2.Controls.Add(this.checkBoxEnableImages);
            flowLayoutPanel2.Controls.Add(this.labelMaximumSize);
            flowLayoutPanel2.Controls.Add(this.txtMaximumSize);
            flowLayoutPanel2.Controls.Add(this.chkPublishLogImages);
            flowLayoutPanel2.Controls.Add(this.chkRemoveOtherImages);
            flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new System.Drawing.Size(432, 106);
            flowLayoutPanel2.TabIndex = 3;
            // 
            // checkBoxEnableImages
            // 
            this.checkBoxEnableImages.AutoSize = true;
            this.checkBoxEnableImages.Checked = true;
            this.checkBoxEnableImages.CheckState = System.Windows.Forms.CheckState.Checked;
            flowLayoutPanel2.SetFlowBreak(this.checkBoxEnableImages, true);
            this.checkBoxEnableImages.Location = new System.Drawing.Point(3, 3);
            this.checkBoxEnableImages.Name = "checkBoxEnableImages";
            this.checkBoxEnableImages.Size = new System.Drawing.Size(249, 21);
            this.checkBoxEnableImages.TabIndex = 0;
            this.checkBoxEnableImages.Text = "Enable automatic image publishing";
            this.singletonTooltip.SetToolTip(this.checkBoxEnableImages, resources.GetString("checkBoxEnableImages.ToolTip"));
            this.checkBoxEnableImages.UseVisualStyleBackColor = true;
            this.checkBoxEnableImages.CheckedChanged += new System.EventHandler(this.checkBoxEnableImages_CheckedChanged);
            // 
            // labelMaximumSize
            // 
            this.labelMaximumSize.Location = new System.Drawing.Point(43, 27);
            this.labelMaximumSize.Margin = new System.Windows.Forms.Padding(43, 0, 3, 3);
            this.labelMaximumSize.MinimumSize = new System.Drawing.Size(0, 21);
            this.labelMaximumSize.Name = "labelMaximumSize";
            this.labelMaximumSize.Size = new System.Drawing.Size(131, 21);
            this.labelMaximumSize.TabIndex = 11;
            this.labelMaximumSize.Text = "Maximum size:";
            this.labelMaximumSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.singletonTooltip.SetToolTip(this.labelMaximumSize, resources.GetString("labelMaximumSize.ToolTip"));
            // 
            // txtMaximumSize
            // 
            flowLayoutPanel2.SetFlowBreak(this.txtMaximumSize, true);
            this.txtMaximumSize.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtMaximumSize.Location = new System.Drawing.Point(180, 27);
            this.txtMaximumSize.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.txtMaximumSize.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.txtMaximumSize.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txtMaximumSize.Name = "txtMaximumSize";
            this.txtMaximumSize.Size = new System.Drawing.Size(82, 22);
            this.txtMaximumSize.TabIndex = 13;
            this.txtMaximumSize.Value = new decimal(new int[] {
            600,
            0,
            0,
            0});
            // 
            // chkPublishLogImages
            // 
            this.chkPublishLogImages.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            flowLayoutPanel2.SetFlowBreak(this.chkPublishLogImages, true);
            this.chkPublishLogImages.Location = new System.Drawing.Point(5, 55);
            this.chkPublishLogImages.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.chkPublishLogImages.MinimumSize = new System.Drawing.Size(0, 21);
            this.chkPublishLogImages.Name = "chkPublishLogImages";
            this.chkPublishLogImages.Size = new System.Drawing.Size(191, 21);
            this.chkPublishLogImages.TabIndex = 6;
            this.chkPublishLogImages.Text = "Publish images from logs:";
            this.chkPublishLogImages.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.singletonTooltip.SetToolTip(this.chkPublishLogImages, resources.GetString("chkPublishLogImages.ToolTip"));
            this.chkPublishLogImages.UseVisualStyleBackColor = true;
            // 
            // chkRemoveOtherImages
            // 
            this.chkRemoveOtherImages.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkRemoveOtherImages.Checked = true;
            this.chkRemoveOtherImages.CheckState = System.Windows.Forms.CheckState.Checked;
            flowLayoutPanel2.SetFlowBreak(this.chkRemoveOtherImages, true);
            this.chkRemoveOtherImages.Location = new System.Drawing.Point(5, 82);
            this.chkRemoveOtherImages.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.chkRemoveOtherImages.MinimumSize = new System.Drawing.Size(0, 21);
            this.chkRemoveOtherImages.Name = "chkRemoveOtherImages";
            this.chkRemoveOtherImages.Size = new System.Drawing.Size(191, 21);
            this.chkRemoveOtherImages.TabIndex = 14;
            this.chkRemoveOtherImages.Text = "Remove existing images:";
            this.chkRemoveOtherImages.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.singletonTooltip.SetToolTip(this.chkRemoveOtherImages, resources.GetString("chkRemoveOtherImages.ToolTip"));
            this.chkRemoveOtherImages.UseVisualStyleBackColor = true;
            // 
            // Configuration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(flowLayoutPanel2);
            this.Name = "Configuration";
            this.Size = new System.Drawing.Size(432, 106);
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaximumSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxEnableImages;
        private System.Windows.Forms.Label labelMaximumSize;
        private System.Windows.Forms.CheckBox chkPublishLogImages;
        private System.Windows.Forms.NumericUpDown txtMaximumSize;
        private UI.SingletonTooltip singletonTooltip;
        private System.Windows.Forms.CheckBox chkRemoveOtherImages;
    }
}
