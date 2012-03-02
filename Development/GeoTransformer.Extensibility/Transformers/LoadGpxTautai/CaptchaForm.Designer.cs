namespace GeoTransformer.Transformers.LoadGpxTautai
{
    partial class CaptchaForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CaptchaForm));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripClose = new System.Windows.Forms.ToolStripButton();
            this.toolStripOK = new System.Windows.Forms.ToolStripButton();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripClose,
            this.toolStripOK});
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip.Location = new System.Drawing.Point(0, 215);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip.Size = new System.Drawing.Size(500, 31);
            this.toolStrip.TabIndex = 4;
            this.toolStrip.Text = "toolStrip";
            // 
            // toolStripClose
            // 
            this.toolStripClose.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripClose.Image = ((System.Drawing.Image)(resources.GetObject("toolStripClose.Image")));
            this.toolStripClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripClose.Name = "toolStripClose";
            this.toolStripClose.Size = new System.Drawing.Size(81, 28);
            this.toolStripClose.Text = "Cancel";
            this.toolStripClose.Click += new System.EventHandler(this.toolStripClose_Click);
            // 
            // toolStripOK
            // 
            this.toolStripOK.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripOK.Image = global::GeoTransformer.Transformers.LoadGpxTautai.Resources.Apply;
            this.toolStripOK.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripOK.Name = "toolStripOK";
            this.toolStripOK.Size = new System.Drawing.Size(96, 28);
            this.toolStripOK.Text = "Continue";
            this.toolStripOK.Click += new System.EventHandler(this.toolStripOK_Click);
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.ScrollBarsEnabled = false;
            this.webBrowser.Size = new System.Drawing.Size(500, 215);
            this.webBrowser.TabIndex = 5;
            // 
            // CaptchaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 246);
            this.ControlBox = false;
            this.Controls.Add(this.webBrowser);
            this.Controls.Add(this.toolStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CaptchaForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Enter Captcha";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CaptchaForm_KeyPress);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripClose;
        private System.Windows.Forms.ToolStripButton toolStripOK;
        private System.Windows.Forms.WebBrowser webBrowser;
    }
}