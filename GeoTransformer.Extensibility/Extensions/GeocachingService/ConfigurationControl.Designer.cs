namespace GeoTransformer.Extensions.GeocachingService
{
    partial class ConfigurationControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationControl));
            this.singletonTooltip = new GeoTransformer.UI.SingletonTooltip();
            this.checkBoxEnable = new System.Windows.Forms.CheckBox();
            this.linkAuthenticate = new System.Windows.Forms.LinkLabel();
            this.labelUsername = new System.Windows.Forms.Label();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelUsernameValue = new System.Windows.Forms.Label();
            this.pictureBoxAvatar = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAvatar)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxEnable
            // 
            this.checkBoxEnable.AutoSize = true;
            this.flowLayoutPanel3.SetFlowBreak(this.checkBoxEnable, true);
            this.checkBoxEnable.Location = new System.Drawing.Point(4, 4);
            this.checkBoxEnable.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxEnable.Name = "checkBoxEnable";
            this.checkBoxEnable.Size = new System.Drawing.Size(236, 21);
            this.checkBoxEnable.TabIndex = 1;
            this.checkBoxEnable.Text = "Enable geocaching.com Live API";
            this.singletonTooltip.SetToolTip(this.checkBoxEnable, resources.GetString("checkBoxEnable.ToolTip"));
            this.checkBoxEnable.UseVisualStyleBackColor = true;
            this.checkBoxEnable.CheckedChanged += new System.EventHandler(this.checkBoxEnable_CheckedChanged);
            // 
            // linkAuthenticate
            // 
            this.linkAuthenticate.AutoSize = true;
            this.linkAuthenticate.Enabled = false;
            this.flowLayoutPanel3.SetFlowBreak(this.linkAuthenticate, true);
            this.linkAuthenticate.Location = new System.Drawing.Point(43, 29);
            this.linkAuthenticate.Margin = new System.Windows.Forms.Padding(43, 0, 4, 4);
            this.linkAuthenticate.MinimumSize = new System.Drawing.Size(0, 26);
            this.linkAuthenticate.Name = "linkAuthenticate";
            this.linkAuthenticate.Size = new System.Drawing.Size(222, 26);
            this.linkAuthenticate.TabIndex = 7;
            this.linkAuthenticate.TabStop = true;
            this.linkAuthenticate.Text = "Authenticate with geocaching.com";
            this.linkAuthenticate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.singletonTooltip.SetToolTip(this.linkAuthenticate, resources.GetString("linkAuthenticate.ToolTip"));
            this.linkAuthenticate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkAuthenticate_LinkClicked);
            // 
            // labelUsername
            // 
            this.labelUsername.Enabled = false;
            this.labelUsername.Location = new System.Drawing.Point(43, 59);
            this.labelUsername.Margin = new System.Windows.Forms.Padding(43, 0, 4, 4);
            this.labelUsername.MinimumSize = new System.Drawing.Size(0, 26);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(131, 26);
            this.labelUsername.TabIndex = 13;
            this.labelUsername.Text = "Username:";
            this.labelUsername.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.singletonTooltip.SetToolTip(this.labelUsername, "This is the Client Secret that is given to you when you register\r\nthe application" +
        " on Azure Marketplace.");
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.checkBoxEnable);
            this.flowLayoutPanel3.Controls.Add(this.linkAuthenticate);
            this.flowLayoutPanel3.Controls.Add(this.labelUsername);
            this.flowLayoutPanel3.Controls.Add(this.labelUsernameValue);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(427, 95);
            this.flowLayoutPanel3.TabIndex = 4;
            // 
            // labelUsernameValue
            // 
            this.labelUsernameValue.AutoSize = true;
            this.labelUsernameValue.Enabled = false;
            this.labelUsernameValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.labelUsernameValue.Location = new System.Drawing.Point(182, 59);
            this.labelUsernameValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelUsernameValue.MinimumSize = new System.Drawing.Size(0, 26);
            this.labelUsernameValue.Name = "labelUsernameValue";
            this.labelUsernameValue.Size = new System.Drawing.Size(118, 26);
            this.labelUsernameValue.TabIndex = 14;
            this.labelUsernameValue.Text = "not authenticated";
            this.labelUsernameValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBoxAvatar
            // 
            this.pictureBoxAvatar.Location = new System.Drawing.Point(332, 4);
            this.pictureBoxAvatar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBoxAvatar.Name = "pictureBoxAvatar";
            this.pictureBoxAvatar.Size = new System.Drawing.Size(64, 59);
            this.pictureBoxAvatar.TabIndex = 15;
            this.pictureBoxAvatar.TabStop = false;
            // 
            // ConfigurationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBoxAvatar);
            this.Controls.Add(this.flowLayoutPanel3);
            this.MinimumSize = new System.Drawing.Size(40, 12);
            this.Name = "ConfigurationControl";
            this.Size = new System.Drawing.Size(427, 95);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAvatar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private UI.SingletonTooltip singletonTooltip;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.CheckBox checkBoxEnable;
        private System.Windows.Forms.LinkLabel linkAuthenticate;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.Label labelUsernameValue;
        private System.Windows.Forms.PictureBox pictureBoxAvatar;
    }
}
