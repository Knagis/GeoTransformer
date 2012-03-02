namespace GeoTransformer.Modules
{
    partial class ListViewers
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
            System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
            this.toolStripViewers = new System.Windows.Forms.ToolStrip();
            this.toolStripViewersRefresh = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripViewers.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(64, 6);
            toolStripSeparator3.TextDirection = System.Windows.Forms.ToolStripTextDirection.Vertical90;
            // 
            // toolStripViewers
            // 
            this.toolStripViewers.Dock = System.Windows.Forms.DockStyle.Left;
            this.toolStripViewers.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripViewers.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripViewersRefresh,
            toolStripSeparator3});
            this.toolStripViewers.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStripViewers.Location = new System.Drawing.Point(0, 0);
            this.toolStripViewers.Name = "toolStripViewers";
            this.toolStripViewers.Size = new System.Drawing.Size(67, 87);
            this.toolStripViewers.TabIndex = 1;
            this.toolStripViewers.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStripViewers_ItemClicked);
            // 
            // toolStripViewersRefresh
            // 
            this.toolStripViewersRefresh.Image = global::GeoTransformer.Properties.Resources.Refresh16;
            this.toolStripViewersRefresh.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripViewersRefresh.Name = "toolStripViewersRefresh";
            this.toolStripViewersRefresh.Size = new System.Drawing.Size(64, 20);
            this.toolStripViewersRefresh.Text = "Refresh";
            this.toolStripViewersRefresh.ToolTipText = "Refresh the information on the caches by reloading all\r\nsource files (including t" +
    "he ones that are downloaded from\r\nthe internet).";
            this.toolStripViewersRefresh.Click += new System.EventHandler(this.ListViewerRefresh_Click);
            // 
            // ListViewers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.toolStripViewers);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(100, 50);
            this.Name = "ListViewers";
            this.Size = new System.Drawing.Size(262, 87);
            this.toolStripViewers.ResumeLayout(false);
            this.toolStripViewers.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripViewers;
        private System.Windows.Forms.ToolStripButton toolStripViewersRefresh;
    }
}
