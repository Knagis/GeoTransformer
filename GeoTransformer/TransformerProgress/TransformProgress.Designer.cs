namespace GeoTransformer
{
    partial class TransformProgress
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
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripClose = new System.Windows.Forms.ToolStripButton();
            this.statusTable = new System.Windows.Forms.TableLayoutPanel();
            this.panel = new System.Windows.Forms.Panel();
            this.toolStrip.SuspendLayout();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripClose});
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip.Location = new System.Drawing.Point(0, 527);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip.Size = new System.Drawing.Size(736, 31);
            this.toolStrip.TabIndex = 3;
            this.toolStrip.Text = "toolStrip";
            // 
            // toolStripClose
            // 
            this.toolStripClose.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripClose.Image = global::GeoTransformer.Properties.Resources.Close;
            this.toolStripClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripClose.Name = "toolStripClose";
            this.toolStripClose.Size = new System.Drawing.Size(81, 28);
            this.toolStripClose.Text = "Cancel";
            this.toolStripClose.Click += new System.EventHandler(this.toolStripClose_Click);
            // 
            // statusTable
            // 
            this.statusTable.AutoSize = true;
            this.statusTable.ColumnCount = 3;
            this.statusTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.statusTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.statusTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statusTable.Dock = System.Windows.Forms.DockStyle.Top;
            this.statusTable.Location = new System.Drawing.Point(3, 3);
            this.statusTable.Name = "statusTable";
            this.statusTable.RowCount = 1;
            this.statusTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.statusTable.Size = new System.Drawing.Size(730, 0);
            this.statusTable.TabIndex = 5;
            // 
            // panel
            // 
            this.panel.AutoScroll = true;
            this.panel.Controls.Add(this.statusTable);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Margin = new System.Windows.Forms.Padding(0);
            this.panel.Name = "panel";
            this.panel.Padding = new System.Windows.Forms.Padding(3);
            this.panel.Size = new System.Drawing.Size(736, 527);
            this.panel.TabIndex = 5;
            // 
            // TransformProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(736, 558);
            this.ControlBox = false;
            this.Controls.Add(this.panel);
            this.Controls.Add(this.toolStrip);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "TransformProgress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Transformation process";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TransformProgress_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.TransformProgress_VisibleChanged);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TransformProgress_KeyPress);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripClose;
        private System.Windows.Forms.TableLayoutPanel statusTable;
        private System.Windows.Forms.Panel panel;
    }
}