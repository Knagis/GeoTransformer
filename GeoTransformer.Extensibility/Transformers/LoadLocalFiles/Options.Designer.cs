namespace GeoTransformer.Transformers.LoadLocalFiles
{
    partial class Options
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
            this.components = new System.ComponentModel.Container();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.linkFolder = new System.Windows.Forms.LinkLabel();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.treeLoadedGpx = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.fileSystemWatcher = new System.IO.FileSystemWatcher();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.linkFolder);
            this.flowLayoutPanel1.Controls.Add(this.buttonBrowse);
            this.flowLayoutPanel1.Controls.Add(this.treeLoadedGpx);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(532, 133);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Margin = new System.Windows.Forms.Padding(4);
            this.label1.MinimumSize = new System.Drawing.Size(0, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "Folder:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // linkFolder
            // 
            this.linkFolder.AutoSize = true;
            this.linkFolder.Location = new System.Drawing.Point(64, 4);
            this.linkFolder.Margin = new System.Windows.Forms.Padding(4);
            this.linkFolder.MinimumSize = new System.Drawing.Size(0, 28);
            this.linkFolder.Name = "linkFolder";
            this.linkFolder.Size = new System.Drawing.Size(112, 28);
            this.linkFolder.TabIndex = 1;
            this.linkFolder.TabStop = true;
            this.linkFolder.Text = "C:\\Temp\\Caches";
            this.linkFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkFolder_LinkClicked);
            // 
            // buttonBrowse
            // 
            this.flowLayoutPanel1.SetFlowBreak(this.buttonBrowse, true);
            this.buttonBrowse.Location = new System.Drawing.Point(183, 3);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(100, 28);
            this.buttonBrowse.TabIndex = 4;
            this.buttonBrowse.Text = "Browse...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // treeLoadedGpx
            // 
            this.treeLoadedGpx.CheckBoxes = true;
            this.treeLoadedGpx.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.treeLoadedGpx.ImageIndex = 0;
            this.treeLoadedGpx.ImageList = this.imageList;
            this.treeLoadedGpx.Location = new System.Drawing.Point(4, 40);
            this.treeLoadedGpx.Margin = new System.Windows.Forms.Padding(4);
            this.treeLoadedGpx.MinimumSize = new System.Drawing.Size(4, 32);
            this.treeLoadedGpx.Name = "treeLoadedGpx";
            this.treeLoadedGpx.SelectedImageIndex = 0;
            this.treeLoadedGpx.Size = new System.Drawing.Size(524, 89);
            this.treeLoadedGpx.TabIndex = 3;
            this.treeLoadedGpx.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeLoadedGpx_AfterCheck);
            this.treeLoadedGpx.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeLoadedGpx_AfterCollapse);
            this.treeLoadedGpx.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeLoadedGpx_AfterExpand);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // fileSystemWatcher
            // 
            this.fileSystemWatcher.EnableRaisingEvents = true;
            this.fileSystemWatcher.SynchronizingObject = this;
            this.fileSystemWatcher.Changed += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_Changed);
            this.fileSystemWatcher.Created += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_Changed);
            this.fileSystemWatcher.Deleted += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_Changed);
            this.fileSystemWatcher.Renamed += new System.IO.RenamedEventHandler(this.fileSystemWatcher_Renamed);
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Choose the folder that contains the GPX files - GeoTransformer will automatically" +
    " pick up any ZIP or GPX files you put in this folder.";
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(40, 12);
            this.Name = "Options";
            this.Size = new System.Drawing.Size(532, 133);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkFolder;
        private System.Windows.Forms.TreeView treeLoadedGpx;
        private System.IO.FileSystemWatcher fileSystemWatcher;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;

    }
}
