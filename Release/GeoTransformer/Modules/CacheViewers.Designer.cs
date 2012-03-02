namespace GeoTransformer.Modules
{
    partial class CacheViewers
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
            this.toolStripViewers = new System.Windows.Forms.ToolStrip();
            this.SuspendLayout();
            // 
            // toolStripViewers
            // 
            this.toolStripViewers.Dock = System.Windows.Forms.DockStyle.Left;
            this.toolStripViewers.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripViewers.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStripViewers.Location = new System.Drawing.Point(0, 0);
            this.toolStripViewers.Name = "toolStripViewers";
            this.toolStripViewers.Size = new System.Drawing.Size(32, 87);
            this.toolStripViewers.TabIndex = 1;
            this.toolStripViewers.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStripViewers_ItemClicked);
            // 
            // CacheViewers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.toolStripViewers);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(100, 50);
            this.Name = "CacheViewers";
            this.Size = new System.Drawing.Size(262, 87);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripViewers;
    }
}
