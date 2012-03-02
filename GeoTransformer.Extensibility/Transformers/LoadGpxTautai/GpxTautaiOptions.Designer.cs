namespace GeoTransformer.Transformers.LoadGpxTautai
{
    partial class GpxTautaiOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GpxTautaiOptions));
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxEnableGpxTautai = new System.Windows.Forms.CheckBox();
            this.labelGpxTautaiCacheTypes = new System.Windows.Forms.Label();
            this.comboBoxGpxTautaiCacheTypes = new GeoTransformer.UI.CheckedComboBox();
            this.labelGpxTautaiDifficulty = new System.Windows.Forms.Label();
            this.comboBoxGpxTautaiDifficultyMin = new System.Windows.Forms.ComboBox();
            this.labelGpxTautaiDifficultyTo = new System.Windows.Forms.Label();
            this.comboBoxGpxTautaiDifficultyMax = new System.Windows.Forms.ComboBox();
            this.labelGpxTautaiTerrain = new System.Windows.Forms.Label();
            this.comboBoxGpxTautaiTerrainMin = new System.Windows.Forms.ComboBox();
            this.labelGpxTautaiTerrainTo = new System.Windows.Forms.Label();
            this.comboBoxGpxTautaiTerrainMax = new System.Windows.Forms.ComboBox();
            this.labelGpxTautaiDownload = new System.Windows.Forms.Label();
            this.checkBoxGpxTautaiDownloadActive = new System.Windows.Forms.CheckBox();
            this.checkBoxGpxTautaiDownloadDisabled = new System.Windows.Forms.CheckBox();
            this.checkBoxGpxTautaiDownloadArchived = new System.Windows.Forms.CheckBox();
            this.labelGpxTautaiUserName = new System.Windows.Forms.Label();
            this.textBoxGpxTautaiUserName = new System.Windows.Forms.TextBox();
            this.labelGpxTautaiIncludeLogs = new System.Windows.Forms.Label();
            this.checkBoxGpxTautaiIncludeLogs = new System.Windows.Forms.CheckBox();
            this.textBoxGpxTautaiMaxLogs = new System.Windows.Forms.MaskedTextBox();
            this.labelGpxTautaiMaxLogs = new System.Windows.Forms.Label();
            this.toolTipForOptions = new GeoTransformer.UI.SingletonTooltip();
            this.flowLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.checkBoxEnableGpxTautai);
            this.flowLayoutPanel3.Controls.Add(this.labelGpxTautaiCacheTypes);
            this.flowLayoutPanel3.Controls.Add(this.comboBoxGpxTautaiCacheTypes);
            this.flowLayoutPanel3.Controls.Add(this.labelGpxTautaiDifficulty);
            this.flowLayoutPanel3.Controls.Add(this.comboBoxGpxTautaiDifficultyMin);
            this.flowLayoutPanel3.Controls.Add(this.labelGpxTautaiDifficultyTo);
            this.flowLayoutPanel3.Controls.Add(this.comboBoxGpxTautaiDifficultyMax);
            this.flowLayoutPanel3.Controls.Add(this.labelGpxTautaiTerrain);
            this.flowLayoutPanel3.Controls.Add(this.comboBoxGpxTautaiTerrainMin);
            this.flowLayoutPanel3.Controls.Add(this.labelGpxTautaiTerrainTo);
            this.flowLayoutPanel3.Controls.Add(this.comboBoxGpxTautaiTerrainMax);
            this.flowLayoutPanel3.Controls.Add(this.labelGpxTautaiDownload);
            this.flowLayoutPanel3.Controls.Add(this.checkBoxGpxTautaiDownloadActive);
            this.flowLayoutPanel3.Controls.Add(this.checkBoxGpxTautaiDownloadDisabled);
            this.flowLayoutPanel3.Controls.Add(this.checkBoxGpxTautaiDownloadArchived);
            this.flowLayoutPanel3.Controls.Add(this.labelGpxTautaiUserName);
            this.flowLayoutPanel3.Controls.Add(this.textBoxGpxTautaiUserName);
            this.flowLayoutPanel3.Controls.Add(this.labelGpxTautaiIncludeLogs);
            this.flowLayoutPanel3.Controls.Add(this.checkBoxGpxTautaiIncludeLogs);
            this.flowLayoutPanel3.Controls.Add(this.textBoxGpxTautaiMaxLogs);
            this.flowLayoutPanel3.Controls.Add(this.labelGpxTautaiMaxLogs);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel3.MinimumSize = new System.Drawing.Size(0, 186);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(347, 186);
            this.flowLayoutPanel3.TabIndex = 2;
            // 
            // checkBoxEnableGpxTautai
            // 
            this.checkBoxEnableGpxTautai.AutoSize = true;
            this.flowLayoutPanel3.SetFlowBreak(this.checkBoxEnableGpxTautai, true);
            this.checkBoxEnableGpxTautai.Location = new System.Drawing.Point(3, 3);
            this.checkBoxEnableGpxTautai.Name = "checkBoxEnableGpxTautai";
            this.checkBoxEnableGpxTautai.Size = new System.Drawing.Size(343, 17);
            this.checkBoxEnableGpxTautai.TabIndex = 1;
            this.checkBoxEnableGpxTautai.Text = "Enable automatic download from GPX Tautai (geocaches in Latvia)";
            this.toolTipForOptions.SetToolTip(this.checkBoxEnableGpxTautai, resources.GetString("checkBoxEnableGpxTautai.ToolTip"));
            this.checkBoxEnableGpxTautai.UseVisualStyleBackColor = true;
            this.checkBoxEnableGpxTautai.CheckedChanged += new System.EventHandler(this.checkBoxEnableGpxTautai_CheckedChanged);
            // 
            // labelGpxTautaiCacheTypes
            // 
            this.labelGpxTautaiCacheTypes.Enabled = false;
            this.labelGpxTautaiCacheTypes.Location = new System.Drawing.Point(32, 23);
            this.labelGpxTautaiCacheTypes.Margin = new System.Windows.Forms.Padding(32, 0, 3, 3);
            this.labelGpxTautaiCacheTypes.MinimumSize = new System.Drawing.Size(0, 21);
            this.labelGpxTautaiCacheTypes.Name = "labelGpxTautaiCacheTypes";
            this.labelGpxTautaiCacheTypes.Size = new System.Drawing.Size(98, 21);
            this.labelGpxTautaiCacheTypes.TabIndex = 3;
            this.labelGpxTautaiCacheTypes.Text = "Cache types:";
            this.labelGpxTautaiCacheTypes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTipForOptions.SetToolTip(this.labelGpxTautaiCacheTypes, "Select the cache types you want to be included in the download.");
            // 
            // comboBoxGpxTautaiCacheTypes
            // 
            this.comboBoxGpxTautaiCacheTypes.CheckOnClick = true;
            this.comboBoxGpxTautaiCacheTypes.DisplayMember = "Item2";
            this.comboBoxGpxTautaiCacheTypes.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.comboBoxGpxTautaiCacheTypes.DropDownHeight = 1;
            this.comboBoxGpxTautaiCacheTypes.DropDownWidth = 200;
            this.comboBoxGpxTautaiCacheTypes.Enabled = false;
            this.flowLayoutPanel3.SetFlowBreak(this.comboBoxGpxTautaiCacheTypes, true);
            this.comboBoxGpxTautaiCacheTypes.FormattingEnabled = true;
            this.comboBoxGpxTautaiCacheTypes.IntegralHeight = false;
            this.comboBoxGpxTautaiCacheTypes.Location = new System.Drawing.Point(136, 23);
            this.comboBoxGpxTautaiCacheTypes.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.comboBoxGpxTautaiCacheTypes.MaxDropDownItems = 10;
            this.comboBoxGpxTautaiCacheTypes.Name = "comboBoxGpxTautaiCacheTypes";
            this.comboBoxGpxTautaiCacheTypes.Size = new System.Drawing.Size(153, 21);
            this.comboBoxGpxTautaiCacheTypes.TabIndex = 5;
            this.comboBoxGpxTautaiCacheTypes.ValueMember = "Item1";
            this.comboBoxGpxTautaiCacheTypes.ValueSeparator = ", ";
            // 
            // labelGpxTautaiDifficulty
            // 
            this.labelGpxTautaiDifficulty.Enabled = false;
            this.labelGpxTautaiDifficulty.Location = new System.Drawing.Point(32, 50);
            this.labelGpxTautaiDifficulty.Margin = new System.Windows.Forms.Padding(32, 3, 3, 3);
            this.labelGpxTautaiDifficulty.MinimumSize = new System.Drawing.Size(0, 21);
            this.labelGpxTautaiDifficulty.Name = "labelGpxTautaiDifficulty";
            this.labelGpxTautaiDifficulty.Size = new System.Drawing.Size(98, 21);
            this.labelGpxTautaiDifficulty.TabIndex = 20;
            this.labelGpxTautaiDifficulty.Text = "Difficulty:";
            this.labelGpxTautaiDifficulty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTipForOptions.SetToolTip(this.labelGpxTautaiDifficulty, "Use these fields to filter the caches by their difficulty.");
            // 
            // comboBoxGpxTautaiDifficultyMin
            // 
            this.comboBoxGpxTautaiDifficultyMin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGpxTautaiDifficultyMin.Enabled = false;
            this.comboBoxGpxTautaiDifficultyMin.FormattingEnabled = true;
            this.comboBoxGpxTautaiDifficultyMin.Items.AddRange(new object[] {
            "1",
            "1.5",
            "2",
            "2.5",
            "3",
            "3.5",
            "4",
            "4.5",
            "5"});
            this.comboBoxGpxTautaiDifficultyMin.Location = new System.Drawing.Point(136, 50);
            this.comboBoxGpxTautaiDifficultyMin.Name = "comboBoxGpxTautaiDifficultyMin";
            this.comboBoxGpxTautaiDifficultyMin.Size = new System.Drawing.Size(49, 21);
            this.comboBoxGpxTautaiDifficultyMin.TabIndex = 23;
            // 
            // labelGpxTautaiDifficultyTo
            // 
            this.labelGpxTautaiDifficultyTo.AutoSize = true;
            this.labelGpxTautaiDifficultyTo.Enabled = false;
            this.labelGpxTautaiDifficultyTo.Location = new System.Drawing.Point(191, 50);
            this.labelGpxTautaiDifficultyTo.Margin = new System.Windows.Forms.Padding(3);
            this.labelGpxTautaiDifficultyTo.MinimumSize = new System.Drawing.Size(0, 21);
            this.labelGpxTautaiDifficultyTo.Name = "labelGpxTautaiDifficultyTo";
            this.labelGpxTautaiDifficultyTo.Size = new System.Drawing.Size(16, 21);
            this.labelGpxTautaiDifficultyTo.TabIndex = 24;
            this.labelGpxTautaiDifficultyTo.Text = "to";
            this.labelGpxTautaiDifficultyTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxGpxTautaiDifficultyMax
            // 
            this.comboBoxGpxTautaiDifficultyMax.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGpxTautaiDifficultyMax.Enabled = false;
            this.flowLayoutPanel3.SetFlowBreak(this.comboBoxGpxTautaiDifficultyMax, true);
            this.comboBoxGpxTautaiDifficultyMax.FormattingEnabled = true;
            this.comboBoxGpxTautaiDifficultyMax.Items.AddRange(new object[] {
            "1",
            "1.5",
            "2",
            "2.5",
            "3",
            "3.5",
            "4",
            "4.5",
            "5"});
            this.comboBoxGpxTautaiDifficultyMax.Location = new System.Drawing.Point(213, 50);
            this.comboBoxGpxTautaiDifficultyMax.Name = "comboBoxGpxTautaiDifficultyMax";
            this.comboBoxGpxTautaiDifficultyMax.Size = new System.Drawing.Size(49, 21);
            this.comboBoxGpxTautaiDifficultyMax.TabIndex = 25;
            // 
            // labelGpxTautaiTerrain
            // 
            this.labelGpxTautaiTerrain.Enabled = false;
            this.labelGpxTautaiTerrain.Location = new System.Drawing.Point(32, 77);
            this.labelGpxTautaiTerrain.Margin = new System.Windows.Forms.Padding(32, 3, 3, 3);
            this.labelGpxTautaiTerrain.MinimumSize = new System.Drawing.Size(0, 21);
            this.labelGpxTautaiTerrain.Name = "labelGpxTautaiTerrain";
            this.labelGpxTautaiTerrain.Size = new System.Drawing.Size(98, 21);
            this.labelGpxTautaiTerrain.TabIndex = 22;
            this.labelGpxTautaiTerrain.Text = "Terrain:";
            this.labelGpxTautaiTerrain.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTipForOptions.SetToolTip(this.labelGpxTautaiTerrain, "Use these fields to filter the caches by their terrain rating.");
            // 
            // comboBoxGpxTautaiTerrainMin
            // 
            this.comboBoxGpxTautaiTerrainMin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGpxTautaiTerrainMin.Enabled = false;
            this.comboBoxGpxTautaiTerrainMin.FormattingEnabled = true;
            this.comboBoxGpxTautaiTerrainMin.Items.AddRange(new object[] {
            "1",
            "1.5",
            "2",
            "2.5",
            "3",
            "3.5",
            "4",
            "4.5",
            "5"});
            this.comboBoxGpxTautaiTerrainMin.Location = new System.Drawing.Point(136, 77);
            this.comboBoxGpxTautaiTerrainMin.Name = "comboBoxGpxTautaiTerrainMin";
            this.comboBoxGpxTautaiTerrainMin.Size = new System.Drawing.Size(49, 21);
            this.comboBoxGpxTautaiTerrainMin.TabIndex = 26;
            // 
            // labelGpxTautaiTerrainTo
            // 
            this.labelGpxTautaiTerrainTo.AutoSize = true;
            this.labelGpxTautaiTerrainTo.Enabled = false;
            this.labelGpxTautaiTerrainTo.Location = new System.Drawing.Point(191, 77);
            this.labelGpxTautaiTerrainTo.Margin = new System.Windows.Forms.Padding(3);
            this.labelGpxTautaiTerrainTo.MinimumSize = new System.Drawing.Size(0, 21);
            this.labelGpxTautaiTerrainTo.Name = "labelGpxTautaiTerrainTo";
            this.labelGpxTautaiTerrainTo.Size = new System.Drawing.Size(16, 21);
            this.labelGpxTautaiTerrainTo.TabIndex = 27;
            this.labelGpxTautaiTerrainTo.Text = "to";
            this.labelGpxTautaiTerrainTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxGpxTautaiTerrainMax
            // 
            this.comboBoxGpxTautaiTerrainMax.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGpxTautaiTerrainMax.Enabled = false;
            this.flowLayoutPanel3.SetFlowBreak(this.comboBoxGpxTautaiTerrainMax, true);
            this.comboBoxGpxTautaiTerrainMax.FormattingEnabled = true;
            this.comboBoxGpxTautaiTerrainMax.Items.AddRange(new object[] {
            "1",
            "1.5",
            "2",
            "2.5",
            "3",
            "3.5",
            "4",
            "4.5",
            "5"});
            this.comboBoxGpxTautaiTerrainMax.Location = new System.Drawing.Point(213, 77);
            this.comboBoxGpxTautaiTerrainMax.Name = "comboBoxGpxTautaiTerrainMax";
            this.comboBoxGpxTautaiTerrainMax.Size = new System.Drawing.Size(49, 21);
            this.comboBoxGpxTautaiTerrainMax.TabIndex = 28;
            // 
            // labelGpxTautaiDownload
            // 
            this.labelGpxTautaiDownload.Enabled = false;
            this.labelGpxTautaiDownload.Location = new System.Drawing.Point(32, 104);
            this.labelGpxTautaiDownload.Margin = new System.Windows.Forms.Padding(32, 3, 3, 3);
            this.labelGpxTautaiDownload.MinimumSize = new System.Drawing.Size(0, 21);
            this.labelGpxTautaiDownload.Name = "labelGpxTautaiDownload";
            this.labelGpxTautaiDownload.Size = new System.Drawing.Size(98, 21);
            this.labelGpxTautaiDownload.TabIndex = 9;
            this.labelGpxTautaiDownload.Text = "Download:";
            this.labelGpxTautaiDownload.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkBoxGpxTautaiDownloadActive
            // 
            this.checkBoxGpxTautaiDownloadActive.AutoSize = true;
            this.checkBoxGpxTautaiDownloadActive.Checked = true;
            this.checkBoxGpxTautaiDownloadActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxGpxTautaiDownloadActive.Enabled = false;
            this.checkBoxGpxTautaiDownloadActive.Location = new System.Drawing.Point(136, 104);
            this.checkBoxGpxTautaiDownloadActive.MinimumSize = new System.Drawing.Size(0, 21);
            this.checkBoxGpxTautaiDownloadActive.Name = "checkBoxGpxTautaiDownloadActive";
            this.checkBoxGpxTautaiDownloadActive.Size = new System.Drawing.Size(56, 21);
            this.checkBoxGpxTautaiDownloadActive.TabIndex = 10;
            this.checkBoxGpxTautaiDownloadActive.Text = "Active";
            this.checkBoxGpxTautaiDownloadActive.UseVisualStyleBackColor = true;
            // 
            // checkBoxGpxTautaiDownloadDisabled
            // 
            this.checkBoxGpxTautaiDownloadDisabled.AutoSize = true;
            this.checkBoxGpxTautaiDownloadDisabled.Enabled = false;
            this.checkBoxGpxTautaiDownloadDisabled.Location = new System.Drawing.Point(198, 104);
            this.checkBoxGpxTautaiDownloadDisabled.MinimumSize = new System.Drawing.Size(0, 21);
            this.checkBoxGpxTautaiDownloadDisabled.Name = "checkBoxGpxTautaiDownloadDisabled";
            this.checkBoxGpxTautaiDownloadDisabled.Size = new System.Drawing.Size(67, 21);
            this.checkBoxGpxTautaiDownloadDisabled.TabIndex = 11;
            this.checkBoxGpxTautaiDownloadDisabled.Text = "Disabled";
            this.checkBoxGpxTautaiDownloadDisabled.UseVisualStyleBackColor = true;
            // 
            // checkBoxGpxTautaiDownloadArchived
            // 
            this.checkBoxGpxTautaiDownloadArchived.AutoSize = true;
            this.checkBoxGpxTautaiDownloadArchived.Enabled = false;
            this.flowLayoutPanel3.SetFlowBreak(this.checkBoxGpxTautaiDownloadArchived, true);
            this.checkBoxGpxTautaiDownloadArchived.Location = new System.Drawing.Point(271, 104);
            this.checkBoxGpxTautaiDownloadArchived.MinimumSize = new System.Drawing.Size(0, 21);
            this.checkBoxGpxTautaiDownloadArchived.Name = "checkBoxGpxTautaiDownloadArchived";
            this.checkBoxGpxTautaiDownloadArchived.Size = new System.Drawing.Size(68, 21);
            this.checkBoxGpxTautaiDownloadArchived.TabIndex = 12;
            this.checkBoxGpxTautaiDownloadArchived.Text = "Archived";
            this.checkBoxGpxTautaiDownloadArchived.UseVisualStyleBackColor = true;
            // 
            // labelGpxTautaiUserName
            // 
            this.labelGpxTautaiUserName.Enabled = false;
            this.labelGpxTautaiUserName.Location = new System.Drawing.Point(32, 131);
            this.labelGpxTautaiUserName.Margin = new System.Windows.Forms.Padding(32, 3, 3, 3);
            this.labelGpxTautaiUserName.MinimumSize = new System.Drawing.Size(0, 21);
            this.labelGpxTautaiUserName.Name = "labelGpxTautaiUserName";
            this.labelGpxTautaiUserName.Size = new System.Drawing.Size(98, 21);
            this.labelGpxTautaiUserName.TabIndex = 13;
            this.labelGpxTautaiUserName.Text = "Your user name:";
            this.labelGpxTautaiUserName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTipForOptions.SetToolTip(this.labelGpxTautaiUserName, resources.GetString("labelGpxTautaiUserName.ToolTip"));
            // 
            // textBoxGpxTautaiUserName
            // 
            this.textBoxGpxTautaiUserName.Enabled = false;
            this.flowLayoutPanel3.SetFlowBreak(this.textBoxGpxTautaiUserName, true);
            this.textBoxGpxTautaiUserName.Location = new System.Drawing.Point(136, 131);
            this.textBoxGpxTautaiUserName.Name = "textBoxGpxTautaiUserName";
            this.textBoxGpxTautaiUserName.Size = new System.Drawing.Size(153, 20);
            this.textBoxGpxTautaiUserName.TabIndex = 14;
            // 
            // labelGpxTautaiIncludeLogs
            // 
            this.labelGpxTautaiIncludeLogs.Enabled = false;
            this.labelGpxTautaiIncludeLogs.Location = new System.Drawing.Point(32, 158);
            this.labelGpxTautaiIncludeLogs.Margin = new System.Windows.Forms.Padding(32, 3, 3, 3);
            this.labelGpxTautaiIncludeLogs.MinimumSize = new System.Drawing.Size(0, 21);
            this.labelGpxTautaiIncludeLogs.Name = "labelGpxTautaiIncludeLogs";
            this.labelGpxTautaiIncludeLogs.Size = new System.Drawing.Size(98, 21);
            this.labelGpxTautaiIncludeLogs.TabIndex = 15;
            this.labelGpxTautaiIncludeLogs.Text = "Logs:";
            this.labelGpxTautaiIncludeLogs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTipForOptions.SetToolTip(this.labelGpxTautaiIncludeLogs, resources.GetString("labelGpxTautaiIncludeLogs.ToolTip"));
            // 
            // checkBoxGpxTautaiIncludeLogs
            // 
            this.checkBoxGpxTautaiIncludeLogs.AutoSize = true;
            this.checkBoxGpxTautaiIncludeLogs.Enabled = false;
            this.checkBoxGpxTautaiIncludeLogs.Location = new System.Drawing.Point(136, 158);
            this.checkBoxGpxTautaiIncludeLogs.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.checkBoxGpxTautaiIncludeLogs.MinimumSize = new System.Drawing.Size(0, 21);
            this.checkBoxGpxTautaiIncludeLogs.Name = "checkBoxGpxTautaiIncludeLogs";
            this.checkBoxGpxTautaiIncludeLogs.Size = new System.Drawing.Size(119, 21);
            this.checkBoxGpxTautaiIncludeLogs.TabIndex = 16;
            this.checkBoxGpxTautaiIncludeLogs.Text = "Include maximum of";
            this.checkBoxGpxTautaiIncludeLogs.UseVisualStyleBackColor = true;
            this.checkBoxGpxTautaiIncludeLogs.CheckedChanged += new System.EventHandler(this.checkBoxGpxTautaiIncludeLogs_CheckedChanged);
            // 
            // textBoxGpxTautaiMaxLogs
            // 
            this.textBoxGpxTautaiMaxLogs.AllowPromptAsInput = false;
            this.textBoxGpxTautaiMaxLogs.Culture = new System.Globalization.CultureInfo("");
            this.textBoxGpxTautaiMaxLogs.Enabled = false;
            this.textBoxGpxTautaiMaxLogs.HidePromptOnLeave = true;
            this.textBoxGpxTautaiMaxLogs.Location = new System.Drawing.Point(258, 158);
            this.textBoxGpxTautaiMaxLogs.Mask = "00";
            this.textBoxGpxTautaiMaxLogs.Name = "textBoxGpxTautaiMaxLogs";
            this.textBoxGpxTautaiMaxLogs.PromptChar = ' ';
            this.textBoxGpxTautaiMaxLogs.Size = new System.Drawing.Size(31, 20);
            this.textBoxGpxTautaiMaxLogs.TabIndex = 18;
            this.textBoxGpxTautaiMaxLogs.Text = "5";
            this.textBoxGpxTautaiMaxLogs.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.toolTipForOptions.SetToolTip(this.textBoxGpxTautaiMaxLogs, "Enter the number of logs you want to be included in the file.\r\nValid values are f" +
        "rom 1 to 20.");
            this.textBoxGpxTautaiMaxLogs.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxGpxTautaiMaxLogs_Validating);
            // 
            // labelGpxTautaiMaxLogs
            // 
            this.labelGpxTautaiMaxLogs.AutoSize = true;
            this.labelGpxTautaiMaxLogs.Enabled = false;
            this.flowLayoutPanel3.SetFlowBreak(this.labelGpxTautaiMaxLogs, true);
            this.labelGpxTautaiMaxLogs.Location = new System.Drawing.Point(295, 158);
            this.labelGpxTautaiMaxLogs.Margin = new System.Windows.Forms.Padding(3);
            this.labelGpxTautaiMaxLogs.MinimumSize = new System.Drawing.Size(0, 21);
            this.labelGpxTautaiMaxLogs.Name = "labelGpxTautaiMaxLogs";
            this.labelGpxTautaiMaxLogs.Size = new System.Drawing.Size(26, 21);
            this.labelGpxTautaiMaxLogs.TabIndex = 19;
            this.labelGpxTautaiMaxLogs.Text = "logs";
            this.labelGpxTautaiMaxLogs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GpxTautaiOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel3);
            this.MinimumSize = new System.Drawing.Size(347, 186);
            this.Name = "GpxTautaiOptions";
            this.Size = new System.Drawing.Size(347, 186);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.CheckBox checkBoxEnableGpxTautai;
        private System.Windows.Forms.Label labelGpxTautaiCacheTypes;
        private UI.CheckedComboBox comboBoxGpxTautaiCacheTypes;
        private System.Windows.Forms.Label labelGpxTautaiDifficulty;
        private System.Windows.Forms.ComboBox comboBoxGpxTautaiDifficultyMin;
        private System.Windows.Forms.Label labelGpxTautaiDifficultyTo;
        private System.Windows.Forms.ComboBox comboBoxGpxTautaiDifficultyMax;
        private System.Windows.Forms.Label labelGpxTautaiTerrain;
        private System.Windows.Forms.ComboBox comboBoxGpxTautaiTerrainMin;
        private System.Windows.Forms.Label labelGpxTautaiTerrainTo;
        private System.Windows.Forms.ComboBox comboBoxGpxTautaiTerrainMax;
        private System.Windows.Forms.Label labelGpxTautaiDownload;
        private System.Windows.Forms.CheckBox checkBoxGpxTautaiDownloadActive;
        private System.Windows.Forms.CheckBox checkBoxGpxTautaiDownloadDisabled;
        private System.Windows.Forms.CheckBox checkBoxGpxTautaiDownloadArchived;
        private System.Windows.Forms.Label labelGpxTautaiUserName;
        private System.Windows.Forms.TextBox textBoxGpxTautaiUserName;
        private System.Windows.Forms.Label labelGpxTautaiIncludeLogs;
        private System.Windows.Forms.CheckBox checkBoxGpxTautaiIncludeLogs;
        private System.Windows.Forms.MaskedTextBox textBoxGpxTautaiMaxLogs;
        private System.Windows.Forms.Label labelGpxTautaiMaxLogs;
        private UI.SingletonTooltip toolTipForOptions;

    }
}
