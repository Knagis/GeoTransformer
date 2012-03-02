namespace GeoTransformer.Extensions.GeocachingService
{
    partial class WizardControl
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
            this.buttonAuthenticate = new System.Windows.Forms.Button();
            this.pictureBoxStatusBar = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatusBar)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonAuthenticate
            // 
            this.buttonAuthenticate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAuthenticate.Location = new System.Drawing.Point(3, 7);
            this.buttonAuthenticate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonAuthenticate.Name = "buttonAuthenticate";
            this.buttonAuthenticate.Size = new System.Drawing.Size(397, 52);
            this.buttonAuthenticate.TabIndex = 0;
            this.buttonAuthenticate.Text = "Authenticate using geocaching.com Live API";
            this.buttonAuthenticate.UseVisualStyleBackColor = true;
            this.buttonAuthenticate.Click += new System.EventHandler(this.buttonAuthenticate_Click);
            // 
            // pictureBoxStatusBar
            // 
            this.pictureBoxStatusBar.Location = new System.Drawing.Point(415, 2);
            this.pictureBoxStatusBar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBoxStatusBar.Name = "pictureBoxStatusBar";
            this.pictureBoxStatusBar.Size = new System.Drawing.Size(267, 62);
            this.pictureBoxStatusBar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxStatusBar.TabIndex = 16;
            this.pictureBoxStatusBar.TabStop = false;
            // 
            // WizardControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBoxStatusBar);
            this.Controls.Add(this.buttonAuthenticate);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "WizardControl";
            this.Size = new System.Drawing.Size(693, 66);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatusBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonAuthenticate;
        private System.Windows.Forms.PictureBox pictureBoxStatusBar;
    }
}
