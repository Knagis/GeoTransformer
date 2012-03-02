namespace GeoTransformer.Transformers.Translator
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
            System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationControl));
            this.checkBoxEnableTranslator = new System.Windows.Forms.CheckBox();
            this.linkLabelClientID = new System.Windows.Forms.LinkLabel();
            this.txtClientId = new System.Windows.Forms.TextBox();
            this.labelClientSecret = new System.Windows.Forms.Label();
            this.txtClientSecret = new System.Windows.Forms.TextBox();
            this.labelTargetLanguage = new System.Windows.Forms.Label();
            this.comboBoxTargetLanguage = new System.Windows.Forms.ComboBox();
            this.labelIgnoreLanguages = new System.Windows.Forms.Label();
            this.comboBoxIgnoreLanguages = new GeoTransformer.UI.CheckedComboBox();
            this.labelTranslateOptions = new System.Windows.Forms.Label();
            this.checkBoxTranslateHints = new System.Windows.Forms.CheckBox();
            this.checkBoxTranslateDescriptions = new System.Windows.Forms.CheckBox();
            this.checkBoxTranslateLogs = new System.Windows.Forms.CheckBox();
            this.singletonTooltip = new GeoTransformer.UI.SingletonTooltip();
            flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.AutoScroll = true;
            flowLayoutPanel2.AutoSize = true;
            flowLayoutPanel2.Controls.Add(this.checkBoxEnableTranslator);
            flowLayoutPanel2.Controls.Add(this.linkLabelClientID);
            flowLayoutPanel2.Controls.Add(this.txtClientId);
            flowLayoutPanel2.Controls.Add(this.labelClientSecret);
            flowLayoutPanel2.Controls.Add(this.txtClientSecret);
            flowLayoutPanel2.Controls.Add(this.labelTargetLanguage);
            flowLayoutPanel2.Controls.Add(this.comboBoxTargetLanguage);
            flowLayoutPanel2.Controls.Add(this.labelIgnoreLanguages);
            flowLayoutPanel2.Controls.Add(this.comboBoxIgnoreLanguages);
            flowLayoutPanel2.Controls.Add(this.labelTranslateOptions);
            flowLayoutPanel2.Controls.Add(this.checkBoxTranslateHints);
            flowLayoutPanel2.Controls.Add(this.checkBoxTranslateDescriptions);
            flowLayoutPanel2.Controls.Add(this.checkBoxTranslateLogs);
            flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new System.Drawing.Size(383, 160);
            flowLayoutPanel2.TabIndex = 2;
            // 
            // checkBoxEnableTranslator
            // 
            this.checkBoxEnableTranslator.AutoSize = true;
            flowLayoutPanel2.SetFlowBreak(this.checkBoxEnableTranslator, true);
            this.checkBoxEnableTranslator.Location = new System.Drawing.Point(3, 3);
            this.checkBoxEnableTranslator.Name = "checkBoxEnableTranslator";
            this.checkBoxEnableTranslator.Size = new System.Drawing.Size(209, 21);
            this.checkBoxEnableTranslator.TabIndex = 0;
            this.checkBoxEnableTranslator.Text = "Enable automatic translation";
            this.singletonTooltip.SetToolTip(this.checkBoxEnableTranslator, resources.GetString("checkBoxEnableTranslator.ToolTip"));
            this.checkBoxEnableTranslator.UseVisualStyleBackColor = true;
            this.checkBoxEnableTranslator.CheckedChanged += new System.EventHandler(this.checkBoxEnableTranslator_CheckedChanged);
            // 
            // linkLabelClientID
            // 
            this.linkLabelClientID.AutoSize = true;
            this.linkLabelClientID.Enabled = false;
            this.linkLabelClientID.LinkArea = new System.Windows.Forms.LinkArea(11, 9);
            this.linkLabelClientID.Location = new System.Drawing.Point(32, 27);
            this.linkLabelClientID.Margin = new System.Windows.Forms.Padding(32, 0, 3, 3);
            this.linkLabelClientID.Name = "linkLabelClientID";
            this.linkLabelClientID.Size = new System.Drawing.Size(133, 20);
            this.linkLabelClientID.TabIndex = 13;
            this.linkLabelClientID.TabStop = true;
            this.linkLabelClientID.Text = "Client ID (read more):";
            this.linkLabelClientID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.singletonTooltip.SetToolTip(this.linkLabelClientID, "This is the Client ID value you have entered when registering\r\nthe application on" +
        " the Azure Marketplace.\r\n\r\nFollow the link to register the application.");
            this.linkLabelClientID.UseCompatibleTextRendering = true;
            this.linkLabelClientID.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelClientID_LinkClicked);
            // 
            // txtClientId
            // 
            this.txtClientId.Enabled = false;
            flowLayoutPanel2.SetFlowBreak(this.txtClientId, true);
            this.txtClientId.Location = new System.Drawing.Point(171, 27);
            this.txtClientId.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.txtClientId.Name = "txtClientId";
            this.txtClientId.Size = new System.Drawing.Size(138, 22);
            this.txtClientId.TabIndex = 10;
            // 
            // labelClientSecret
            // 
            this.labelClientSecret.Enabled = false;
            this.labelClientSecret.Location = new System.Drawing.Point(32, 52);
            this.labelClientSecret.Margin = new System.Windows.Forms.Padding(32, 0, 3, 3);
            this.labelClientSecret.MinimumSize = new System.Drawing.Size(0, 21);
            this.labelClientSecret.Name = "labelClientSecret";
            this.labelClientSecret.Size = new System.Drawing.Size(98, 21);
            this.labelClientSecret.TabIndex = 11;
            this.labelClientSecret.Text = "Client secret:";
            this.labelClientSecret.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.singletonTooltip.SetToolTip(this.labelClientSecret, "This is the Client Secret that is given to you when you register\r\nthe application" +
        " on Azure Marketplace.");
            // 
            // txtClientSecret
            // 
            this.txtClientSecret.Enabled = false;
            flowLayoutPanel2.SetFlowBreak(this.txtClientSecret, true);
            this.txtClientSecret.Location = new System.Drawing.Point(136, 52);
            this.txtClientSecret.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.txtClientSecret.Name = "txtClientSecret";
            this.txtClientSecret.Size = new System.Drawing.Size(153, 22);
            this.txtClientSecret.TabIndex = 12;
            // 
            // labelTargetLanguage
            // 
            this.labelTargetLanguage.Enabled = false;
            this.labelTargetLanguage.Location = new System.Drawing.Point(32, 77);
            this.labelTargetLanguage.Margin = new System.Windows.Forms.Padding(32, 0, 3, 3);
            this.labelTargetLanguage.MinimumSize = new System.Drawing.Size(0, 21);
            this.labelTargetLanguage.Name = "labelTargetLanguage";
            this.labelTargetLanguage.Size = new System.Drawing.Size(98, 21);
            this.labelTargetLanguage.TabIndex = 1;
            this.labelTargetLanguage.Text = "Target language:";
            this.labelTargetLanguage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.singletonTooltip.SetToolTip(this.labelTargetLanguage, "This is the target language for the translation.\r\n\r\nIt is recommended to use Engl" +
        "ish as that increases the\r\nprobability of correct translation.");
            // 
            // comboBoxTargetLanguage
            // 
            this.comboBoxTargetLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTargetLanguage.Enabled = false;
            flowLayoutPanel2.SetFlowBreak(this.comboBoxTargetLanguage, true);
            this.comboBoxTargetLanguage.FormattingEnabled = true;
            this.comboBoxTargetLanguage.Location = new System.Drawing.Point(136, 77);
            this.comboBoxTargetLanguage.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.comboBoxTargetLanguage.Name = "comboBoxTargetLanguage";
            this.comboBoxTargetLanguage.Size = new System.Drawing.Size(153, 24);
            this.comboBoxTargetLanguage.TabIndex = 2;
            // 
            // labelIgnoreLanguages
            // 
            this.labelIgnoreLanguages.Enabled = false;
            this.labelIgnoreLanguages.Location = new System.Drawing.Point(32, 107);
            this.labelIgnoreLanguages.Margin = new System.Windows.Forms.Padding(32, 3, 3, 3);
            this.labelIgnoreLanguages.MinimumSize = new System.Drawing.Size(0, 21);
            this.labelIgnoreLanguages.Name = "labelIgnoreLanguages";
            this.labelIgnoreLanguages.Size = new System.Drawing.Size(98, 21);
            this.labelIgnoreLanguages.TabIndex = 3;
            this.labelIgnoreLanguages.Text = "Ignore languages:";
            this.labelIgnoreLanguages.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.singletonTooltip.SetToolTip(this.labelIgnoreLanguages, resources.GetString("labelIgnoreLanguages.ToolTip"));
            // 
            // comboBoxIgnoreLanguages
            // 
            this.comboBoxIgnoreLanguages.CheckOnClick = true;
            this.comboBoxIgnoreLanguages.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.comboBoxIgnoreLanguages.DropDownHeight = 1;
            this.comboBoxIgnoreLanguages.Enabled = false;
            flowLayoutPanel2.SetFlowBreak(this.comboBoxIgnoreLanguages, true);
            this.comboBoxIgnoreLanguages.FormattingEnabled = true;
            this.comboBoxIgnoreLanguages.IntegralHeight = false;
            this.comboBoxIgnoreLanguages.Location = new System.Drawing.Point(136, 107);
            this.comboBoxIgnoreLanguages.MaxDropDownItems = 15;
            this.comboBoxIgnoreLanguages.Name = "comboBoxIgnoreLanguages";
            this.comboBoxIgnoreLanguages.Size = new System.Drawing.Size(153, 23);
            this.comboBoxIgnoreLanguages.TabIndex = 4;
            this.comboBoxIgnoreLanguages.ValueSeparator = ", ";
            // 
            // labelTranslateOptions
            // 
            this.labelTranslateOptions.Enabled = false;
            this.labelTranslateOptions.Location = new System.Drawing.Point(32, 136);
            this.labelTranslateOptions.Margin = new System.Windows.Forms.Padding(32, 3, 3, 3);
            this.labelTranslateOptions.MinimumSize = new System.Drawing.Size(0, 21);
            this.labelTranslateOptions.Name = "labelTranslateOptions";
            this.labelTranslateOptions.Size = new System.Drawing.Size(98, 21);
            this.labelTranslateOptions.TabIndex = 5;
            this.labelTranslateOptions.Text = "Translate:";
            this.labelTranslateOptions.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkBoxTranslateHints
            // 
            this.checkBoxTranslateHints.AutoSize = true;
            this.checkBoxTranslateHints.Checked = true;
            this.checkBoxTranslateHints.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxTranslateHints.Enabled = false;
            this.checkBoxTranslateHints.Location = new System.Drawing.Point(136, 136);
            this.checkBoxTranslateHints.MinimumSize = new System.Drawing.Size(0, 21);
            this.checkBoxTranslateHints.Name = "checkBoxTranslateHints";
            this.checkBoxTranslateHints.Size = new System.Drawing.Size(62, 21);
            this.checkBoxTranslateHints.TabIndex = 6;
            this.checkBoxTranslateHints.Text = "Hints";
            this.checkBoxTranslateHints.UseVisualStyleBackColor = true;
            // 
            // checkBoxTranslateDescriptions
            // 
            this.checkBoxTranslateDescriptions.AutoSize = true;
            this.checkBoxTranslateDescriptions.Checked = true;
            this.checkBoxTranslateDescriptions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxTranslateDescriptions.Enabled = false;
            this.checkBoxTranslateDescriptions.Location = new System.Drawing.Point(204, 136);
            this.checkBoxTranslateDescriptions.MinimumSize = new System.Drawing.Size(0, 21);
            this.checkBoxTranslateDescriptions.Name = "checkBoxTranslateDescriptions";
            this.checkBoxTranslateDescriptions.Size = new System.Drawing.Size(108, 21);
            this.checkBoxTranslateDescriptions.TabIndex = 7;
            this.checkBoxTranslateDescriptions.Text = "Descriptions";
            this.singletonTooltip.SetToolTip(this.checkBoxTranslateDescriptions, "If this is checked then all cache and waypoint descriptions\r\nwill be translated. " +
        "Note that if the text is longer than 10240\r\ncharacters, only the first 10240 cha" +
        "racters will be translated.");
            this.checkBoxTranslateDescriptions.UseVisualStyleBackColor = true;
            // 
            // checkBoxTranslateLogs
            // 
            this.checkBoxTranslateLogs.AutoSize = true;
            this.checkBoxTranslateLogs.Enabled = false;
            flowLayoutPanel2.SetFlowBreak(this.checkBoxTranslateLogs, true);
            this.checkBoxTranslateLogs.Location = new System.Drawing.Point(318, 136);
            this.checkBoxTranslateLogs.MinimumSize = new System.Drawing.Size(0, 21);
            this.checkBoxTranslateLogs.Name = "checkBoxTranslateLogs";
            this.checkBoxTranslateLogs.Size = new System.Drawing.Size(61, 21);
            this.checkBoxTranslateLogs.TabIndex = 8;
            this.checkBoxTranslateLogs.Text = "Logs";
            this.singletonTooltip.SetToolTip(this.checkBoxTranslateLogs, "Translating all logs can be very time consuming so it is not\r\nrecommended to enab" +
        "le this option.");
            this.checkBoxTranslateLogs.UseVisualStyleBackColor = true;
            // 
            // ConfigurationControl
            // 
            this.Controls.Add(flowLayoutPanel2);
            this.Name = "ConfigurationControl";
            this.Size = new System.Drawing.Size(383, 160);
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTargetLanguage;
        private System.Windows.Forms.ComboBox comboBoxTargetLanguage;
        private System.Windows.Forms.Label labelIgnoreLanguages;
        private UI.CheckedComboBox comboBoxIgnoreLanguages;
        private System.Windows.Forms.Label labelTranslateOptions;
        private System.Windows.Forms.CheckBox checkBoxTranslateHints;
        private System.Windows.Forms.CheckBox checkBoxTranslateDescriptions;
        private System.Windows.Forms.CheckBox checkBoxTranslateLogs;
        private System.Windows.Forms.CheckBox checkBoxEnableTranslator;
        private UI.SingletonTooltip singletonTooltip;
        private System.Windows.Forms.Label labelClientSecret;
        private System.Windows.Forms.TextBox txtClientSecret;
        private System.Windows.Forms.TextBox txtClientId;
        private System.Windows.Forms.LinkLabel linkLabelClientID;

    }
}
