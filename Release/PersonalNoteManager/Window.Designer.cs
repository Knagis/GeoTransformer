namespace PersonalNoteManager
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnLoad = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.colCacheCode = new System.Windows.Forms.DataGridViewLinkColumn();
            this.colCacheTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFound = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colArchived = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGrid
            // 
            this.dataGrid.AllowUserToAddRows = false;
            this.dataGrid.AllowUserToDeleteRows = false;
            this.dataGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCacheCode,
            this.colCacheTitle,
            this.colFound,
            this.colArchived,
            this.colResult});
            this.dataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGrid.Location = new System.Drawing.Point(0, 25);
            this.dataGrid.Margin = new System.Windows.Forms.Padding(2);
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.ReadOnly = true;
            this.dataGrid.RowTemplate.Height = 24;
            this.dataGrid.Size = new System.Drawing.Size(478, 227);
            this.dataGrid.TabIndex = 0;
            this.dataGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellContentClick);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLoad});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(478, 25);
            this.toolStrip.TabIndex = 1;
            // 
            // btnLoad
            // 
            this.btnLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnLoad.Image = ((System.Drawing.Image)(resources.GetObject("btnLoad.Image")));
            this.btnLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(238, 22);
            this.btnLoad.Text = "Load personal notes from geocaching.com";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 252);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStrip1.Size = new System.Drawing.Size(478, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(251, 17);
            this.toolStripStatusLabel.Text = "Load the notes by pressing the button above...";
            // 
            // colCacheCode
            // 
            this.colCacheCode.DataPropertyName = "CacheCode";
            this.colCacheCode.HeaderText = "Cache code";
            this.colCacheCode.Name = "colCacheCode";
            this.colCacheCode.ReadOnly = true;
            this.colCacheCode.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colCacheCode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colCacheTitle
            // 
            this.colCacheTitle.DataPropertyName = "CacheTitle";
            this.colCacheTitle.HeaderText = "Cache title";
            this.colCacheTitle.Name = "colCacheTitle";
            this.colCacheTitle.ReadOnly = true;
            this.colCacheTitle.Width = 200;
            // 
            // colFound
            // 
            this.colFound.DataPropertyName = "IsFound";
            this.colFound.HeaderText = "Found";
            this.colFound.Name = "colFound";
            this.colFound.ReadOnly = true;
            this.colFound.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colFound.Width = 50;
            // 
            // colArchived
            // 
            this.colArchived.DataPropertyName = "IsArchived";
            this.colArchived.HeaderText = "Archived";
            this.colArchived.Name = "colArchived";
            this.colArchived.ReadOnly = true;
            this.colArchived.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colArchived.Width = 50;
            // 
            // colResult
            // 
            this.colResult.DataPropertyName = "NoteText";
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colResult.DefaultCellStyle = dataGridViewCellStyle1;
            this.colResult.HeaderText = "Note text";
            this.colResult.Name = "colResult";
            this.colResult.ReadOnly = true;
            this.colResult.Width = 400;
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGrid);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Window";
            this.Size = new System.Drawing.Size(478, 274);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGrid;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnLoad;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.DataGridViewLinkColumn colCacheCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCacheTitle;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colFound;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colArchived;
        private System.Windows.Forms.DataGridViewTextBoxColumn colResult;
    }
}
