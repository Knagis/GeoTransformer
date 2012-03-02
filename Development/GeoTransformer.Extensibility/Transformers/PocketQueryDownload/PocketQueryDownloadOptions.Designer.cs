namespace GeoTransformer.Transformers.PocketQueryDownload
{
    partial class PocketQueryDownloadOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PocketQueryDownloadOptions));
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxEnablePocketQuery = new System.Windows.Forms.CheckBox();
            this.labelPocketQuery = new System.Windows.Forms.Label();
            this.comboBoxPocketQuery = new GeoTransformer.UI.CheckedComboBox();
            this.linkRefresh = new System.Windows.Forms.LinkLabel();
            this.toolTipForOptions = new GeoTransformer.UI.SingletonTooltip();
            this.flowLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.Controls.Add(this.checkBoxEnablePocketQuery);
            this.flowLayoutPanel3.Controls.Add(this.labelPocketQuery);
            this.flowLayoutPanel3.Controls.Add(this.comboBoxPocketQuery);
            this.flowLayoutPanel3.Controls.Add(this.linkRefresh);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(357, 47);
            this.flowLayoutPanel3.TabIndex = 3;
            // 
            // checkBoxEnablePocketQuery
            // 
            this.checkBoxEnablePocketQuery.AutoSize = true;
            this.flowLayoutPanel3.SetFlowBreak(this.checkBoxEnablePocketQuery, true);
            this.checkBoxEnablePocketQuery.Location = new System.Drawing.Point(3, 3);
            this.checkBoxEnablePocketQuery.Name = "checkBoxEnablePocketQuery";
            this.checkBoxEnablePocketQuery.Size = new System.Drawing.Size(242, 17);
            this.checkBoxEnablePocketQuery.TabIndex = 1;
            this.checkBoxEnablePocketQuery.Text = "Enable automatic download of pocket queries";
            this.toolTipForOptions.SetToolTip(this.checkBoxEnablePocketQuery, "This option allows the application to automatically\r\ndownload pocket queries from" +
        " geocaching.com.\r\n\r\nUsing this option will require you to enable the Live\r\nAPI.");
            this.checkBoxEnablePocketQuery.UseVisualStyleBackColor = true;
            this.checkBoxEnablePocketQuery.CheckedChanged += new System.EventHandler(this.checkBoxEnablePocketQuery_CheckedChanged);
            // 
            // labelPocketQuery
            // 
            this.labelPocketQuery.Enabled = false;
            this.labelPocketQuery.Location = new System.Drawing.Point(32, 23);
            this.labelPocketQuery.Margin = new System.Windows.Forms.Padding(32, 0, 3, 3);
            this.labelPocketQuery.MinimumSize = new System.Drawing.Size(0, 21);
            this.labelPocketQuery.Name = "labelPocketQuery";
            this.labelPocketQuery.Size = new System.Drawing.Size(98, 21);
            this.labelPocketQuery.TabIndex = 3;
            this.labelPocketQuery.Text = "Pocket query:";
            this.labelPocketQuery.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTipForOptions.SetToolTip(this.labelPocketQuery, resources.GetString("labelPocketQuery.ToolTip"));
            // 
            // comboBoxPocketQuery
            // 
            this.comboBoxPocketQuery.CheckOnClick = true;
            this.comboBoxPocketQuery.DisplayMember = "Item2";
            this.comboBoxPocketQuery.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.comboBoxPocketQuery.DropDownHeight = 1;
            this.comboBoxPocketQuery.DropDownWidth = 200;
            this.comboBoxPocketQuery.Enabled = false;
            this.comboBoxPocketQuery.FormattingEnabled = true;
            this.comboBoxPocketQuery.IntegralHeight = false;
            this.comboBoxPocketQuery.Location = new System.Drawing.Point(136, 23);
            this.comboBoxPocketQuery.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.comboBoxPocketQuery.MaxDropDownItems = 10;
            this.comboBoxPocketQuery.Name = "comboBoxPocketQuery";
            this.comboBoxPocketQuery.Size = new System.Drawing.Size(153, 21);
            this.comboBoxPocketQuery.TabIndex = 5;
            this.comboBoxPocketQuery.ValueMember = "Item1";
            this.comboBoxPocketQuery.ValueSeparator = ", ";
            // 
            // linkRefresh
            // 
            this.linkRefresh.AutoSize = true;
            this.linkRefresh.Enabled = false;
            this.flowLayoutPanel3.SetFlowBreak(this.linkRefresh, true);
            this.linkRefresh.Location = new System.Drawing.Point(295, 26);
            this.linkRefresh.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.linkRefresh.Name = "linkRefresh";
            this.linkRefresh.Size = new System.Drawing.Size(59, 13);
            this.linkRefresh.TabIndex = 6;
            this.linkRefresh.TabStop = true;
            this.linkRefresh.Text = "Refresh list";
            this.toolTipForOptions.SetToolTip(this.linkRefresh, resources.GetString("linkRefresh.ToolTip"));
            this.linkRefresh.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkRefresh_LinkClicked);
            // 
            // PocketQueryDownloadOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel3);
            this.MinimumSize = new System.Drawing.Size(357, 47);
            this.Name = "PocketQueryDownloadOptions";
            this.Size = new System.Drawing.Size(357, 48);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.CheckBox checkBoxEnablePocketQuery;
        private System.Windows.Forms.Label labelPocketQuery;
        private UI.CheckedComboBox comboBoxPocketQuery;
        private System.Windows.Forms.LinkLabel linkRefresh;
        private UI.SingletonTooltip toolTipForOptions;
    }
}
