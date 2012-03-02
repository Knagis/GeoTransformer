namespace FetchStatistics
{
    partial class Window
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window));
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.label1 = new FetchStatistics.WrapLabel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnRunSpecifics = new System.Windows.Forms.Button();
            this.textBoxIDs = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 139);
            this.webBrowser1.Url = new System.Uri("about:blank");
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ObjectForScripting = null;
            this.webBrowser1.ScrollBarsEnabled = true;
            this.webBrowser1.Size = new System.Drawing.Size(602, 391);
            this.webBrowser1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.MaximumSize = new System.Drawing.Size(600, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(3);
            this.label1.Size = new System.Drawing.Size(600, 110);
            this.label1.TabIndex = 3;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.btnRun);
            this.flowLayoutPanel1.Controls.Add(this.btnRunSpecifics);
            this.flowLayoutPanel1.Controls.Add(this.textBoxIDs);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 110);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(602, 29);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(3, 3);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(146, 23);
            this.btnRun.TabIndex = 5;
            this.btnRun.Text = "Dot savu artavu";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnRunSpecifics
            // 
            this.btnRunSpecifics.Location = new System.Drawing.Point(155, 3);
            this.btnRunSpecifics.Name = "btnRunSpecifics";
            this.btnRunSpecifics.Size = new System.Drawing.Size(179, 23);
            this.btnRunSpecifics.TabIndex = 6;
            this.btnRunSpecifics.Text = "Ielādēt konkrētus slēpņotājus:";
            this.btnRunSpecifics.UseVisualStyleBackColor = true;
            this.btnRunSpecifics.Click += new System.EventHandler(this.btnRunSpecifics_Click);
            // 
            // textBoxIDs
            // 
            this.textBoxIDs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.SetFlowBreak(this.textBoxIDs, true);
            this.textBoxIDs.Location = new System.Drawing.Point(338, 5);
            this.textBoxIDs.Margin = new System.Windows.Forms.Padding(1, 5, 3, 3);
            this.textBoxIDs.Name = "textBoxIDs";
            this.textBoxIDs.Size = new System.Drawing.Size(261, 20);
            this.textBoxIDs.TabIndex = 7;
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.label1);
            this.Name = "Window";
            this.Size = new System.Drawing.Size(602, 530);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private WrapLabel label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnRunSpecifics;
        private System.Windows.Forms.TextBox textBoxIDs;
    }
}
